using System;
using System.Windows.Forms;
using PatternRecognition.FingerprintRecognition.Applications;

namespace VerificationExperimenter
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new FMExperimenterForm());
        }
    }
}
