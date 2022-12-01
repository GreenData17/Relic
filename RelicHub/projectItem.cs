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
    public partial class projectItem : UserControl
    {
        public string name = "";
        public string path = "";

        public projectItem(string projectName, string projectPath)
        {
            InitializeComponent();
            label1.Text = projectName;
            name = projectName;
            path = projectPath;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenProject(path);
        }

        public static void OpenProject(string path)
        {
            string command = $@"/K {Application.StartupPath}\Engine\Relic.exe {path}";
            System.Diagnostics.Process.Start("CMD.exe", command);
            Application.Exit();
        }
    }
}
