using System.Collections.Generic;
using System.Text.Json;
using System.Windows.Forms;

namespace RelicHub
{
    public partial class Form1 : Form
    {
        public static Form1 instance;

        public List<ProjectInfo>? projects = new List<ProjectInfo>();
        private string jsonContent;

        public Form1()
        {
            InitializeComponent();
            instance = this;

            UpdateList();
        }

        private void Form1_ResizeEnd(object sender, EventArgs e)
        {
            box.Size = new Size(Size.Width, Size.Height - 80);
            ProjectViewer.Size = new Size(Size.Width - 40, Size.Height - 135);

            foreach (Control control in ProjectViewer.Controls)
            {
                control.Size = new Size(ProjectViewer.Size.Width - 25, 80);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            CreateProject cp = new CreateProject();
            cp.Show();
            cp.mainWin = this;
            Enabled = false;
        }

        public void UpdateList()
        {
            ProjectViewer.Controls.Clear();
            if (!File.Exists(Application.UserAppDataPath + "/project.json")) {File.WriteAllLines(Application.UserAppDataPath + "/project.json", new []{"[","]"});}
            
            jsonContent = File.ReadAllText(Application.UserAppDataPath + "/project.json");
            projects = JsonSerializer.Deserialize<List<ProjectInfo>>(jsonContent, new JsonSerializerOptions() { WriteIndented = true, IncludeFields = true });

            List<ProjectInfo> info = new List<ProjectInfo>();

            for (int i = 0; i < projects.Count; i++)
            {
                if (!Directory.Exists(projects[i].path)) continue;

                projectItem Item = new projectItem(projects[i].name, projects[i].path);
                Item.Tag = "project_" + projects[i].name;
                Item.Size = new Size(ProjectViewer.Size.Width - 25, 80);

                if (i % 2 == 0) Item.BackColor = Color.LightGray;

                ProjectViewer.Controls.Add(Item);
                info.Add(new ProjectInfo(projects[i].name, projects[i].path));
            }

            projects = info;

            jsonContent = JsonSerializer.Serialize(projects.ToArray(), new JsonSerializerOptions() { WriteIndented = true, IncludeFields = true });
            File.WriteAllText($"{Application.UserAppDataPath}/project.json", jsonContent);
        }

        public class ProjectInfo
        {
            public string name;
            public string path;

            public ProjectInfo(string name, string path)
            {
                this.name = name;
                this.path = path;
            }
        }
    }
}