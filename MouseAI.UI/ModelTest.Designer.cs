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
            this.btnBack = new System.Windows.Forms.Button();
            this.btnExit = new System.Windows.Forms.Button();
            this.btnValidate = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnBack
            // 
            this.btnBack.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnBack.Location = new System.Drawing.Point(203, 231);
            this.btnBack.Name = "btnBack";
            this.btnBack.Size = new System.Drawing.Size(77, 26);
            this.btnBack.TabIndex = 18;
            this.btnBack.Text = "BACK";
            this.btnBack.UseVisualStyleBackColor = true;
            // 
            // btnExit
            // 
            this.btnExit.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnExit.Location = new System.Drawing.Point(341, 231);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(77, 26);
            this.btnExit.TabIndex = 19;
            this.btnExit.Text = "EXIT";
            this.btnExit.UseVisualStyleBackColor = true;
            // 
            // btnValidate
            // 
            this.btnValidate.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnValidate.Location = new System.Drawing.Point(34, 12);
            this.btnValidate.Name = "btnValidate";
            this.btnValidate.Size = new System.Drawing.Size(84, 26);
            this.btnValidate.TabIndex = 20;
            this.btnValidate.Text = "VALIDATE";
            this.btnValidate.UseVisualStyleBackColor = true;
            // 
            // ModelTest
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(686, 298);
            this.ControlBox = false;
            this.Controls.Add(this.btnValidate);
            this.Controls.Add(this.btnExit);
            this.Controls.Add(this.btnBack);
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

        }

        #endregion
        public System.Windows.Forms.Button btnBack;
        public System.Windows.Forms.Button btnExit;
        public System.Windows.Forms.Button btnValidate;
    }
}