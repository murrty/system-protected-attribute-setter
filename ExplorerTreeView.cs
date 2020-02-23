using System;
using System.Windows.Forms;
using System.Runtime.InteropServices;

public class ExplorerTreeview : TreeView {
    internal class NativeMethods {
        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        internal static extern int SendMessage(IntPtr hWnd, UInt32 Msg, int wParam, int lParam);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        internal static extern void SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        internal static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("uxtheme.dll", CharSet = CharSet.Unicode)]
        public extern static int SetWindowTheme(IntPtr hWnd, string pszSubAppName, string pszSubIdList);
    }
    [DllImport("uxtheme", CharSet = CharSet.Unicode)]
    public extern static Int32 SetWindowTheme(IntPtr hWnd, String textSubAppName, String textSubIdList);
    public ExplorerTreeview() {
        SetWindowTheme(this.Handle, "explorer", null);
        this.HotTracking = true;
        this.ShowLines = false;
        NativeMethods.SendMessage(this.Handle, (0x1100 + 44), 0, 0x0040);
    }
}