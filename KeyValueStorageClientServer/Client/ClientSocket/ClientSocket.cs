using Server.ServerSocket;
using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Client.ClientSocket
{
    /// Socket programming references:
    /// TcpListener/TcpClient: https://docs.microsoft.com/en-us/dotnet/api/system.net.sockets.tcpclient?view=netcore-3.1
    /// Asynchronous: https://docs.microsoft.com/en-us/dotnet/framework/network-programming/asynchronous-client-socket-example
    /// Multithreaded Clients: http://www.pulpfreepress.com/csharp-for-artists-2nd-edition/
    public class ClientSocket
    {
        private const int TIMEOUT = 5000;

        private TcpClient client = null;

        // ManualResetEvent instances signal completion.

        private static ManualResetEvent connectDone = new ManualResetEvent(false);
        private static ManualResetEvent sendDone = new ManualResetEvent(false);
        private static ManualResetEvent receiveDone = new ManualResetEvent(false);

        private CancellationTokenSource cancellationTokenSource;
        private CancellationToken cancellationToken;

        // The response from the remote device.
        private static String response = String.Empty;

        public event EventHandler ClientConnected;

        public event EventHandler ClientDisconnected;

        public event EventHandler ClientTimeout;

        public event EventHandler<string> MessageReceived;

        public event EventHandler<string> MessageSent;

        /// <summary>
        /// Start the client from the UI thread
        /// </summary>
        /// <param name="ipAddress"></param>
        /// <param name="portString"></param>
        public async Task StartClientAsync(string ipAddress, string port)
        {
            cancellationTokenSource = new CancellationTokenSource();
            cancellationToken = cancellationTokenSource.Token;

            // Create a TCP/Ip Socket (TcpLClient)
            client = new TcpClient();

            try
            {
                await Task.Run(() =>
                {
                    while (!cancellationToken.IsCancellationRequested && client != null)
                    {
                        var connectResult = client.BeginConnect(ipAddress, int.Parse(port),
                             new AsyncCallback(ConnectCallback), client);
                        // Block until a connection is made
                        var connectSucess = connectDone.WaitOne(TIMEOUT);
                        if (connectSucess || client.Connected)
                        {
                            //TODO:
                            //Send test data to the remote device.
                            Send(client, "This is a test<EOF>");
                            sendDone.WaitOne();

                            // Receive the response from the remote device.
                            Receive(client);
                            receiveDone.WaitOne();

                            // Write the response to the console.
                            Console.WriteLine("Response received : {0}", response);

                            // Release the socket.
                            //client.Shutdown(SocketShutdown.Both);
                            //client.Close();
                        }
                        else
                        {
                            // TODO: client timeout event
                            TimeoutClient();
                        }
                    }
                }, cancellationToken);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        /*
         * ConnectClientCallback
         * Receive -> ReceiveCallback
         * Send -> SendCallback
         */

        private void ConnectCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object (object ar).
                TcpClient client = (TcpClient)ar.AsyncState;

                // Complete the connection.
                client.EndConnect(ar);

                // Signal that the connection has been made.
                connectDone.Set();
                OnClientConnectSuccess();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private void Receive(TcpClient client)
        {
            try
            {
                // Create the state object
                StateObject state = new StateObject
                {
                    workSocket = client
                };

                // Begin receiving data
                client.GetStream().BeginRead(state.buffer, 0, StateObject.BufferSize,
                    new AsyncCallback(ReceiveCallback), state);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private void ReceiveCallback(IAsyncResult ar)
        {
            String content = String.Empty;
            try
            {
                // Retrieve state object and client socket
                // from asynchronous state object ar.
                StateObject state = (StateObject)ar.AsyncState;
                TcpClient client = state.workSocket;

                // Read data
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
                    // All the data has arrived; put it in response.
                    if (state.sb.Length > 1)
                    {
                        response = state.sb.ToString();
                    }
                    // Signal that all bytes have been received.
                    receiveDone.Set();
                    OnMessageReceived($"Read {content.Length} bytes from socket. \n Data : {content}");
                }
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

            // Begin sending the data to the listening device.
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
                // Signal that all bytes have been sent.
                sendDone.Set();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        public void StopClient()
        {
            // cancellation token to cancel start server task
            // tasks: https://msdn.microsoft.com/en-us/library/system.threading.tasks.task(v=vs.110).aspx
            // CancellationToken: https://msdn.microsoft.com/en-us/library/system.threading.cancellationtoken(v=vs.110).aspx
            cancellationTokenSource?.Cancel();
            if (client != null)
            {
                try
                {
                    client.Close();
                    OnClientDisconnect();
                }
                catch (SocketException se)
                {
                    Console.WriteLine("SocketException: {0}", se);
                }
            }
        }

        public void TimeoutClient()
        {
            cancellationTokenSource?.Cancel();
            if (client != null)
            {
                try
                {
                    client.Close();
                    OnClientTimeout();
                }
                catch (SocketException se)
                {
                    Console.WriteLine("SocketException: {0}", se);
                }
            }
        }

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
    }
}