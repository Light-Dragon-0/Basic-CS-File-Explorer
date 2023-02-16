using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace File_Explorer
{
    public partial class Form1 : Form
    {
        Stack<string> trace = new Stack<string>();
        Stack<string> rtrace = new Stack<string>();

        string currentDir = @"D:\";
        public Form1()
        {
            InitializeComponent();
            textBox2.GotFocus += TextBox2_GotFocus;
            textBox2.LostFocus += TextBox2_LostFocus;
        }

        private void TextBox2_LostFocus(object sender, EventArgs e)
        {
            if(textBox2.Text == "")
            textBox2.Text = "Search";
        }

        private void TextBox2_GotFocus(object sender, EventArgs e)
        {
            if(textBox2.Text == "Search")
            this.textBox2.Text = "";
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            LoadDirectories();
        }
        private void LoadDirectories()
        {
            if (System.IO.Directory.Exists(currentDir)) { 
                this.listView1.Items.Clear();
                string[] dirs = System.IO.Directory.GetDirectories(currentDir);
                foreach(string dir in dirs) this.listView1.Items.Add(new ListViewItem(dir.Split('\\').Last()) { BackColor = Color.Yellow } );
                string[] files = System.IO.Directory.GetFiles(currentDir);
                foreach (string file in files) this.listView1.Items.Add(new ListViewItem(file.Split('\\').Last()) { BackColor = Color.Blue,ForeColor = Color.White });
                this.textBox1.Text = currentDir;
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
        private void listView1_DockChanged(object sender, EventArgs e)
        {

        }

        private void listView1_DoubleClick(object sender, EventArgs e)
        {
            trace.Push(currentDir);
            currentDir += this.listView1.SelectedItems[0].Text + '\\';
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
                if(System.IO.Directory.Exists(this.textBox1.Text))
                {
                    trace.Push(currentDir);
                    if (textBox1.Text.Last() != '\\')
                        textBox1.Text += '\\';
                    currentDir = textBox1.Text;
                    LoadDirectories();
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Forward();
        }

        private void textBox2_Enter(object sender, EventArgs e)
        {
            MessageBox.Show("WOOOWWWW ARE YOU REALLY TRYING TO SEARCH SOMETHING HERE ??!!!!!");
        }
    }
}
