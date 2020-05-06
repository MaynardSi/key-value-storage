using System;

namespace Client
{
    public interface IClientFormView
    {
        //TODO: Finish client view abstraction

        event EventHandler NewConnection;

        event EventHandler SendPing;

        event EventHandler AddKeyValuePair;

        event EventHandler UpdateKeyValuePairList;

        event EventHandler SearchKeyValue;

        string hostIP { get; set; }
        string portNumber { get; set; }
        string addKeyInput { get; set; }
        string addValueInput { get; set; }

        void AppendLogMessage(string message);
    }
}