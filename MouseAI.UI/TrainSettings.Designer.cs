namespace MouseAI.UI
{
    partial class TrainSettings
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
            this.btnTrain = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.nudEpochs = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.nudBatch = new System.Windows.Forms.NumericUpDown();
            this.chkNormalize = new System.Windows.Forms.CheckBox();
            this.chkEarlyStop = new System.Windows.Forms.CheckBox();
            this.label3 = new System.Windows.Forms.Label();
            this.nudLayers = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.nudNodes = new System.Windows.Forms.NumericUpDown();
            this.chkDropOut = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.nudEpochs)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudBatch)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudLayers)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudNodes)).BeginInit();
            this.SuspendLayout();
            // 
            // btnTrain
            // 
            this.btnTrain.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnTrain.Location = new System.Drawing.Point(109, 206);
            this.btnTrain.Name = "btnTrain";
            this.btnTrain.Size = new System.Drawing.Size(75, 23);
            this.btnTrain.TabIndex = 0;
            this.btnTrain.Text = "Train";
            this.btnTrain.UseVisualStyleBackColor = true;
            this.btnTrain.Click += new System.EventHandler(this.btnTrain_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCancel.Location = new System.Drawing.Point(250, 206);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // nudEpochs
            // 
            this.nudEpochs.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.nudEpochs.Location = new System.Drawing.Point(113, 46);
            this.nudEpochs.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.nudEpochs.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudEpochs.Name = "nudEpochs";
            this.nudEpochs.Size = new System.Drawing.Size(71, 22);
            this.nudEpochs.TabIndex = 2;
            this.nudEpochs.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(57, 49);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(52, 16);
            this.label1.TabIndex = 3;
            this.label1.Text = "Epochs";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(65, 92);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(42, 16);
            this.label2.TabIndex = 5;
            this.label2.Text = "Batch";
            // 
            // nudBatch
            // 
            this.nudBatch.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.nudBatch.Location = new System.Drawing.Point(113, 89);
            this.nudBatch.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudBatch.Name = "nudBatch";
            this.nudBatch.Size = new System.Drawing.Size(71, 22);
            this.nudBatch.TabIndex = 4;
            this.nudBatch.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // chkNormalize
            // 
            this.chkNormalize.AutoSize = true;
            this.chkNormalize.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.chkNormalize.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkNormalize.Location = new System.Drawing.Point(71, 150);
            this.chkNormalize.Name = "chkNormalize";
            this.chkNormalize.Size = new System.Drawing.Size(85, 20);
            this.chkNormalize.TabIndex = 6;
            this.chkNormalize.Text = "Normalize";
            this.chkNormalize.UseVisualStyleBackColor = true;
            // 
            // chkEarlyStop
            // 
            this.chkEarlyStop.AutoSize = true;
            this.chkEarlyStop.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.chkEarlyStop.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkEarlyStop.Location = new System.Drawing.Point(182, 150);
            this.chkEarlyStop.Name = "chkEarlyStop";
            this.chkEarlyStop.Size = new System.Drawing.Size(88, 20);
            this.chkEarlyStop.TabIndex = 7;
            this.chkEarlyStop.Text = "Early Stop";
            this.chkEarlyStop.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(219, 48);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(47, 16);
            this.label3.TabIndex = 9;
            this.label3.Text = "Layers";
            // 
            // nudLayers
            // 
            this.nudLayers.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.nudLayers.Location = new System.Drawing.Point(266, 46);
            this.nudLayers.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.nudLayers.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudLayers.Name = "nudLayers";
            this.nudLayers.Size = new System.Drawing.Size(71, 22);
            this.nudLayers.TabIndex = 8;
            this.nudLayers.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(219, 92);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(45, 16);
            this.label4.TabIndex = 11;
            this.label4.Text = "Nodes";
            // 
            // nudNodes
            // 
            this.nudNodes.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.nudNodes.Location = new System.Drawing.Point(266, 90);
            this.nudNodes.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.nudNodes.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudNodes.Name = "nudNodes";
            this.nudNodes.Size = new System.Drawing.Size(71, 22);
            this.nudNodes.TabIndex = 10;
            this.nudNodes.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // chkDropOut
            // 
            this.chkDropOut.AutoSize = true;
            this.chkDropOut.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.chkDropOut.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkDropOut.Location = new System.Drawing.Point(291, 150);
            this.chkDropOut.Name = "chkDropOut";
            this.chkDropOut.Size = new System.Drawing.Size(79, 20);
            this.chkDropOut.TabIndex = 12;
            this.chkDropOut.Text = "Drop Out";
            this.chkDropOut.UseVisualStyleBackColor = true;
            // 
            // TrainSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(435, 287);
            this.ControlBox = false;
            this.Controls.Add(this.chkDropOut);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.nudNodes);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.nudLayers);
            this.Controls.Add(this.chkEarlyStop);
            this.Controls.Add(this.chkNormalize);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.nudBatch);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.nudEpochs);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnTrain);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "TrainSettings";
            this.ShowInTaskbar = false;
            this.Text = "Train Settings";
            this.TopMost = true;
            ((System.ComponentModel.ISupportInitialize)(this.nudEpochs)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudBatch)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudLayers)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudNodes)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.NumericUpDown nudEpochs;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown nudBatch;
        private System.Windows.Forms.CheckBox chkNormalize;
        private System.Windows.Forms.CheckBox chkEarlyStop;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown nudLayers;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.NumericUpDown nudNodes;
        private System.Windows.Forms.CheckBox chkDropOut;
        public System.Windows.Forms.Button btnTrain;
    }
}