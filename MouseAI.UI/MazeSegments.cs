using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MouseAI.UI
{
    public partial class MazeSegments : Form
    {
        private Maze maze;
        private int width;
        private int height;
        private const int COLUMNS = 4;
        private const int ROWS = 8;
        private const int MARGIN = 10;

        public MazeSegments(int width, int height, Maze maze)
        {
            InitializeComponent();

            this.maze = maze;
            this.width = width * 2;
            this.height = height * 2;

            Width = COLUMNS * (width + MARGIN);
            Height = ROWS * (height + MARGIN);

            for (int x = 0; x < COLUMNS; x++)
            {
                for (int y = 0; y < ROWS; y++)
                {
                    PictureBox pbx = new PictureBox
                    {
                        Width = width,
                        Height = height,
                        Location = new Point(x * (width + MARGIN), y * (height + MARGIN)),
                        Visible = false,
                        SizeMode = PictureBoxSizeMode.StretchImage
                        
                    };
                    Controls.Add(pbx);
                }
            }
        }

        public void ShowImages()
        {
            if (!Visible)
                return;

            List<byte[]> segments = maze.GetSegments();
            if (segments == null || segments.Count == 0 || segments.Count > ROWS * COLUMNS)
            {
                Console.WriteLine("Invalid Object Segments!");
                return;
            }

            MemoryStream ms;
            int index = 0;
            foreach (Control c in Controls)
            {
                if (c is PictureBox)
                {
                    if (index >= segments.Count)
                    {
                        ((PictureBox) c).Visible = false;
                    }
                    else
                    {
                        ms = new MemoryStream(segments[index++]);
                        ((PictureBox) c).Image = (Bitmap) Image.FromStream(ms);
                        ((PictureBox) c).Visible = true;
                    }
                }
            }
        }
    }
}
