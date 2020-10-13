using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Windows.Forms;

namespace MazeAI
{
    public class Maze
    {
        private string maze;
        public readonly int maze_width;
        public readonly int maze_height;

        public enum DIRECTIONS
        {
            NORTH,
            EAST,
            SOUTH,
            WEST
        }

        private readonly DIRECTIONS[] dirs;
        private const char BLOCK = '█';
        private const char VISITED = '×';
        private const char SPACE = '░';
        private const char MOUSE = 'ô';
        private const char CHEESE = 'Δ';
        private const char SCANNED = ':';

        private static Random r;
        private readonly List<AI.Path> aipaths;
        private readonly StringBuilder sb;

        public enum ELEMENT_TYPE
        {
            SPACE,
            BLOCK
        }

        public enum ELEMENT_STATE
        {
            NONE,
            VISITED,
            MOUSE,
            CHEESE,
            SCANNED
        }

        public class MazeElement
        {
            public ELEMENT_TYPE element_type { get; set; }
            public bool isVisited { get; set; }
            public ELEMENT_STATE element_state { get; set; }

            public int x { get; set; }
            public int y { get; set; }
            public bool isScanned { get; set; }

            public MazeElement(ELEMENT_TYPE element_type, int x, int y)
            {
                this.element_type = element_type;
                element_state = ELEMENT_STATE.NONE;
                isVisited = false;
                isScanned = false;
                this.x = x;
                this.y = y;
            }
        }

        private readonly MazeElement[,] MazeElements;

        public Maze(int maze_width, int maze_height, List<AI.Path> aipaths)
        {
            this.maze_width = maze_width;
            this.maze_height = maze_height;

            maze = new string(new char[maze_width * maze_height]);
            MazeElements = new MazeElement[maze_width,maze_height];

            dirs = new DIRECTIONS[4];
            dirs[0] = DIRECTIONS.NORTH; // NORTH;
            dirs[1] = DIRECTIONS.EAST; // EAST;
            dirs[2] = DIRECTIONS.SOUTH; // SOUTH;
            dirs[3] = DIRECTIONS.WEST; // WEST;

            this.aipaths = aipaths;

            r = new Random();
            sb = new StringBuilder();
        }

        public void AddMouse(int x = 1, int y = 1)
        {
            MazeElements[x, y].element_state = ELEMENT_STATE.MOUSE;
        }

        public void AddCheese(int x_min, int x_max, int y_min, int y_max)
        {
            int x, y;

            while (true)
            {
                x = r.Next(x_min, x_max);
                y = r.Next(y_min, y_max);

                if (MazeElements[x, y].element_type != ELEMENT_TYPE.BLOCK &&
                    MazeElements[x, y].element_state != ELEMENT_STATE.MOUSE)
                {
                    MazeElements[x, y].element_state = ELEMENT_STATE.CHEESE;
                    return;
                }
            }
        }

        public void Reset()
        {
            // Fills the maze with blocks

            for (int i = 0; i < maze_width * maze_height; i++)
            {
                maze = ChangeCharacter(maze, i, '█');
            }
        }


        // Convert linear chars to x/y
        private int XYToIndex(int x, int y)
        {
            return y * maze_width + x;
        }

        private bool IsInBounds(int x, int y)
        {
            // Returns "true" if x and y are both in-bounds.   
            if (x < 0 || x >= maze_width)
                return false;

            return (y >= 0 && y < maze_height);
        }

        public void Generate(int x = 1, int y = 1)
        {
            // Starting at the given index, recursively visits every direction in a    
            // randomized order. 
            if (x == 1 && y == 1)
                Reset();

            // Set my current location to be an empty passage.   
            maze = ChangeCharacter(maze, XYToIndex(x, y), SPACE);
            aipaths.Add(new AI.Path(x, y, DIRECTIONS.WEST));

            int rand;
            DIRECTIONS dir_temp;

            for (int i = 0; i < 4; i++)
            {
                rand = r.Next(0, 4);
                dir_temp = dirs[rand];
                dirs[rand] = dirs[i];
                dirs[i] = dir_temp;
            }

            // Iterate directions and attempt to move
            for (int i = 0; i < 4; ++i)
            {
                // Offset from current location
                int dx = 0;
                int dy = 0;

                switch (dirs[i])
                {
                    case DIRECTIONS.NORTH:
                        dy = -1;
                        break;
                    case DIRECTIONS.SOUTH:
                        dy = 1;
                        break;
                    case DIRECTIONS.EAST:
                        dx = 1;
                        break;
                    case DIRECTIONS.WEST:
                        dx = -1;
                        break;
                }

                // Find coordinates 2 spaces from direction
                int x2 = x + (dx << 1);
                int y2 = y + (dy << 1);

                if (IsInBounds(x2, y2))
                {
                    if (maze[XYToIndex(x2, y2)] == BLOCK)
                    {
                        maze = ChangeCharacter(maze, XYToIndex(x2 - dx, y2 - dy), SPACE);
                        aipaths.Add(new AI.Path(x2 - dx, y2 - dy, dirs[i]));
                        Generate(x2, y2);
                    }
                }
            }
        }

        public void Update()
        {
            for (int y = 0; y < maze_height; ++y)
            {
                for (int x = 0; x < maze_width; ++x)
                {
                    // MazeElements[x, y] = new MazeElement(GetElementType(x, y));
                    MazeElements[x, y] = new MazeElement(GetElementType(x, y), x, y);
                }
            }
        }

        public MazeElement ScanElements(int x, int y)
        {
            ELEMENT_STATE es;
            int scan_count = 0;
            int scanned_count = 0;

            // Scan West
            for (int x_idx = x - 1; x_idx > 0; x_idx--)
            {
                if (!isScanValid(x_idx, y))
                    break;

                es = CheckScannedElement(x_idx, y);

                if (es == ELEMENT_STATE.CHEESE)
                    return MazeElements[x_idx, y];
                if (es == ELEMENT_STATE.SCANNED)
                {
                    scanned_count++;
                    break;
                }

                scan_count++;
            }

            // Scan East
            for (int x_idx = x + 1; x_idx < maze_width; x_idx++)
            {
                if (!isScanValid(x_idx, y))
                    break;

                es = CheckScannedElement(x_idx, y);

                if (es == ELEMENT_STATE.CHEESE)
                    return MazeElements[x_idx, y];
                if (es == ELEMENT_STATE.SCANNED)
                {
                    scanned_count++;
                    break;
                }

                scan_count++;
            }

            // Scan North
            for (int y_idx = y - 1; y_idx > 0; y_idx--)
            {
                if (!isScanValid(x, y_idx))
                    break;

                es = CheckScannedElement(x, y_idx);

                if (es == ELEMENT_STATE.CHEESE)
                    return MazeElements[x, y_idx];
                if (es == ELEMENT_STATE.SCANNED)
                {
                    scanned_count++;
                    break;
                }

                scan_count++;
            }

            // Scan South
            for (int y_idx = y + 1; y_idx < maze_height; y_idx++)
            {
                if (!isScanValid(x, y_idx))
                    break;

                es = CheckScannedElement(x, y_idx);

                if (es == ELEMENT_STATE.CHEESE)
                    return MazeElements[x, y_idx];
                if (es == ELEMENT_STATE.SCANNED)
                {
                    scanned_count++;
                    break;
                }

                scan_count++;
            }

            Console.WriteLine("Scan {0} Scanned {1}", scan_count, scanned_count);

            return null;
        }

        private ELEMENT_STATE CheckScannedElement(int x, int y)
        {
            if (MazeElements[x, y].element_state == ELEMENT_STATE.CHEESE)
                return ELEMENT_STATE.CHEESE;

            if (MazeElements[x, y].isScanned)
            {
                return ELEMENT_STATE.SCANNED;
            }

            MazeElements[x, y].isScanned = true;
            return ELEMENT_STATE.NONE;
        }

        private bool isScanValid(int x, int y)
        {
            return (IsInBounds(x, y) && MazeElements[x, y].element_type == ELEMENT_TYPE.SPACE);
        }

        public List<MazeElement> CheckNode(int x, int y)
        {
            List<MazeElement> elements = new List<MazeElement>
            {
                MazeElements[x, y]
            };

            for (int x_idx = x - 1; x_idx < x + 2; x_idx+=2)
            {
                if (IsInBounds(x_idx, y) && GetElementType(x_idx, y) == ELEMENT_TYPE.SPACE)
                {
                    elements.Add(MazeElements[x_idx, y]);
                }
            }

            for (int y_idx = y - 1; y_idx < y + 2; y_idx+=2)
            {
                if (IsInBounds(x, y_idx) && GetElementType(x, y_idx) == ELEMENT_TYPE.SPACE)
                {
                    elements.Add(MazeElements[x, y_idx]);
                }
            }

            
            return elements;
        }

        private ELEMENT_TYPE GetElementType(int x, int y)
        {
            return (maze[XYToIndex(x, y)] == SPACE) ? ELEMENT_TYPE.SPACE : ELEMENT_TYPE.BLOCK;
        }

        public bool SetPath(int x, int y)
        {
            MazeElement me = MazeElements[x, y];

            if (me.element_type == ELEMENT_TYPE.BLOCK)
            {
                throw new Exception("Invalid Block Element at " + x + "," + y);
            }

            if (me.element_state == ELEMENT_STATE.MOUSE)
            {
                //Console.WriteLine("Skipped Mouse Element at %d,%d", x, y);
                return false;
            }
            if (me.element_state == ELEMENT_STATE.CHEESE)
            {
                //Console.WriteLine("Cheese found at Element at %d,%d!", x, y);
                return true;
            }

            me.element_state = ELEMENT_STATE.VISITED;
            return false;
        }

        public void Display()
        {
            Console.Clear();

            for (int y = 0; y < maze_height; ++y)
            {
                sb.Clear();
                for (int x = 0; x < maze_width; ++x)
                {
                    sb.Append(GetElementChar(MazeElements[x, y]));
                }
                Console.WriteLine(sb.ToString());
            }
        }

        public static string ChangeCharacter(string sourceString, int charIndex, char newChar)
        {
            return (charIndex > 0 ? sourceString.Substring(0, charIndex) : "")
                   + newChar +
                   (charIndex < sourceString.Length - 1 ? sourceString.Substring(charIndex + 1) : "");
        }

        private static char GetElementChar(MazeElement me)
        {
            if (me.element_type == ELEMENT_TYPE.BLOCK)
                return BLOCK;

            // ToDd: Scan Debug
            if (me.element_state == ELEMENT_STATE.MOUSE)
                return MOUSE;

            if (me.isScanned)
                return SCANNED;

            switch (me.element_state)
            {
                case ELEMENT_STATE.NONE: return SPACE;
                case ELEMENT_STATE.VISITED: return VISITED;
                case ELEMENT_STATE.CHEESE: return CHEESE;
                case ELEMENT_STATE.MOUSE: return MOUSE;
            }
            
            Console.WriteLine("Invalid Character - type:" + me.element_type + " state:" + me.element_state);
            return SPACE;
        }
    }
}
