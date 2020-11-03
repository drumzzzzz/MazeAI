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
        // private Maze maze;
        private List<MazeObject> PathObjects;
        private MazeObject[,] MazeObjects;
        private bool isInit;

        public const char BLOCK = '█';
        private const char VISITED = '>';
        public const char SPACE = ' ';
        private const char MOUSE = 'ô';
        private const char CHEESE = 'Δ';
        private const char SCANNED = ':';
        private const char DEADEND = 'X';
        private const char JUNCTION = '+';
        private const char PATH = '●';
        private const char PATHOBJ = '#';
        private const char SEGMENT = '$';
        private const int MARGIN = 30;

        public MazeText(int maze_width, int maze_height)
        {
            InitializeComponent();

            this.maze_width = maze_width;
            this.maze_height = maze_height;
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
            txtMaze.Width = size.Width;
            txtMaze.Height = size.Height;
            txtMaze.Location = new Point(0,0);

            Width = txtMaze.Width + MARGIN;
            Height = txtMaze.Height + (MARGIN * 2);
        }

        public void Display(Maze maze)
        {
            sb.Clear();
            MazeObjects = maze.GetMazeObjects();

            for (int y = 0; y < maze_height; y++)
            {
                for (int x = 0; x < maze_width; x++)
                {
                    sb.Append(GetObjectChar(MazeObjects[x, y]));
                }
                if (y < maze_height - 1)
                    sb.Append(Environment.NewLine);
            }

            txtMaze.Text = sb.ToString();
        }

        public void DisplayPaths(Maze maze)
        {
            sb.Clear();
            PathObjects = maze.GetPathObjects();
            MazeObjects = maze.GetMazeObjects();

            for (int y = 0; y < maze_height; y++)
            {
                for (int x = 0; x < maze_width; x++)
                {
                    if (PathObjects.Any(o => o.x == x && o.y == y))
                        sb.Append(PATHOBJ);
                    else
                        sb.Append(GetObjectChar(MazeObjects[x, y]));
                }
                if (y < maze_height - 1)
                    sb.Append(Environment.NewLine);
            }

            txtMaze.Text = sb.ToString();
        }

        private static char GetObjectChar(MazeObject mo)
        {
            if (mo.object_type == OBJECT_TYPE.BLOCK)
                return BLOCK;

            // ToDo: Scan Debug
            if (mo.object_state == OBJECT_STATE.MOUSE)
                return MOUSE;

            if (mo.isSegment)
                return SEGMENT;

            if (mo.isDeadEnd)
                return DEADEND;

            if (mo.isPath)
                return PATH;

            if (mo.isJunction)
                return JUNCTION;

            if (mo.object_state == OBJECT_STATE.VISITED)
                return VISITED;

            if (mo.isScanned)
                return SCANNED;

            switch (mo.object_state)
            {
                case OBJECT_STATE.NONE: return SPACE;
                case OBJECT_STATE.CHEESE: return CHEESE;
            }

            return SPACE;
        }

        public bool CheckInit()
        {
            return isInit;
        }
    }
}
