namespace MouseAI.UI
{
    partial class ModelTest
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
            this.pnlImages = new System.Windows.Forms.Panel();
            this.btnPredict = new System.Windows.Forms.Button();
            this.txtResults = new System.Windows.Forms.TextBox();
            this.btnBack = new System.Windows.Forms.Button();
            this.btnExit = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txtInfo = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // pnlImages
            // 
            this.pnlImages.BackColor = System.Drawing.Color.White;
            this.pnlImages.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlImages.Location = new System.Drawing.Point(0, 128);
            this.pnlImages.Name = "pnlImages";
            this.pnlImages.Size = new System.Drawing.Size(532, 447);
            this.pnlImages.TabIndex = 23;
            // 
            // btnPredict
            // 
            this.btnPredict.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnPredict.Location = new System.Drawing.Point(82, 12);
            this.btnPredict.Name = "btnPredict";
            this.btnPredict.Size = new System.Drawing.Size(84, 26);
            this.btnPredict.TabIndex = 20;
            this.btnPredict.Text = "PREDICT";
            this.btnPredict.UseVisualStyleBackColor = true;
            // 
            // txtResults
            // 
            this.txtResults.BackColor = System.Drawing.Color.White;
            this.txtResults.Location = new System.Drawing.Point(82, 54);
            this.txtResults.Name = "txtResults";
            this.txtResults.ReadOnly = true;
            this.txtResults.Size = new System.Drawing.Size(427, 22);
            this.txtResults.TabIndex = 22;
            // 
            // btnBack
            // 
            this.btnBack.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnBack.Location = new System.Drawing.Point(203, 12);
            this.btnBack.Name = "btnBack";
            this.btnBack.Size = new System.Drawing.Size(84, 26);
            this.btnBack.TabIndex = 18;
            this.btnBack.Text = "BACK";
            this.btnBack.UseVisualStyleBackColor = true;
            // 
            // btnExit
            // 
            this.btnExit.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnExit.Location = new System.Drawing.Point(323, 12);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(84, 26);
            this.btnExit.TabIndex = 19;
            this.btnExit.Text = "EXIT";
            this.btnExit.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(20, 57);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(56, 16);
            this.label1.TabIndex = 24;
            this.label1.Text = "Results:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(44, 93);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(32, 16);
            this.label2.TabIndex = 26;
            this.label2.Text = "Info:";
            // 
            // txtInfo
            // 
            this.txtInfo.BackColor = System.Drawing.Color.White;
            this.txtInfo.Location = new System.Drawing.Point(82, 90);
            this.txtInfo.Name = "txtInfo";
            this.txtInfo.ReadOnly = true;
            this.txtInfo.Size = new System.Drawing.Size(427, 22);
            this.txtInfo.TabIndex = 25;
            // 
            // ModelTest
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(549, 591);
            this.ControlBox = false;
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtInfo);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.pnlImages);
            this.Controls.Add(this.btnBack);
            this.Controls.Add(this.btnExit);
            this.Controls.Add(this.txtResults);
            this.Controls.Add(this.btnPredict);
            this.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ModelTest";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "Model Test";
            this.TopMost = true;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Panel pnlImages;
        public System.Windows.Forms.Button btnPredict;
        public System.Windows.Forms.TextBox txtResults;
        public System.Windows.Forms.Button btnBack;
        public System.Windows.Forms.Button btnExit;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        public System.Windows.Forms.TextBox txtInfo;
    }
}