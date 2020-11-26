namespace MouseAI.UI
{
    partial class ModelRun
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
            this.pnlPlot = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.btnExit = new System.Windows.Forms.Button();
            this.btnRun = new System.Windows.Forms.Button();
            this.tbxTime = new System.Windows.Forms.TextBox();
            this.btnRunAll = new System.Windows.Forms.Button();
            this.chkRandomWander = new System.Windows.Forms.CheckBox();
            this.btnBack = new System.Windows.Forms.Button();
            this.btnStep = new System.Windows.Forms.Button();
            this.btnPause = new System.Windows.Forms.Button();
            this.btnStop = new System.Windows.Forms.Button();
            this.tbxMouseStatus = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.chkDebug = new System.Windows.Forms.CheckBox();
            this.rdoPaths = new System.Windows.Forms.RadioButton();
            this.rdoVisible = new System.Windows.Forms.RadioButton();
            this.rdoNone = new System.Windows.Forms.RadioButton();
            this.nudRate = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.rdoNeural = new System.Windows.Forms.RadioButton();
            ((System.ComponentModel.ISupportInitialize)(this.nudRate)).BeginInit();
            this.SuspendLayout();
            // 
            // pnlPlot
            // 
            this.pnlPlot.Location = new System.Drawing.Point(364, 12);
            this.pnlPlot.Margin = new System.Windows.Forms.Padding(0);
            this.pnlPlot.Name = "pnlPlot";
            this.pnlPlot.Size = new System.Drawing.Size(646, 318);
            this.pnlPlot.TabIndex = 38;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(14, 126);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(40, 16);
            this.label2.TabIndex = 49;
            this.label2.Text = "Time:";
            // 
            // btnExit
            // 
            this.btnExit.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnExit.Location = new System.Drawing.Point(128, 290);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(100, 40);
            this.btnExit.TabIndex = 40;
            this.btnExit.Text = "EXIT";
            this.btnExit.UseVisualStyleBackColor = true;
            // 
            // btnRun
            // 
            this.btnRun.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnRun.Location = new System.Drawing.Point(13, 172);
            this.btnRun.Name = "btnRun";
            this.btnRun.Size = new System.Drawing.Size(100, 40);
            this.btnRun.TabIndex = 41;
            this.btnRun.Text = "RUN";
            this.btnRun.UseVisualStyleBackColor = true;
            // 
            // tbxTime
            // 
            this.tbxTime.BackColor = System.Drawing.SystemColors.Window;
            this.tbxTime.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tbxTime.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbxTime.Location = new System.Drawing.Point(64, 124);
            this.tbxTime.Name = "tbxTime";
            this.tbxTime.ReadOnly = true;
            this.tbxTime.Size = new System.Drawing.Size(100, 22);
            this.tbxTime.TabIndex = 47;
            this.tbxTime.WordWrap = false;
            // 
            // btnRunAll
            // 
            this.btnRunAll.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnRunAll.Location = new System.Drawing.Point(128, 172);
            this.btnRunAll.Name = "btnRunAll";
            this.btnRunAll.Size = new System.Drawing.Size(100, 40);
            this.btnRunAll.TabIndex = 51;
            this.btnRunAll.Text = "RUN ALL";
            this.btnRunAll.UseVisualStyleBackColor = true;
            // 
            // chkRandomWander
            // 
            this.chkRandomWander.AutoSize = true;
            this.chkRandomWander.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.chkRandomWander.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkRandomWander.Location = new System.Drawing.Point(169, 24);
            this.chkRandomWander.Name = "chkRandomWander";
            this.chkRandomWander.Size = new System.Drawing.Size(75, 36);
            this.chkRandomWander.TabIndex = 39;
            this.chkRandomWander.Text = "Random\r\nSearch:";
            this.chkRandomWander.UseVisualStyleBackColor = true;
            // 
            // btnBack
            // 
            this.btnBack.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnBack.Location = new System.Drawing.Point(13, 290);
            this.btnBack.Name = "btnBack";
            this.btnBack.Size = new System.Drawing.Size(100, 40);
            this.btnBack.TabIndex = 45;
            this.btnBack.Text = "BACK";
            this.btnBack.UseVisualStyleBackColor = true;
            // 
            // btnStep
            // 
            this.btnStep.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnStep.Location = new System.Drawing.Point(243, 231);
            this.btnStep.Name = "btnStep";
            this.btnStep.Size = new System.Drawing.Size(100, 40);
            this.btnStep.TabIndex = 44;
            this.btnStep.Text = "STEP";
            this.btnStep.UseVisualStyleBackColor = true;
            // 
            // btnPause
            // 
            this.btnPause.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnPause.Location = new System.Drawing.Point(128, 231);
            this.btnPause.Name = "btnPause";
            this.btnPause.Size = new System.Drawing.Size(100, 40);
            this.btnPause.TabIndex = 43;
            this.btnPause.Text = "PAUSE";
            this.btnPause.UseVisualStyleBackColor = true;
            // 
            // btnStop
            // 
            this.btnStop.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnStop.Location = new System.Drawing.Point(13, 231);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(100, 40);
            this.btnStop.TabIndex = 42;
            this.btnStop.Text = "STOP";
            this.btnStop.UseVisualStyleBackColor = true;
            // 
            // tbxMouseStatus
            // 
            this.tbxMouseStatus.BackColor = System.Drawing.SystemColors.Window;
            this.tbxMouseStatus.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tbxMouseStatus.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbxMouseStatus.Location = new System.Drawing.Point(231, 124);
            this.tbxMouseStatus.Name = "tbxMouseStatus";
            this.tbxMouseStatus.ReadOnly = true;
            this.tbxMouseStatus.Size = new System.Drawing.Size(122, 22);
            this.tbxMouseStatus.TabIndex = 48;
            this.tbxMouseStatus.WordWrap = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(178, 118);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(50, 32);
            this.label1.TabIndex = 46;
            this.label1.Text = "Mouse\r\nStatus:";
            // 
            // chkDebug
            // 
            this.chkDebug.AutoSize = true;
            this.chkDebug.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.chkDebug.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkDebug.Location = new System.Drawing.Point(261, 24);
            this.chkDebug.Name = "chkDebug";
            this.chkDebug.Size = new System.Drawing.Size(70, 36);
            this.chkDebug.TabIndex = 57;
            this.chkDebug.Text = "Debug\r\nOutput:";
            this.chkDebug.UseVisualStyleBackColor = true;
            // 
            // rdoPaths
            // 
            this.rdoPaths.AutoSize = true;
            this.rdoPaths.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.rdoPaths.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rdoPaths.Location = new System.Drawing.Point(250, 80);
            this.rdoPaths.Margin = new System.Windows.Forms.Padding(0);
            this.rdoPaths.Name = "rdoPaths";
            this.rdoPaths.Size = new System.Drawing.Size(60, 20);
            this.rdoPaths.TabIndex = 62;
            this.rdoPaths.Text = "Paths";
            this.rdoPaths.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.rdoPaths.UseVisualStyleBackColor = true;
            // 
            // rdoVisible
            // 
            this.rdoVisible.AutoSize = true;
            this.rdoVisible.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.rdoVisible.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rdoVisible.Location = new System.Drawing.Point(96, 80);
            this.rdoVisible.Margin = new System.Windows.Forms.Padding(0);
            this.rdoVisible.Name = "rdoVisible";
            this.rdoVisible.Size = new System.Drawing.Size(65, 20);
            this.rdoVisible.TabIndex = 61;
            this.rdoVisible.Text = "Visible";
            this.rdoVisible.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.rdoVisible.UseVisualStyleBackColor = true;
            // 
            // rdoNone
            // 
            this.rdoNone.AutoSize = true;
            this.rdoNone.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.rdoNone.Checked = true;
            this.rdoNone.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rdoNone.Location = new System.Drawing.Point(32, 80);
            this.rdoNone.Margin = new System.Windows.Forms.Padding(0);
            this.rdoNone.Name = "rdoNone";
            this.rdoNone.Size = new System.Drawing.Size(56, 20);
            this.rdoNone.TabIndex = 60;
            this.rdoNone.TabStop = true;
            this.rdoNone.Text = "None";
            this.rdoNone.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.rdoNone.UseVisualStyleBackColor = true;
            // 
            // nudRate
            // 
            this.nudRate.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.nudRate.Increment = new decimal(new int[] {
            25,
            0,
            0,
            0});
            this.nudRate.Location = new System.Drawing.Point(61, 31);
            this.nudRate.Maximum = new decimal(new int[] {
            2000,
            0,
            0,
            0});
            this.nudRate.Name = "nudRate";
            this.nudRate.Size = new System.Drawing.Size(82, 22);
            this.nudRate.TabIndex = 58;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(16, 33);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(39, 16);
            this.label3.TabIndex = 59;
            this.label3.Text = "Rate:";
            // 
            // rdoNeural
            // 
            this.rdoNeural.AutoSize = true;
            this.rdoNeural.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.rdoNeural.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rdoNeural.Location = new System.Drawing.Point(174, 80);
            this.rdoNeural.Margin = new System.Windows.Forms.Padding(0);
            this.rdoNeural.Name = "rdoNeural";
            this.rdoNeural.Size = new System.Drawing.Size(63, 20);
            this.rdoNeural.TabIndex = 63;
            this.rdoNeural.Text = "Neural";
            this.rdoNeural.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.rdoNeural.UseVisualStyleBackColor = true;
            // 
            // ModelRun
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1014, 332);
            this.ControlBox = false;
            this.Controls.Add(this.rdoNeural);
            this.Controls.Add(this.chkDebug);
            this.Controls.Add(this.rdoPaths);
            this.Controls.Add(this.chkRandomWander);
            this.Controls.Add(this.rdoVisible);
            this.Controls.Add(this.rdoNone);
            this.Controls.Add(this.nudRate);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.btnExit);
            this.Controls.Add(this.btnRun);
            this.Controls.Add(this.tbxTime);
            this.Controls.Add(this.btnRunAll);
            this.Controls.Add(this.btnBack);
            this.Controls.Add(this.btnStep);
            this.Controls.Add(this.btnPause);
            this.Controls.Add(this.btnStop);
            this.Controls.Add(this.tbxMouseStatus);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.pnlPlot);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "ModelRun";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "Model Run";
            this.TopMost = true;
            ((System.ComponentModel.ISupportInitialize)(this.nudRate)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Panel pnlPlot;
        private System.Windows.Forms.Label label2;
        public System.Windows.Forms.Button btnExit;
        public System.Windows.Forms.Button btnRun;
        public System.Windows.Forms.TextBox tbxTime;
        public System.Windows.Forms.Button btnRunAll;
        public System.Windows.Forms.CheckBox chkRandomWander;
        public System.Windows.Forms.Button btnBack;
        public System.Windows.Forms.Button btnStep;
        public System.Windows.Forms.Button btnPause;
        public System.Windows.Forms.Button btnStop;
        public System.Windows.Forms.TextBox tbxMouseStatus;
        private System.Windows.Forms.Label label1;
        public System.Windows.Forms.CheckBox chkDebug;
        public System.Windows.Forms.RadioButton rdoPaths;
        public System.Windows.Forms.RadioButton rdoVisible;
        public System.Windows.Forms.RadioButton rdoNone;
        public System.Windows.Forms.NumericUpDown nudRate;
        private System.Windows.Forms.Label label3;
        public System.Windows.Forms.RadioButton rdoNeural;
    }
}