using Server;
using System;
using System.Windows.Forms;

namespace ServerApp
{
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