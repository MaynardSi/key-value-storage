using Client.ClientSocket;
using System;
using System.Windows.Forms;
using Client;
using Client.ClientUserInterface;

namespace ClientApp
{
    /// <summary>
    /// Main Client Program.
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

            var mainForm = new ClientUI();
            new ClientUIPresenter(mainForm, new ClientSocket());

            Application.Run(mainForm);
        }
    }
}