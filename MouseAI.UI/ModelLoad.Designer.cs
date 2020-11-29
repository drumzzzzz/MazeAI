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
            this.pbxPlot = new System.Windows.Forms.PictureBox();
            this.btnPredict = new System.Windows.Forms.Button();
            this.btnRun = new System.Windows.Forms.Button();
            this.llblLog = new System.Windows.Forms.LinkLabel();
            this.btnDelete = new System.Windows.Forms.Button();
            this.btnModel = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pbxPlot)).BeginInit();
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
            this.lbxModels.Size = new System.Drawing.Size(184, 276);
            this.lbxModels.TabIndex = 18;
            // 
            // btnCancel
            // 
            this.btnCancel.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCancel.Location = new System.Drawing.Point(130, 371);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(81, 33);
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
            this.tbxSummary.Size = new System.Drawing.Size(161, 276);
            this.tbxSummary.TabIndex = 20;
            this.tbxSummary.WordWrap = false;
            // 
            // pbxPlot
            // 
            this.pbxPlot.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pbxPlot.Location = new System.Drawing.Point(421, 12);
            this.pbxPlot.Name = "pbxPlot";
            this.pbxPlot.Size = new System.Drawing.Size(308, 276);
            this.pbxPlot.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbxPlot.TabIndex = 22;
            this.pbxPlot.TabStop = false;
            // 
            // btnPredict
            // 
            this.btnPredict.Enabled = false;
            this.btnPredict.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnPredict.Location = new System.Drawing.Point(22, 308);
            this.btnPredict.Margin = new System.Windows.Forms.Padding(0);
            this.btnPredict.Name = "btnPredict";
            this.btnPredict.Size = new System.Drawing.Size(81, 33);
            this.btnPredict.TabIndex = 23;
            this.btnPredict.Text = "PREDICT";
            this.btnPredict.UseVisualStyleBackColor = true;
            // 
            // btnRun
            // 
            this.btnRun.Enabled = false;
            this.btnRun.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnRun.Location = new System.Drawing.Point(130, 308);
            this.btnRun.Margin = new System.Windows.Forms.Padding(0);
            this.btnRun.Name = "btnRun";
            this.btnRun.Size = new System.Drawing.Size(81, 33);
            this.btnRun.TabIndex = 24;
            this.btnRun.Text = "RUN";
            this.btnRun.UseVisualStyleBackColor = true;
            // 
            // llblLog
            // 
            this.llblLog.AutoSize = true;
            this.llblLog.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.llblLog.Location = new System.Drawing.Point(496, 318);
            this.llblLog.Name = "llblLog";
            this.llblLog.Size = new System.Drawing.Size(0, 16);
            this.llblLog.TabIndex = 25;
            // 
            // btnDelete
            // 
            this.btnDelete.Enabled = false;
            this.btnDelete.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnDelete.Location = new System.Drawing.Point(22, 371);
            this.btnDelete.Margin = new System.Windows.Forms.Padding(0);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(81, 33);
            this.btnDelete.TabIndex = 26;
            this.btnDelete.Text = "DELETE";
            this.btnDelete.UseVisualStyleBackColor = true;
            // 
            // btnModel
            // 
            this.btnModel.Enabled = false;
            this.btnModel.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnModel.Location = new System.Drawing.Point(239, 308);
            this.btnModel.Margin = new System.Windows.Forms.Padding(0);
            this.btnModel.Name = "btnModel";
            this.btnModel.Size = new System.Drawing.Size(81, 33);
            this.btnModel.TabIndex = 27;
            this.btnModel.Text = "INFO";
            this.btnModel.UseVisualStyleBackColor = true;
            // 
            // ModelLoad
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(741, 420);
            this.ControlBox = false;
            this.Controls.Add(this.btnModel);
            this.Controls.Add(this.btnDelete);
            this.Controls.Add(this.llblLog);
            this.Controls.Add(this.btnRun);
            this.Controls.Add(this.btnPredict);
            this.Controls.Add(this.pbxPlot);
            this.Controls.Add(this.tbxSummary);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.lbxModels);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "ModelLoad";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Model Load";
            this.TopMost = true;
            ((System.ComponentModel.ISupportInitialize)(this.pbxPlot)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        public System.Windows.Forms.ListBox lbxModels;
        public System.Windows.Forms.TextBox tbxSummary;
        public System.Windows.Forms.Button btnCancel;
        public System.Windows.Forms.PictureBox pbxPlot;
        public System.Windows.Forms.Button btnPredict;
        public System.Windows.Forms.Button btnRun;
        public System.Windows.Forms.LinkLabel llblLog;
        public System.Windows.Forms.Button btnDelete;
        public System.Windows.Forms.Button btnModel;
    }
}