using Common;
using Newtonsoft.Json;
using System;
using static Common.RequestResponseEnum;

namespace Client.ClientSocket
{
    /// <summary>
    /// Serves as the Presenter module that allows the Application
    /// logic to communicate with the UI
    /// </summary>
    public class ClientUIPresenter
    {
        #region Fields

        private readonly IClientFormView _view;
        private ClientSocket client;

        #endregion Fields

        #region Constructor

        public ClientUIPresenter(IClientFormView mainView, ClientSocket clientSocket)
        {
            _view = mainView;
            client = clientSocket;

            // Register UI events
            mainView.EstablishConnection += EstablishConnectionAsync;
            mainView.DisconnectClient += DisconnectClient;
            mainView.SendPing += SendPingAsync;
            mainView.AddKeyValuePair += AddKeyValuePair;
            mainView.ListKeyValuePairs += ListKeyValuePairs;
            mainView.SearchKeyValuePair += SearchKeyValue;

            // Register Client events
            clientSocket.ClientConnected += OnClientConnected;
            clientSocket.ClientTimeout += OnClientTimeout;
            clientSocket.ClientDisconnected += OnClientDisconnected;
            clientSocket.MessageReceived += MessageReceived;
            clientSocket.MessageSent += MessageSent;
        }

        #endregion Constructor

        #region Methods

        /// <summary>
        /// Appends the Log textbox with the given message.
        /// </summary>
        /// <param name="message"></param>
        public void AddLogMessage(string message)
        {
            _view.UpdateLog(message);
        }

        #endregion Methods

        #region UIEvents

        /// <summary>
        /// Client is started and attempts connection with Server.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void EstablishConnectionAsync(object sender, EventArgs e)
        {
            AddLogMessage("Establishing Connection...");
            await client.StartClientAsync(_view.IpAddress, _view.PortNumber);
        }

        /// <summary>
        /// Client instance is stopped.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DisconnectClient(object sender, EventArgs e)
        {
            AddLogMessage("Disconnecting...");
            client.StopClient();
        }

        /// <summary>
        /// Sends a GET request to the server and a waits a response.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void SearchKeyValue(object sender, string e)
        {
            // GET
            string response = await client.SendRequest(_view.IpAddress, _view.PortNumber,
                RequestResponseTypes.GET, e);
            Response deserializedResponse = JsonConvert.DeserializeObject<Response>(response);
            string getResponse = deserializedResponse.Message;
            _view.UpdateKeySearchResultLog($"\nKey resulted in value {getResponse}");
        }

        /// <summary>
        /// Sends a GETALL request to the server and a waits a response.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void ListKeyValuePairs(object sender, EventArgs e)
        {
            //GETALL
            string response = await client.SendRequest(_view.IpAddress, _view.PortNumber,
               RequestResponseTypes.GETALL, "GETALL");
            Response deserializedResponse = JsonConvert.DeserializeObject<Response>(response);
            string getResponse = deserializedResponse.Message;
            _view.UpdateKeyValueListLog($"{getResponse}");
        }

        /// <summary>
        /// Sends a SET request to the server and a waits a response.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void AddKeyValuePair(object sender, (string key, string value) e)
        {
            // SET
            string response = await client.SendRequest(_view.IpAddress, _view.PortNumber,
                RequestResponseTypes.SET, $"{e.key},{e.value}");
            Response deserializedResponse = JsonConvert.DeserializeObject<Response>(response);
            string setResponse = deserializedResponse.Message;
            AddLogMessage($"Response:\n\t{setResponse}");
        }

        /// <summary>
        /// Sends a PING request to the server and a waits a response.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void SendPingAsync(object sender, EventArgs e)
        {
            string response = await client.SendRequest(_view.IpAddress, _view.PortNumber,
                RequestResponseTypes.PING, "PING");
            Response deserializedResponse = JsonConvert.DeserializeObject<Response>(response);
            string pingResponse = deserializedResponse.Message;
            AddLogMessage($"Response:\n\t{pingResponse}");
        }

        #endregion UIEvents

        #region Socket Events

        /// <summary>
        /// Notifies UI thread that the client has connected.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnClientConnected(object sender, EventArgs e)
        {
            AddLogMessage("Connected to server...");
            _view.ClientStatusFormUpdate(ClientStatus.CONNECTED);
        }

        /// <summary>
        /// Notifies UI thread that the client has disconnected.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnClientDisconnected(object sender, EventArgs e)
        {
            AddLogMessage("Disconnected to server...");
            _view.ClientStatusFormUpdate(ClientStatus.DISCONNECTED);
        }

        /// <summary>
        /// Notifies UI thread that the client has timed out.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnClientTimeout(object sender, EventArgs e)
        {
            AddLogMessage("Client timeout, stopping connection...");
            _view.ClientStatusFormUpdate(ClientStatus.TIMEOUT);
        }

        /// <summary>
        /// Display to log the received message.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MessageReceived(object sender, string e)
        {
            AddLogMessage($"Received: {e}");
        }

        /// <summary>
        /// Display to log the sent request.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MessageSent(object sender, string e)
        {
            AddLogMessage($"Sent: {e}");
        }

        #endregion Socket Events
    }
}