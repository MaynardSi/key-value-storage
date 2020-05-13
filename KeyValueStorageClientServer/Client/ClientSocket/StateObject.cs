using System.Net.Sockets;
using System.Text;

namespace Server.ServerSocket
{
    /// <summary>
    /// State object for reading client data asynchronously
    /// https://docs.microsoft.com/en-us/dotnet/framework/network-programming/asynchronous-server-socket-example
    /// </summary>
    internal class StateObject
    {
        // Client  socket.
        public TcpClient workSocket = null;

        // Size of receive buffer.
        public const int BufferSize = 1024;

        // Receive buffer.
        public byte[] buffer = new byte[BufferSize];

        // Received data string.
        public StringBuilder sb = new StringBuilder();
    }
}