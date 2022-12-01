using System.Text.Json;
using System.Windows.Forms;

namespace RelicHub
{
    public partial class Form1 : Form
    {
        public List<projectInfo>? projects = new List<projectInfo>();
        private string jsonContent;

        public Form1()
        {
            InitializeComponent();

            jsonContent = File.ReadAllText(Application.StartupPath + "/project.json");
            projects = JsonSerializer.Deserialize<List<projectInfo>>(jsonContent, new JsonSerializerOptions() { WriteIndented = true, IncludeFields = true });

            for (int i = 0; i < projects.Count; i++)
            {
                projectItem Item = new projectItem(projects[i].name, projects[i].path);
                Item.Tag = "project_" + projects[i].name;
                Item.Size = new Size(ProjectViewer.Size.Width - 25, 80);

                if (i % 2 == 0) Item.BackColor = Color.LightGray;

                ProjectViewer.Controls.Add(Item);
            }
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

        public class projectInfo
        {
            public string name;
            public string path;

            public projectInfo(string name, string path)
            {
                this.name = name;
                this.path = path;
            }
        }
    }
}