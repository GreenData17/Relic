using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RelicHub
{
    public partial class CreateProject : Form
    {
        public Form1 mainWin;

        private string path;

        public CreateProject()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                path = folderBrowserDialog1.SelectedPath;
                PathText.Text = path + @"\" + NameText.Text;
            }
        }

        private void NameText_TextChanged(object sender, EventArgs e)
        {
            PathText.Text = path + @"\" + NameText.Text;
        }

        private void CreateProject_FormClosing(object sender, FormClosingEventArgs e)
        {
            mainWin.Enabled = true;
        }

        private void btt_cancle_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
