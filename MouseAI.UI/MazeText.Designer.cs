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
            this.pnlSelections = new System.Windows.Forms.Panel();
            this.rbVision = new System.Windows.Forms.RadioButton();
            this.rbAll = new System.Windows.Forms.RadioButton();
            this.rbPaths = new System.Windows.Forms.RadioButton();
            this.rbSegments = new System.Windows.Forms.RadioButton();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.lbxSegments = new System.Windows.Forms.ListBox();
            this.pnlSelections.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtMaze
            // 
            this.txtMaze.CausesValidation = false;
            this.txtMaze.Font = new System.Drawing.Font("Consolas", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtMaze.HideSelection = false;
            this.txtMaze.Location = new System.Drawing.Point(0, 64);
            this.txtMaze.Margin = new System.Windows.Forms.Padding(0);
            this.txtMaze.Multiline = true;
            this.txtMaze.Name = "txtMaze";
            this.txtMaze.ShortcutsEnabled = false;
            this.txtMaze.Size = new System.Drawing.Size(171, 131);
            this.txtMaze.TabIndex = 0;
            this.txtMaze.TabStop = false;
            this.txtMaze.WordWrap = false;
            // 
            // pnlSelections
            // 
            this.pnlSelections.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.pnlSelections.Controls.Add(this.rbVision);
            this.pnlSelections.Controls.Add(this.rbAll);
            this.pnlSelections.Controls.Add(this.rbPaths);
            this.pnlSelections.Controls.Add(this.rbSegments);
            this.pnlSelections.Controls.Add(this.btnRefresh);
            this.pnlSelections.Location = new System.Drawing.Point(0, -1);
            this.pnlSelections.Name = "pnlSelections";
            this.pnlSelections.Size = new System.Drawing.Size(486, 43);
            this.pnlSelections.TabIndex = 3;
            // 
            // rbVision
            // 
            this.rbVision.AutoSize = true;
            this.rbVision.Checked = true;
            this.rbVision.Location = new System.Drawing.Point(396, 13);
            this.rbVision.Name = "rbVision";
            this.rbVision.Size = new System.Drawing.Size(72, 18);
            this.rbVision.TabIndex = 7;
            this.rbVision.TabStop = true;
            this.rbVision.Text = "NN Vision";
            this.rbVision.UseVisualStyleBackColor = true;
            this.rbVision.CheckedChanged += new System.EventHandler(this.rbVision_CheckedChanged);
            // 
            // rbAll
            // 
            this.rbAll.AutoSize = true;
            this.rbAll.Location = new System.Drawing.Point(135, 13);
            this.rbAll.Name = "rbAll";
            this.rbAll.Size = new System.Drawing.Size(37, 18);
            this.rbAll.TabIndex = 6;
            this.rbAll.Text = "All";
            this.rbAll.UseVisualStyleBackColor = true;
            this.rbAll.CheckedChanged += new System.EventHandler(this.rbAll_CheckedChanged);
            // 
            // rbPaths
            // 
            this.rbPaths.AutoSize = true;
            this.rbPaths.Location = new System.Drawing.Point(309, 13);
            this.rbPaths.Name = "rbPaths";
            this.rbPaths.Size = new System.Drawing.Size(63, 18);
            this.rbPaths.TabIndex = 5;
            this.rbPaths.Text = "NN Path";
            this.rbPaths.UseVisualStyleBackColor = true;
            this.rbPaths.CheckedChanged += new System.EventHandler(this.rbPaths_CheckedChanged);
            // 
            // rbSegments
            // 
            this.rbSegments.AutoSize = true;
            this.rbSegments.Location = new System.Drawing.Point(219, 13);
            this.rbSegments.Name = "rbSegments";
            this.rbSegments.Size = new System.Drawing.Size(73, 18);
            this.rbSegments.TabIndex = 4;
            this.rbSegments.Text = "Segments";
            this.rbSegments.UseVisualStyleBackColor = true;
            this.rbSegments.CheckedChanged += new System.EventHandler(this.rbSegments_CheckedChanged);
            // 
            // btnRefresh
            // 
            this.btnRefresh.Location = new System.Drawing.Point(12, 11);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(75, 23);
            this.btnRefresh.TabIndex = 3;
            this.btnRefresh.Text = "Refresh";
            this.btnRefresh.UseVisualStyleBackColor = true;
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // lbxSegments
            // 
            this.lbxSegments.Enabled = false;
            this.lbxSegments.FormattingEnabled = true;
            this.lbxSegments.ItemHeight = 14;
            this.lbxSegments.Location = new System.Drawing.Point(620, 10);
            this.lbxSegments.Name = "lbxSegments";
            this.lbxSegments.ScrollAlwaysVisible = true;
            this.lbxSegments.Size = new System.Drawing.Size(73, 494);
            this.lbxSegments.TabIndex = 4;
            this.lbxSegments.SelectedIndexChanged += new System.EventHandler(this.lbxSegments_SelectedIndexChanged);
            // 
            // MazeText
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(705, 517);
            this.ControlBox = false;
            this.Controls.Add(this.lbxSegments);
            this.Controls.Add(this.txtMaze);
            this.Controls.Add(this.pnlSelections);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Name = "MazeText";
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "Mouse AI";
            this.pnlSelections.ResumeLayout(false);
            this.pnlSelections.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.TextBox txtMaze;
        private System.Windows.Forms.Panel pnlSelections;
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.RadioButton rbSegments;
        private System.Windows.Forms.RadioButton rbPaths;
        private System.Windows.Forms.RadioButton rbAll;
        private System.Windows.Forms.ListBox lbxSegments;
        private System.Windows.Forms.RadioButton rbVision;
    }
}