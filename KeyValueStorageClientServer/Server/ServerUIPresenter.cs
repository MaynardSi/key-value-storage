using Server.ServerSocket;
using Server.ServerUserInterface;
using System;

namespace ServerApp
{
    public class ServerUIPresenter
    {
        private readonly IMainServerView _view;
        private readonly KeyValuePairRepository _repository;
        private ServerSocket _server;

        public ServerUIPresenter(IMainServerView mainView, KeyValuePairRepository repository, ServerSocket serverSocket)
        {
            // I need to find out differnence when using _view and mainView
            _view = mainView;
            _repository = repository;
            _server = serverSocket;

            mainView.ServerStarting += StartServerAsync;
            mainView.ServerEnding += StopServer;
            //mainView.SendPing += SendPing;

            serverSocket.ServerStarted += ServerStarted;
            serverSocket.ServerStopped += ServerStopped;
            serverSocket.WaitingClient += WaitingClient;
            serverSocket.ClientConnected += ClientConnected;
            serverSocket.MessageReceived += MessageReceived;
            serverSocket.MessageSent += MessageSent;
        }

        public void AddLogMessage(string message)
        {
            _view.UpdateLog(message);
        }

        // UI Events
        private async void StartServerAsync(object sender, EventArgs e)
        {
            AddLogMessage("Starting server...");
            // Discard variable (_) to surpress warning 4014 (Because this call is not awaited,
            // execution of the current method continues before the call is completed.
            // Consider applying the 'await' operator to the result of the call.)
            await _server.StartServerAsync(_view.IpAddress, _view.PortNumber);
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