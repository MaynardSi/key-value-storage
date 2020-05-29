using System;
using System.Net.Sockets;
using Client.ConnectionStatus;
using Common;

namespace Client
{
    /// <summary>
    /// Serves as the Presenter module that allows the Application
    /// logic to communicate with the UI
    /// </summary>
    public class ClientUIPresenter
    {
        #region Fields

        private readonly IClientFormView _view;
        private readonly ClientSocket.ClientSocket _client;

        #endregion Fields

        #region Constructor

        public ClientUIPresenter(IClientFormView mainView, ClientSocket.ClientSocket clientSocket)
        {
            _view = mainView;
            _client = clientSocket;

            // Register UI events
            mainView.EstablishConnection += establishConnectionAsync;
            mainView.DisconnectClient += disconnectClient;
            mainView.SendPing += sendPingAsync;
            mainView.AddKeyValuePair += addKeyValuePair;
            mainView.ListKeyValuePairs += listKeyValuePairs;
            mainView.SearchKeyValuePair += searchKeyValue;

            // Register Client events
            clientSocket.MessageReceived += messageReceived;
            clientSocket.MessageSent += messageSent;

            // Subscribe to Mediator and update the Ui when the connection status has changed
            ClientConnectionStatusMediator.GetInstance().ConnectionStatusChanged += (s, e) =>
            {
                OnConnectionStatusChanged(e.ConnectionStatus);
            };
        }

        #endregion Constructor

        #region Methods

        /// <summary>
        /// Appends the Log text box with the given message.
        /// </summary>
        /// <param name="message"></param>
        public void AddLogMessage(string message)
        {
            _view.UpdateLog(message);
        }

        /// <summary>
        /// Displays a message box containing the message.
        /// </summary>
        /// <param name="message"></param>
        public void ShowMessageBox(string message)
        {
            _view.ShowMessage(message);
        }

        /// <summary>
        /// A try catch blocks to be used by request events to handle
        /// Exceptions such as TIMEOUT.
        /// </summary>
        /// <param name="function">The function.</param>
        public void NewRequestErrorHandling(Action function)
        {
            try
            {
                function();
            }
            catch (Exception e)
            {
                AddLogMessage(e.Message);
            }
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
                await _client.StartClientAsync(_view.IpAddress, _view.PortNumber);
            }
            catch (SocketException)
            {
                serverNotFound();
            }
        }

        /// <summary>
        /// Client instance is stopped.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void disconnectClient(object sender, EventArgs e)
        {
            NewRequestErrorHandling(() =>
            {
                AddLogMessage("Disconnecting...");
                _client.StopClient();
            });
        }

        /// <summary>
        /// Sends a GET request to the server and a waits a response.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void searchKeyValue(object sender, string e)
        {
            NewRequestErrorHandling(async () =>
            {
                string response = await _client.SendRequest("GET", e);
                _view.UpdateKeySearchResultLog($"[ {e} ] : [ {MessageWrapper.GetResponseMessaage(response)} ]");
            });
        }

        /// <summary>
        /// Sends a GETALL request to the server and a waits a response.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void listKeyValuePairs(object sender, EventArgs e)
        {
            NewRequestErrorHandling(async () =>
            {
                string response = await _client.SendRequest("GETALL", "GETALL");
                _view.UpdateKeyValueListLog($"{MessageWrapper.GetResponseMessaage(response)}");
            });
        }

        /// <summary>
        /// Sends a SET request to the server and a waits a response.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void addKeyValuePair(object sender, (string key, string value) e)
        {
            NewRequestErrorHandling(async () =>
            {
                string response = await _client.SendRequest("SET", $"{e.key},{e.value}");
                ShowMessageBox($"Response: {MessageWrapper.GetResponseMessaage(response)}");
            });
        }

        /// <summary>
        /// Sends a PING request to the server and a waits a response.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void sendPingAsync(object sender, EventArgs e)
        {
            NewRequestErrorHandling(async () =>
            {
                string response = await _client.SendRequest("PING", "PING");
                ShowMessageBox($"Response: {MessageWrapper.GetResponseMessaage(response)}");
            });
        }

        /// <summary>
        /// Notifies UI thread that the server is not available.
        /// </summary>
        private void serverNotFound()
        {
            AddLogMessage("Server not available, Disconnecting...");
            _view.ClientStatusFormUpdate(ConnectionStatusEnum.ClientConstants.DISCONNECTED);
        }

        #endregion UIEvents

        #region Socket Events

        /// <summary>
        /// Called when connection status has changed.
        /// These events are raised from the calling Socket class.
        /// </summary>
        /// <param name="status">The status.</param>
        public void OnConnectionStatusChanged(int status)
        {
            switch (status)
            {
                case ConnectionStatusEnum.ClientConstants.CONNECTED:
                    AddLogMessage("Connected to server...");
                    _view.ClientStatusFormUpdate(ConnectionStatusEnum.ClientConstants.CONNECTED);
                    break;

                case ConnectionStatusEnum.ClientConstants.DISCONNECTED:
                    AddLogMessage("Disconnected to server...");
                    _view.ClientStatusFormUpdate(ConnectionStatusEnum.ClientConstants.DISCONNECTED);
                    break;

                case ConnectionStatusEnum.ClientConstants.TIMEOUT:
                    AddLogMessage("CLIENT TIMEOUT: stopping connection...");
                    _view.ClientStatusFormUpdate(ConnectionStatusEnum.ClientConstants.TIMEOUT);
                    break;
            }
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