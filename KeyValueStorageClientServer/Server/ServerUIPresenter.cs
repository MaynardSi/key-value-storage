using System;
using Common;
using Server.ConnectionStatus;
using Server.ServerUserInterface;

namespace Server
{
    /// <summary>
    /// Serves as the Presenter module that allows the Application
    /// logic to communicate with the UI
    /// </summary>
    public class ServerUIPresenter
    {
        #region Fields

        private readonly IMainServerView _view;
        private readonly ServerSocket.ServerSocket _server;

        #endregion Fields

        #region Constructor

        public ServerUIPresenter(IMainServerView mainView, ServerSocket.ServerSocket serverSocket)
        {
            _view = mainView;
            _server = serverSocket;

            // Register UI Events
            mainView.ServerStarting += startServerAsync;
            mainView.ServerEnding += stopServer;

            // Register Socket Events
            serverSocket.ClientConnected += clientConnected;
            serverSocket.ClientDisconnected += clientDisconnected;
            serverSocket.MessageReceived += messageReceived;
            serverSocket.MessageSent += messageSent;

            // Subscribe to Mediator and update the Ui when the connection status has changed
            ServerConnectionStatusMediator.GetInstance().ConnectionStatusChanged += (s, e) =>
            {
                OnConnectionStatusChanged(e.ConnectionStatus);
            };
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
        /// A server instance is created and starts listening to connection attempts.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void startServerAsync(object sender, EventArgs e)
        {
            AddLogMessage("Starting server...");
            await _server.StartServer(_view.IpAddress, _view.PortNumber);
        }

        /// <summary>
        /// Server instance is stopped.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void stopServer(object sender, EventArgs e)
        {
            AddLogMessage("Stopping server...");
            _server.StopServer();
        }

        #endregion UIEvents

        #region SocketEvents

        /// <summary>
        /// Called when connection status has changed.
        /// These events are raised from the calling Socket class.
        /// </summary>
        /// <param name="status">The status.</param>
        public void OnConnectionStatusChanged(int status)
        {
            switch (status)
            {
                case ConnectionStatusEnum.ServerConstants.SERVER_STOPPED:
                    AddLogMessage("Server has shutdown...");
                    break;

                case ConnectionStatusEnum.ServerConstants.SERVER_STARTED:
                    AddLogMessage("Server successfully started...");
                    break;

                case ConnectionStatusEnum.ServerConstants.WAITING:
                    AddLogMessage("Waiting for a connection... ");
                    break;

                case ConnectionStatusEnum.ServerConstants.CLIENT_CONNECTED:
                    break;

                case ConnectionStatusEnum.ServerConstants.CLIENT_DISCONNECTED:
                    break;
            }
        }

        /// <summary>
        /// Display to log that a client of IpAddress: Port has connected.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void clientConnected(object sender, string e)
        {
            AddLogMessage($"Connected: {e}");
        }

        /// <summary>
        /// Display to log that a client has disconnected.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void clientDisconnected(object sender, string e)
        {
            AddLogMessage($"Disconnected: {e}");
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
        /// Display to log the sent response.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void messageSent(object sender, string e)
        {
            AddLogMessage($"Sent: {e}");
        }

        #endregion SocketEvents
    }
}