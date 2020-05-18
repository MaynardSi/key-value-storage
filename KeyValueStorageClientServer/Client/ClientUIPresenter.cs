using Common;
using Newtonsoft.Json;
using System;
using System.Net.Sockets;
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
            mainView.EstablishConnection += establishConnectionAsync;
            mainView.DisconnectClient += disconnectClient;
            mainView.SendPing += sendPingAsync;
            mainView.AddKeyValuePair += addKeyValuePair;
            mainView.ListKeyValuePairs += listKeyValuePairs;
            mainView.SearchKeyValuePair += searchKeyValue;

            // Register Client events
            clientSocket.ClientConnected += clientConnected;
            clientSocket.ClientTimeout += clientTimeout;
            clientSocket.ClientDisconnected += clientDisconnected;
            clientSocket.MessageReceived += messageReceived;
            clientSocket.MessageSent += messageSent;
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
        private async void establishConnectionAsync(object sender, EventArgs e)
        {
            AddLogMessage("Establishing Connection...");
            try
            {
                await client.StartClientAsync(_view.IpAddress, _view.PortNumber);
            }
            catch (SocketException)
            {
                onServerNotFound();
            }
        }

        /// <summary>
        /// Client instance is stopped.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void disconnectClient(object sender, EventArgs e)
        {
            AddLogMessage("Disconnecting...");
            client.StopClient();
        }

        /// <summary>
        /// Sends a GET request to the server and a waits a response.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void searchKeyValue(object sender, string e)
        {
            // GET
            string response = await client.SendRequest(_view.IpAddress, _view.PortNumber,
                RequestResponseTypes.GET, e);
            _view.UpdateKeySearchResultLog($"[ {e} ] : [ {createResponse(response)} ]");
        }

        /// <summary>
        /// Sends a GETALL request to the server and a waits a response.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void listKeyValuePairs(object sender, EventArgs e)
        {
            //GETALL
            string response = await client.SendRequest(_view.IpAddress, _view.PortNumber,
               RequestResponseTypes.GETALL, "GETALL");
            _view.UpdateKeyValueListLog($"{createResponse(response)}");
        }

        /// <summary>
        /// Sends a SET request to the server and a waits a response.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void addKeyValuePair(object sender, (string key, string value) e)
        {
            // SET
            string response = await client.SendRequest(_view.IpAddress, _view.PortNumber,
                RequestResponseTypes.SET, $"{e.key},{e.value}");
            AddLogMessage($"Response:\n\t{createResponse(response)}");
        }

        /// <summary>
        /// Sends a PING request to the server and a waits a response.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void sendPingAsync(object sender, EventArgs e)
        {
            string response = await client.SendRequest(_view.IpAddress, _view.PortNumber,
                RequestResponseTypes.PING, "PING");
            AddLogMessage($"Response:\n\t{createResponse(response)}");
        }

        private string createResponse(string response)
        {
            try
            {
                Response deserializedResponse = JsonConvert.DeserializeObject<Response>(response);
                return deserializedResponse.Message;
            }
            catch (Exception)
            {
                return "ERROR: NO RESPONSE";
            }
        }

        #endregion UIEvents

        #region Socket Events

        /// <summary>
        /// Notifies UI thread that the server is not available.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onServerNotFound()
        {
            AddLogMessage("TIMEOUT: Server not available...");
            _view.ClientStatusFormUpdate(ClientStatus.DISCONNECTED);
        }

        /// <summary>
        /// Notifies UI thread that the client has connected.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void clientConnected(object sender, EventArgs e)
        {
            AddLogMessage("Connected to server...");
            _view.ClientStatusFormUpdate(ClientStatus.CONNECTED);
        }

        /// <summary>
        /// Notifies UI thread that the client has disconnected.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void clientDisconnected(object sender, EventArgs e)
        {
            AddLogMessage("Disconnected to server...");
            _view.ClientStatusFormUpdate(ClientStatus.DISCONNECTED);
        }

        /// <summary>
        /// Notifies UI thread that the client has timed out.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void clientTimeout(object sender, EventArgs e)
        {
            AddLogMessage("CLIENT TIMEOUT: stopping connection...");
            _view.ClientStatusFormUpdate(ClientStatus.TIMEOUT);
        }

        /// <summary>
        /// Display to log the received message.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void messageReceived(object sender, string e)
        {
            AddLogMessage($"Received: {e}");
        }

        /// <summary>
        /// Display to log the sent request.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void messageSent(object sender, string e)
        {
            AddLogMessage($"Sent: {e}");
        }

        #endregion Socket Events
    }
}