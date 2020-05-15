using Server.ServerSocket;
using System;
using System.Windows.Forms;

namespace ServerApp
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
            var presenter = new ServerUIPresenter(mainForm, new KeyValuePairRepository(), new ServerSocket());

            Application.Run(mainForm);
        }
    }
}