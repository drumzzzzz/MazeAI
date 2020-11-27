namespace MouseAI.UI
{
    partial class ModelInfo
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
            this.tbxModelInfo = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // tbxModelInfo
            // 
            this.tbxModelInfo.BackColor = System.Drawing.SystemColors.Window;
            this.tbxModelInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbxModelInfo.Font = new System.Drawing.Font("Arial Narrow", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbxModelInfo.Location = new System.Drawing.Point(0, 0);
            this.tbxModelInfo.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.tbxModelInfo.Multiline = true;
            this.tbxModelInfo.Name = "tbxModelInfo";
            this.tbxModelInfo.ReadOnly = true;
            this.tbxModelInfo.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.tbxModelInfo.Size = new System.Drawing.Size(561, 511);
            this.tbxModelInfo.TabIndex = 0;
            this.tbxModelInfo.WordWrap = false;
            // 
            // ModelInfo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(5F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(561, 511);
            this.Controls.Add(this.tbxModelInfo);
            this.Font = new System.Drawing.Font("Arial Narrow", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ModelInfo";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Model Information";
            this.TopMost = true;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox tbxModelInfo;
    }
}