using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace Text_Spammer
{
    public partial class TextSpammer : Form
    {
        public TextSpammer()
        {
            InitializeComponent();
        }

        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        [DllImport("user32.dll")]
        public static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        static extern uint GetWindowThreadProcessId(IntPtr hWnd, out int processId);

        bool formMaximized = false;
        bool firstdelay = false;
        bool started = false;
        bool blockspam = true;
        bool sendenter = true;
        int delay = 1000;

        private void panel1_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Environment.Exit(Environment.ExitCode);
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                TopMost = true;
            } else
            {
                TopMost = false;
            }
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox2.Checked)
            {
                firstdelay = true;
            }
            else
            {
                firstdelay = false;
            }
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            try
            {
                int i = Convert.ToInt32(textBox2.Text);
                if (i >= 0 && i < 2000000000)
                {
                    delay = i;
                }
            }
            catch { }
        }

        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.'))
            {
                e.Handled = true;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            textBox2.Text = delay.ToString();

            Size = new Size(420, 87);
            button3.Location = new Point(3, 62);
            button3.Text = "v";
            textBox1.Visible = false;
            label9.Visible = true;
            label10.Visible = false;

            CheckForIllegalCrossThreadCalls = false;

            new Thread(() =>
            {
                while (true)
                {
                    if (started)
                    {
                        int foregroundProcessID = 0;
                        GetWindowThreadProcessId(GetForegroundWindow(), out foregroundProcessID);

                        if (blockspam && foregroundProcessID == Process.GetCurrentProcess().Id)
                        {
                            continue;
                        }

                        string[] lines = textBox1.Text.Split('\n');

                        if (firstdelay)
                        {
                            Thread.Sleep(delay);
                        }

                        foreach (string line in lines)
                        {
                            if (started)
                            {
                                SendKeys.SendWait(line.Replace("{ENTER}", "\r\n").Replace("{TAB}", "\t") + (sendenter ? "{ENTER}" : ""));
                                Thread.Sleep(delay);
                                continue;
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                }
            }).Start();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (started)
            {
                button1.Text = "Enable";
                panel2.BackColor = Color.Red;
                started = false;
            }
            else
            {
                button1.Text = "Disable";
                panel2.BackColor = Color.Green;
                started = true;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            formMaximized = !formMaximized;
            if (!formMaximized)
            {
                label10.Visible = false;
                while (Height > 87 && button3.Location.Y > 62)
                {
                    if (Height > 87)
                    {
                        Height -= 2;
                        if (Height == 107)
                        {
                            textBox1.Visible = false;
                        }
                    }
                    if (button3.Location.Y > 62)
                    {
                        button3.Location = new Point(button3.Location.X, button3.Location.Y - 2);
                    }
                    Application.DoEvents();
                }
                button3.Text = "v";
                label9.Visible = true;
            }
            else
            {
                textBox1.Visible = true;
                label9.Visible = false;
                label10.Visible = true;
                while (Height < 389 && button3.Location.Y < 362)
                {
                    if (Height < 389)
                    {
                        Height += 2;
                    }
                    if (button3.Location.Y < 362)
                    {
                        button3.Location = new Point(button3.Location.X, button3.Location.Y + 2);
                    }
                    Application.DoEvents();
                }
                button3.Text = "^";
            }
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox3.Checked)
            {
                blockspam = true;
            }
            else
            {
                DialogResult dialogResult = MessageBox.Show("Turning this option off may make the program glitchy or even break it. Are you sure that you want to change?", "Warning", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    blockspam = false;
                }
                else
                {
                    blockspam = true;
                    checkBox3.Checked = true;
                }
            }
        }

        private void checkBox4_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox4.Checked)
            {
                sendenter = true;
            }
            else
            {
                sendenter = false;
            }
        }
    }
}
