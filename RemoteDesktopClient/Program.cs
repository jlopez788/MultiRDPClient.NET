using Microsoft.VisualBasic.ApplicationServices;
using System;
using System.Windows.Forms;

namespace MultiRemoteDesktopClient
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            Application.SetCompatibleTextRenderingDefault(false);
            Application.EnableVisualStyles();

            SingleInstanceController controler = new SingleInstanceController();
            controler.Run(Environment.GetCommandLineArgs());
        }
    }

    public class SingleInstanceController : WindowsFormsApplicationBase
    {
        public SingleInstanceController()
        {
            IsSingleInstance = true;

            StartupNextInstance += new StartupNextInstanceEventHandler(SingleInstanceController_StartupNextInstance);
        }

        private void SingleInstanceController_StartupNextInstance(object sender, StartupNextInstanceEventArgs e)
        {
            RemoteDesktopClient rdc = (RemoteDesktopClient)MainForm;

            string[] args = new string[e.CommandLine.Count];
            e.CommandLine.CopyTo(args, 0);

            rdc.DoArguments(args);
        }

        protected override void OnCreateMainForm()
        {
            MainForm = new RemoteDesktopClient();
        }
    }
}