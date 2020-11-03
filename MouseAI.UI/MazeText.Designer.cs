namespace MouseAI.UI
{
    partial class MazeText
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
            this.txtMaze = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // txtMaze
            // 
            this.txtMaze.CausesValidation = false;
            this.txtMaze.Font = new System.Drawing.Font("Consolas", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtMaze.HideSelection = false;
            this.txtMaze.Location = new System.Drawing.Point(0, 0);
            this.txtMaze.Margin = new System.Windows.Forms.Padding(0);
            this.txtMaze.Multiline = true;
            this.txtMaze.Name = "txtMaze";
            this.txtMaze.ShortcutsEnabled = false;
            this.txtMaze.Size = new System.Drawing.Size(171, 131);
            this.txtMaze.TabIndex = 0;
            this.txtMaze.TabStop = false;
            this.txtMaze.WordWrap = false;
            // 
            // MazeText
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1139, 630);
            this.ControlBox = false;
            this.Controls.Add(this.txtMaze);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Name = "MazeText";
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "Mouse AI";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.TextBox txtMaze;
    }
}