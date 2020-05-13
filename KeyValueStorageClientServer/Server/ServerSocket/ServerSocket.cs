using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Server.ServerSocket
{
    /// Socket programming references:
    /// TcpListener/TcpClient: https://docs.microsoft.com/en-us/dotnet/api/system.net.sockets.tcplistener?redirectedfrom=MSDN&view=netcore-3.1
    /// Asynchronous: https://docs.microsoft.com/en-us/dotnet/framework/network-programming/asynchronous-server-socket-example
    /// Multithreaded Clients: http://www.pulpfreepress.com/csharp-for-artists-2nd-edition/
    /// TcpListener.BeginAcceptTcpClient: https://docs.microsoft.com/en-us/dotnet/api/system.net.sockets.tcplistener.beginaccepttcpclient?view=netframework-4.8
    public class ServerSocket
    {
        private const int TIMEOUT = 5000;

        public event EventHandler ServerStarted;

        public event EventHandler ServerStopped;

        public event EventHandler WaitingClient;

        public event EventHandler ClientConnected;

        public event EventHandler<string> MessageReceived;

        public event EventHandler<string> MessageSent;

        private TcpListener server = null;

        private CancellationTokenSource cancellationTokenSource;
        private CancellationToken cancellationToken;

        // Thread signal.
        public static ManualResetEvent tcpClientConnected = new ManualResetEvent(false);

        /// <summary>
        /// Start the server from the UI thread.
        /// </summary>
        /// <param name="ipAddress"></param>
        /// <param name="port"></param>
        public async Task StartServerAsync(string ipAddressString, string portString)
        {
            // Check if TcpListener exists to avoid socket error
            if (server == null)
            {
                // Create a TCP/Ip Socket (TcpListener)
                server = new TcpListener(IPAddress.Parse(ipAddressString), int.Parse(portString));
                cancellationTokenSource = new CancellationTokenSource();
                cancellationToken = cancellationTokenSource.Token;
                server.Start();
                OnServerStarted();
            }
            try
            {
                await Task.Run(() =>
                    {
                        if (server != null)
                            DoBeginAcceptTcpClient(server);
                    }, cancellationToken);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        /*
         * DoBeginAcceptClient -> DoAcceptTcpClientCallback -> ReadCallback
         * -> Send -> SendCallback
         */

        // TODO: I still might have problems in making the connection persistent

        // Wait and Accept one client connection asynchronously.
        private void DoBeginAcceptTcpClient(TcpListener listener)
        {
            // Set the event to nonsignaled state (Reset).
            // So that threads once again block, when they call WaitOne()
            tcpClientConnected.Reset();

            // Start to listen for connections from a client.
            // Signal subscribers that client is waiting
            OnWaitingClient();

            // Accept the connection.
            // BeginAcceptSocket() creates the accepted socket.
            listener.BeginAcceptTcpClient(new AsyncCallback(DoAcceptTcpClientCallback), listener);

            // Wait until a connection is made and processed before continuing.
            tcpClientConnected.WaitOne();
        }

        private void DoAcceptTcpClientCallback(IAsyncResult ar)
        {
            if (!cancellationToken.IsCancellationRequested)
            {
                // Get the listener that handles the client request.
                TcpListener listener = (TcpListener)ar.AsyncState;
                TcpClient client = listener.EndAcceptTcpClient(ar);

                // Signal subscribers that client is connected
                OnClientConnected();

                // TODO: Using new tcpclient Process the connection here. (Add the client to aserver table, read data, etc.)
                // Read call back using state
                // Create the state object
                StateObject state = new StateObject
                {
                    workSocket = client
                };

                // TODO: Persistent read method
                // Uses the GetStream public method to return the NetworkStream.
                client.GetStream().BeginRead(state.buffer, 0, StateObject.BufferSize,
                    new AsyncCallback(ReceiveCallback), state);

                // Signal the main thread to continue (Set).
                // Threads that call WaitOne() do not block when Set.
                tcpClientConnected.Set();
            }
        }

        private void ReceiveCallback(IAsyncResult ar)
        {
            String content = String.Empty;

            try
            {
                // Retrieve the state object and the handler socket
                // from the asynchronous state object.
                StateObject state = (StateObject)ar.AsyncState;
                TcpClient client = state.workSocket;

                if (client.GetStream().CanRead)
                {
                    // Read data from the client socket.
                    int noOfBytesRead = client.GetStream().EndRead(ar);
                    state.sb.Append(Encoding.ASCII.GetString(state.buffer, 0, noOfBytesRead));
                    content = state.sb.ToString();
                    while (client.GetStream().DataAvailable)
                    {
                        client.GetStream().BeginRead(state.buffer, 0, StateObject.BufferSize,
                            new AsyncCallback(ReceiveCallback), state);
                    }
                }
                // All the data has been read from the client. Display it on the console.
                OnMessageReceived($"Read {content.Length} bytes from socket. \n Data : {content}");
                // TODO Process string method and take command
                // Echo the data back to the client.
                Send(client, content);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private void Send(TcpClient client, String data)
        {
            // Convert the string data to byte data using ASCII encoding.
            byte[] byteData = Encoding.ASCII.GetBytes(data);

            // Begin sending the data to the remote device.
            client.GetStream().BeginWrite(byteData, 0, byteData.Length,
                new AsyncCallback(SendCallback), client);
        }

        private void SendCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object.
                TcpClient client = (TcpClient)ar.AsyncState;

                // Complete sending the data to the remote device.
                client.GetStream().EndWrite(ar);
                OnMessageSent($"Sent bytes to client.");

                //TODO: client.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        public void StopServer()
        {
            // cancellation token to cancel start server task
            // tasks: https://msdn.microsoft.com/en-us/library/system.threading.tasks.task(v=vs.110).aspx
            // CancellationToken: https://msdn.microsoft.com/en-us/library/system.threading.cancellationtoken(v=vs.110).aspx
            if (cancellationTokenSource != null)
            {
                cancellationTokenSource.Cancel();
            }
            if (server != null)
            {
                try
                {
                    server.Stop();
                    OnServerStopped();
                }
                catch (SocketException se)
                {
                    Console.WriteLine("SocketException: {0}", se);
                }
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
}