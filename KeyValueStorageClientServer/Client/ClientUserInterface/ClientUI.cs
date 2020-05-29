using System;
using System.Windows.Forms;
using Common;

namespace Client.ClientUserInterface
{
    public partial class ClientUI : Form, IClientFormView
    {
        #region Constructor

        public ClientUI()
        {
            InitializeComponent();
        }

        #endregion Constructor

        #region Properties

        public string IpAddress { get { return ipAddressTextBox.Text; } }
        public string PortNumber { get { return portTextBox.Text; } }

        #endregion Properties

        #region Events

        public event EventHandler EstablishConnection;

        public event EventHandler DisconnectClient;

        public event EventHandler SendPing;

        public event EventHandler<(string key, string value)> AddKeyValuePair;

        public event EventHandler<string> SearchKeyValuePair;

        public event EventHandler ListKeyValuePairs;

        #endregion Events

        #region Methods

        /// <summary>
        /// Displays a MessageBox containing the message string.
        /// </summary>
        /// <param name="message"></param>
        public void ShowMessage(string message)
        {
            InvokeUI(() =>
            {
                MessageBox.Show(message);
            });
        }

        /// <summary>
        /// Appends the message to the Client log.
        /// </summary>
        /// <param name="message"></param>
        public void UpdateLog(string message)
        {
            InvokeUI(() =>
            {
                logRichTextBox.Text += $"{ message }\n";
                logRichTextBox.Refresh();
            });
        }

        /// <summary>
        /// Appends the Key Value Pair string List message to the List Log
        /// </summary>
        /// <param name="message"></param>
        public void UpdateKeyValueListLog(string message)
        {
            InvokeUI(() =>
            {
                keyValuePairListBox.Text = String.Empty;
                keyValuePairListBox.Text += $"{ message }\n";
                keyValuePairListBox.Refresh();
            });
        }

        /// <summary>
        /// Displays the search result to the search result log
        /// </summary>
        /// <param name="message"></param>
        public void UpdateKeySearchResultLog(string message)
        {
            InvokeUI(() =>
            {
                keySearchResultRichTextBox.Text = String.Empty;
                keySearchResultRichTextBox.Text = $"{ message }\n";
                keySearchResultRichTextBox.Refresh();
            });
        }

        /// <summary>
        /// Updates UI form based on the client status.
        /// </summary>
        /// <param name="status"></param>
        public void ClientStatusFormUpdate(int status)
        {
            switch (status)
            {
                case ConnectionStatusEnum.ClientConstants.CONNECTED:
                    ClientConnectedFormUpdate();
                    break;

                case ConnectionStatusEnum.ClientConstants.DISCONNECTED:
                    ClientDisconnectedFormUpdate();
                    break;

                case ConnectionStatusEnum.ClientConstants.TIMEOUT:
                    ClientTimeoutFormUpdate();
                    break;
            }
        }

        /// <summary>
        /// If the client is connected, Enable form buttons.
        /// </summary>
        private void ClientConnectedFormUpdate()
        {
            InvokeUI(() =>
            {
                connectToggleButton.Enabled = true;
                connectToggleButton.Checked = true;
                connectToggleButton.Text = @"Disconnect";
                pingButton.Enabled = true;
                addKeyValuePairButton.Enabled = true;
                keySearchButton.Enabled = true;
                getAllValuesButton.Enabled = true;
            });
        }

        /// <summary>
        /// If the client is disconnected, disable form buttons.
        /// </summary>
        private void ClientDisconnectedFormUpdate()
        {
            ClientFormReset();
        }

        /// <summary>
        /// If the client has timed out, disable form buttons.
        /// </summary>
        private void ClientTimeoutFormUpdate()
        {
            ClientFormReset();
        }

        /// <summary>
        /// Resets the form buttons back to its original state
        /// </summary>
        private void ClientFormReset()
        {
            InvokeUI(() =>
            {
                connectToggleButton.Enabled = true;
                connectToggleButton.Checked = false;
                connectToggleButton.Text = @"Connect";
                pingButton.Enabled = false;
                addKeyValuePairButton.Enabled = false;
                keySearchButton.Enabled = false;
                getAllValuesButton.Enabled = false;
            });
        }

        /// <summary>
        /// Safely update UI elements by returning to the UI thread
        /// before updating elements.
        /// </summary>
        /// <param name="action"></param>
        private void InvokeUI(Action action)
        {
            this.Invoke(action);
        }

        #endregion Methods

        #region Button events

        /// <summary>
        /// Button event that enables/disables the client.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void ConnectToggleButton_Click(object sender, EventArgs e)
        {
            InvokeUI(() =>
            {
                if (connectToggleButton.Checked)
                {
                    connectToggleButton.Enabled = false;
                    EstablishConnection?.Invoke(sender, e);
                }
                else
                {
                    connectToggleButton.Enabled = false;
                    DisconnectClient?.Invoke(sender, e);
                }
            });
        }

        /// <summary>
        /// Button event for sending a GET request to the server.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void KeySearchButton_Click(object sender, EventArgs e)
        {
            string searchKey = keySearchTextBox.Text;
            SearchKeyValuePair?.Invoke(sender, searchKey);
        }

        /// <summary>
        /// Button event for sending a SET request to the server.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void AddKeyValuePairButton_Click(object sender, EventArgs e)
        {
            string addKey = addKeyTextBox.Text;
            string addValue = addValueTextBox.Text;
            AddKeyValuePair?.Invoke(sender, (addKey, addValue));
        }

        /// <summary>
        /// Button event for sending a GETALL request to the server.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void GetAllValuesButton_Click(object sender, EventArgs e)
        {
            ListKeyValuePairs?.Invoke(sender, e);
        }

        /// <summary>
        /// Button event for sending a PING request to the server.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void PingButton_Click(object sender, EventArgs e)
        {
            SendPing?.Invoke(sender, e);
        }

        #endregion Button events
    }
}