using Common;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using static Common.RequestResponseEnum;

namespace Server.ServerSocket
{
    /// <summary>
    /// Application logic of the Server Application
    /// </summary>
    /// Socket programming references:
    /// TcpListener/TcpClient: https://docs.microsoft.com/en-us/dotnet/api/system.net.sockets.tcplistener?redirectedfrom=MSDN&view=netcore-3.1
    /// Asynchronous: https://docs.microsoft.com/en-us/dotnet/framework/network-programming/asynchronous-server-socket-example
    /// Multithreaded Clients: http://www.pulpfreepress.com/csharp-for-artists-2nd-edition/
    /// TcpListener.BeginAcceptTcpClient: https://docs.microsoft.com/en-us/dotnet/api/system.net.sockets.tcplistener.beginaccepttcpclient?view=netframework-4.8
    /// async programming tcp: https://docs.microsoft.com/en-us/archive/msdn-magazine/2014/march/async-programming-asynchronous-tcp-sockets-as-an-alternative-to-wcf
    public class ServerSocket
    {
        #region Constants

        private const int TIMEOUT = 5000;

        #endregion Constants

        #region Fields

        public TcpListener Server;
        public bool IsStarted = false;

        private CancellationTokenSource cancellationTokenSource;
        private CancellationToken cancellationToken;

        private KeyValuePairRepository repository = new KeyValuePairRepository();

        #endregion Fields

        #region Events

        public event EventHandler ServerStarted;

        public event EventHandler ServerStopped;

        public event EventHandler WaitingClient;

        public event EventHandler<string> ClientConnected;

        public event EventHandler<string> ClientDisconnected;

        public event EventHandler<string> MessageReceived;

        public event EventHandler<string> MessageSent;

        #endregion Events

        #region Methods

        /// <summary>
        /// Create and start an instance of a TcpListener
        /// </summary>
        /// <param name="ipAddress"></param>
        /// <param name="port"></param>
        public async Task StartServer(string ipAddressString, string portString)
        {
            // Check if TcpListener exists to avoid socket error
            if (!IsStarted)
            {
                // Create a TCP/Ip Socket (TcpListener)
                Server = new TcpListener(IPAddress.Parse(ipAddressString), int.Parse(portString));
                // Set the cancellation token.
                cancellationTokenSource = new CancellationTokenSource();
                cancellationToken = cancellationTokenSource.Token;
                // Start the server.
                Server.Start();
                IsStarted = true;
                // Notify UI that server has started.
                OnServerStarted();
            }

            if (IsStarted)
            {
                try
                {
                    // Notify UI that Server is waiting for a client.
                    OnWaitingClient();
                    // Start accepting client.
                    cancellationToken.ThrowIfCancellationRequested();
                    TcpClient client = await Server.AcceptTcpClientAsync();
                    // Notify UI that a client has connected.
                    string clientEndPoint = client.Client.RemoteEndPoint.ToString();
                    OnClientConnected(clientEndPoint);
                    // Start receiving client request and generate response.
                    Task t = _process(client, clientEndPoint, ipAddressString, portString);
                    cancellationToken.ThrowIfCancellationRequested();
                    await t;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }

        /// <summary>
        /// Stops the TcpListener instance and resets it to the original state.
        /// </summary>
        public void StopServer()
        {
            // cancellation token to cancel start server task
            // tasks: https://msdn.microsoft.com/en-us/library/system.threading.tasks.task(v=vs.110).aspx
            // CancellationToken: https://msdn.microsoft.com/en-us/library/system.threading.cancellationtoken(v=vs.110).aspx
            cancellationTokenSource?.Cancel();
            if (IsStarted)
            {
                try
                {
                    Server.Stop();
                    IsStarted = false;
                    OnServerStopped();
                }
                catch (Exception e)
                {
                    Console.WriteLine("Exception: {0}", e);
                }
            }
        }

        /// <summary>
        /// Gives the appropriate response to a client request.
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        private async Task _process(TcpClient client, string clientEndPoint, string ipAddressString, string portString)
        {
            try
            {
                NetworkStream networkStream = client.GetStream();
                StreamReader reader = new StreamReader(networkStream);
                StreamWriter writer = new StreamWriter(networkStream);

                // Flush its buffer to the underlying stream after every
                // call to StreamWriter.Write.
                writer.AutoFlush = true;
                while (true)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    string request = await reader.ReadLineAsync();
                    if (request != null)
                    {
                        // Notify UI that request has been received.
                        Request deserializedRequest = JsonConvert.DeserializeObject<Request>(request);
                        OnMessageReceived($"\n\t{deserializedRequest.MessageType} : {deserializedRequest.Message}\n");

                        // Parse and process client request.
                        string response = _processClientRequest(deserializedRequest);
                        cancellationToken.ThrowIfCancellationRequested();
                        await writer.WriteLineAsync(response);

                        // Notify UI that response has been sent.
                        Response deserializedResponse = JsonConvert.DeserializeObject<Response>(response);
                        OnMessageSent($"\n\t{deserializedResponse.MessageType} : {deserializedResponse.Message}");
                    }
                    else
                    {
                        clientEndPoint = clientEndPoint ?? "No client found";
                        OnClientDisconnected(clientEndPoint); // Notify UI Client Disconnected.
                        await this.StartServer(ipAddressString, portString); // Seems like a bad idea
                        break; // Client has closed the connection or network stream unavailable.
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                if (client.Connected)
                {
                    client.Close();
                }
            }
        }

        /// <summary>
        /// Processes a clients request and generates a response string based on the request type.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        private string _processClientRequest(Request request)
        {
            string response;

            switch (request.RequestType)
            {
                case RequestResponseTypes.GET:
                    // Return KVP string
                    response = _createResponse(request.RequestType, _processClientGET(request.Message));
                    break;

                case RequestResponseTypes.GETALL:
                    // Return KVP string
                    response = _createResponse(request.RequestType, _processClientGETALL(request.Message));
                    break;

                case RequestResponseTypes.SET:
                    // Store KVP in repository and return OK
                    response = _createResponse(request.RequestType, _processClientSET(request.Message));
                    break;

                case RequestResponseTypes.PING:
                    // Return a response of PONG
                    response = _createResponse(request.RequestType, _processClientPING(request.Message));
                    break;

                default:
                    // Unknown response type
                    response = "Request Error";
                    break;
            }
            return response;
        }

        /// <summary>
        /// Returns the value of and item in the repository given a queried key from a request.
        /// </summary>
        /// <param name="keyFromMessage"></param>
        /// <returns></returns>
        private string _processClientGET(string keyFromMessage)
        {
            if (repository.KeyValuePairs.TryGetValue(keyFromMessage, out string result))
            {
                return $"{result}";
            }
            return $"ERROR NOT FOUND";
        }

        /// <summary>
        /// Returns the list of items in the repository.
        /// </summary>
        /// <param name="keyFromMessage"></param>
        /// <returns></returns>
        private string _processClientGETALL(string keyFromMessage)
        {
            if (repository.KeyValuePairs.Count > 0)
            {
                string lines = string.Join(Environment.NewLine,
                    repository.KeyValuePairs.Select(kvp => kvp.Key + ": " + kvp.Value.ToString()));
                return lines;
            }

            return $"ERROR NOT FOUND";
        }

        /// <summary>
        /// Adds a Key Value Pair to the repository from a given request.
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        private string _processClientSET(string message)
        {
            string[] keyValuePair = message.Split(',');
            string key = keyValuePair[0];
            string value = keyValuePair[1];
            if (!repository.KeyValuePairs.ContainsKey(key))
            {
                repository.KeyValuePairs.Add(key, value);
                return $"OK: {key}, {value} ADDED ";
            }
            return $"ERROR NOT ADDED";
        }

        /// <summary>
        /// Returns "PONG" as a response to the request PING.
        /// </summary>
        /// <param name="keyFromMessage"></param>
        /// <returns></returns>
        private string _processClientPING(string keyFromMessage)
        {
            return "PONG";
        }

        /// <summary>
        /// Returns a JSON string built from the serialized Response class and process response.
        /// </summary>
        /// <param name="requestType"></param>
        /// <param name="response"></param>
        /// <returns></returns>
        private static string _createResponse(RequestResponseTypes requestType, string response)
        {
            Response responseObj = new Response(requestType, response);
            string jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(responseObj);
            return jsonString;
        }

        #endregion Methods

        #region EventsHandlers

        protected virtual void OnServerStarted()
        {
            ServerStarted?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void OnServerStopped()
        {
            ServerStopped?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void OnWaitingClient()
        {
            WaitingClient?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void OnClientConnected(string clientInfo)
        {
            ClientConnected?.Invoke(this, clientInfo);
        }

        protected virtual void OnClientDisconnected(string clientInfo)
        {
            ClientDisconnected?.Invoke(this, clientInfo);
        }

        protected virtual void OnMessageReceived(string data)
        {
            MessageReceived?.Invoke(this, data);
        }

        protected virtual void OnMessageSent(string data)
        {
            MessageSent?.Invoke(this, data);
        }

        #endregion EventsHandlers
    }
}