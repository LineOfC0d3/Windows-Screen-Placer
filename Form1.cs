using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Windows_Screen_Placer
{
    public partial class MainMenu : Form
    {
        //TODO: Move Logic to "WindowPlacer"

        [DllImport("user32.dll", SetLastError = true)]
        internal static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);

        [DllImport("user32.dll", SetLastError = true)]
        internal static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("user32.dll", EntryPoint = "FindWindow", SetLastError = true)]
        internal static extern IntPtr FindWindowByCaption(IntPtr ZeroOnly, string lpWindowName);

        [DllImport("user32.dll")]
        public static extern bool GetWindowRect(IntPtr hwnd, ref Rect rectangle);

        private Process activeProcess = null;
        private IntPtr activeWindow = IntPtr.Zero;
        private const int SW_RESTORE = 9;
        private const int SW_MINIMIZE = 6;
        private Dictionary<IntPtr, string> WindowsHandlers = new Dictionary<IntPtr, string>();

        public MainMenu()
        {
            InitializeComponent();
        }

        private void buttonStart_Click(object sender, EventArgs e)
        {
            string programPath = openFileDialog1.FileName;
            if (activeProcess != null)
            {
                DialogResult result = MessageBox.Show("Program already running... Start new?", "Program running", MessageBoxButtons.YesNo);
                if (result == DialogResult.No)
                {
                    return;
                }
                else
                {
                    //TODO: activeProcess can't be checked or killed (WHY?)
                    activeProcess.Kill();
                }
            }
            try
            {
                ProcessHandler.Start(programPath);
            }
            finally
            {
                if (ProcessHandler.HandledProcess != null)
                {
                    ReadyProcess(ProcessHandler.HandledProcess);
                    UpdateWindowBoundaries(activeProcess);
                    ScanForWindows();
                }
                else
                {
                    MessageBox.Show("Program could not be started!!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void ReadyProcess(Process process)
        {
            using (process)
            {
                process.WaitForInputIdle();
                Debug.WriteLine(process.Id);
                activeProcess = process;
                activeWindow = process.MainWindowHandle;
            }
        }

        private void buttonPlace_Click(object sender, EventArgs e)
        {
            if (this.activeWindow == IntPtr.Zero)
            {
                MessageBox.Show("Chosen program not launched");
            }
            else
            {
                try
                {
                    int x = Convert.ToInt32(textBoxX.Text);
                    int y = Convert.ToInt32(textBoxY.Text);
                    int width = Convert.ToInt32(textBoxWidth.Text);
                    int heigth = Convert.ToInt32(textBoxHeigth.Text);
                    ShowWindow(this.activeWindow, SW_RESTORE);
                    MoveWindow(this.activeWindow, x, y, width, heigth, true);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Malformed position/size input");
                    Debug.WriteLine(ex.StackTrace);
                }
            }
        }

        private void buttonScan_Click(object sender, EventArgs e)
        {
            ScanForWindows();
        }

        private void ScanForWindows()
        {
            this.checkedListBox1.Items.Clear();
            this.WindowsHandlers = (Dictionary<IntPtr, string>)OpenWindowGetter.GetOpenWindows();
            foreach (KeyValuePair<IntPtr, string> window in this.WindowsHandlers)
            {
                IntPtr handle = window.Key;
                string title = window.Value;

                //Debug.WriteLine("{0}: {1}", handle, title);
                this.checkedListBox1.Items.Add(title);
            }
        }

        private void buttonSelectFile_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = openFileDialog1.ShowDialog();
            if (dialogResult == DialogResult.OK)
            {
                string filepath = openFileDialog1.FileName;
                Debug.WriteLine(filepath);
                textBox1.Text = filepath;
            }
        }

        private void buttonRefresh_Click(object sender, EventArgs e)
        {
            UpdateWindowBoundaries(activeWindow);
        }

        private void buttonAdd_Click(object sender, EventArgs e)
        {
        }

        private void UpdateWindowBoundaries(Process p)
        {
            UpdateWindowBoundaries(p.MainWindowHandle);
        }

        private void UpdateWindowBoundaries(IntPtr windowHandle)
        {
            if (windowHandle == IntPtr.Zero)
            {
                MessageBox.Show("Chosen program not launched");
            }
            else
            {
                Rect result = GetWindowBoundaries(windowHandle);

                textBoxX.Text = Convert.ToString(result.Left);
                textBoxY.Text = Convert.ToString(result.Top);
                textBoxWidth.Text = Convert.ToString(result.Right - result.Left);
                textBoxHeigth.Text = Convert.ToString(result.Bottom - result.Top);
            }
        }

        private Rect GetWindowBoundaries(IntPtr windowHandle)
        {
            Rect appRect = new Rect();
            GetWindowRect(windowHandle, ref appRect);
            return appRect;
        }

        private Rect GetWindowBoundaries(Process process)
        {
            IntPtr mwh = process.MainWindowHandle;
            return GetWindowBoundaries(mwh);
        }

        private void checkedListBox1_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (e.NewValue == CheckState.Unchecked)
            {
                ShowWindow(activeWindow, SW_MINIMIZE);
            }
            else
            {
                activeWindow = this.WindowsHandlers.ElementAt(e.Index).Key;
                ShowWindow(this.WindowsHandlers.ElementAt(e.Index).Key, SW_RESTORE);
                UpdateWindowBoundaries(activeWindow);
            }
        }
    }
}