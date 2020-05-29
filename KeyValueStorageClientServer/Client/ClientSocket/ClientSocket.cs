using Common;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Client.ConnectionStatus;
using static Common.MessageWrapper;

namespace Client.ClientSocket
{
    /// <summary>
    /// Application logic of the Server Application
    /// </summary>
    /// Socket programming references:
    /// TcpListener/TcpClient: https://docs.microsoft.com/en-us/dotnet/api/system.net.sockets.tcpclient?view=netcore-3.1
    /// Asynchronous: https://docs.microsoft.com/en-us/dotnet/framework/network-programming/asynchronous-client-socket-example
    /// Multi-threaded Clients: http://www.pulpfreepress.com/csharp-for-artists-2nd-edition/
    public class ClientSocket
    {
        #region Constants

        private const int TIMEOUT = 5000;

        #endregion Constants

        #region Fields

        public TcpClient Client;
        public bool IsStarted;

        private CancellationTokenSource cancellationTokenSource;
        private CancellationToken cancellationToken;

        #endregion Fields

        #region Events

        public event EventHandler<string> MessageReceived;

        public event EventHandler<string> MessageSent;

        #endregion Events

        #region Methods

        /// <summary>
        /// Start the client from the UI thread.
        /// </summary>
        /// <param name="ipAddress"></param>
        /// <param name="port"></param>
        public async Task StartClientAsync(string ipAddress, string port)
        {
            // Check if TcpClient exists to avoid socket error.
            if (!IsStarted)
            {
                // Create a TCP/Ip Socket (TcpLClient).
                Client = new TcpClient();

                // Set the cancellation token.
                cancellationTokenSource = new CancellationTokenSource();
                cancellationToken = cancellationTokenSource.Token;

                // Start connecting to server on IP-Port
                cancellationToken.ThrowIfCancellationRequested();
                await Client.ConnectAsync(IPAddress.Parse(ipAddress), Int32.Parse(port));   // Connect
                IsStarted = true;
                OnClientConnectSuccess();
            }
        }

        /// <summary>
        /// Disposes the current instance of the Client.
        /// </summary>
        public void StopClient()
        {
            // cancellation token to cancel start server task.
            // tasks: https://msdn.microsoft.com/en-us/library/system.threading.tasks.task(v=vs.110).aspx
            // CancellationToken: https://msdn.microsoft.com/en-us/library/system.threading.cancellationtoken(v=vs.110).aspx
            cancellationTokenSource?.Cancel();
            if (IsStarted)
            {
                try
                {
                    disposeClient();
                    OnClientDisconnect();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }

        /// <summary>
        /// Cancels operations and notifies the UI once a timeout has occured.
        /// </summary>
        public void TimeoutClient()
        {
            cancellationTokenSource?.Cancel();
            if (IsStarted)
            {
                try
                {
                    disposeClient();
                    OnClientTimeout();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }

        private void disposeClient()
        {
            Client.Close();
            Client = null;
            IsStarted = false;
        }

        /// <summary>
        /// Sends a request to send to the Server and returns the Server's response.
        /// </summary>
        /// <param name="requestType"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task<string> SendRequest(string requestType, string message)
        {
            try
            {
                Byte[] responseByte = new byte[256];
                NetworkStream networkStream = Client.GetStream();
                StreamWriter writer = new StreamWriter(networkStream) { AutoFlush = true };

                string requestData = CreateRequest(requestType, message);

                // Check if process has been cancelled before and after sending data.
                cancellationToken.ThrowIfCancellationRequested();
                await writer.WriteLineAsync(requestData);
                cancellationToken.ThrowIfCancellationRequested();
                OnMessageSent(requestData);

                // Receiving Response
                Task timeoutTask = Task.Delay(TIMEOUT, cancellationToken);
                Task readResponseTask =
                    networkStream.ReadAsync(responseByte, 0, responseByte.Length, cancellationToken);
                Task completedTask = await Task.WhenAny(timeoutTask, readResponseTask);
                if (completedTask == timeoutTask)
                {
                    cancellationTokenSource?.Cancel();
                    throw new Exception("TIMEOUT");
                }

                string responseString = WrapResponse(System.Text.Encoding.ASCII.GetString(responseByte));
                OnMessageReceived(responseString);

                return responseString;
            }
            catch (Exception e)
            {
                TimeoutClient();
                return $"ERROR: {e.Message}";
            }
        }

        #endregion Methods

        #region EventRaisers

        protected virtual void OnClientConnectSuccess()
        {
            ClientConnectionStatusMediator.GetInstance().OnStatusChanged(this, ConnectionStatusEnum.ClientConstants.CONNECTED);
        }

        protected virtual void OnClientTimeout()
        {
            ClientConnectionStatusMediator.GetInstance().OnStatusChanged(this, ConnectionStatusEnum.ClientConstants.TIMEOUT);
        }

        protected virtual void OnClientDisconnect()
        {
            ClientConnectionStatusMediator.GetInstance().OnStatusChanged(this, ConnectionStatusEnum.ClientConstants.DISCONNECTED);
        }

        protected virtual void OnMessageReceived(string data)
        {
            MessageReceived?.Invoke(this, data);
        }

        protected virtual void OnMessageSent(string data)
        {
            MessageSent?.Invoke(this, data);
        }

        #endregion EventRaisers
    }
}