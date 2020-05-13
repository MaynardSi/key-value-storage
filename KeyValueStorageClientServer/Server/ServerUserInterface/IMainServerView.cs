using System;

namespace Server.ServerUserInterface
{
    public interface IMainServerView
    {
        string IpAddress { get; }
        string PortNumber { get; }

        event EventHandler ServerStarting;

        event EventHandler ServerEnding;

        void UpdateLog(string message);
    }
}