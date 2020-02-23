using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Reflection;
using System.Security.Principal;

namespace SystemProtectedAttribute {
    static class Program {
        public static volatile bool IsDebug;
        public static bool IsAdmin = (new WindowsPrincipal(WindowsIdentity.GetCurrent())).IsInRole(WindowsBuiltInRole.Administrator);
        public static volatile bool IsArg = false;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main() {
#if DEBUG
            IsDebug = true;
#else
            IsDebug = false;
#endif
            if (Environment.GetCommandLineArgs().Length > 1) {
                if (Environment.GetCommandLineArgs()[1] == "-setattribute" || Environment.GetCommandLineArgs()[1] == "-s") { IsArg = true; } else { IsArg = false; }
            }
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new frmMain());
        }

        public static bool IsWindowsVistaOrLater {
            get { return Environment.OSVersion.Platform == PlatformID.Win32NT && Environment.OSVersion.Version >= new Version(6, 0, 6000); }
        }
    }
}