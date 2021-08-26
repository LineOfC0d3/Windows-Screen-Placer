using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Windows_Screen_Placer
{
    public partial class MainMenu : Form
    {
        //TODO: Move Logic to "WindowPlacer"

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
            }
            try
            {
                ProcessHandler.Start(programPath);
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
                activeProcess = process;
                IntPtr test = process.MainWindowHandle;
                bool testbool = process.HasExited;
            }
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

        private void buttonAdd_Click(object sender, EventArgs e)
        {

        }
    }
}
