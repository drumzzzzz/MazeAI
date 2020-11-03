using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MouseAI.UI
{
    public partial class MazeText : Form
    {
        private readonly StringBuilder sb;
        private readonly int maze_height;
        private readonly int maze_width;
        private readonly Maze maze;

        private List<MazeObject> pathObjects;
        private MazeObject[,] mazeObjects;
        private MazeSegments mazeSegments;

        public const char BLOCK = '█';
        public const char SPACE = ' ';
        private const char MOUSE = 'ô';
        private const char CHEESE = 'Δ';
        private const char DEADEND = 'X';
        private const char JUNCTION = '+';
        private const char NNPATH = '>';
        private const char PATHOBJ = '#';
        private const char NULL = '?';
        private const char ERROR = '!';
        private const int MARGIN = 30;
        private readonly char[] SEGMENTS = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();

        public MazeText(int maze_width, int maze_height, Maze maze)
        {
            InitializeComponent();

            this.maze_width = maze_width;
            this.maze_height = maze_height;
            this.maze = maze;

            sb = new StringBuilder();

            for (int y = 0; y < maze_height; y++)
            {
                for (int x = 0; x < maze_width; x++)
                {
                    sb.Append(SPACE);
                }

                if (y < maze_height - 1)
                    sb.Append(Environment.NewLine);
            }
            txtMaze.Text = sb.ToString();

            Size size = TextRenderer.MeasureText(txtMaze.Text, txtMaze.Font);
            pnlSelections.Width = size.Width;
            txtMaze.Width = size.Width;
            txtMaze.Height = size.Height;
            txtMaze.Location = new Point(MARGIN / 2, pnlSelections.Height);

            Width = txtMaze.Width + (MARGIN * 2);
            Height = txtMaze.Height + pnlSelections.Height + (MARGIN * 2);
        }

        public void DisplayMaze()
        {
            sb.Clear();
            pathObjects = maze.GetPathObjects();
            mazeObjects = maze.GetMazeObjects();
            mazeSegments = maze.GetMazeSegments();

            char c;
            for (int y = 0; y < maze_height; y++)
            {
                for (int x = 0; x < maze_width; x++)
                {
                    if (rbSegments.Checked)
                    {
                        c = GetSegmentChar(x, y);
                        if (c != NULL)
                        {
                            sb.Append(c);
                            continue;
                        }
                    }
                    else if (rbPaths.Checked)
                    {
                        c = GetPathChar(x, y);
                        if (c != NULL)
                        {
                            sb.Append(c);
                            continue;
                        }
                    }
     
                    sb.Append(GetObjectChar(mazeObjects[x, y]));
                }
                if (y < maze_height - 1)
                    sb.Append(Environment.NewLine);
            }
            txtMaze.Text = sb.ToString();
        }

        private char GetPathChar(int x, int y)
        {
            if (pathObjects == null || pathObjects.Count == 0)
                return NULL;

            MazeObject mo = pathObjects.FirstOrDefault(o => o.x == x && o.y == y);

            if (mo != null)
            {
                return !mo.isDeadEnd ? NNPATH : PATHOBJ;
            }

            return NULL;
        }

        private char GetSegmentChar(int x, int y)
        {
            if (mazeSegments == null || mazeSegments.Count == 0)
                return NULL;

            int index = 0;
            foreach (MazeObjects mos in mazeSegments)
            {
                if (mos.Any(mo => mo.x == x && mo.y == y))
                {
                    return index < SEGMENTS.Length ? SEGMENTS[index] : ERROR;
                }
                index++;
            }

            return NULL;
        }

        private static char GetObjectChar(MazeObject mo)
        {
            if (mo.object_type == OBJECT_TYPE.BLOCK)
                return BLOCK;

            // ToDo: Scan Debug
            if (mo.object_state == OBJECT_STATE.MOUSE)
                return MOUSE;

            if (mo.isDeadEnd)
                return DEADEND;

            //if (mo.isPath)
            //    return PATH;

            if (mo.isJunction)
                return JUNCTION;

            //if (mo.object_state == OBJECT_STATE.VISITED)
            //    return VISITED;

            //if (mo.isScanned)
            //    return SCANNED;

            switch (mo.object_state)
            {
                case OBJECT_STATE.NONE: return SPACE;
                case OBJECT_STATE.CHEESE: return CHEESE;
            }

            return SPACE;
        }

        #region Controls

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            DisplayMaze();
        }

        private void rbSegments_CheckedChanged(object sender, EventArgs e)
        {
            DisplayMaze();
        }

        private void rbPaths_CheckedChanged(object sender, EventArgs e)
        {
            DisplayMaze();
        }

        private void rbAll_CheckedChanged(object sender, EventArgs e)
        {
            DisplayMaze();
        }

        #endregion
    }
}
