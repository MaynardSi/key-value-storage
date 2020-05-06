using Server;
using Server.ServerUserInterface;
using System;

namespace ServerApp
{
    public class ServerUIPresenter
    {
        public ServerUIPresenter(IMainServerView mainView, KeyValuePairRepository repository, ServerSocket serverSocket)
        {
            // I need to find out differnence when using _view and mainView
            _view = mainView;
            _repository = repository;
            _server = serverSocket;

            mainView.StartServer += StartServer;
            mainView.StopServer += StopServer;
            mainView.SendPing += SendPing;

            serverSocket.ServerStarted += ServerStarted;
            serverSocket.ServerStopped += ServerStopped;
            serverSocket.WaitingClient += WaitingClient;
            serverSocket.ClientConnected += ClientConnected;
            serverSocket.MessageReceived += MessageReceived;
            serverSocket.MessageSent += MessageSent;
        }

        private delegate void SafeCallDelegate(string text);

        private readonly IMainServerView _view;
        private readonly KeyValuePairRepository _repository;
        private ServerSocket _server;

        public void AddLogMessage(string message)
        {
            //string newLog = $"{_view.LogDisplay}\r\n{message}";
            //_view.LogDisplay = newLog;
            _view.UpdateLog(message);
        }

        // UI Events
        private void StartServer(object sender, EventArgs e)
        {
            //Instantiate new Server
            AddLogMessage("Starting server...");
            _server.StartServer(_view.IpAddress, _view.PortNumber);
            // Call function to start server

            //_server = new ServerSocket(this);
            //_server.StartServer(_view.IpAddress, _view.PortNumber);
        }

        private void StopServer(object sender, EventArgs e)
        {
            AddLogMessage("Stopping server...");
            _server.StopServer();
        }

        private void SendPing(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        // Socket events
        private void ServerStarted(object sender, EventArgs e)
        {
            // Display to UI that server successfuly started
            AddLogMessage("Server successfully started...");
        }

        private void ServerStopped(object sender, EventArgs e)
        {
            // Display to UI that server successfuly stopped
            AddLogMessage("Server has shutdown...");
        }

        private void WaitingClient(object sender, EventArgs e)
        {
            AddLogMessage("Waiting for a connection... ");
        }

        private void ClientConnected(object sender, EventArgs e)
        {
            AddLogMessage("Connected...");
        }

        private void MessageReceived(object sender, string e)
        {
            AddLogMessage($"Received: {e}");
        }

        private void MessageSent(object sender, string e)
        {
            AddLogMessage($"Sent: {e}");
        }
    }
}