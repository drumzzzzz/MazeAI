namespace MouseAI.UI
{
    partial class NeuralLayer
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.nudNodes = new System.Windows.Forms.NumericUpDown();
            this.nudDropout = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.nudNodes)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudDropout)).BeginInit();
            this.SuspendLayout();
            // 
            // nudNodes
            // 
            this.nudNodes.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.nudNodes.Location = new System.Drawing.Point(55, 5);
            this.nudNodes.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.nudNodes.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudNodes.Name = "nudNodes";
            this.nudNodes.Size = new System.Drawing.Size(70, 22);
            this.nudNodes.TabIndex = 0;
            this.nudNodes.Value = new decimal(new int[] {
            100,
            0,
            0,
            0});
            // 
            // nudDropout
            // 
            this.nudDropout.DecimalPlaces = 3;
            this.nudDropout.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.nudDropout.Increment = new decimal(new int[] {
            1,
            0,
            0,
            196608});
            this.nudDropout.Location = new System.Drawing.Point(190, 5);
            this.nudDropout.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            196608});
            this.nudDropout.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            196608});
            this.nudDropout.Name = "nudDropout";
            this.nudDropout.Size = new System.Drawing.Size(58, 22);
            this.nudDropout.TabIndex = 1;
            this.nudDropout.Value = new decimal(new int[] {
            5,
            0,
            0,
            131072});
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(4, 7);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(45, 16);
            this.label1.TabIndex = 2;
            this.label1.Text = "Nodes";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(131, 7);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 16);
            this.label2.TabIndex = 3;
            this.label2.Text = "Dropout";
            // 
            // NeuralLayer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.nudDropout);
            this.Controls.Add(this.nudNodes);
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "NeuralLayer";
            this.Size = new System.Drawing.Size(256, 31);
            ((System.ComponentModel.ISupportInitialize)(this.nudNodes)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudDropout)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        public System.Windows.Forms.NumericUpDown nudNodes;
        public System.Windows.Forms.NumericUpDown nudDropout;
    }
}
