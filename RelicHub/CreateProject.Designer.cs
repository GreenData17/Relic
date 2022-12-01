namespace RelicHub
{
    partial class CreateProject
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.NameText = new System.Windows.Forms.TextBox();
            this.PathText = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.btt_cancle = new System.Windows.Forms.Button();
            this.btt_Create = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(55, 21);
            this.label1.TabIndex = 0;
            this.label1.Text = "Name:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label2.Location = new System.Drawing.Point(12, 59);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(95, 21);
            this.label2.TabIndex = 1;
            this.label2.Text = "Project Path:";
            // 
            // NameText
            // 
            this.NameText.Location = new System.Drawing.Point(12, 33);
            this.NameText.Name = "NameText";
            this.NameText.Size = new System.Drawing.Size(235, 23);
            this.NameText.TabIndex = 2;
            this.NameText.TextChanged += new System.EventHandler(this.NameText_TextChanged);
            // 
            // PathText
            // 
            this.PathText.Location = new System.Drawing.Point(12, 83);
            this.PathText.Name = "PathText";
            this.PathText.ReadOnly = true;
            this.PathText.Size = new System.Drawing.Size(154, 23);
            this.PathText.TabIndex = 3;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(172, 83);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 4;
            this.button1.Text = "search...";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // folderBrowserDialog1
            // 
            this.folderBrowserDialog1.RootFolder = System.Environment.SpecialFolder.MyDocuments;
            // 
            // btt_cancle
            // 
            this.btt_cancle.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.btt_cancle.Location = new System.Drawing.Point(11, 112);
            this.btt_cancle.Name = "btt_cancle";
            this.btt_cancle.Size = new System.Drawing.Size(115, 27);
            this.btt_cancle.TabIndex = 5;
            this.btt_cancle.Text = "Cancle";
            this.btt_cancle.UseVisualStyleBackColor = true;
            this.btt_cancle.Click += new System.EventHandler(this.btt_cancle_Click);
            // 
            // btt_Create
            // 
            this.btt_Create.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.btt_Create.Location = new System.Drawing.Point(132, 112);
            this.btt_Create.Name = "btt_Create";
            this.btt_Create.Size = new System.Drawing.Size(115, 27);
            this.btt_Create.TabIndex = 6;
            this.btt_Create.Text = "Create";
            this.btt_Create.UseVisualStyleBackColor = true;
            this.btt_Create.Click += new System.EventHandler(this.btt_Create_Click);
            // 
            // CreateProject
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(257, 144);
            this.Controls.Add(this.btt_Create);
            this.Controls.Add(this.btt_cancle);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.PathText);
            this.Controls.Add(this.NameText);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "CreateProject";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "CreateProject";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.CreateProject_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Label label1;
        private Label label2;
        private TextBox NameText;
        private TextBox PathText;
        private Button button1;
        private FolderBrowserDialog folderBrowserDialog1;
        private Button btt_cancle;
        private Button btt_Create;
    }
}