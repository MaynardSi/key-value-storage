using Common;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using static Common.MessageWrapper;

namespace Client.ClientSocket
{
    /// <summary>
    /// Application logic of the Server Application
    /// </summary>
    /// Socket programming references:
    /// TcpListener/TcpClient: https://docs.microsoft.com/en-us/dotnet/api/system.net.sockets.tcpclient?view=netcore-3.1
    /// Asynchronous: https://docs.microsoft.com/en-us/dotnet/framework/network-programming/asynchronous-client-socket-example
    /// Multithreaded Clients: http://www.pulpfreepress.com/csharp-for-artists-2nd-edition/
    public class ClientSocket
    {
        #region Constants

        private const int TIMEOUT = 5000;

        #endregion Constants

        #region Fields

        public TcpClient Client;
        public bool IsStarted = false;

        private CancellationTokenSource cancellationTokenSource;
        private CancellationToken cancellationToken;

        #endregion Fields

        #region Events

        public event EventHandler ClientConnected;

        public event EventHandler ClientDisconnected;

        public event EventHandler ClientTimeout;

        public event EventHandler<string> MessageReceived;

        public event EventHandler<string> MessageSent;

        #endregion Events

        #region Methods

        /// <summary>
        /// Start the client from the UI thread.
        /// </summary>
        /// <param name="ipAddress"></param>
        /// <param name="portString"></param>
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
                    Client.Close();
                    Client = null;
                    IsStarted = false;
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
                    Client.Close();
                    Client = null;
                    IsStarted = false;
                    OnClientTimeout();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }

        /// <summary>
        /// Sends a request to send to the Server and returns the Server's response.
        /// </summary>
        /// <param name="ipAddress"></param>
        /// <param name="port"></param>
        /// <param name="requestType"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task<string> SendRequest(string ipAddress, string port,
            string requestType, string message)
        {
            try
            {
                Byte[] responseByte;
                string responseString = String.Empty;

                NetworkStream networkStream = Client.GetStream();
                networkStream.ReadTimeout = TIMEOUT * 2;
                networkStream.WriteTimeout = TIMEOUT * 2;
                StreamWriter writer = new StreamWriter(networkStream);
                writer.AutoFlush = true;

                string requestData = CreateRequest(requestType, message);

                // Check if process has been cancelled before and after sending data.
                cancellationToken.ThrowIfCancellationRequested();
                await writer.WriteLineAsync(requestData);
                cancellationToken.ThrowIfCancellationRequested();
                OnMessageSent(requestData);

                responseByte = new byte[256];
                await networkStream.ReadAsync(responseByte, 0, responseByte.Length).WithCancellation(cancellationToken);
                responseString = System.Text.Encoding.ASCII.GetString(responseByte);
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

        #region EventHandlers

        protected virtual void OnClientConnectSuccess()
        {
            ClientConnected?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void OnClientTimeout()
        {
            ClientTimeout?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void OnClientDisconnect()
        {
            ClientDisconnected?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void OnMessageReceived(string data)
        {
            MessageReceived?.Invoke(this, data);
        }

        protected virtual void OnMessageSent(string data)
        {
            MessageSent?.Invoke(this, data);
        }

        #endregion EventHandlers
    }
}