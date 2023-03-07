using Microsoft.WindowsAPICodePack.Shell;
using Microsoft.WindowsAPICodePack.Shell.PropertySystem;
using System.Runtime.InteropServices;
using System.Text;

namespace Player
{
    public partial class Form1 : Form
    {
        OpenFileDialog openFile1 = new OpenFileDialog();
        bool indexch = false;
        int oldindx = -1;

        public Form1()
        {
            InitializeComponent();
            openFile1.Filter = "Music files (*.mp3)|*.mp3";
            timer1.Interval = 1000;
        }
        
        [DllImport("winmm.dll")]
        private static extern long mciSendString(string strCommand, StringBuilder strReturn, int iReturnLength, IntPtr hwndCallback);
        private string sCommand = "";

        private void button1_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex < 0)
                return;
            int index = listBox1.SelectedIndex;
            if (oldindx != -1 && oldindx != index)
            {
                mciSendString("close MediaFile", null, 0, IntPtr.Zero);
                progressBar1.Value = 0;
            }
            oldindx = index;
            TimeSpan ts = GetSongDuration(listBox2.Items[index].ToString());
            int max = ts.Minutes * 60 + ts.Seconds;
            progressBar1.Maximum = max;
            timer1.Start();
            sCommand = "open \"" + listBox2.Items[index] + "\" type mpegvideo alias MediaFile";
            mciSendString(sCommand, null, 0, IntPtr.Zero);
            sCommand = "play MediaFile";
            mciSendString(sCommand, null, 0, IntPtr.Zero);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            mciSendString("stop MediaFile", null, 0, IntPtr.Zero);
            timer1.Stop();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (BackColor == Color.White)
            {
                BackColor = Color.DimGray;
                listBox1.ForeColor = Color.Green;
                button6.BackgroundImage = Properties.Resources.light_bulb;
            }
            else
            {
                BackColor = Color.White;
                listBox1.ForeColor = Color.White;
                button6.BackgroundImage = Properties.Resources.lamp;

            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            if (openFile1.ShowDialog() == DialogResult.Cancel)
                return;
            string systemname = openFile1.FileName;
            listBox2.Items.Add(systemname);
            string name = "";
            foreach(char c in systemname)
            {
                if (c == '.')
                    break;
                name += c;
                if (c == '\\')
                    name = "";
            }
            listBox1.Items.Add(name);
        }

        private void button8_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex < 0)
                return;
            listBox2.Items.RemoveAt(listBox1.SelectedIndex);
            listBox1.Items.RemoveAt(listBox1.SelectedIndex);

        }

        private void button3_Click(object sender, EventArgs e)
        {
            if(listBox1.Items.Count == 0 || listBox1.SelectedIndex == 0)
                return;
            if (listBox1.SelectedIndex < 0)
                listBox1.SelectedIndex = 0;
            else
                listBox1.SelectedIndex--;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (listBox1.Items.Count == 0)
                return;
            if (listBox1.SelectedIndex < 0)
                listBox1.SelectedIndex = 0;
            else if (listBox1.SelectedIndex != listBox1.Items.Count - 1)
                listBox1.SelectedIndex++;
        }
        private static TimeSpan GetSongDuration(string filePath)
        {
            using (var shell = ShellObject.FromParsingName(filePath))
            {
                IShellProperty prop = shell.Properties.System.Media.Duration;
                var t = (ulong)prop.ValueAsObject;
                return TimeSpan.FromTicks((long)t);
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if(progressBar1.Value != progressBar1.Maximum)
                progressBar1.Value++;
        }
    }
}