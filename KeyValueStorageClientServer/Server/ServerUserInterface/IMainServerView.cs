using System;

namespace Server.ServerUserInterface
{
    public interface IMainServerView
    {
        event EventHandler StartServer;

        event EventHandler StopServer;

        event EventHandler SendPing;

        string IpAddress { get; }
        string PortNumber { get; }

        void UpdateLog(string message);
    }
}