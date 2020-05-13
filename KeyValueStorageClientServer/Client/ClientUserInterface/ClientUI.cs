using Client;
using System;
using System.Windows.Forms;

namespace ClientApp
{
    public partial class ClientUI : Form, IClientFormView
    {
        public ClientUI()
        {
            InitializeComponent();
        }

        public string IpAddress { get { return ipAddressTextBox.Text as string; } }
        public string PortNumber { get { return portTextBox.Text as string; } }
        public string AddKeyInput { get { return addKeyTextBox.Text as string; } }
        public string AddValueInput { get { return addValueTextBox.Text as string; } }

        public event EventHandler EstablishConnection;

        public event EventHandler DisconnectClient;

        public void UpdateLog(string message)
        {
            // Thread safety implementation
            // https://stackoverflow.com/questions/142003/cross-thread-operation-not-valid-control-accessed-from-a-thread-other-than-the
            if (logRichTextBox.InvokeRequired)
            {
                MethodInvoker action = delegate { logRichTextBox.Text += $"{ message } \n"; logRichTextBox.Refresh(); };
                logRichTextBox.BeginInvoke(action);
            }
            else
            {
                logRichTextBox.Text += $"{ message } \n";
                logRichTextBox.Refresh();
            }
        }

        public void UpdateKeyValuePairLog(string message)
        {
            //TODO Fetch KVP list from server -> repository
            MethodInvoker action = delegate { keyValuePairListBox.Text += $"{ message } \n"; };
            logRichTextBox.BeginInvoke(action);
        }

        public void UpdateKeySearchResultLog(string message)
        {
            //TODO Fetch KVP from server -> repository
        }

        public void ClientStatusFormUpdate(ClientStatus status)
        {
            switch (status)
            {
                case ClientStatus.CONNECTED:
                    ClientConnectedFormUpdate();
                    break;

                case ClientStatus.DISCONNECTED:
                    ClientDisconnectedFormUpdate();
                    break;

                case ClientStatus.TIMEOUT:
                    ClientTimeoutFormUpdate();
                    break;

                default:
                    break;
            }
        }

        private void ClientConnectedFormUpdate()
        {
            if (InvokeRequired)
            {
                this.BeginInvoke(new MethodInvoker(delegate
                {
                    connectToggleButton.Enabled = true;
                    connectToggleButton.Checked = true;
                    connectToggleButton.Text = "Disconnect";
                    pingButton.Enabled = true;
                }));
            }
            else
            {
                connectToggleButton.Enabled = true;
                connectToggleButton.Checked = true;
                connectToggleButton.Text = "Disconnect";
                pingButton.Enabled = true;
            }
        }

        private void ClientDisconnectedFormUpdate()
        {
            ClientFormReset();
        }

        private void ClientTimeoutFormUpdate()
        {
            ClientFormReset();
        }

        private void ClientFormReset()
        {
            if (InvokeRequired)
            {
                this.BeginInvoke(new MethodInvoker(delegate
                {
                    connectToggleButton.Enabled = true;
                    connectToggleButton.Checked = false;
                    connectToggleButton.Text = "Connect";
                    pingButton.Enabled = false;
                }));
            }
            else
            {
                connectToggleButton.Enabled = true;
                connectToggleButton.Checked = false;
                connectToggleButton.Text = "Connect";
                pingButton.Enabled = false;
            }
        }

        public void connectToggleButton_Click(object sender, EventArgs e)
        {
            if (InvokeRequired)
            {
                this.BeginInvoke(new MethodInvoker(delegate
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
                }));
            }
            else
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
            }
        }

        private void pingButton_Click(object sender, EventArgs e)
        {
        }

        private void InvokeUI(Action action)
        {
            this.Invoke(action);
        }
    }
}