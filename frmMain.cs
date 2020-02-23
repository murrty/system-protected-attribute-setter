using Ookii.Dialogs;
using System;
using System.Drawing;
using System.Linq;
using System.Security.Permissions;
using System.Windows.Forms;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Security;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using System.Threading;

namespace SystemProtectedAttribute {
    public partial class frmMain : Form {
        int IndexNull = 0;
        int IndexFinished = 1;
        int IndexFailed = 2;

        Thread RunArguments;
        bool ArgSetHidden = false;       // If argument wants to hide files
        bool ActiveFlagging = false;    // If flagging is currently underway
        enum ExitCode : int {
            Successful = 0,
            CommandLineError = 1,
            DoesNotExist = 2,
            Unimplemented = 3,
        };
        List<int> ExitStatus = new List<int>(); // List of ExitStatus, per lvFiles indexing
        string[] ExitCodeString = { "Item was set as {0} successfully",             // ExitCode[0]
                                    "Item failed to be set as {0}, command error",  // ExitCode[1]
                                    "Item failed to be set as {0}, does not exist", // ExitCode[2]
                                    "Error code 3 not implemented"};                // ExitCode[3]



        System.Reflection.Assembly ResolveResources(object sender, ResolveEventArgs args) {
            string ParseResources = args.Name.Contains(',') ? args.Name.Substring(0, args.Name.IndexOf(',')) : args.Name.Replace(".dll", "");
            ParseResources = ParseResources.Replace(".", "_");

            if (ParseResources.EndsWith("_resources")) { return null; }

            System.Resources.ResourceManager ResourceManager = new System.Resources.ResourceManager(GetType().Namespace + ".Properties.Resources", System.Reflection.Assembly.GetExecutingAssembly());

            byte[] ResourceBytes = (byte[])ResourceManager.GetObject(ParseResources);

            return System.Reflection.Assembly.Load(ResourceBytes);
        }

        public frmMain() {
            AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(ResolveResources);
            InitializeComponent();
        }

        private void tdNotification_HyperlinkClicked(object sender, HyperlinkClickedEventArgs e) {
            Process.Start(e.Href);
        }

        private void frmMain_Load(object sender, EventArgs e) {
            if ((new WindowsPrincipal(WindowsIdentity.GetCurrent())).IsInRole(WindowsBuiltInRole.Administrator)) {
                tdNotification.MainIcon = TaskDialogIcon.Information;
            }

            if (!Program.IsArg) { RunAttributeSetter(); }
        }
        private void frmMain_Shown(object sender, EventArgs e) {
            if (Program.IsArg) {
                RunArguments = new Thread(() => { RunAttributeSetterArgs(Environment.GetCommandLineArgs()); });
                RunArguments.Start();
            }
        }

        private void btnClose_Click(object sender, EventArgs e) {
            if (ActiveFlagging) {
                if (MessageBox.Show("Files are being flagged, abort?", "", MessageBoxButtons.YesNo) == DialogResult.No) { return; }
                else { RunArguments.Abort(); }
            }

            Environment.Exit(0);
        }
        private void tvFiles_AfterSelect(object sender, TreeViewEventArgs e) {
            string hidden = "null";
            if (ArgSetHidden) {
                hidden = "hidden";
            }
            else {
                hidden = "visible";
            }

            int SelectedIndex = tvFiles.SelectedNode.Index;

            switch (ExitStatus[SelectedIndex]) {
                case -1:
                    lbStatus.Text = "This item has not been processed yet";
                    break;
                case 0:
                    lbStatus.Text = string.Format(ExitCodeString[0], hidden);
                    break;
                case 1:
                    lbStatus.Text = string.Format(ExitCodeString[1], hidden);
                    break;
                case 2:
                    lbStatus.Text = string.Format(ExitCodeString[2], hidden);
                    break;
                default:
                    lbStatus.Text = "This item does not have a status code";
                    break;
            }
        }

        private void RunAttributeSetter() {
            if (Program.IsWindowsVistaOrLater) {
                this.Visible = false;
                this.Opacity = 0;
                this.ShowInTaskbar = false;
retry:
                if (!Program.IsAdmin) { System.Media.SystemSounds.Exclamation.Play(); }
                tdNotification.WindowTitle = Application.ProductName;
                if (Program.IsAdmin) {
                    tdNotification.WindowTitle = tdNotification.WindowTitle + " (Administrator)";
                }
                TaskDialogButton clickedButton = tdNotification.ShowDialog(this);
                #region Hide file(s)
                if (clickedButton == HideFile) {
                    using (VistaOpenFileDialog ofd = new VistaOpenFileDialog()) {
                        ofd.Multiselect = true;
                        ofd.Title = "Select a file to hide...";
                        if (ofd.ShowDialog() == DialogResult.OK) {
                            string ArgumentBuffer = "-s true \"";
                            if (ofd.FileNames.Length > 1) {
                                for (int i = 0; i < ofd.FileNames.Length; i++) {
                                    ArgumentBuffer += ofd.FileNames[i] + "|";
                                }
                                ArgumentBuffer = ArgumentBuffer.Trim('|') + "\"";
                            }
                            else {
                                ArgumentBuffer += ofd.FileName;
                            }

                            Process CMD = new Process();
                            CMD.StartInfo.FileName = Environment.CurrentDirectory + "\\" + AppDomain.CurrentDomain.FriendlyName;
                            CMD.StartInfo.Arguments = ArgumentBuffer;
                            CMD.StartInfo.Verb = "runas";
                            CMD.Start();
                            CMD.WaitForExit();
                            Environment.Exit(0);
                        }
                        else { goto retry; }
                    }
                }
                #endregion
                #region Hide folder(s)
                else if (clickedButton == HideFolder) {
                    using (VistaFolderBrowserDialog fbd = new VistaFolderBrowserDialog()) {
                        List<string> Directories = new List<string>();
                    openfdagain:
                        if (Directories.Count > 0) {
                            fbd.Description = "Press cancel to hide folders";
                            fbd.UseDescriptionForTitle = false;
                        }
                        else {
                            fbd.Description = "Select a folder to hide...";
                            fbd.UseDescriptionForTitle = true;
                        }
                        if (fbd.ShowDialog() == DialogResult.OK) {
                            if (!Directories.Contains(fbd.SelectedPath)) {
                                Directories.Add(fbd.SelectedPath);
                            }
                            goto openfdagain;
                        }
                        else {
                            if (Directories.Count > 0) {
                                string ArgumentBuffer = "-s true \"";
                                for (int i = 0; i < Directories.Count; i++) {
                                    ArgumentBuffer += Directories[i] + "|";
                                }
                                ArgumentBuffer = ArgumentBuffer.Trim('|') + "\"";
                                

                                Process CMD = new Process();
                                CMD.StartInfo.FileName = Environment.CurrentDirectory + "\\" + AppDomain.CurrentDomain.FriendlyName;
                                CMD.StartInfo.Arguments = ArgumentBuffer;
                                CMD.StartInfo.Verb = "runas";
                                CMD.Start();
                                CMD.WaitForExit();
                                Environment.Exit(0);

                                Environment.Exit(0);
                            }
                            else {
                                goto retry;
                            }
                        }
                    }
                }
                #endregion
                #region Show file(s)
                else if (clickedButton == ShowFile) {
                    using (VistaFolderBrowserDialog fbd = new VistaFolderBrowserDialog()) {
                        fbd.Description = "Select a file's directory to reveal...";
                        fbd.UseDescriptionForTitle = true;
                        if (fbd.ShowDialog() == DialogResult.OK) {
                            using (InputDialog id = new InputDialog()) {
                                id.MainInstruction = string.Format("Enter the name of the {0} you want to reveal.\nYou can separate inputs with \"|\".", "file(s)");
                                id.WindowTitle = "Reveal hidden file";
                                if (id.ShowDialog(this) == DialogResult.OK) {
                                    string ArgumentBuffer = "-s false \"";
                                    if (id.Input.Contains("|")) {
                                        string[] ItemCount = id.Input.Split('|');
                                        for (int i = 0; i < ItemCount.Length; i++) {
                                            ArgumentBuffer += fbd.SelectedPath + "\\" + ItemCount[i].Trim(' ') + "|";
                                        }
                                        ArgumentBuffer = ArgumentBuffer.Trim('|') + "\"";
                                    }
                                    else {
                                        ArgumentBuffer += fbd.SelectedPath + "\\" + id.Input;
                                    }

                                    Process CMD = new Process();
                                    CMD.StartInfo.FileName = Environment.CurrentDirectory + "\\" + AppDomain.CurrentDomain.FriendlyName;
                                    CMD.StartInfo.Arguments = ArgumentBuffer;
                                    CMD.StartInfo.Verb = "runas";
                                    CMD.Start();
                                    CMD.WaitForExit();
                                    Environment.Exit(0);
                                }
                            }
                        }
                        else { goto retry; }
                    }
                }
                #endregion
                #region Show folder(s)
                else if (clickedButton == ShowFolder) {
                    using (VistaFolderBrowserDialog fbd = new VistaFolderBrowserDialog()) {
                        fbd.Description = "Select a folder's directory to reveal...";
                        fbd.UseDescriptionForTitle = true;
                        if (fbd.ShowDialog() == DialogResult.OK) {
                            using (InputDialog id = new InputDialog()) {
                                id.MainInstruction = string.Format("Enter the name of the {0} you want to reveal.\nYou can separate inputs with \"|\".", "folder(s)");
                                id.WindowTitle = "Reveal hidden file";
                                if (id.ShowDialog(this) == DialogResult.OK) {
                                    string ArgumentBuffer = "-s false \"";
                                    if (id.Input.Contains("|")) {
                                        string[] ItemCount = id.Input.Split('|');
                                        for (int i = 0; i < ItemCount.Length; i++) {
                                            ArgumentBuffer += fbd.SelectedPath + "\\" + ItemCount[i].Trim(' ') + "|";
                                        }
                                        ArgumentBuffer = ArgumentBuffer.Trim('|') + "\"";
                                    }
                                    else {
                                        ArgumentBuffer += fbd.SelectedPath + "\\" + id.Input;
                                    }

                                    Process CMD = new Process();
                                    CMD.StartInfo.FileName = Environment.CurrentDirectory + "\\" + AppDomain.CurrentDomain.FriendlyName;
                                    CMD.StartInfo.Arguments = ArgumentBuffer;
                                    CMD.StartInfo.Verb = "runas";
                                    CMD.Start();
                                    CMD.WaitForExit();
                                    Environment.Exit(0);
                                }
                            }
                        }
                        else { goto retry; }
                    }
                }
                #endregion
                else {
                    Environment.Exit(0);
                }
            }
        }
        private void RunAttributeSetterLegacy() {
            if (Program.IsWindowsVistaOrLater) {
                this.Visible = false;
                this.Opacity = 0;
                this.ShowInTaskbar = false;
retry:
                TaskDialogButton clickedButton = tdNotification.ShowDialog(this);
                #region Hide file(s)
                if (clickedButton == HideFile) {
                    using (VistaOpenFileDialog ofd = new VistaOpenFileDialog()) {
                        ofd.Multiselect = true;
                        ofd.Title = "Select a file to hide...";
                        if (ofd.ShowDialog() == DialogResult.OK) {
                            bool OneSet = false;
                            bool OneFailed = false;
                            int i = 0;

                            do {
                                //if (SetAttribute(true, ofd.FileNames[i])) { OneSet = true; }
                                //else { OneFailed = true; }
                                i++;
                            } while (i < ofd.FileNames.Length);

                            if (OneSet && !OneFailed) { MessageBox.Show("Your file(s) have been set as hidden."); }
                            else if (OneSet && OneFailed) { MessageBox.Show("Some of your files have been set as hidden, but some have failed."); }
                            else if (!OneSet && OneFailed) { MessageBox.Show("None of your file(s) were set as hidden due to an error."); }
                            else { MessageBox.Show("No file(s) have even been attempted to be set as hidden."); }

                        }
                        else { goto retry; }
                    }
                }
                #endregion
                #region Hide folder
                else if (clickedButton == HideFolder) {
                    using (VistaFolderBrowserDialog fbd = new VistaFolderBrowserDialog()) {
                        fbd.Description = "Select a folder to hide...";
                        fbd.UseDescriptionForTitle = true;
                        if (fbd.ShowDialog() == DialogResult.OK) {
                            if (SetAttribute(true, fbd.SelectedPath, -1)) {
                                MessageBox.Show("Folder has been set as hidden.");
                            }
                            else {
                                MessageBox.Show("Couldn't set the folder as hidden.");
                            }
                        }
                        else { goto retry; }
                    }
                }
                #endregion
                #region Show file(s)
                else if (clickedButton == ShowFile) {
                    using (VistaFolderBrowserDialog fbd = new VistaFolderBrowserDialog()) {
                        fbd.Description = "Select a file's directory to reveal...";
                        fbd.UseDescriptionForTitle = true;
                        if (fbd.ShowDialog() == DialogResult.OK) {
                            using (InputDialog id = new InputDialog()) {
                                id.MainInstruction = string.Format("Enter the name of the {0} you want to reveal.\nYou can separate inputs with \"|\".", "file(s)");
                                id.WindowTitle = "Reveal hidden file";
                                if (id.ShowDialog(this) == DialogResult.OK) {
                                    List<string> Files = new List<string>();
                                    if (id.Input.Contains("|")) {
                                        string[] Input = id.Input.Split('|');
                                        for (int i = 0; i < Input.Length; i++) { Files.Add(Input[i]); }
                                    }
                                    else { Files.Add(id.Input); }

                                    bool OneSet = false;
                                    bool OneFailed = false;

                                    for (int i = 0; i < Files.Count; i++) {
                                        //if (SetAttribute(false, fbd.SelectedPath + "\\" + Files[i], i)) { OneSet = true; }
                                        //else { OneFailed = true; }
                                    }

                                    if (OneSet && !OneFailed) { MessageBox.Show("Your file(s) have been set as visible."); }
                                    else if (OneSet && OneFailed) { MessageBox.Show("Some of your files have been set as visible, but some have failed."); }
                                    else if (!OneSet && OneFailed) { MessageBox.Show("None of your file(s) were set as visible due to an error."); }
                                    else { MessageBox.Show("No file(s) have even been attempted to be set as visible."); }
                                }
                            }
                        }
                        else { goto retry; }
                    }
                }
                #endregion
                #region Show folder(s)
                else if (clickedButton == ShowFolder) {
                    using (VistaFolderBrowserDialog fbd = new VistaFolderBrowserDialog()) {
                        fbd.Description = "Select a folder's directory to reveal...";
                        fbd.UseDescriptionForTitle = true;
                        if (fbd.ShowDialog() == DialogResult.OK) {
                            using (InputDialog id = new InputDialog()) {
                                id.MainInstruction = string.Format("Enter the name of the {0} you want to reveal.\nYou can separate inputs with \"|\".", "folder(s)");
                                id.WindowTitle = "Reveal hidden file";
                                if (id.ShowDialog(this) == DialogResult.OK) {
                                    List<string> Files = new List<string>();
                                    if (id.Input.Contains("|")) {
                                        string[] Input = id.Input.Split('|');
                                        for (int i = 0; i < Input.Length; i++) { Files.Add(Input[i]); }
                                    }
                                    else { Files.Add(id.Input); }

                                    bool OneSet = false;
                                    bool OneFailed = false;

                                    for (int i = 0; i < Files.Count; i++) {
                                        //if (SetAttribute(false, fbd.SelectedPath + "\\" + Files[i])) { OneSet = true; }
                                        //else { OneFailed = true; }
                                    }

                                    if (OneSet && !OneFailed) { MessageBox.Show("Your folder(s) have been set as visible."); }
                                    else if (OneSet && OneFailed) { MessageBox.Show("Some of your folders have been set as visible, but some have failed."); }
                                    else if (!OneSet && OneFailed) { MessageBox.Show("None of your folder(s) were set as visible due to an error."); }
                                    else { MessageBox.Show("No folder(s) have even been attempted to be set as visible."); }
                                }
                            }
                        }
                        else { goto retry; }
                    }
                }
                #endregion
                else {
                    Environment.Exit(0);
                }
            }
        }
        private void RunAttributeSetterArgs(string[] args) {
            if (args[2].ToLower() == "true") { ArgSetHidden = true; }
            else if (args[2].ToLower() == "false") { ArgSetHidden = false; }
            else { Console.WriteLine("usage: systemprotectedattribute -setattribute <true/false> <file list separated by \"|\" char>"); return; }

            ActiveFlagging = true;

            this.BeginInvoke(new MethodInvoker(() => { lbStatus.Text = "Processing arguments"; }));

            string[] Input = args[3].Split('|');
            for (int i = 0; i < Input.Length; i++) {
                TreeNode tn = new TreeNode();
                tn.Text = Input[i];
                tn.SelectedImageIndex = IndexNull;
                tn.ImageIndex = IndexNull;
                this.BeginInvoke(new MethodInvoker(() => {
                    tvFiles.Nodes.Add(tn);
                }));
                ExitStatus.Add(-1);
            }

            Thread.Sleep(1000);

            for (int i = 0; i < Input.Length; i++) {
                if (SetAttribute(ArgSetHidden, Input[i], i)) {
                    this.BeginInvoke(new MethodInvoker(() => {
                        tvFiles.Nodes[i].ImageIndex = IndexFinished;
                        tvFiles.Nodes[i].SelectedImageIndex = IndexFinished;
                        lbStatus.Text = Input[i] + " has been flagged";
                    }));
                    Thread.Sleep(50);
                }
                else {
                    this.BeginInvoke(new MethodInvoker(() => {
                        tvFiles.Nodes[i].ImageIndex = IndexFailed;
                        tvFiles.Nodes[i].SelectedImageIndex = IndexFailed;
                        lbStatus.Text = Input[i] = " has failed";
                    }));
                    Thread.Sleep(50);
                }
                Thread.Sleep(50);
            }


            this.BeginInvoke(new MethodInvoker(() => { lbStatus.Text = "Processing completed."; }));

            return;
        }

        private bool SetAttribute(bool IsHidden, string Input, int StatusIndex) {
            ActiveFlagging = true;
            string Hidden = "/C attrib +s +h \"{0}\"";
            string Shown = "/C attrib -s -h \"{0}\"";

            Input = Input.Replace("////", "//");

            if (System.IO.File.Exists(Input) || System.IO.Directory.Exists(Input)) {
                try {
                    Process CMD = new Process();
                    CMD.StartInfo.FileName = "cmd.exe";
                    switch (IsHidden) {
                        case true:
                            CMD.StartInfo.Arguments = string.Format(Hidden, Input);
                            break;
                        case false:
                            CMD.StartInfo.Arguments = string.Format(Shown, Input);
                            break;
                    }
                    CMD.StartInfo.Verb = "runas";
                    CMD.StartInfo.CreateNoWindow = true;
                    CMD.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                    CMD.Start();
                    CMD.WaitForExit();
                    ExitStatus[StatusIndex] = 0;
                    ActiveFlagging = false;
                    return true;
                }
                catch (Exception ex) {
                    Debug.Print(ex.ToString());
                    ExitStatus[StatusIndex] = 1;
                    ActiveFlagging = false;
                    return false;
                }
            }
            else {
                ExitStatus[StatusIndex] = 2;
                ActiveFlagging = false;
                return false;
            }
        }

    }

    public static class RunElevatedCommand {
        internal enum HRESULT : long {
            S_FALSE = 0x0001,
            S_OK = 0x0000,
            E_INVALIDARG = 0x80070057,
            E_OUTOFMEMORY = 0x8007000E
        }

        [DllImport("ole32.dll", CharSet = CharSet.Unicode, ExactSpelling = true, PreserveSig = false)]
        [return: MarshalAs(UnmanagedType.Interface)]
        internal static extern object CoGetObject(
           string pszName,
           [In] ref BIND_OPTS3 pBindOptions,
           [In, MarshalAs(UnmanagedType.LPStruct)] Guid riid);


        [StructLayout(LayoutKind.Sequential)]
        internal struct BIND_OPTS3 {
            internal uint cbStruct;
            internal uint grfFlags;
            internal uint grfMode;
            internal uint dwTickCountDeadline;
            internal uint dwTrackFlags;
            internal uint dwClassContext;
            internal uint locale;
            object pServerInfo; // will be passing null, so type doesn't matter
            internal IntPtr hwnd;
        }
        [Flags]
        internal enum CLSCTX {
            CLSCTX_INPROC_SERVER = 0x1,
            CLSCTX_INPROC_HANDLER = 0x2,
            CLSCTX_LOCAL_SERVER = 0x4,
            CLSCTX_REMOTE_SERVER = 0x10,
            CLSCTX_NO_CODE_DOWNLOAD = 0x400,
            CLSCTX_NO_CUSTOM_MARSHAL = 0x1000,
            CLSCTX_ENABLE_CODE_DOWNLOAD = 0x2000,
            CLSCTX_NO_FAILURE_LOG = 0x4000,
            CLSCTX_DISABLE_AAA = 0x8000,
            CLSCTX_ENABLE_AAA = 0x10000,
            CLSCTX_FROM_DEFAULT_CONTEXT = 0x20000,
            CLSCTX_INPROC = CLSCTX_INPROC_SERVER | CLSCTX_INPROC_HANDLER,
            CLSCTX_SERVER = CLSCTX_INPROC_SERVER | CLSCTX_LOCAL_SERVER | CLSCTX_REMOTE_SERVER,
            CLSCTX_ALL = CLSCTX_SERVER | CLSCTX_INPROC_HANDLER
        }

        [return: MarshalAs(UnmanagedType.Interface)]
        public static object LaunchElevatedCOMObject(Guid Clsid, Guid InterfaceID) {
            string CLSID = Clsid.ToString("B"); // B formatting directive: returns {xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx} 
            string monikerName = "Elevation:Administrator!new:" + CLSID;

            BIND_OPTS3 bo = new BIND_OPTS3();
            bo.cbStruct = (uint)Marshal.SizeOf(bo);
            bo.hwnd = IntPtr.Zero;
            bo.dwClassContext = (int)CLSCTX.CLSCTX_LOCAL_SERVER;

            object retVal = CoGetObject(monikerName, ref bo, InterfaceID);

            return (retVal);
        }

        const ulong SEE_MASK_DEFAULT = 0x0;
        const ulong SW_SHOW = 0x5;

        public static void SetAttribute(bool IsHidden, string Input) {
            Guid classId = new Guid("3E5FC7F9-9A51-4367-9063-A120244FBEC7");
            Guid interfaceId = new Guid("6EDD6D74-C007-4E75-B76A-E5740995E24C");

            object o = LaunchElevatedCOMObject(classId, interfaceId);

            ILua ihw = (ILua)o;
            ihw.ShellExec(@"C:\windows\system32\cmd.exe", null, null, SEE_MASK_DEFAULT, SW_SHOW);
            //switch (IsHidden) {
            //    case true:
            //        ihw.ShellExec(@"C:\Windows\System32\cmd.exe", "+s +h \"" + Input + "\"", "C:\\Windows", SEE_MASK_DEFAULT, SW_SHOW);
            //        break;
            //    case false:
            //        ihw.ShellExec(@"C:\Windows\System32\cmd.exe", "-s -h \"" + Input + "\"", "C:\\Windows", SEE_MASK_DEFAULT, SW_SHOW);
            //        break;
            //}

            Marshal.ReleaseComObject(o);
        }

        [ComImport, Guid("6EDD6D74-C007-4E75-B76A-E5740995E24C"), InterfaceType(ComInterfaceType.InterfaceIsDual)]
        interface ILua {
            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), PreserveSig]
            void QueryInterface([In, MarshalAs(UnmanagedType.LPStruct)] Guid riid, [In, Out] ref IntPtr ppv);
            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), PreserveSig]
            void AddRef();
            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), PreserveSig]
            void Release();
            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), PreserveSig]
            void Method1();
            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), PreserveSig]
            void Method2();
            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), PreserveSig]
            void Method3();
            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), PreserveSig]
            void Method4();
            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), PreserveSig]
            void Method5();
            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), PreserveSig]
            void Method6();
            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), PreserveSig]
            HRESULT ShellExec(
                [In, MarshalAs(UnmanagedType.LPWStr)]string file,
                [In, MarshalAs(UnmanagedType.LPWStr)]string paramaters,
                [In, MarshalAs(UnmanagedType.LPWStr)]string directory,
                [In]ulong fMask,
                [In]ulong nShow);
            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), PreserveSig]
            void Method8();
            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), PreserveSig]
            void Method9();
            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), PreserveSig]
            void Method10();
            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), PreserveSig]
            void Method11();
            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), PreserveSig]
            void Method12();
            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), PreserveSig]
            void Method13();
            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), PreserveSig]
            void Method14();
            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), PreserveSig]
            void Method15();
            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), PreserveSig]
            void Method16();
            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), PreserveSig]
            void Method17();
            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), PreserveSig]
            void Method18();
            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), PreserveSig]
            void Method19();
            [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime), PreserveSig]
            void Method20();
        }
    }
}