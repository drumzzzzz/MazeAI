using System;
using System.Collections.Generic;
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
        private readonly MazeObject[,] MazeObjects;

        public Maze(int maze_width, int maze_height, List<AI.Path> aipaths)
        {
            this.maze_width = maze_width;
            this.maze_height = maze_height;

            maze = new string(new char[maze_width * maze_height]);
            MazeObjects = new MazeObject[maze_width,maze_height];

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
            MazeObjects[x, y].object_state = OBJECT_STATE.MOUSE;
        }

        public void AddCheese(int x_min, int x_max, int y_min, int y_max)
        {
            int x, y;

            while (true)
            {
                x = r.Next(x_min, x_max);
                y = r.Next(y_min, y_max);

                if (MazeObjects[x, y].object_type != OBJECT_TYPE.BLOCK &&
                    MazeObjects[x, y].object_state != OBJECT_STATE.MOUSE)
                {
                    MazeObjects[x, y].object_state = OBJECT_STATE.CHEESE;
                    return;
                }
            }
        }

        public void Reset()
        {
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
                    MazeObjects[x, y] = new MazeObject(GetObjectType(x, y), x, y);
                }
            }
        }

        #region Scanning

        public MazeObject ScanObjects(int x, int y)
        {
            OBJECT_STATE os;
            int scan_count = 0;
            int scanned_count = 0;

            // Scan West
            for (int x_idx = x - 1; x_idx > 0; x_idx--)
            {
                if (!isScanValid(x_idx, y))
                    break;

                os = CheckScannedObject(x_idx, y);

                if (os == OBJECT_STATE.CHEESE)
                    return MazeObjects[x_idx, y];
                if (os == OBJECT_STATE.SCANNED)
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

                os = CheckScannedObject(x_idx, y);

                if (os == OBJECT_STATE.CHEESE)
                    return MazeObjects[x_idx, y];
                if (os == OBJECT_STATE.SCANNED)
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

                os = CheckScannedObject(x, y_idx);

                if (os == OBJECT_STATE.CHEESE)
                    return MazeObjects[x, y_idx];
                if (os == OBJECT_STATE.SCANNED)
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

                os = CheckScannedObject(x, y_idx);

                if (os == OBJECT_STATE.CHEESE)
                    return MazeObjects[x, y_idx];
                if (os == OBJECT_STATE.SCANNED)
                {
                    scanned_count++;
                    break;
                }

                scan_count++;
            }

            Console.WriteLine("Scan {0} Scanned {1}", scan_count, scanned_count);

            return null;
        }

        private OBJECT_STATE CheckScannedObject(int x, int y)
        {
            if (MazeObjects[x, y].object_state == OBJECT_STATE.CHEESE)
                return OBJECT_STATE.CHEESE;

            if (MazeObjects[x, y].isScanned)
            {
                return OBJECT_STATE.SCANNED;
            }

            MazeObjects[x, y].isScanned = true;
            return OBJECT_STATE.NONE;
        }

        private bool isScanValid(int x, int y)
        {
            return (IsInBounds(x, y) && MazeObjects[x, y].object_type == OBJECT_TYPE.SPACE);
        }

        #endregion

        public List<MazeObject> CheckNode(int x, int y)
        {
            List<MazeObject> mazeobjects = new List<MazeObject>
            {
                MazeObjects[x, y]
            };

            for (int x_idx = x - 1; x_idx < x + 2; x_idx+=2)
            {
                if (IsInBounds(x_idx, y) && GetObjectType(x_idx, y) == OBJECT_TYPE.SPACE)
                {
                    mazeobjects.Add(MazeObjects[x_idx, y]);
                }
            }

            for (int y_idx = y - 1; y_idx < y + 2; y_idx+=2)
            {
                if (IsInBounds(x, y_idx) && GetObjectType(x, y_idx) == OBJECT_TYPE.SPACE)
                {
                    mazeobjects.Add(MazeObjects[x, y_idx]);
                }
            }

            return mazeobjects;
        }

        private OBJECT_TYPE GetObjectType(int x, int y)
        {
            return (maze[XYToIndex(x, y)] == SPACE) ? OBJECT_TYPE.SPACE : OBJECT_TYPE.BLOCK;
        }

        public bool SetPath(int x, int y)
        {
            MazeObject mo = MazeObjects[x, y];

            if (mo.object_type == OBJECT_TYPE.BLOCK)
            {
                throw new Exception("Invalid Block Object at " + x + "," + y);
            }

            if (mo.object_state == OBJECT_STATE.MOUSE || mo.object_state == OBJECT_STATE.CHEESE)
            {
                return false;
            }

            mo.object_state = OBJECT_STATE.VISITED;
            return false;
        }

        #region Rendering

        public void Display()
        {
            Console.Clear();

            for (int y = 0; y < maze_height; ++y)
            {
                sb.Clear();
                for (int x = 0; x < maze_width; ++x)
                {
                    sb.Append(GetObjectChar(MazeObjects[x, y]));
                }
                Console.WriteLine(sb.ToString());
            }
        }

        private static char GetObjectChar(MazeObject me)
        {
            if (me.object_type == OBJECT_TYPE.BLOCK)
                return BLOCK;

            // ToDd: Scan Debug
            if (me.object_state == OBJECT_STATE.MOUSE)
                return MOUSE;

            if (me.isScanned)
                return SCANNED;

            switch (me.object_state)
            {
                case OBJECT_STATE.NONE: return SPACE;
                case OBJECT_STATE.VISITED: return VISITED;
                case OBJECT_STATE.CHEESE: return CHEESE;
                case OBJECT_STATE.MOUSE: return MOUSE;
            }
            
            Console.WriteLine("Invalid Character - type:" + me.object_type + " state:" + me.object_state);
            return SPACE;
        }

        public static string ChangeCharacter(string sourceString, int charIndex, char newChar)
        {
            return (charIndex > 0 ? sourceString.Substring(0, charIndex) : "")
                   + newChar +
                   (charIndex < sourceString.Length - 1 ? sourceString.Substring(charIndex + 1) : "");
        }

        #endregion
    }
}
