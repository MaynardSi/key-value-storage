using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Server
{
    public class ServerSocket
    {
        private const int TIMEOUT = 5000;

        private TcpListener _serverSocket;
        private TcpClient _clientSocket;
        private readonly int _clientCount = 0;

        public event EventHandler ServerStarted;

        public event EventHandler ServerStopped;

        public event EventHandler WaitingClient;

        public event EventHandler ClientConnected;

        public event EventHandler<string> MessageReceived;

        public event EventHandler<string> MessageSent;

        /// <summary>
        /// Socket programming references:
        /// Listener: https://docs.microsoft.com/en-us/dotnet/api/system.net.sockets.tcplistener?redirectedfrom=MSDN&view=netcore-3.1
        /// </summary>
        /// <param name="ipAddress"></param>
        /// <param name="port"></param>
        public void StartServer(string ipAddressString, string portString)
        {
            IPAddress ipAddress = IPAddress.Parse(ipAddressString);
            int port = Int32.Parse(portString);
            try
            {
                // Set TcpListener using given ipAddress and port from the Client UI
                _serverSocket = new TcpListener(ipAddress, port);

                // Start listening for client requests.
                _serverSocket.Start();

                OnServerStarted();
                StartConnection();
            }
            catch (SocketException se)
            {
                Console.WriteLine("SocketException: {0}", se);
            }
        }

        public void StopServer()
        {
            if (_serverSocket != null)
            {
                try
                {
                    _serverSocket.Stop();
                    OnServerStopped();
                }
                catch (SocketException se)
                {
                    Console.WriteLine("SocketException: {0}", se);
                }
            }
            //TODO Check if I have to make server = null
        }

        public async void StartConnection()
        {
            //Buffer for reading data
            Byte[] bytes = new Byte[256];
            String data = null;

            try
            {
                while (true)
                {
                    OnWaitingClient();
                    // TODO Replace with functionality.
                    //Create instances of client doing an action

                    await Task.Run(async () =>
                    {
                        // Perform a blocking call to accept requests.
                        // You could also use server.AcceptSocket() here.
                        _clientSocket = await _serverSocket.AcceptTcpClientAsync();
                        OnClientConnected();

                        data = null;

                        // Get a stream object for reading and writing
                        NetworkStream stream = _clientSocket.GetStream();

                        int i;

                        // Loop to receive all the data sent by the client.
                        while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
                        {
                            // Translate data bytes to a ASCII string.
                            data = System.Text.Encoding.ASCII.GetString(bytes, 0, i);
                            OnMessageReceived(data);
                            //Console.WriteLine("Received: {0}", data);

                            // Process the data sent by the client.
                            data = data.ToUpper();

                            byte[] msg = System.Text.Encoding.ASCII.GetBytes(data);

                            // Send back a response.
                            stream.Write(msg, 0, msg.Length);
                            OnMessageSent(data);
                            //Console.WriteLine("Sent: {0}", data);
                            //// Shutdown and end connection
                            //_clientSocket.Close();
                        }
                    });
                }
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
            }
        }

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

        protected virtual void OnClientConnected()
        {
            ClientConnected?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void OnMessageReceived(string data)
        {
            MessageReceived?.Invoke(this, data);
        }

        protected virtual void OnMessageSent(string data)
        {
            MessageSent?.Invoke(this, data);
        }
    }

    public class StartClient
    {
        private TcpClient _clientSocket { get; set; }
        private int _clientId { get; set; }

        public StartClient(TcpClient inputClientSocket, int inputClientId)
        {
            _clientSocket = inputClientSocket;
            _clientId = inputClientId;
        }
    }
}