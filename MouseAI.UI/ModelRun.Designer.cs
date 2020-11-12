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
            this.SuspendLayout();
            // 
            // btnExit
            // 
            this.btnExit.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnExit.Location = new System.Drawing.Point(197, 149);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(84, 26);
            this.btnExit.TabIndex = 21;
            this.btnExit.Text = "EXIT";
            this.btnExit.UseVisualStyleBackColor = true;
            // 
            // btnRun
            // 
            this.btnRun.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnRun.Location = new System.Drawing.Point(24, 82);
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
            this.tbxMaze.Size = new System.Drawing.Size(296, 22);
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
            this.btnStop.Location = new System.Drawing.Point(370, 82);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(84, 26);
            this.btnStop.TabIndex = 25;
            this.btnStop.Text = "STOP";
            this.btnStop.UseVisualStyleBackColor = true;
            // 
            // btnPause
            // 
            this.btnPause.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnPause.Location = new System.Drawing.Point(255, 82);
            this.btnPause.Name = "btnPause";
            this.btnPause.Size = new System.Drawing.Size(84, 26);
            this.btnPause.TabIndex = 26;
            this.btnPause.Text = "PAUSE";
            this.btnPause.UseVisualStyleBackColor = true;
            // 
            // btnStep
            // 
            this.btnStep.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnStep.Location = new System.Drawing.Point(142, 82);
            this.btnStep.Name = "btnStep";
            this.btnStep.Size = new System.Drawing.Size(84, 26);
            this.btnStep.TabIndex = 27;
            this.btnStep.Text = "STEP";
            this.btnStep.UseVisualStyleBackColor = true;
            // 
            // ModelRun
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(495, 205);
            this.ControlBox = false;
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
    }
}