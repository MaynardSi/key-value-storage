//using System;
//using System.IO;
//using System.Net;
//using System.Net.Sockets;
//using System.Threading;
//using System.Threading.Tasks;

//namespace Server.ServerSocket
//{
//    public class ServerSocket
//    {
//        private const int TIMEOUT = 5000;
//        private TcpListener _serverSocket = null;

//        public event EventHandler ServerStarted;

//        public event EventHandler ServerStopped;

//        public event EventHandler WaitingClient;

//        public event EventHandler ClientConnected;

//        public event EventHandler<string> MessageReceived;

//        public event EventHandler<string> MessageSent;

//        // Thread signal.
//        public static ManualResetEvent allDone = new ManualResetEvent(false);

//        /// <summary>
//        /// Socket programming references:
//        /// TcpListener/TcpClient: https://docs.microsoft.com/en-us/dotnet/api/system.net.sockets.tcplistener?redirectedfrom=MSDN&view=netcore-3.1
//        /// Multithreaded Clients: http://www.pulpfreepress.com/csharp-for-artists-2nd-edition/
//        /// </summary>
//        /// <param name="ipAddress"></param>
//        /// <param name="port"></param>
//        public async Task StartServerAsync(string ipAddressString, string portString)
//        {
//            IPAddress ipAddress = IPAddress.Parse(ipAddressString);
//            int port = Int32.Parse(portString);
//            int clientCount = 0;
//            await Task.Run(() =>
//            {
//                try
//                {
//                    // Set TcpListener using given ipAddress and port from the Client UI
//                    _serverSocket = new TcpListener(ipAddress, port);

//                    // Start listening for client requests.
//                    _serverSocket.Start();
//                    OnServerStarted();
//                    while (true)
//                    {
//                        OnWaitingClient();
//                        TcpClient client = _serverSocket.AcceptTcpClient();
//                        clientCount += 1;
//                        OnClientConnected();
//                        Thread t = new Thread(ClientRequest);
//                        t.Start(client);
//                    }
//                }
//                catch (Exception e)
//                {
//                    Console.WriteLine("Exception: {0}", e);
//                }
//            });
//        }

//        public void StopServer()
//        {
//            if (_serverSocket != null)
//            {
//                try
//                {
//                    _serverSocket.Stop();
//                    OnServerStopped();
//                }
//                catch (SocketException se)
//                {
//                    Console.WriteLine("SocketException: {0}", se);
//                }
//            }
//            //TODO Check if I have to make server = null
//        }

//        private void ClientRequest(object arg)
//        {
//            TcpClient client = (TcpClient)arg;
//            try
//            {
//                StreamReader reader = new StreamReader(client.GetStream());
//                StreamWriter writer = new StreamWriter(client.GetStream());
//                string s = String.Empty;
//                while (!(s = reader.ReadLine()).Equals("Exit") || (s == null))
//                {
//                    //Console.WriteLine("From client -> " + s);
//                    OnMessageReceived(s);
//                    writer.WriteLine("From server -> " + s);
//                    writer.Flush();
//                    String client_string = reader.ReadLine();
//                }
//                reader.Close();
//                writer.Close();
//                client.Close();
//                OnMessageSent("Closing client connection!");
//                Console.WriteLine("Closing client connection!");
//            }
//            catch (Exception e)
//            {
//                Console.WriteLine("Exception: {0}", e);
//            }
//            finally
//            {
//                if (client != null)
//                {
//                    client.Close();
//                }
//            }
//        }

//        protected virtual void OnServerStarted()
//        {
//            ServerStarted?.Invoke(this, EventArgs.Empty);
//        }

//        protected virtual void OnServerStopped()
//        {
//            ServerStopped?.Invoke(this, EventArgs.Empty);
//        }

//        protected virtual void OnWaitingClient()
//        {
//            WaitingClient?.Invoke(this, EventArgs.Empty);
//        }

//        protected virtual void OnClientConnected()
//        {
//            ClientConnected?.Invoke(this, EventArgs.Empty);
//        }

//        protected virtual void OnMessageReceived(string data)
//        {
//            MessageReceived?.Invoke(this, data);
//        }

//        protected virtual void OnMessageSent(string data)
//        {
//            MessageSent?.Invoke(this, data);
//        }
//    }
//}