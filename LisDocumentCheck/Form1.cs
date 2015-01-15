using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace LisDocumentCheck
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DateTime checkDate = dateTimePicker1.Value;
            //textBox1.Text= string.Format("{0:yyyy-MM-dd}", checkDate);
            SuperBusiness mb = new LisBusiness();
            mb.CheckCondition = string.Format("{0:yyyy-MM-dd}", checkDate);
            if (mb.IsChecked())
            {
                //此内容已检查
            }
            else
            {
                mb.Check();
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
