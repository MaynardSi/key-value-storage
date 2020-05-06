using Server.ServerUserInterface;
using System;
using System.Windows.Forms;

namespace ServerApp
{
    public partial class ServerUI : Form, IMainServerView
    {
        public ServerUI()
        {
            InitializeComponent();
        }

        public event EventHandler StartServer;

        public event EventHandler StopServer;

        public event EventHandler SendPing;

        public string IpAddress
        {
            get { return ipAddressTextBox.Text as string; }
        }

        public string PortNumber
        {
            get { return portTextBox.Text as string; }
        }

        public void UpdateLog(string message)
        {
            // Thread safety implementation
            // https://stackoverflow.com/questions/142003/cross-thread-operation-not-valid-control-accessed-from-a-thread-other-than-the
            MethodInvoker action = delegate { logRichTextBox.Text += $"{ message } \n"; };
            logRichTextBox.BeginInvoke(action);
        }

        private void startServerToggleButton_CheckedChanged(object sender, EventArgs e)
        {
            if (startServerToggleButton.Checked)
            {
                StartServer?.Invoke(sender, e);
                startServerToggleButton.Text = "Stop";
                // TODO Check if client connected
                // pingButton.Enabled = true;
            }
            else
            {
                StopServer?.Invoke(sender, e);
                startServerToggleButton.Text = "Start";
                pingButton.Enabled = false;
            }
        }

        private void pingButton_Click(object sender, EventArgs e)
        {
            //TODO PING CLIENT FUNCTION
        }
    }
}