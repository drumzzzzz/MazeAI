namespace MouseAI.UI
{
    partial class MouseAI
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MouseAI));
            this.stpStatus = new System.Windows.Forms.StatusStrip();
            this.tsStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.msMain = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.buildToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.testToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.trainToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.testToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.debugToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.autorunToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadLastToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mazeTextToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.btnRun = new System.Windows.Forms.Button();
            this.btnStop = new System.Windows.Forms.Button();
            this.btnPause = new System.Windows.Forms.Button();
            this.btnStep = new System.Windows.Forms.Button();
            this.lvwMazes = new System.Windows.Forms.ListView();
            this.c1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.c2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.btnReset = new System.Windows.Forms.Button();
            this.pbxPath = new System.Windows.Forms.PictureBox();
            this.pbxMaze = new System.Windows.Forms.PictureBox();
            this.pbxSegment = new System.Windows.Forms.PictureBox();
            this.stpStatus.SuspendLayout();
            this.msMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbxPath)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbxMaze)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbxSegment)).BeginInit();
            this.SuspendLayout();
            // 
            // stpStatus
            // 
            this.stpStatus.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsStatus});
            this.stpStatus.Location = new System.Drawing.Point(0, 685);
            this.stpStatus.Name = "stpStatus";
            this.stpStatus.Size = new System.Drawing.Size(894, 40);
            this.stpStatus.TabIndex = 5;
            this.stpStatus.Text = "statusStrip1";
            // 
            // tsStatus
            // 
            this.tsStatus.AutoSize = false;
            this.tsStatus.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
            this.tsStatus.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsStatus.Font = new System.Drawing.Font("Arial Narrow", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tsStatus.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.tsStatus.Margin = new System.Windows.Forms.Padding(10, 3, 0, 2);
            this.tsStatus.Name = "tsStatus";
            this.tsStatus.Overflow = System.Windows.Forms.ToolStripItemOverflow.Never;
            this.tsStatus.Size = new System.Drawing.Size(400, 35);
            this.tsStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // msMain
            // 
            this.msMain.AutoSize = false;
            this.msMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.buildToolStripMenuItem,
            this.editToolStripMenuItem});
            this.msMain.Location = new System.Drawing.Point(0, 0);
            this.msMain.Name = "msMain";
            this.msMain.Size = new System.Drawing.Size(894, 35);
            this.msMain.TabIndex = 6;
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newToolStripMenuItem,
            this.loadToolStripMenuItem,
            this.saveToolStripMenuItem});
            this.fileToolStripMenuItem.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(41, 31);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // newToolStripMenuItem
            // 
            this.newToolStripMenuItem.Name = "newToolStripMenuItem";
            this.newToolStripMenuItem.Size = new System.Drawing.Size(104, 22);
            this.newToolStripMenuItem.Text = "New";
            this.newToolStripMenuItem.Click += new System.EventHandler(this.newToolStripMenuItem_Click);
            // 
            // loadToolStripMenuItem
            // 
            this.loadToolStripMenuItem.Name = "loadToolStripMenuItem";
            this.loadToolStripMenuItem.Size = new System.Drawing.Size(104, 22);
            this.loadToolStripMenuItem.Text = "Load";
            this.loadToolStripMenuItem.Click += new System.EventHandler(this.loadToolStripMenuItem_Click);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(104, 22);
            this.saveToolStripMenuItem.Text = "Save";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
            // 
            // buildToolStripMenuItem
            // 
            this.buildToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.testToolStripMenuItem,
            this.trainToolStripMenuItem,
            this.testToolStripMenuItem1});
            this.buildToolStripMenuItem.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buildToolStripMenuItem.Name = "buildToolStripMenuItem";
            this.buildToolStripMenuItem.Size = new System.Drawing.Size(49, 31);
            this.buildToolStripMenuItem.Text = "Build";
            // 
            // testToolStripMenuItem
            // 
            this.testToolStripMenuItem.Name = "testToolStripMenuItem";
            this.testToolStripMenuItem.Size = new System.Drawing.Size(110, 22);
            this.testToolStripMenuItem.Text = "Paths";
            this.testToolStripMenuItem.Click += new System.EventHandler(this.testToolStripMenuItem_Click);
            // 
            // trainToolStripMenuItem
            // 
            this.trainToolStripMenuItem.Name = "trainToolStripMenuItem";
            this.trainToolStripMenuItem.Size = new System.Drawing.Size(110, 22);
            this.trainToolStripMenuItem.Text = "Train";
            this.trainToolStripMenuItem.Click += new System.EventHandler(this.trainToolStripMenuItem_Click);
            // 
            // testToolStripMenuItem1
            // 
            this.testToolStripMenuItem1.Name = "testToolStripMenuItem1";
            this.testToolStripMenuItem1.Size = new System.Drawing.Size(110, 22);
            this.testToolStripMenuItem1.Text = "Test";
            this.testToolStripMenuItem1.Click += new System.EventHandler(this.testToolStripMenuItem1_Click);
            // 
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.debugToolStripMenuItem,
            this.autorunToolStripMenuItem,
            this.loadLastToolStripMenuItem,
            this.mazeTextToolStripMenuItem});
            this.editToolStripMenuItem.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            this.editToolStripMenuItem.Size = new System.Drawing.Size(65, 31);
            this.editToolStripMenuItem.Text = "Options";
            // 
            // debugToolStripMenuItem
            // 
            this.debugToolStripMenuItem.Name = "debugToolStripMenuItem";
            this.debugToolStripMenuItem.Size = new System.Drawing.Size(136, 22);
            this.debugToolStripMenuItem.Text = "Debug";
            this.debugToolStripMenuItem.Click += new System.EventHandler(this.debugToolStripMenuItem_Click);
            // 
            // autorunToolStripMenuItem
            // 
            this.autorunToolStripMenuItem.Name = "autorunToolStripMenuItem";
            this.autorunToolStripMenuItem.Size = new System.Drawing.Size(136, 22);
            this.autorunToolStripMenuItem.Text = "Autorun";
            this.autorunToolStripMenuItem.Click += new System.EventHandler(this.autorunToolStripMenuItem_Click);
            // 
            // loadLastToolStripMenuItem
            // 
            this.loadLastToolStripMenuItem.Name = "loadLastToolStripMenuItem";
            this.loadLastToolStripMenuItem.Size = new System.Drawing.Size(136, 22);
            this.loadLastToolStripMenuItem.Text = "Load Last";
            this.loadLastToolStripMenuItem.Click += new System.EventHandler(this.loadLastToolStripMenuItem_Click);
            // 
            // mazeTextToolStripMenuItem
            // 
            this.mazeTextToolStripMenuItem.Name = "mazeTextToolStripMenuItem";
            this.mazeTextToolStripMenuItem.Size = new System.Drawing.Size(136, 22);
            this.mazeTextToolStripMenuItem.Text = "Maze Text";
            this.mazeTextToolStripMenuItem.Click += new System.EventHandler(this.mazeTextToolStripMenuItem_Click);
            // 
            // btnRun
            // 
            this.btnRun.Enabled = false;
            this.btnRun.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnRun.Location = new System.Drawing.Point(299, 9);
            this.btnRun.Margin = new System.Windows.Forms.Padding(0);
            this.btnRun.Name = "btnRun";
            this.btnRun.Size = new System.Drawing.Size(66, 23);
            this.btnRun.TabIndex = 7;
            this.btnRun.Text = "RUN";
            this.btnRun.UseVisualStyleBackColor = true;
            this.btnRun.Click += new System.EventHandler(this.btnRun_Click);
            // 
            // btnStop
            // 
            this.btnStop.Enabled = false;
            this.btnStop.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnStop.Location = new System.Drawing.Point(586, 9);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(66, 23);
            this.btnStop.TabIndex = 8;
            this.btnStop.Text = "STOP";
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // btnPause
            // 
            this.btnPause.Enabled = false;
            this.btnPause.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnPause.Location = new System.Drawing.Point(377, 9);
            this.btnPause.Name = "btnPause";
            this.btnPause.Size = new System.Drawing.Size(66, 23);
            this.btnPause.TabIndex = 9;
            this.btnPause.Text = "PAUSE";
            this.btnPause.UseVisualStyleBackColor = true;
            this.btnPause.Click += new System.EventHandler(this.btnPause_Click);
            // 
            // btnStep
            // 
            this.btnStep.Enabled = false;
            this.btnStep.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnStep.Location = new System.Drawing.Point(461, 9);
            this.btnStep.Name = "btnStep";
            this.btnStep.Size = new System.Drawing.Size(66, 23);
            this.btnStep.TabIndex = 10;
            this.btnStep.Text = "STEP";
            this.btnStep.UseVisualStyleBackColor = true;
            this.btnStep.Click += new System.EventHandler(this.btnStep_Click);
            // 
            // lvwMazes
            // 
            this.lvwMazes.AutoArrange = false;
            this.lvwMazes.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.c1,
            this.c2});
            this.lvwMazes.FullRowSelect = true;
            this.lvwMazes.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.lvwMazes.HideSelection = false;
            this.lvwMazes.Location = new System.Drawing.Point(797, 38);
            this.lvwMazes.MultiSelect = false;
            this.lvwMazes.Name = "lvwMazes";
            this.lvwMazes.Size = new System.Drawing.Size(95, 500);
            this.lvwMazes.TabIndex = 11;
            this.lvwMazes.UseCompatibleStateImageBehavior = false;
            this.lvwMazes.View = System.Windows.Forms.View.Details;
            this.lvwMazes.SelectedIndexChanged += new System.EventHandler(this.lvwMazes_SelectedIndexChanged);
            // 
            // c1
            // 
            this.c1.Text = "#";
            // 
            // c2
            // 
            this.c2.Text = "Path";
            // 
            // btnReset
            // 
            this.btnReset.Enabled = false;
            this.btnReset.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnReset.Location = new System.Drawing.Point(678, 9);
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(66, 23);
            this.btnReset.TabIndex = 12;
            this.btnReset.Text = "RESET";
            this.btnReset.UseVisualStyleBackColor = true;
            this.btnReset.Click += new System.EventHandler(this.btnReset_Click);
            // 
            // pbxPath
            // 
            this.pbxPath.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.pbxPath.BackColor = System.Drawing.Color.White;
            this.pbxPath.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pbxPath.Location = new System.Drawing.Point(797, 541);
            this.pbxPath.Margin = new System.Windows.Forms.Padding(0);
            this.pbxPath.Name = "pbxPath";
            this.pbxPath.Size = new System.Drawing.Size(51, 25);
            this.pbxPath.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbxPath.TabIndex = 13;
            this.pbxPath.TabStop = false;
            // 
            // pbxMaze
            // 
            this.pbxMaze.BackColor = System.Drawing.Color.Blue;
            this.pbxMaze.Location = new System.Drawing.Point(9, 44);
            this.pbxMaze.Margin = new System.Windows.Forms.Padding(0);
            this.pbxMaze.Name = "pbxMaze";
            this.pbxMaze.Size = new System.Drawing.Size(100, 100);
            this.pbxMaze.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.pbxMaze.TabIndex = 4;
            this.pbxMaze.TabStop = false;
            // 
            // pbxSegment
            // 
            this.pbxSegment.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.pbxSegment.BackColor = System.Drawing.Color.White;
            this.pbxSegment.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pbxSegment.Location = new System.Drawing.Point(797, 576);
            this.pbxSegment.Margin = new System.Windows.Forms.Padding(0);
            this.pbxSegment.Name = "pbxSegment";
            this.pbxSegment.Size = new System.Drawing.Size(51, 25);
            this.pbxSegment.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbxSegment.TabIndex = 14;
            this.pbxSegment.TabStop = false;
            // 
            // MouseAI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(894, 725);
            this.Controls.Add(this.pbxSegment);
            this.Controls.Add(this.pbxPath);
            this.Controls.Add(this.btnReset);
            this.Controls.Add(this.lvwMazes);
            this.Controls.Add(this.btnStep);
            this.Controls.Add(this.btnPause);
            this.Controls.Add(this.btnStop);
            this.Controls.Add(this.btnRun);
            this.Controls.Add(this.stpStatus);
            this.Controls.Add(this.msMain);
            this.Controls.Add(this.pbxMaze);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.msMain;
            this.Name = "MouseAI";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "MouseAI";
            this.Shown += new System.EventHandler(this.MazeAI_Shown);
            this.stpStatus.ResumeLayout(false);
            this.stpStatus.PerformLayout();
            this.msMain.ResumeLayout(false);
            this.msMain.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbxPath)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbxMaze)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbxSegment)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.MenuStrip msMain;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadToolStripMenuItem;
        private System.Windows.Forms.Button btnRun;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
        private System.Windows.Forms.Button btnPause;
        private System.Windows.Forms.ToolStripMenuItem autorunToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadLastToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem newToolStripMenuItem;
        private System.Windows.Forms.Button btnStep;
        private System.Windows.Forms.ToolStripMenuItem buildToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem trainToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem testToolStripMenuItem;
        public System.Windows.Forms.StatusStrip stpStatus;
        public System.Windows.Forms.ToolStripStatusLabel tsStatus;
        private System.Windows.Forms.ListView lvwMazes;
        private System.Windows.Forms.ColumnHeader c1;
        private System.Windows.Forms.ColumnHeader c2;
        private System.Windows.Forms.Button btnReset;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem debugToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mazeTextToolStripMenuItem;
        private System.Windows.Forms.PictureBox pbxPath;
        private System.Windows.Forms.PictureBox pbxMaze;
        private System.Windows.Forms.ToolStripMenuItem testToolStripMenuItem1;
        private System.Windows.Forms.PictureBox pbxSegment;
    }
}

