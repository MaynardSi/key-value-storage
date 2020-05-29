namespace Common
{
    public class ConnectionStatusEnum
    {
        public class ClientConstants
        {
            public const int CONNECTED = 1;
            public const int DISCONNECTED = 0;
            public const int TIMEOUT = 2;
        }

        public class ServerConstants
        {
            public const int SERVER_STOPPED = 0;
            public const int SERVER_STARTED = 1;
            public const int WAITING = 2;
            public const int CLIENT_CONNECTED = 3;
            public const int CLIENT_DISCONNECTED = 4;
            public const int TIMEOUT = 5;
        }
    }
}