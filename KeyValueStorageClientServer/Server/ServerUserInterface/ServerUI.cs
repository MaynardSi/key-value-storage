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

        public string IpAddress { get { return ipAddressTextBox.Text as string; } }
        public string PortNumber { get { return portTextBox.Text as string; } }

        public event EventHandler ServerStarting;

        public event EventHandler ServerEnding;

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

        private void startServerToggleButton_CheckedChanged(object sender, EventArgs e)
        {
            if (startServerToggleButton.Checked)
            {
                ServerStarting?.Invoke(sender, e);
                startServerToggleButton.Text = "Stop";
                pingButton.Enabled = true;
            }
            else
            {
                ServerEnding?.Invoke(sender, e);
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