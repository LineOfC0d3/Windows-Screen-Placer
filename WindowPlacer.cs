using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace Windows_Screen_Placer
{
    class WindowPlacer
    {
        [DllImport("user32.dll", SetLastError = true)]
        internal static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);
        [DllImport("user32.dll", SetLastError = true)]
        internal static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
        [DllImport("user32.dll", SetLastError = true)]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
        [DllImport("user32.dll", EntryPoint = "FindWindow", SetLastError = true)]
        internal static extern IntPtr FindWindowByCaption(IntPtr ZeroOnly, string lpWindowName);
        [DllImport("user32.dll")]
        public static extern bool GetWindowRect(IntPtr hwnd, ref Rect rectangle);

        private const int SW_RESTORE = 9;

        private List<ProcessWithPlacement> elements = new List<ProcessWithPlacement>();
        private Rect GetWindowBoundaries(Process process)
        {
            IntPtr mwh = process.MainWindowHandle;
            Rect appRect = new Rect();
            GetWindowRect(mwh, ref appRect);
            return appRect;
        }
    }
}
