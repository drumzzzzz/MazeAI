
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace MouseAI.UI
{
    public partial class MazeSegments : Form
    {
        private readonly Maze maze;
        private const int COLUMNS = 10;
        private const int ROWS = 10;
        private const int MARGIN = 5;
        private const int SCALE = 2;

        public MazeSegments(int width, int height, Maze maze, Color backcolor)
        {
            InitializeComponent();

            this.maze = maze;
            width *= SCALE;
            height *= SCALE;
            Width = COLUMNS * (width + (MARGIN * SCALE));
            Height = (ROWS + 1) * (height + (MARGIN * SCALE));
            lvwSegments.BackColor = backcolor;
        }

        public void ShowImages()
        {
            if (!Visible)
                return;

            List<byte[]> segments = maze.GetSegments();
            if (segments == null || segments.Count == 0 || segments.Count > ROWS * COLUMNS)
            {
                Console.WriteLine("Error displaying maze segments");
                return;
            }

            MemoryStream ms;
            lvwSegments.Items.Clear();
            ImageList imglist = new ImageList
            {
                ImageSize = new Size(100, 100)
            };

            for (int i = 0; i < segments.Count; i++)
            {
                ms = new MemoryStream(segments[i]);
                imglist.Images.Add((Bitmap)Image.FromStream(ms));
                lvwSegments.Items.Add(new ListViewItem
                {
                    Text = i.ToString(),
                    ImageIndex = i
                });
            }
            lvwSegments.LargeImageList = imglist;
        }
    }
}
