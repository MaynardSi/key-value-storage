using System;
using System.Windows.Forms;
using Server.ServerUserInterface;

namespace Server
{
    /// <summary>
    /// Main Server Program.
    /// </summary>
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            var mainForm = new ServerUI();

            var serverUiPresenter = new ServerUIPresenter(mainForm, new ServerSocket.ServerSocket());

            Application.Run(mainForm);
        }
    }
}