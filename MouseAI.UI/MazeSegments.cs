
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace MouseAI.UI
{
    public partial class MazeSegments : Form
    {
        private readonly Maze maze;
        private const int COLUMNS = 3;
        private const int ROWS = 7;
        private const int MARGIN = 5;
        private const int SCALE = 3;

        public MazeSegments(int width, int height, Maze maze)
        {
            InitializeComponent();

            this.maze = maze;
            width *= SCALE;
            height *= SCALE;
            Width = COLUMNS * (width + (MARGIN * SCALE));
            Height = (ROWS + 1) * (height + (MARGIN * SCALE));

            for (int x = 0; x < COLUMNS + 1; x++)
            {
                for (int y = 0; y < ROWS + 1; y++)
                {
                    PictureBox pbx = new PictureBox
                    {
                        Width = width,
                        Height = height,
                        Location = new Point(x * width + (MARGIN * SCALE), y * height + (MARGIN * SCALE)),
                        Visible = false,
                        SizeMode = PictureBoxSizeMode.StretchImage,
                        BorderStyle = BorderStyle.FixedSingle
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
                return;

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
                        ((PictureBox) c).SizeMode = PictureBoxSizeMode.StretchImage;
                    }
                }
            }
        }
    }
}
