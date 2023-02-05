using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Media;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RelicHub
{
    public partial class CreateProject : Form
    {
        public Form1? mainWin;

        private string path = "";

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

        private void btt_Create_Click(object sender, EventArgs e)
        {
            if (!Directory.Exists(path)) {SystemSounds.Hand.Play(); return;}

            string savePath = Application.UserAppDataPath + "/project.json";
            if (!File.Exists(savePath)) { File.Create(savePath); }

            Form1.ProjectInfo info = new Form1.ProjectInfo(NameText.Text, PathText.Text);
            mainWin.projects.Insert(0, info);

            string jsonContent = JsonSerializer.Serialize(mainWin.projects.ToArray(), new JsonSerializerOptions(){WriteIndented = true, IncludeFields = true});
            File.WriteAllText(savePath, jsonContent);

            Directory.CreateDirectory(PathText.Text);
            Directory.CreateDirectory(PathText.Text + @"\Assets");
            Directory.CreateDirectory(PathText.Text + @"\Builds");
            Directory.CreateDirectory(PathText.Text + @"\Settings");
            Directory.CreateDirectory(PathText.Text + @"\temp");
            File.WriteAllText(PathText.Text + $@"\{NameText.Text}.project", "{}");

            mainWin.Enabled = true;
            projectItem.OpenProject(PathText.Text, NameText.Text);
        }
    }
}
