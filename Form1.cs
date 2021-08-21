using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Windows_Screen_Placer
{
    public partial class MainMenu : Form
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

        private Process activeProcess = null;
        private const int SW_RESTORE = 9;
        public MainMenu()
        {
            InitializeComponent();
        }

        private void buttonStart_Click(object sender, EventArgs e)
        {
            string programPath = openFileDialog1.FileName;
            Process process = null;
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
                process = Process.Start(programPath);
            }
            catch (Win32Exception)
            {
                process = ExecuteAsAdmin(programPath);
            }
            finally
            {
                if (process != null)
                {
                    ReadyProcess(process);
                    UpdateWindowBoundaries(activeProcess);
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
                IntPtr test = process.MainWindowHandle;
                bool testbool = process.HasExited;
            }
        }

        public Process ExecuteAsAdmin(string fileName)
        {
            Process proc = new Process();
            proc.StartInfo.FileName = fileName;
            proc.StartInfo.UseShellExecute = true;
            proc.StartInfo.Verb = "runas";
            proc.Start();
            return proc;
        }

        private void buttonPlace_Click(object sender, EventArgs e)
        {
            if (activeProcess == null)
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
                    IntPtr notepadMWH = activeProcess.MainWindowHandle;
                    ShowWindow(notepadMWH, SW_RESTORE);
                    MoveWindow(notepadMWH, x, y, width, heigth, true);
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
            MessageBox.Show("Not yet implemented");
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
            UpdateWindowBoundaries(activeProcess);
        }

        private void UpdateWindowBoundaries(Process p)
        {
            if (activeProcess == null)
            {
                MessageBox.Show("Chosen program not launched");
            }
            else
            {
                Rect result = GetWindowBoundaries(activeProcess);

                textBoxX.Text = Convert.ToString(result.Left);
                textBoxY.Text = Convert.ToString(result.Top);
                textBoxWidth.Text = Convert.ToString(result.Right - result.Left);
                textBoxHeigth.Text = Convert.ToString(result.Bottom - result.Top);
            }
        }

        private Rect GetWindowBoundaries(Process process)
        {
            IntPtr mwh = process.MainWindowHandle;
            Rect appRect = new Rect();
            GetWindowRect(mwh, ref appRect);
            return appRect;
        }
    }
}
