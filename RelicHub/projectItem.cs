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
        public projectItem(string gameName, string projectPath)
        {
            InitializeComponent();
            label1.Text = gameName;
        }
    }
}
