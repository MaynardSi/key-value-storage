using Server.ServerUserInterface;
using System;
using System.Windows.Forms;

namespace ServerApp
{
    public partial class ServerUI : Form, IMainServerView
    {
        #region Constructor

        public ServerUI()
        {
            InitializeComponent();
        }

        #endregion Constructor

        #region Fields

        public string IpAddress { get { return ipAddressTextBox.Text as string; } }
        public string PortNumber { get { return portTextBox.Text as string; } }

        #endregion Fields

        #region Events

        public event EventHandler ServerStarting;

        public event EventHandler ServerEnding;

        #endregion Events

        #region Methods

        /// <summary>
        /// Appends the log with the given message.
        /// </summary>
        /// <param name="message"></param>
        public void UpdateLog(string message)
        {
            InvokeUI(() =>
            {
                logRichTextBox.Text += $"{ message } \n";
                logRichTextBox.Refresh();
            });
        }

        /// <summary>
        /// Thread safety implementation
        /// https://stackoverflow.com/questions/142003/cross-thread-operation-not-valid-control-accessed-from-a-thread-other-than-the
        /// </summary>
        /// <param name="action"></param>
        private void InvokeUI(Action action)
        {
            this.Invoke(action);
        }

        #endregion Methods

        #region button events

        /// <summary>
        /// Button event to start/stop the server.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void startServerToggleButton_CheckedChanged(object sender, EventArgs e)
        {
            if (startServerToggleButton.Checked)
            {
                ServerStarting?.Invoke(sender, e);
                InvokeUI(() =>
                {
                    startServerToggleButton.Text = "Stop";
                    clearButton.Enabled = true;
                });
            }
            else
            {
                ServerEnding?.Invoke(sender, e);
                InvokeUI(() =>
                {
                    startServerToggleButton.Text = "Start";
                    clearButton.Enabled = false;
                });
            }
        }

        /// <summary>
        /// Button event to clear the existing log.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void clearButton_Click(object sender, EventArgs e)
        {
            InvokeUI(() =>
            {
                logRichTextBox.Text = String.Empty;
                logRichTextBox.Refresh();
            });
        }

        #endregion button events
    }
}