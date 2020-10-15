﻿#region Using Statements

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

#endregion

namespace MouseAI
{
    public class Maze
    {
        #region Declarations

        private string maze;
        private readonly int maze_width;
        private readonly int maze_height;

        private readonly DIRECTION[] dirs;
        private const char BLOCK = '█';
        private const char VISITED = '●';
        private const char SPACE = '░';
        private const char MOUSE = 'ô';
        private const char CHEESE = 'Δ';
        private const char SCANNED = ':';
        private const char DEADEND = 'X';
        private const char JUNCTION = '+';

        private static Random r;
        private readonly StringBuilder sb;
        private readonly MazeObject[,] MazeObjects;
        private MazeObject oMouse;

        #endregion

        #region Initialization

        public Maze(int maze_width, int maze_height, List<MazePath.Path> aipaths)
        {
            this.maze_width = maze_width;
            this.maze_height = maze_height;

            maze = new string(new char[maze_width * maze_height]);
            MazeObjects = new MazeObject[maze_width, maze_height];

            dirs = new DIRECTION[4];
            dirs[0] = DIRECTION.NORTH; // NORTH;
            dirs[1] = DIRECTION.EAST; // EAST;
            dirs[2] = DIRECTION.SOUTH; // SOUTH;
            dirs[3] = DIRECTION.WEST; // WEST;

            r = new Random();
            sb = new StringBuilder();
        }

        public void AddMouse(int x = 1, int y = 1)
        {
            MazeObjects[x, y].object_state = OBJECT_STATE.MOUSE;

            oMouse = new MazeObject(OBJECT_TYPE.BLOCK, x, y)
            {
                object_state = OBJECT_STATE.MOUSE,
                object_type = OBJECT_TYPE.SPACE,
                isVisited = true
            };
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
                    MazeObjects[x, y].object_type = OBJECT_TYPE.SPACE;
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

        // Starting at the given index, recursively visit every direction randomly
        public void Generate(int x = 1, int y = 1)
        {
            if (x == 1 && y == 1)
                Reset();

            // Set current location to empty
            maze = ChangeCharacter(maze, XYToIndex(x, y), SPACE);

            int rand;
            DIRECTION dir_temp;

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
                    case DIRECTION.NORTH:
                        dy = -1;
                        break;
                    case DIRECTION.SOUTH:
                        dy = 1;
                        break;
                    case DIRECTION.EAST:
                        dx = 1;
                        break;
                    case DIRECTION.WEST:
                        dx = -1;
                        break;
                }

                // Find the coords 2 spaces away
                int x2 = x + (dx << 1);
                int y2 = y + (dy << 1);

                if (IsInBounds(x2, y2))
                {
                    if (maze[XYToIndex(x2, y2)] == BLOCK)
                    {
                        maze = ChangeCharacter(maze, XYToIndex(x2 - dx, y2 - dy), SPACE);
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

        #endregion

        #region Scanning

        public bool ScanObjects(int x, int y)
        {
            OBJECT_STATE os;
            List<MazeObject> mo = new List<MazeObject>();

            // Scan West
            for (int x_idx = x - 1; x_idx > 0; x_idx--)
            {
                if (!isScanValid(x_idx, y))
                    break;

                os = CheckScannedObject(x_idx, y);

                if (os == OBJECT_STATE.CHEESE)
                    return true;

                if (os == OBJECT_STATE.SCANNED)
                    break;

                mo.Add(MazeObjects[x_idx, y]);
            }

            CheckEndPoints(mo);

            // Scan East
            for (int x_idx = x + 1; x_idx < maze_width; x_idx++)
            {
                if (!isScanValid(x_idx, y))
                    break;

                os = CheckScannedObject(x_idx, y);

                if (os == OBJECT_STATE.CHEESE)
                    return true;
                if (os == OBJECT_STATE.SCANNED)
                    break;

                mo.Add(MazeObjects[x_idx, y]);
            }

            CheckEndPoints(mo);

            // Scan North
            for (int y_idx = y - 1; y_idx > 0; y_idx--)
            {
                if (!isScanValid(x, y_idx))
                    break;

                os = CheckScannedObject(x, y_idx);

                if (os == OBJECT_STATE.CHEESE)
                    return true;
                if (os == OBJECT_STATE.SCANNED)
                    break;

                mo.Add(MazeObjects[x, y_idx]);
            }

            CheckEndPoints(mo);

            // Scan South
            for (int y_idx = y + 1; y_idx < maze_height; y_idx++)
            {
                if (!isScanValid(x, y_idx))
                    break;

                os = CheckScannedObject(x, y_idx);

                if (os == OBJECT_STATE.CHEESE)
                    return true;
                if (os == OBJECT_STATE.SCANNED)
                    break;

                mo.Add(MazeObjects[x, y_idx]);
            }

            CheckEndPoints(mo);

            return false;
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

        private void CheckEndPoints(IList<MazeObject> mos)
        {
            return;
            if (mos.Count != 0 && GetPerimiter(mos.Last()) == 1)
            {
                mos.Last().isDeadEnd = true;

                if (mos.Count > 1)
                {
                    for (int i = mos.Count - 2; i > -1; i--)
                    {
                        if (GetPerimiter(mos[i]) <= 2)
                            mos[i].isDeadEnd = true;
                        else
                            break;
                    }
                }

                mos.Clear();
            }
        }

        private int GetPerimiter(MazeObject mo)
        {
            int x = mo.x;
            int y = mo.y;
            int count = 0;

            // Scan West
            if (isScanValid(x - 1, y))
                count++;

            // Scan East
            if (isScanValid(x + 1, y))
                count++;

            // Scan North
            if (isScanValid(x, y - 1))
                count++;


            // Scan South
            if (isScanValid(x, y + 1))
                count++;

            return count;
        }

        #endregion

        #region Movement

        public bool ProcessMouseMove()
        {
            int x = oMouse.x;
            int y = oMouse.y;

            // Can mouse see the cheese?
            if (ScanObjects(x, y))
                return true;

            List<MazeObject> mazeobjects = CheckNode(x, y);

            MazeObject mouse = mazeobjects.FirstOrDefault(o => o.object_state == OBJECT_STATE.MOUSE);

            if (mouse == null)
            {
                throw new Exception("Mouse Object Null!");
            }
            MazeObject mo = mazeobjects.FirstOrDefault(o => o.isVisited == false && 
                                                            o.object_state != OBJECT_STATE.MOUSE && o.isDeadEnd == false);
            //MazeObject mo = mazeobjects.FirstOrDefault(o => o.isVisited == false &&
            //                                                o.object_state != OBJECT_STATE.MOUSE);
            if (mo != null)
            {
                if (mazeobjects.Count >= 4)
                {
                    mouse.isJunction = true;
                }

                oMouse.direction = GetMouseDirection(oMouse.x, oMouse.y, mo.x, mo.y);

                oMouse.x = mo.x;
                oMouse.y = mo.y;
                mo.object_state = OBJECT_STATE.MOUSE;
                mo.dtLastVisit = DateTime.UtcNow;
                mouse.isVisited = true;
                mouse.object_state = OBJECT_STATE.VISITED;
            }
            else
            {
                DateTime dtOldest = DateTime.UtcNow;
                MazeObject mo_oldest = null;

                foreach (var m in mazeobjects.Where(m => m.object_state != OBJECT_STATE.MOUSE))
                {
                    if (m.dtLastVisit < dtOldest)
                    {
                        mo_oldest = m;
                        dtOldest = mo_oldest.dtLastVisit;
                    }
                }

                if (mo_oldest == null)
                    throw new Exception("Object was null!");

                oMouse.x = mo_oldest.x;
                oMouse.y = mo_oldest.y;
                oMouse.direction = GetMouseDirection(oMouse.x, oMouse.y, mo_oldest.x, mo_oldest.y);

                if (!mouse.isJunction)
                    mouse.isDeadEnd = true;

                MazeObjects[mo_oldest.x, mo_oldest.y].object_state = OBJECT_STATE.MOUSE;
                mouse.isVisited = true;
                mouse.object_state = OBJECT_STATE.VISITED;
            }

            Display();
            return false;
        }

        private DIRECTION GetMouseDirection(int x_last, int y_last, int x_curr, int y_curr)
        {
            if (x_last != x_curr)
            {
                return (x_last > x_curr) ? DIRECTION.WEST : DIRECTION.EAST;
            }

            if (y_last != y_curr)
            {
                return (y_last > y_curr) ? DIRECTION.NORTH : DIRECTION.SOUTH;
            }

            return oMouse.direction;
        }

        public List<MazeObject> CheckNode(int x, int y)
        {
            List<MazeObject> mazeobjects = new List<MazeObject>
            {
                MazeObjects[x, y]
            };

            for (int x_idx = x - 1; x_idx < x + 2; x_idx += 2)
            {
                if (IsInBounds(x_idx, y) && GetObjectType(x_idx, y) == OBJECT_TYPE.SPACE)
                {
                    mazeobjects.Add(MazeObjects[x_idx, y]);
                }
            }

            for (int y_idx = y - 1; y_idx < y + 2; y_idx += 2)
            {
                if (IsInBounds(x, y_idx) && GetObjectType(x, y_idx) == OBJECT_TYPE.SPACE)
                {
                    mazeobjects.Add(MazeObjects[x, y_idx]);
                }
            }

            return mazeobjects;
        }

        #endregion

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

        #endregion

        #region Object Tools

        private static char GetObjectChar(MazeObject mo)
        {
            if (mo.object_type == OBJECT_TYPE.BLOCK)
                return BLOCK;

            // ToDd: Scan Debug
            if (mo.object_state == OBJECT_STATE.MOUSE)
                return MOUSE;

            if (mo.isDeadEnd)
                return DEADEND;

            if (mo.isJunction)
                return JUNCTION;

            if(mo.object_state == OBJECT_STATE.VISITED)
                return VISITED;

            if (mo.isScanned)
                return SCANNED;

            switch (mo.object_state)
            {
                case OBJECT_STATE.NONE: return SPACE;
                //case OBJECT_STATE.VISITED: return VISITED;
                case OBJECT_STATE.CHEESE: return CHEESE;
                //case OBJECT_STATE.MOUSE: return MOUSE;
            }

            Console.WriteLine("Invalid Character - type:" + mo.object_type + " state:" + mo.object_state);
            return SPACE;
        }

        public Point GetMousePosition()
        {
            return new Point(oMouse.x, oMouse.y);
        }

        public int GetMouseDirection()
        {
            return (int)oMouse.direction;
        }

        private int XYToIndex(int x, int y)
        {
            return y * maze_width + x;
        }

        private bool IsInBounds(int x, int y)
        {
            if (x < 0 || x >= maze_width)
                return false;

            return (y >= 0 && y < maze_height);
        }

        public OBJECT_TYPE GetObjectType(int x, int y)
        {
            return (maze[XYToIndex(x, y)] == SPACE) ? OBJECT_TYPE.SPACE : OBJECT_TYPE.BLOCK;
        }

        public OBJECT_STATE GetObjectState(int x, int y)
        {
            return (MazeObjects[x, y].object_state);
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
