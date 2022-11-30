using System.Windows.Forms;

namespace RelicHub
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            for (int i = 0; i < 200; i++)
            {
                projectItem Item = new projectItem("Game_"+i, "");
                Item.Tag = i;
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
    }
}