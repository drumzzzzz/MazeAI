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
        private MazeObjectSegments mazeObjectSegments;

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
        private string guid_last;
        private string guid_current;

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
            lbxSegments.Height = txtMaze.Height + (MARGIN / 2);
            lbxSegments.Location = new Point(txtMaze.Width + (MARGIN / 2), pnlSelections.Height);
            Width = txtMaze.Width + lbxSegments.Width + (MARGIN * 2);
            Height = txtMaze.Height + pnlSelections.Height + (MARGIN * 2);
        }

        public void DisplayMaze()
        {
            if (!Visible)
                return;

            sb.Clear();
            guid_current = maze.GetGUID();

            pathObjects = maze.GetPathObjects();
            mazeObjects = maze.GetMazeObjects();
            mazeObjectSegments = maze.GetMazeSegments();
            UpdateSegmentList();
            int segmentIndex = -1;

            if (rbSegments.Checked && lbxSegments.SelectedItem != null)
            {
                segmentIndex = lbxSegments.SelectedIndex;
            }

            char c;
            for (int y = 0; y < maze_height; y++)
            {
                for (int x = 0; x < maze_width; x++)
                {
                    if (rbSegments.Checked)
                    {
                        c = GetSegmentChar(x, y, segmentIndex);
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

                    c = GetObjectChar(mazeObjects[x, y]);

                    if ((rbPaths.Checked || rbSegments.Checked) && c == DEADEND)
                        c = SPACE;

                    sb.Append(c);
                }
                if (y < maze_height - 1)
                    sb.Append(Environment.NewLine);
            }
            txtMaze.Text = sb.ToString();
        }

        private void UpdateSegmentList()
        {
            if (!isGuidValid() || mazeObjectSegments == null || mazeObjectSegments.Count == 0)
            {
                ClearSegmentList();
                return;
            }

            if (guid_last != guid_current || lbxSegments.Items.Count == 0)
            {
                guid_last = guid_current;
                ClearSegmentList();
                for (int i = 0; i < mazeObjectSegments.Count; i++)
                {
                    lbxSegments.Items.Add(GetSegmentChar(i));
                }
            }
        }

        private void ClearSegmentList()
        {
            if (lbxSegments.Items.Count != 0)
                lbxSegments.Items.Clear();
        }

        private bool isGuidValid()
        {
            guid_current = maze.GetGUID();

            if (string.IsNullOrEmpty(guid_last))
                guid_last = maze.GetGUID();

            return !string.IsNullOrEmpty(guid_last) && !string.IsNullOrEmpty(guid_current);
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

        private char GetSegmentChar(int x, int y, int segmentIndex)
        {
            if (mazeObjectSegments == null || mazeObjectSegments.Count == 0)
                return NULL;

            int index = 0;
            foreach (MazeObjects mos in mazeObjectSegments)
            {
                if (segmentIndex == -1)
                {
                    if (mos.Any(mo => mo.x == x && mo.y == y))
                    {
                        return index < SEGMENTS.Length ? SEGMENTS[index] : ERROR;
                    }
                }
                else if (index == segmentIndex)
                {
                    if (mos.Any(mo => mo.x == x && mo.y == y))
                    {
                        MazeObject mo = mos.FirstOrDefault(o => o.x == x && o.y == y);
                        
                        return index < SEGMENTS.Length ? SEGMENTS[index] : ERROR;
                    }

                    return NULL;
                }

                index++;
            }

            return NULL;
        }

        private char GetSegmentChar(int index)
        {
            return index < SEGMENTS.Length ? SEGMENTS[index] : ERROR;
        }

        private char GetObjectChar(MazeObject mo)
        {
            if (mo.object_type == OBJECT_TYPE.BLOCK)
                return BLOCK;

            // ToDo: Scan Debug
            if (mo.object_state == OBJECT_STATE.MOUSE)
                return MOUSE;

            if (mo.isDeadEnd)
                return DEADEND;

            if (mo.isPath && rbAll.Checked)
                return NNPATH;

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
            if (rbSegments.Checked)
                DisplayMaze();
            else
            {
                lbxSegments.SelectedItem = null;
            }

            lbxSegments.Enabled = rbSegments.Checked;
        }

        private void lbxSegments_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lbxSegments.SelectedItem != null)
            {
                DisplayMaze();
            }
        }

        private void rbPaths_CheckedChanged(object sender, EventArgs e)
        {
            if (rbPaths.Checked)
                DisplayMaze();
        }

        private void rbAll_CheckedChanged(object sender, EventArgs e)
        {
            if (rbAll.Checked)
                DisplayMaze();
        }

        #endregion


    }
}
