using Server.ServerSocket;
using Server.ServerUserInterface;
using System;

namespace ServerApp
{
    /// <summary>
    /// Serves as the Presenter module that allows the Application
    /// logic to communicate with the UI
    /// </summary>
    public class ServerUIPresenter
    {
        #region Fields

        private readonly IMainServerView _view;
        private ServerSocket server;

        #endregion Fields

        #region Constructor

        public ServerUIPresenter(IMainServerView mainView, KeyValuePairRepository repository, ServerSocket serverSocket)
        {
            _view = mainView;
            server = serverSocket;

            // Register UI Events
            mainView.ServerStarting += StartServerAsync;
            mainView.ServerEnding += StopServer;

            // Register Socket Events
            serverSocket.ServerStarted += ServerStarted;
            serverSocket.ServerStopped += ServerStopped;
            serverSocket.WaitingClient += WaitingClient;
            serverSocket.ClientConnected += ClientConnected;
            serverSocket.ClientDisconnected += ClientDisconnected;
            serverSocket.MessageReceived += MessageReceived;
            serverSocket.MessageSent += MessageSent;
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
        private async void StartServerAsync(object sender, EventArgs e)
        {
            AddLogMessage("Starting server...");
            await server.StartServer(_view.IpAddress, _view.PortNumber);
        }

        /// <summary>
        /// Server instance is stopped.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StopServer(object sender, EventArgs e)
        {
            AddLogMessage("Stopping server...");
            server.StopServer();
        }

        #endregion UIEvents

        #region SocketEvents

        /// <summary>
        /// Display to log that server successfuly started.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ServerStarted(object sender, EventArgs e)
        {
            AddLogMessage("Server successfully started...");
        }

        /// <summary>
        /// Display to log that server successfuly stopped.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ServerStopped(object sender, EventArgs e)
        {
            // Display to UI that server successfuly stopped
            AddLogMessage("Server has shutdown...");
        }

        /// <summary>
        /// Display to log that the server is awaiting a connection.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WaitingClient(object sender, EventArgs e)
        {
            AddLogMessage("Waiting for a connection... ");
        }

        /// <summary>
        /// Display to log that a client of IpAddress: Port has connected.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClientConnected(object sender, string e)
        {
            AddLogMessage($"Connected: {e}");
        }

        /// <summary>
        /// Display to log that a client has disconnected.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClientDisconnected(object sender, string e)
        {
            AddLogMessage($"Disconnected: {e}");
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
        /// Display to log the sent response.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MessageSent(object sender, string e)
        {
            AddLogMessage($"Sent: {e}");
        }

        #endregion SocketEvents
    }
}