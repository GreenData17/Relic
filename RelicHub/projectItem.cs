using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.Json;
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
            OpenProject(path, name);
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            if(MessageBox.Show("Are you sure you want to delete this project?", "Warning!", MessageBoxButtons.OKCancel,
                MessageBoxIcon.Warning) == DialogResult.Cancel) return;

            Form1.ProjectInfo info = null;
            try
            {
                foreach (var project in Form1.instance.projects)
                {
                    if (project.name != name) continue;
                    if (project.path != path) continue;

                    info = project;
                    Form1.instance.projects.Remove(project);
                    Directory.Delete(project.path, true);
                }
            }
            catch { }
            if (info == null) Application.Exit();

            Form1.instance.UpdateList();
        }

        public static void OpenProject(string path, string name)
        {
            Form1.ProjectInfo info = null;

            try
            {
                foreach (var project in Form1.instance.projects)
                {
                    if (project.name != name) continue;
                    if (project.path != path) continue;

                    info = project;
                    Form1.instance.projects.Remove(project);
                    Form1.instance.projects.Insert(0, info);
                }
            }
            catch { }

            string jsonContent = JsonSerializer.Serialize(Form1.instance.projects.ToArray(), new JsonSerializerOptions() { WriteIndented = true, IncludeFields = true });
            File.WriteAllText($"{Application.UserAppDataPath}/project.json", jsonContent);

            if (info == null) Application.Exit();

#if DEBUG
            string buildState = "Debug";
#elif RELEASE
            string buildState = "Release";
#endif

            string command = $"/C \"{Application.StartupPath}\\Engine\\{buildState}\\net5.0\\Relic.exe\" {path}";
            System.Diagnostics.Process.Start("CMD.exe", command);
            Application.Exit();
        }
    }
}
