using System;

namespace Client.ClientSocket
{
    public class ClientUIPresenter
    {
        // TODO Change to interface
        private readonly IClientFormView _view;

        private ClientSocket client;

        public ClientUIPresenter(IClientFormView mainView, ClientSocket clientSocket)
        {
            _view = mainView;
            client = clientSocket;

            mainView.EstablishConnection += EstablishConnection;
            mainView.DisconnectClient += DisconnectClient;
            //mainView.SendPing += SendPing;
            //mainView.AddKeyValuePair += AddKeyValuePair;
            //mainView.UpdateKeyValuePairList += UpdateKeyValuePairList;
            //mainView.SearchKeyValue += SearchKeyValue;

            clientSocket.ClientConnected += OnClientConnected;
            clientSocket.ClientTimeout += OnClientTimeout;
            clientSocket.ClientDisconnected += OnClientDisconnected;
            clientSocket.MessageReceived += MessageReceived;
            clientSocket.MessageSent += MessageSent;
        }

        public void AddLogMessage(string message)
        {
            _view.UpdateLog(message);
        }

        // UI Events
        private async void EstablishConnection(object sender, EventArgs e)
        {
            AddLogMessage("Establishing Connection...");
            await client.StartClientAsync(_view.IpAddress, _view.PortNumber);
        }

        private void DisconnectClient(object sender, EventArgs e)
        {
            AddLogMessage("Disconnecting...");
            client.StopClient();
        }

        private void SendPing(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void AddKeyValuePair(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void UpdateKeyValuePairList(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void SearchKeyValue(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        // Socket Events
        private void OnClientConnected(object sender, EventArgs e)
        {
            AddLogMessage("Connected to server...");
            _view.ClientStatusFormUpdate(ClientStatus.CONNECTED);
        }

        private void OnClientDisconnected(object sender, EventArgs e)
        {
            AddLogMessage("Disconnected to server...");
            _view.ClientStatusFormUpdate(ClientStatus.DISCONNECTED);
        }

        private void OnClientTimeout(object sender, EventArgs e)
        {
            AddLogMessage("Client timeout, stopping connection...");
            _view.ClientStatusFormUpdate(ClientStatus.TIMEOUT);
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