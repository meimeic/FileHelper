using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FileHelper;
using System.Diagnostics;
namespace LisDocumentCheck
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (!textBox1.Text.Trim().Equals("你没有选择目录"))
            {
            }
            else
            {
                MessageBox.Show("你没有选择目录！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);  
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            this.folderBrowserDialog1.SelectedPath = Record.GetLisHosPathRoot();
            this.folderBrowserDialog1.ShowDialog();
            string path = this.folderBrowserDialog1.SelectedPath;
            //string testString = "F:\\hadoop-2.2.0-src\\hadoop-common-project\\hadoop-common\\src";
            //Record.SetPathRoot("F:\\hadoop-2.2.0-src\\hadoop-common-project");
            //string result = Record.Intercepts(testString);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            LisBusiness lb = new LisBusiness();
            lb.Check();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.folderBrowserDialog1.ShowDialog();
            Record.SetLisHosPathRoot(this.folderBrowserDialog1.SelectedPath);
            this.textBox2.Text = this.folderBrowserDialog1.SelectedPath;
        }

        private void folderBrowserDialog1_HelpRequest(object sender, EventArgs e)
        {
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
