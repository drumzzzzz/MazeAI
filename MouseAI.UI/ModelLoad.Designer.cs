namespace MouseAI.UI
{
    partial class ModelLoad
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
            this.lbxModels = new System.Windows.Forms.ListBox();
            this.btnCancel = new System.Windows.Forms.Button();
            this.tbxSummary = new System.Windows.Forms.TextBox();
            this.btnLoadModel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lbxModels
            // 
            this.lbxModels.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbxModels.FormattingEnabled = true;
            this.lbxModels.ItemHeight = 16;
            this.lbxModels.Location = new System.Drawing.Point(12, 12);
            this.lbxModels.Name = "lbxModels";
            this.lbxModels.ScrollAlwaysVisible = true;
            this.lbxModels.Size = new System.Drawing.Size(184, 244);
            this.lbxModels.TabIndex = 18;
            // 
            // btnCancel
            // 
            this.btnCancel.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCancel.Location = new System.Drawing.Point(228, 286);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(80, 23);
            this.btnCancel.TabIndex = 19;
            this.btnCancel.Text = "CANCEL";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // tbxSummary
            // 
            this.tbxSummary.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbxSummary.Location = new System.Drawing.Point(230, 12);
            this.tbxSummary.Multiline = true;
            this.tbxSummary.Name = "tbxSummary";
            this.tbxSummary.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.tbxSummary.ShortcutsEnabled = false;
            this.tbxSummary.Size = new System.Drawing.Size(161, 244);
            this.tbxSummary.TabIndex = 20;
            this.tbxSummary.WordWrap = false;
            // 
            // btnLoadModel
            // 
            this.btnLoadModel.Enabled = false;
            this.btnLoadModel.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnLoadModel.Location = new System.Drawing.Point(94, 286);
            this.btnLoadModel.Margin = new System.Windows.Forms.Padding(0);
            this.btnLoadModel.Name = "btnLoadModel";
            this.btnLoadModel.Size = new System.Drawing.Size(69, 23);
            this.btnLoadModel.TabIndex = 21;
            this.btnLoadModel.Text = "LOAD";
            this.btnLoadModel.UseVisualStyleBackColor = true;
            // 
            // ModelLoad
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(402, 327);
            this.ControlBox = false;
            this.Controls.Add(this.btnLoadModel);
            this.Controls.Add(this.tbxSummary);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.lbxModels);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "ModelLoad";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Model Load";
            this.TopMost = true;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        public System.Windows.Forms.ListBox lbxModels;
        public System.Windows.Forms.TextBox tbxSummary;
        public System.Windows.Forms.Button btnCancel;
        public System.Windows.Forms.Button btnLoadModel;
    }
}