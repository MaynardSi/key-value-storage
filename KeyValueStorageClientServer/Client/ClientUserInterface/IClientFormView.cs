using System;

namespace Client
{
    public interface IClientFormView
    {
        //TODO: Finish client view abstraction

        event EventHandler EstablishConnection;

        event EventHandler DisconnectClient;

        string IpAddress { get; }
        string PortNumber { get; }
        string AddKeyInput { get; }
        string AddValueInput { get; }

        void UpdateLog(string message);

        void UpdateKeyValuePairLog(string message);

        void UpdateKeySearchResultLog(string message);

        void ClientStatusFormUpdate(ClientStatus status);
    }
}