//using System;
//using System.IO;
//using System.Net.Sockets;

//namespace Client.ClientSocket
//{
//    public class ClientSocket
//    {
//        private const int TIMEOUT = 5000;
//        private TcpClient _clientSocket = null;

//        public event EventHandler ClientConnected;

//        public event EventHandler ClientDisconnected;

//        public event EventHandler<string> MessageReceived;

//        public event EventHandler<string> MessageSent;

//        public void StartClient(string ipAddress, string portString)
//        {
//            try
//            {
//                // Actions Here
//                int port = Int32.Parse(portString);
//                _clientSocket = new TcpClient(ipAddress, port);
//                OnClientConnect();

//                StreamReader reader = new StreamReader(_clientSocket.GetStream());
//                StreamWriter writer = new StreamWriter(_clientSocket.GetStream());
//                String s = String.Empty;
//                while (!s.Equals("Exit"))
//                {
//                    Console.Write("Enter a string to send to the server: ");
//                    //s = Console.ReadLine();
//                    s = "Ping";
//                    //Console.WriteLine();
//                    writer.WriteLine(s);
//                    writer.Flush();
//                    String server_string = reader.ReadLine();
//                    OnMessageReceived(server_string);
//                    //Console.WriteLine(server_string);
//                    //s = "Gay";
//                    //writer.WriteLine(s);
//                    //writer.Flush();
//                    s = "Exit";
//                }
//                reader.Close();
//                writer.Close();
//                _clientSocket.Close();
//            }
//            catch (SocketException e)
//            {
//                // TODO Server error, no server found
//                Console.WriteLine("SocketException: {0}", e);
//            }
//        }

//        public void StopClient()
//        {
//            if (_clientSocket != null)
//            {
//                try
//                {
//                    _clientSocket.Close();
//                    OnClientDisconnect();
//                }
//                catch (SocketException se)
//                {
//                    Console.WriteLine("SocketException: {0}", se);
//                }
//            }
//        }

//        protected virtual void OnClientConnect()
//        {
//            ClientConnected?.Invoke(this, EventArgs.Empty);
//        }

//        protected virtual void OnClientDisconnect()
//        {
//            ClientDisconnected?.Invoke(this, EventArgs.Empty);
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