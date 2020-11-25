namespace MouseAI.UI
{
    partial class MazeSegments
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
            this.lvwSegments = new System.Windows.Forms.ListView();
            this.SuspendLayout();
            // 
            // lvwSegments
            // 
            this.lvwSegments.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lvwSegments.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvwSegments.HideSelection = false;
            this.lvwSegments.Location = new System.Drawing.Point(0, 0);
            this.lvwSegments.MultiSelect = false;
            this.lvwSegments.Name = "lvwSegments";
            this.lvwSegments.Size = new System.Drawing.Size(778, 510);
            this.lvwSegments.TabIndex = 0;
            this.lvwSegments.UseCompatibleStateImageBehavior = false;
            // 
            // MazeSegments
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(778, 510);
            this.ControlBox = false;
            this.Controls.Add(this.lvwSegments);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "MazeSegments";
            this.ShowIcon = false;
            this.Text = "Maze Segments";
            this.TopMost = true;
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView lvwSegments;
    }
}