using System;

namespace Client
{
    /// <summary>
    /// Client Form abstraction.
    /// </summary>
    public interface IClientFormView
    {
        event EventHandler EstablishConnection;

        event EventHandler DisconnectClient;

        event EventHandler SendPing;

        event EventHandler<(string key, string value)> AddKeyValuePair;

        event EventHandler<string> SearchKeyValuePair;

        event EventHandler ListKeyValuePairs;

        string IpAddress { get; }
        string PortNumber { get; }

        void ShowMessage(string message);

        void UpdateLog(string message);

        void UpdateKeyValueListLog(string message);

        void UpdateKeySearchResultLog(string message);

        void ClientStatusFormUpdate(int status);
    }
}