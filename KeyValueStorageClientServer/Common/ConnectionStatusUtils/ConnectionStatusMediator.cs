using System;

namespace Common.ConnectionStatusUtils
{
    /// <summary>
    /// Singleton mediator class for notifying event subscribers
    /// of change in client connection status. Partially lazy,
    /// thread safe implementation.
    /// </summary>
    public abstract class ConnectionStatusMediator<T> where T : ConnectionStatusMediator<T>, new()
    {
        // Singleton static members

        private static readonly T _instance = new T();

        public static T GetInstance()
        {
            return _instance;
        }

        // Instance functionality
        public event EventHandler<ConnectionStatusChangedEventArgs> ConnectionStatusChanged;

        public void OnStatusChanged(object sender, int connectionStatus)
        {
            EventHandler<ConnectionStatusChangedEventArgs> connectionStatusChangedDelegate
                = ConnectionStatusChanged;

            connectionStatusChangedDelegate?.Invoke(sender,
                new ConnectionStatusChangedEventArgs { ConnectionStatus = connectionStatus });
        }
    }
}