using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace File_Explorer
{
    public partial class Form1 : Form
    {
        private const string KW_THIS_PC = "This PC";
        private const string KW_LOCAL_DISK = "Local Disk";
        private const string KW_SEARCH = "Search";
        Stack<string> trace = new Stack<string>();
        Stack<string> rtrace = new Stack<string>();

        string currentDir = KW_THIS_PC;
        public Form1()
        {
            InitializeComponent();
            textBox2.GotFocus += TextBox2_GotFocus;
            textBox2.LostFocus += TextBox2_LostFocus;

            textBox1.GotFocus += TextBox1_GotFocus;
            textBox1.LostFocus += TextBox1_LostFocus;
        }

        private void TextBox1_LostFocus(object sender, EventArgs e)
        {
            if (textBox1.Text.Trim() == "")
                textBox1.Text = currentDir;
            else if(!(Directory.Exists(textBox1.Text.Trim()) || File.Exists(textBox1.Text.Trim())))
                textBox1.Text = currentDir;
        }
        private void textBox1_Enter(object sender, EventArgs e)
        {
            
        }

        private void TextBox1_GotFocus(object sender, EventArgs e)
        {
            if (textBox1.Text == KW_THIS_PC)
                textBox1.Text = "";
        }

        private void TextBox2_LostFocus(object sender, EventArgs e)
        {
            if(textBox2.Text.Trim() == "")
            textBox2.Text = KW_SEARCH;
        }

        private void TextBox2_GotFocus(object sender, EventArgs e)
        {
            if(textBox2.Text.Trim() == KW_SEARCH)
            this.textBox2.Text = "";
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            LoadDirectories();
        }
        private void LoadDirectories()
        {
            if(currentDir == KW_THIS_PC)
            {
                this.listView1.Items.Clear();
                DriveInfo[] drives = DriveInfo.GetDrives();
                foreach(DriveInfo drive in drives)
                {
                    this.listView1.Items.Add(new ListViewItem(string.Format("{0} ({1})",(drive.VolumeLabel.Length == 0)? KW_LOCAL_DISK: drive.VolumeLabel, drive.Name)) { BackColor = Color.Red, ForeColor = Color.White });
                }
                this.textBox1.Text = currentDir;
                return;
            }
            if (System.IO.Directory.Exists(currentDir))
            {
                this.listView1.Items.Clear();
                string[] dirs = System.IO.Directory.GetDirectories(currentDir);
                foreach(string dir in dirs) this.listView1.Items.Add(new ListViewItem(dir.Split('\\').Last()) { BackColor = Color.Yellow, ForeColor = Color.Black } );
                //string[] files = System.IO.Directory.GetFiles(currentDir);
                //foreach (string file in files) this.listView1.Items.Add(new ListViewItem(file.Split('\\').Last()) { BackColor = Color.Blue,ForeColor = Color.White });
                this.textBox1.Text = currentDir;
                UpdateUI();
            }
            else if(System.IO.File.Exists(currentDir.Remove(currentDir.Length -1,1)))
            {
                System.Diagnostics.Process.Start(currentDir);
                Previous();
            }
        }
        private void Previous()
        {
            if (trace.Count <= 0)
                return;
            rtrace.Push(currentDir);
            currentDir = trace.Pop();
            LoadDirectories();
        }
        private void Forward()
        {
            if (rtrace.Count <= 0)
                return;
            trace.Push(currentDir);
            currentDir = rtrace.Pop();
            LoadDirectories();
        }
        private void Back()
        {
            string[] r = currentDir.Split('\\');
            if (r.Length > 1)
                rtrace.Push(currentDir);
            currentDir = currentDir.Replace("\\" + currentDir.Split('\\')[r.Length - 2] + "\\", "\\");
            LoadDirectories();
        }

        private void listView1_DoubleClick(object sender, EventArgs e)
        {
            trace.Push(currentDir);
            if (currentDir == KW_THIS_PC)
            {
                int indexOfOpenBracket = this.listView1.SelectedItems[0].Text.LastIndexOf('(');
                currentDir = this.listView1.SelectedItems[0].Text.Substring(1 + indexOfOpenBracket, this.listView1.SelectedItems[0].Text.LastIndexOf(')') - indexOfOpenBracket - 1);
            }
            else { 
                currentDir += this.listView1.SelectedItems[0].Text + '\\';
            }
            LoadDirectories();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Previous();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Back();
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if(e.KeyChar == (char)13)
            {
                if (textBox1.Text.Trim() == "")
                    textBox1.Text = currentDir;
                else if (System.IO.Directory.Exists(this.textBox1.Text))
                {
                    trace.Push(currentDir);
                    if (textBox1.Text.Last() != '\\')
                        textBox1.Text += '\\';
                    currentDir = textBox1.Text;
                    LoadDirectories();
                }
                else if(System.IO.File.Exists(textBox1.Text))
                {
                    System.Diagnostics.Process.Start(textBox1.Text);
                    textBox1.Text = currentDir;
                }
                else
                {
                    MessageBox.Show(string.Format("File / Directory \'{0}\' does not exist",currentDir));
                }

            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Forward();
        }

        private void textBox2_Enter(object sender, EventArgs e)
        {
            MessageBox.Show("TODO: Implement a search");
        }

        private void UpdateUI() //refresh
        {
            textBox1.Text = currentDir;
            ListViewItem item;
            listView1.BeginUpdate();

            foreach (System.IO.FileInfo file in new DirectoryInfo(currentDir).GetFiles())
            {
                Icon iconForFile = SystemIcons.WinLogo;

                item = new ListViewItem(file.Name, 1);

                if (!imageList1.Images.ContainsKey(file.Extension))
                {
                    iconForFile = System.Drawing.Icon.ExtractAssociatedIcon(file.FullName);
                    imageList1.Images.Add(file.Extension, iconForFile);
                }
                item.ImageKey = file.Extension;
                listView1.Items.Add(item);
            }
            listView1.EndUpdate();
        }
        
    }
}
