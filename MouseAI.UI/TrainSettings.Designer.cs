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
            this.label5 = new System.Windows.Forms.Label();
            this.nudSeed = new System.Windows.Forms.NumericUpDown();
            this.label6 = new System.Windows.Forms.Label();
            this.nudSplit = new System.Windows.Forms.NumericUpDown();
            this.label7 = new System.Windows.Forms.Label();
            this.nudDropOut = new System.Windows.Forms.NumericUpDown();
            this.label8 = new System.Windows.Forms.Label();
            this.nudDropOutLayers = new System.Windows.Forms.NumericUpDown();
            this.chkCNN = new System.Windows.Forms.CheckBox();
            this.nudLearnRate = new System.Windows.Forms.NumericUpDown();
            this.label9 = new System.Windows.Forms.Label();
            this.nudLearnDecay = new System.Windows.Forms.NumericUpDown();
            this.label10 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            ((System.ComponentModel.ISupportInitialize)(this.nudEpochs)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudBatch)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudLayers)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudNodes)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudSeed)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudSplit)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudDropOut)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudDropOutLayers)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudLearnRate)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudLearnDecay)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnTrain
            // 
            this.btnTrain.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnTrain.Location = new System.Drawing.Point(557, 178);
            this.btnTrain.Name = "btnTrain";
            this.btnTrain.Size = new System.Drawing.Size(100, 40);
            this.btnTrain.TabIndex = 0;
            this.btnTrain.Text = "Train";
            this.btnTrain.UseVisualStyleBackColor = true;
            this.btnTrain.Click += new System.EventHandler(this.btnTrain_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCancel.Location = new System.Drawing.Point(557, 236);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(100, 40);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // nudEpochs
            // 
            this.nudEpochs.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.nudEpochs.Location = new System.Drawing.Point(77, 39);
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
            this.label1.Location = new System.Drawing.Point(21, 42);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(52, 16);
            this.label1.TabIndex = 3;
            this.label1.Text = "Epochs";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(29, 85);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(42, 16);
            this.label2.TabIndex = 5;
            this.label2.Text = "Batch";
            // 
            // nudBatch
            // 
            this.nudBatch.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.nudBatch.Location = new System.Drawing.Point(77, 82);
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
            this.chkNormalize.Location = new System.Drawing.Point(22, 83);
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
            this.chkEarlyStop.Location = new System.Drawing.Point(169, 75);
            this.chkEarlyStop.Name = "chkEarlyStop";
            this.chkEarlyStop.Size = new System.Drawing.Size(61, 36);
            this.chkEarlyStop.TabIndex = 7;
            this.chkEarlyStop.Text = "Early \r\nStop";
            this.chkEarlyStop.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(20, 39);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(47, 16);
            this.label3.TabIndex = 9;
            this.label3.Text = "Layers";
            // 
            // nudLayers
            // 
            this.nudLayers.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.nudLayers.Location = new System.Drawing.Point(77, 37);
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
            this.label4.Location = new System.Drawing.Point(20, 83);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(45, 16);
            this.label4.TabIndex = 11;
            this.label4.Text = "Nodes";
            // 
            // nudNodes
            // 
            this.nudNodes.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.nudNodes.Location = new System.Drawing.Point(77, 81);
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
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(19, 39);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(34, 16);
            this.label5.TabIndex = 14;
            this.label5.Text = "Split";
            // 
            // nudSeed
            // 
            this.nudSeed.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.nudSeed.Location = new System.Drawing.Point(210, 39);
            this.nudSeed.Name = "nudSeed";
            this.nudSeed.Size = new System.Drawing.Size(71, 22);
            this.nudSeed.TabIndex = 13;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(166, 42);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(38, 16);
            this.label6.TabIndex = 16;
            this.label6.Text = "Seed";
            // 
            // nudSplit
            // 
            this.nudSplit.DecimalPlaces = 2;
            this.nudSplit.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.nudSplit.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.nudSplit.Location = new System.Drawing.Point(78, 39);
            this.nudSplit.Maximum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudSplit.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.nudSplit.Name = "nudSplit";
            this.nudSplit.Size = new System.Drawing.Size(71, 22);
            this.nudSplit.TabIndex = 15;
            this.nudSplit.Value = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(19, 83);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(47, 16);
            this.label7.TabIndex = 20;
            this.label7.Text = "Layers";
            // 
            // nudDropOut
            // 
            this.nudDropOut.DecimalPlaces = 2;
            this.nudDropOut.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.nudDropOut.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.nudDropOut.Location = new System.Drawing.Point(78, 39);
            this.nudDropOut.Maximum = new decimal(new int[] {
            99,
            0,
            0,
            131072});
            this.nudDropOut.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.nudDropOut.Name = "nudDropOut";
            this.nudDropOut.Size = new System.Drawing.Size(71, 22);
            this.nudDropOut.TabIndex = 19;
            this.nudDropOut.Value = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(19, 41);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(37, 16);
            this.label8.TabIndex = 18;
            this.label8.Text = "Level";
            // 
            // nudDropOutLayers
            // 
            this.nudDropOutLayers.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.nudDropOutLayers.Location = new System.Drawing.Point(78, 81);
            this.nudDropOutLayers.Name = "nudDropOutLayers";
            this.nudDropOutLayers.Size = new System.Drawing.Size(71, 22);
            this.nudDropOutLayers.TabIndex = 17;
            // 
            // chkCNN
            // 
            this.chkCNN.AutoSize = true;
            this.chkCNN.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.chkCNN.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkCNN.Location = new System.Drawing.Point(174, 38);
            this.chkCNN.Name = "chkCNN";
            this.chkCNN.Size = new System.Drawing.Size(54, 20);
            this.chkCNN.TabIndex = 21;
            this.chkCNN.Text = "CNN";
            this.chkCNN.UseVisualStyleBackColor = true;
            // 
            // nudLearnRate
            // 
            this.nudLearnRate.DecimalPlaces = 4;
            this.nudLearnRate.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.nudLearnRate.Increment = new decimal(new int[] {
            1,
            0,
            0,
            262144});
            this.nudLearnRate.Location = new System.Drawing.Point(84, 39);
            this.nudLearnRate.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            196608});
            this.nudLearnRate.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            262144});
            this.nudLearnRate.Name = "nudLearnRate";
            this.nudLearnRate.Size = new System.Drawing.Size(71, 22);
            this.nudLearnRate.TabIndex = 23;
            this.nudLearnRate.Value = new decimal(new int[] {
            1,
            0,
            0,
            196608});
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.Location = new System.Drawing.Point(28, 41);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(35, 16);
            this.label9.TabIndex = 22;
            this.label9.Text = "Rate";
            // 
            // nudLearnDecay
            // 
            this.nudLearnDecay.Cursor = System.Windows.Forms.Cursors.Default;
            this.nudLearnDecay.DecimalPlaces = 4;
            this.nudLearnDecay.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.nudLearnDecay.Increment = new decimal(new int[] {
            1,
            0,
            0,
            262144});
            this.nudLearnDecay.Location = new System.Drawing.Point(84, 81);
            this.nudLearnDecay.Maximum = new decimal(new int[] {
            99,
            0,
            0,
            131072});
            this.nudLearnDecay.Name = "nudLearnDecay";
            this.nudLearnDecay.Size = new System.Drawing.Size(71, 22);
            this.nudLearnDecay.TabIndex = 25;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label10.Location = new System.Drawing.Point(28, 83);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(45, 16);
            this.label10.TabIndex = 24;
            this.label10.Text = "Decay";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.nudDropOut);
            this.groupBox1.Controls.Add(this.nudDropOutLayers);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.Location = new System.Drawing.Point(331, 161);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(175, 132);
            this.groupBox1.TabIndex = 26;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Drop Out";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.chkEarlyStop);
            this.groupBox2.Controls.Add(this.nudEpochs);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.nudBatch);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.nudSeed);
            this.groupBox2.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox2.Location = new System.Drawing.Point(12, 161);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(304, 132);
            this.groupBox2.TabIndex = 27;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Training";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.nudLearnRate);
            this.groupBox3.Controls.Add(this.label9);
            this.groupBox3.Controls.Add(this.label10);
            this.groupBox3.Controls.Add(this.nudLearnDecay);
            this.groupBox3.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox3.Location = new System.Drawing.Point(512, 12);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(184, 132);
            this.groupBox3.TabIndex = 27;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Learning";
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.chkCNN);
            this.groupBox4.Controls.Add(this.nudLayers);
            this.groupBox4.Controls.Add(this.label3);
            this.groupBox4.Controls.Add(this.nudNodes);
            this.groupBox4.Controls.Add(this.label4);
            this.groupBox4.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox4.Location = new System.Drawing.Point(12, 12);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(304, 132);
            this.groupBox4.TabIndex = 27;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Network";
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.chkNormalize);
            this.groupBox5.Controls.Add(this.nudSplit);
            this.groupBox5.Controls.Add(this.label5);
            this.groupBox5.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox5.Location = new System.Drawing.Point(331, 12);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(175, 132);
            this.groupBox5.TabIndex = 27;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Data Sets";
            // 
            // TrainSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(739, 310);
            this.ControlBox = false;
            this.Controls.Add(this.groupBox5);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
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
            ((System.ComponentModel.ISupportInitialize)(this.nudSeed)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudSplit)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudDropOut)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudDropOutLayers)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudLearnRate)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudLearnDecay)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.ResumeLayout(false);

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
        public System.Windows.Forms.Button btnTrain;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.NumericUpDown nudSeed;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.NumericUpDown nudSplit;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.NumericUpDown nudDropOut;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.NumericUpDown nudDropOutLayers;
        private System.Windows.Forms.CheckBox chkCNN;
        private System.Windows.Forms.NumericUpDown nudLearnRate;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.NumericUpDown nudLearnDecay;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.GroupBox groupBox5;
    }
}