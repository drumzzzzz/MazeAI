﻿namespace MazeAI
{
    partial class MazeAI
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
            this.components = new System.ComponentModel.Container();
            this.txtMaze = new System.Windows.Forms.RichTextBox();
            this.btnExit = new System.Windows.Forms.Button();
            this.bindingSource1 = new System.Windows.Forms.BindingSource(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource1)).BeginInit();
            this.skiaView = new SkiaSharp.Views.Desktop.SKControl();
            this.SuspendLayout();
            // 
            // skiaView
            // 
            this.skiaView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.skiaView.Location = new System.Drawing.Point(0, 0);
            this.skiaView.Name = "skiaView";
            this.skiaView.Size = new System.Drawing.Size(774, 529);
            this.skiaView.TabIndex = 0;
            this.skiaView.Text = "skControl1";
            this.skiaView.PaintSurface += new System.EventHandler<SkiaSharp.Views.Desktop.SKPaintSurfaceEventArgs>(this.skiaView_PaintSurface);
            // 
            // txtMaze
            // 
            this.txtMaze.DetectUrls = false;
            this.txtMaze.Font = new System.Drawing.Font("Comic Sans MS", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtMaze.Location = new System.Drawing.Point(12, 12);
            this.txtMaze.Name = "txtMaze";
            this.txtMaze.ReadOnly = true;
            this.txtMaze.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;
            this.txtMaze.ShortcutsEnabled = false;
            this.txtMaze.Size = new System.Drawing.Size(755, 537);
            this.txtMaze.TabIndex = 0;
            this.txtMaze.Text = "";
            this.txtMaze.WordWrap = false;
            // 
            // btnExit
            // 
            this.btnExit.Location = new System.Drawing.Point(361, 568);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(75, 23);
            this.btnExit.TabIndex = 1;
            this.btnExit.Text = "Exit";
            this.btnExit.UseVisualStyleBackColor = true;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // MazeAI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(779, 603);
            this.Controls.Add(this.btnExit);
            this.Controls.Add(this.skiaView);
            this.Controls.Add(this.txtMaze);
            this.Name = "MazeAI";
            this.Text = "Form1";
            this.WindowState = System.Windows.Forms.FormWindowState.Minimized;
            this.Shown += new System.EventHandler(this.MazeAI_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RichTextBox txtMaze;
        private System.Windows.Forms.Button btnExit;
        private System.Windows.Forms.BindingSource bindingSource1;
        private SkiaSharp.Views.Desktop.SKControl skiaView;
    }
}

