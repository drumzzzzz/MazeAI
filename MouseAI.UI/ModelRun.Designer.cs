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
            this.btnExit = new System.Windows.Forms.Button();
            this.btnRun = new System.Windows.Forms.Button();
            this.tbxMaze = new System.Windows.Forms.TextBox();
            this.lblMaze = new System.Windows.Forms.Label();
            this.btnStop = new System.Windows.Forms.Button();
            this.btnPause = new System.Windows.Forms.Button();
            this.btnStep = new System.Windows.Forms.Button();
            this.btnBack = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.tbxMouseStatus = new System.Windows.Forms.TextBox();
            this.nudRate = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.btnRunAll = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.nudRate)).BeginInit();
            this.SuspendLayout();
            // 
            // btnExit
            // 
            this.btnExit.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnExit.Location = new System.Drawing.Point(243, 221);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(84, 26);
            this.btnExit.TabIndex = 21;
            this.btnExit.Text = "EXIT";
            this.btnExit.UseVisualStyleBackColor = true;
            // 
            // btnRun
            // 
            this.btnRun.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnRun.Location = new System.Drawing.Point(12, 157);
            this.btnRun.Name = "btnRun";
            this.btnRun.Size = new System.Drawing.Size(84, 26);
            this.btnRun.TabIndex = 22;
            this.btnRun.Text = "RUN";
            this.btnRun.UseVisualStyleBackColor = true;
            // 
            // tbxMaze
            // 
            this.tbxMaze.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbxMaze.Location = new System.Drawing.Point(71, 32);
            this.tbxMaze.Name = "tbxMaze";
            this.tbxMaze.ReadOnly = true;
            this.tbxMaze.Size = new System.Drawing.Size(373, 22);
            this.tbxMaze.TabIndex = 23;
            this.tbxMaze.WordWrap = false;
            // 
            // lblMaze
            // 
            this.lblMaze.AutoSize = true;
            this.lblMaze.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMaze.Location = new System.Drawing.Point(21, 35);
            this.lblMaze.Name = "lblMaze";
            this.lblMaze.Size = new System.Drawing.Size(44, 16);
            this.lblMaze.TabIndex = 24;
            this.lblMaze.Text = "Maze:";
            // 
            // btnStop
            // 
            this.btnStop.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnStop.Location = new System.Drawing.Point(358, 157);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(84, 26);
            this.btnStop.TabIndex = 25;
            this.btnStop.Text = "STOP";
            this.btnStop.UseVisualStyleBackColor = true;
            // 
            // btnPause
            // 
            this.btnPause.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnPause.Location = new System.Drawing.Point(243, 157);
            this.btnPause.Name = "btnPause";
            this.btnPause.Size = new System.Drawing.Size(84, 26);
            this.btnPause.TabIndex = 26;
            this.btnPause.Text = "PAUSE";
            this.btnPause.UseVisualStyleBackColor = true;
            // 
            // btnStep
            // 
            this.btnStep.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnStep.Location = new System.Drawing.Point(130, 157);
            this.btnStep.Name = "btnStep";
            this.btnStep.Size = new System.Drawing.Size(84, 26);
            this.btnStep.TabIndex = 27;
            this.btnStep.Text = "STEP";
            this.btnStep.UseVisualStyleBackColor = true;
            // 
            // btnBack
            // 
            this.btnBack.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnBack.Location = new System.Drawing.Point(130, 221);
            this.btnBack.Name = "btnBack";
            this.btnBack.Size = new System.Drawing.Size(84, 26);
            this.btnBack.TabIndex = 28;
            this.btnBack.Text = "BACK";
            this.btnBack.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(11, 83);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(50, 32);
            this.label1.TabIndex = 30;
            this.label1.Text = "Mouse\r\nStatus:";
            this.label1.Click += new System.EventHandler(this.label1_Click);
            // 
            // tbxMouseStatus
            // 
            this.tbxMouseStatus.BackColor = System.Drawing.SystemColors.Window;
            this.tbxMouseStatus.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tbxMouseStatus.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbxMouseStatus.Location = new System.Drawing.Point(71, 89);
            this.tbxMouseStatus.Name = "tbxMouseStatus";
            this.tbxMouseStatus.ReadOnly = true;
            this.tbxMouseStatus.Size = new System.Drawing.Size(166, 22);
            this.tbxMouseStatus.TabIndex = 32;
            this.tbxMouseStatus.WordWrap = false;
            // 
            // nudRate
            // 
            this.nudRate.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.nudRate.Location = new System.Drawing.Point(358, 89);
            this.nudRate.Maximum = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.nudRate.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudRate.Name = "nudRate";
            this.nudRate.Size = new System.Drawing.Size(55, 22);
            this.nudRate.TabIndex = 34;
            this.nudRate.Value = new decimal(new int[] {
            3,
            0,
            0,
            0});
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(313, 91);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(39, 16);
            this.label3.TabIndex = 35;
            this.label3.Text = "Rate:";
            // 
            // btnRunAll
            // 
            this.btnRunAll.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnRunAll.Location = new System.Drawing.Point(12, 220);
            this.btnRunAll.Name = "btnRunAll";
            this.btnRunAll.Size = new System.Drawing.Size(84, 26);
            this.btnRunAll.TabIndex = 36;
            this.btnRunAll.Text = "RUN ALL";
            this.btnRunAll.UseVisualStyleBackColor = true;
            // 
            // ModelRun
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(469, 272);
            this.ControlBox = false;
            this.Controls.Add(this.btnRunAll);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.nudRate);
            this.Controls.Add(this.tbxMouseStatus);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnBack);
            this.Controls.Add(this.btnStep);
            this.Controls.Add(this.btnPause);
            this.Controls.Add(this.btnStop);
            this.Controls.Add(this.lblMaze);
            this.Controls.Add(this.tbxMaze);
            this.Controls.Add(this.btnExit);
            this.Controls.Add(this.btnRun);
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

        public System.Windows.Forms.Button btnExit;
        public System.Windows.Forms.Button btnRun;
        private System.Windows.Forms.Label lblMaze;
        public System.Windows.Forms.TextBox tbxMaze;
        public System.Windows.Forms.Button btnStop;
        public System.Windows.Forms.Button btnPause;
        public System.Windows.Forms.Button btnStep;
        public System.Windows.Forms.Button btnBack;
        private System.Windows.Forms.Label label1;
        public System.Windows.Forms.TextBox tbxMouseStatus;
        private System.Windows.Forms.NumericUpDown nudRate;
        private System.Windows.Forms.Label label3;
        public System.Windows.Forms.Button btnRunAll;
    }
}