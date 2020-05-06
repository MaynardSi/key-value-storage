using System.Windows.Forms;

namespace Client
{
    internal class ClientController
    {
        public ClientController(Form clientUIForm)
        {
            this.clientUI = clientUIForm;
        }

        private Form clientUI { get; set; }

        public static void updateLog(string stringToAdd)
        {
            //clientUI.get
        }
    }
}