using Common;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using static Common.RequestResponseEnum;

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
        /// Sends a request to send to the Server and returns the Server's response.
        /// </summary>
        /// <param name="ipAddress"></param>
        /// <param name="port"></param>
        /// <param name="requestType"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task<string> SendRequest(string ipAddress, string port,
            RequestResponseTypes requestType, string message)
        {
            try
            {
                NetworkStream networkStream = Client.GetStream();
                StreamWriter writer = new StreamWriter(networkStream);
                StreamReader reader = new StreamReader(networkStream);
                writer.AutoFlush = true;
                string requestData = CreateRequest(requestType, message);
                cancellationToken.ThrowIfCancellationRequested();
                await writer.WriteLineAsync(requestData);

                Request deserializedRequest = JsonConvert.DeserializeObject<Request>(requestData);
                OnMessageSent($"\n\t{deserializedRequest.MessageType} : {deserializedRequest.Message}\n");

                cancellationToken.ThrowIfCancellationRequested();
                string response = await reader.ReadLineAsync();
                Response deserializedResponse = JsonConvert.DeserializeObject<Response>(response);
                OnMessageReceived($"\n\t{deserializedResponse.MessageType} : {deserializedResponse.Message}\n");
                //client.Close();
                return response;
            }
            catch (Exception e)
            {
                OnClientTimeout();
                Console.WriteLine(e.Message);
                return $"ERROR: {e.Message}";
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
                    OnClientTimeout();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }

        /// <summary>
        /// Returns a JSON string built from the serialized Response class and process response.
        /// </summary>
        /// <param name="requestType"></param>
        /// <param name="response"></param>
        /// <returns></returns>
        private static string CreateRequest(RequestResponseTypes requestType, string requestMessage)
        {
            Request requestObj = new Request(requestType, requestMessage);
            string jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(requestObj);
            return jsonString;
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