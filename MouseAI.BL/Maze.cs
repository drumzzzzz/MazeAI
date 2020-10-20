#region Using Statements

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using MouseAI.BL;
using MouseAI.PL;

#endregion

namespace MouseAI
{
    public class Maze
    {
        #region Declarations

        private static MazeDb mazeDb;
        private byte[,] mazedata;
        private string maze;
        private readonly int maze_width;
        private readonly int maze_height;
        private int mouse_x;
        private int mouse_y;
        private int cheese_x;
        private int cheese_y;

        private readonly DIRECTION[] dirs;
        private const char BLOCK = '█';
        private const char VISITED = '>';
        private const char SPACE = '░';
        private const char MOUSE = 'ô';
        private const char CHEESE = 'Δ';
        private const char SCANNED = ':';
        private const char DEADEND = 'X';
        private const char JUNCTION = '+';
        private const char PATH = '●';
        private const string FILE_EXT = "mze";
        private const string FILE_DIR = "mazes";
        private const byte BLACK = 0x00;
        private const byte WHITE = 0xff;

        private Random r;
        private StringBuilder sb;
        private MazeObject[,] MazeObjects;
        private MazeModels mazeModels;
        private List<MazeObject> PathObjects;
        private MazeObject oMouse;
        private string FileName;
        private readonly string AppDir;

        private static DbTable_Stats dbtblStats;

        #endregion

        #region Initialization

        public Maze(int maze_width, int maze_height, string FileName)
        {
            this.maze_width = maze_width;
            this.maze_height = maze_height;

            maze = new string(new char[maze_width * maze_height]);
            mazedata = new byte[maze_width,maze_height];
            MazeObjects = new MazeObject[maze_width, maze_height];
            mazeModels = new MazeModels();

            dirs = new DIRECTION[4];
            dirs[0] = DIRECTION.NORTH; // NORTH;
            dirs[1] = DIRECTION.EAST; // EAST;
            dirs[2] = DIRECTION.SOUTH; // SOUTH;
            dirs[3] = DIRECTION.WEST; // WEST;

            r = new Random();
            sb = new StringBuilder();
            PathObjects = new List<MazeObject>();
            
            this.FileName = FileName;
            AppDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\" + FILE_DIR;

            if (mazeDb == null)
                mazeDb = new MazeDb();
        }

        public void ClearMazeModels()
        {
            mazeModels.Clear();
        }

        public void AddMazeModel()
        {
            mazeModels.Add(new MazeModel(mazeModels.Count + 1, maze_width, maze_height, mouse_x, mouse_y, cheese_x, cheese_y, mazedata));
        }

        public bool isMazeModels()
        {
            return mazeModels.Count > 0;
        }

        public bool AddCharacters()
        {
            DIRECTION dir = (DIRECTION) r.Next(1, 4);

            MazeObject mo_mouse = GetMazeObject(dir);
            if (mo_mouse == null)
                return false;

            MazeObject mo_cheese = GetMazeObject(OppositeDirection(dir));

            if (mo_cheese == null)
                return false;

            int x = mo_mouse.x;
            int y = mo_mouse.y;

            mouse_x = x;
            mouse_y = y;

            MazeObjects[x, y].object_state = OBJECT_STATE.MOUSE;

            oMouse = new MazeObject(OBJECT_TYPE.BLOCK, x, y)
            {
                object_state = OBJECT_STATE.MOUSE,
                object_type = OBJECT_TYPE.SPACE,
                isVisited = true
            };

            x = mo_cheese.x;
            y = mo_cheese.y;

            MazeObjects[x, y].object_state = OBJECT_STATE.CHEESE;
            MazeObjects[x, y].object_type = OBJECT_TYPE.SPACE;
            cheese_x = x;
            cheese_y = y;

            return true;
        }

        private MazeObject GetMazeObject(DIRECTION dir)
        {
            List<MazeObject> mos;
 
            if (dir == DIRECTION.WEST)
                mos = MazeObjects.Cast<MazeObject>().Where(m => m.object_type == OBJECT_TYPE.SPACE && m.x == 1).ToList();
            else if (dir == DIRECTION.NORTH)
                mos = MazeObjects.Cast<MazeObject>().Where(m => m.object_type == OBJECT_TYPE.SPACE && m.y == 1).ToList();
            else if (dir == DIRECTION.EAST)
                mos = MazeObjects.Cast<MazeObject>().Where(m => m.object_type == OBJECT_TYPE.SPACE && m.x == maze_width - 2).ToList();
            else
                mos = MazeObjects.Cast<MazeObject>().Where(m => m.object_type == OBJECT_TYPE.SPACE && m.y == maze_height - 2).ToList();

            return mos.Count == 0 ? null : mos.ElementAt(r.Next(mos.Count - 1));
        }

        private static DIRECTION OppositeDirection(DIRECTION dir)
        {
            if (dir == DIRECTION.EAST)
                return DIRECTION.WEST;
            if (dir == DIRECTION.WEST)
                return DIRECTION.EAST;
            return dir == DIRECTION.NORTH ? DIRECTION.SOUTH : DIRECTION.NORTH;
        }

        public void Reset()
        {
            sb.Clear();
            PathObjects.Clear();

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
                    mazedata[x, y] = GetObjectByte(x, y);
                    MazeObjects[x, y] = new MazeObject(GetObjectDataType(x, y), x, y);
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

                //if (os == OBJECT_STATE.SCANNED)
                //    break;
                if (MazeObjects[x_idx, y].isDeadEnd)
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
                //if (os == OBJECT_STATE.SCANNED)
                //    break;
                if (MazeObjects[x_idx, y].isDeadEnd)
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
                //if (os == OBJECT_STATE.SCANNED)
                //    break;
                if (MazeObjects[x, y_idx].isDeadEnd)
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

                //if (os == OBJECT_STATE.SCANNED)
                //    break;
                if (MazeObjects[x, y_idx].isDeadEnd)
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

            //if (MazeObjects[x, y].isScanned)
            //{
            //    return OBJECT_STATE.SCANNED;
            //}

            //MazeObjects[x, y].isScanned = true;
            return OBJECT_STATE.NONE;
        }

        private bool isScanValid(int x, int y)
        {
            return (IsInBounds(x, y) && MazeObjects[x, y].object_type == OBJECT_TYPE.SPACE);
        }

        private int GetScanValid(int x, int y)
        {
            return (IsInBounds(x, y) && MazeObjects[x, y].object_type == OBJECT_TYPE.SPACE) ? 1 : 0;
        }

        private void CheckEndPoints(IList<MazeObject> mos)
        {
            // If there are objects and the last point is a dead end
            if (mos.Count != 0 && GetPerimiter(mos.Last()) == 1)
            {
                SetEndpoint(mos.Last());
                mos.RemoveAt(mos.Count - 1);

                for (int i = mos.Count - 1; i > -1; i--)
                {
                    if (GetPerimiter(mos[i]) == 2)
                        SetEndpoint(mos[i]);
                    else
                        break;
                }
            }
            mos.Clear();
        }

        private void SetEndpoint(MazeObject mo)
        {
            mo.isDeadEnd = true;
            mo.isVisited = true;
            mo.dtLastVisit = DateTime.UtcNow;
        }

        private int GetPerimiter(MazeObject mo)
        {
            int x = mo.x;
            int y = mo.y;
            int count = 0;

            // Scan West, East, North, South Respectively
            count += GetScanValid(x - 1, y);
            count += GetScanValid(x + 1, y);
            count += GetScanValid(x, y - 1);
            count += GetScanValid(x, y + 1);

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
            {
                CleanPathObjects();
                //ClearPaths();
                Display();
                return true;
            }

            List<MazeObject> mazeobjects = CheckNode(x, y);

            MazeObject mouse = mazeobjects.FirstOrDefault(o => o.object_state == OBJECT_STATE.MOUSE);

            if (mouse == null)
            {
                throw new Exception("Mouse Object Null!");
            }
            MazeObject mo = mazeobjects.FirstOrDefault(o => o.isVisited == false && 
                                                            o.object_state != OBJECT_STATE.MOUSE && o.isDeadEnd == false);
            if (mo != null)
            {
                if (mazeobjects.Count >= 4)
                {
                    mouse.isJunction = true;
                    CleanPathObjects();
                }

                oMouse.direction = GetMouseDirection(oMouse.x, oMouse.y, mo.x, mo.y);

                oMouse.x = mo.x;
                oMouse.y = mo.y;
                mo.object_state = OBJECT_STATE.MOUSE;
                mo.dtLastVisit = DateTime.UtcNow;
                mo.isPath = true;
                mouse.isVisited = true;
                mouse.object_state = OBJECT_STATE.VISITED;
                PathObjects.Add(mo);
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

        private void CleanPathObjects()
        {
            for (int i = PathObjects.Count - 1; i > -1; i--)
            {
                if (PathObjects[i].isDeadEnd)
                {
                    PathObjects[i].isPath = false;
                    PathObjects.RemoveAt(i);
                }
                else if (PathObjects[i].isJunction)
                {
                    List<MazeObject> mo = CheckNode(PathObjects[i].x, PathObjects[i].y);

                    if (mo == null)
                        throw  new Exception("Objects is null!");

                    if (mo.Count(x => x.isDeadEnd) == mo.Count - 1)
                    {
                        PathObjects[i].isDeadEnd = true;
                        PathObjects.RemoveAt(i);
                    }
                }
            }
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

        #region Saving and Loading

        public string GetSaveName()
        {
            return FileIO.SaveFileAs_Dialog(AppDir, FILE_EXT);
        }

        public string SaveMazeModels(string filename)
        {
            try
            {
                dbtblStats = new DbTable_Stats();

                foreach (MazeModel mm in mazeModels)
                {
                    dbtblStats.Guid = mm.guid;
                    dbtblStats.LastUsed = DateTime.UtcNow.ToString();

                    if (!mazeDb.InsertStats(dbtblStats))
                        throw new Exception("Failed to create stats");
                }

                FileIO.SerializeXml(mazeModels, filename);
                return string.Empty;
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        //public string SaveMazeModel()
        //{
        //    try
        //    {
        //        MazeModel mm = new MazeModel(maze_width, maze_height, mouse_x, mouse_y, cheese_x, cheese_y, mazedata);

        //        dbtblStats = new DbTable_Stats();
        //        dbtblStats.Guid = mm.guid;
        //        dbtblStats.LastUsed = DateTime.UtcNow.ToString();

        //        if (!mazeDb.InsertStats(dbtblStats))
        //            throw new Exception("Failed to create stats");

        //        if (string.IsNullOrEmpty(FileName) || !File.Exists(FileName))
        //        {
        //            FileName = FileIO.SaveFileAs_Dialog(AppDir, FILE_EXT);

        //            if (FileName == null)
        //                throw new Exception("Error Creating File");

        //            FileIO.SerializeXml(mm, FileName);
        //        }

        //        return string.Empty;
        //    }
        //    catch (Exception e)
        //    {
        //        return e.Message;
        //    }
        //}

        public string LoadMazeModel()
        {
            try
            {
                string filename = FileIO.OpenFile_Dialog(AppDir, FILE_EXT);

                if (!string.IsNullOrEmpty(filename) && File.Exists(filename))
                {
                    MazeModel mm = (MazeModel)FileIO.DeSerializeXml(typeof(MazeModel), filename);

                    if (mm == null)
                        throw new Exception("Error Loading File");

                    DbTable_Stats dbTableStats = mazeDb.ReadStats(mm.guid);

                    if (dbTableStats == null)
                    {
                        throw new Exception("Error Loading Stats");
                    }

                    FileName = filename;
                }

                return string.Empty;
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        #endregion

        #region Object Tools

        public bool SelectMazeModel(int index)
        {
            if (index > mazeModels.Count - 1)
                return false;

            MazeModel m = mazeModels[index];

            if (m != null)
            {
                return true;
            }

            return false;
        }

        public int GetMazeModelSize()
        {
            return mazeModels.Count;
        }

        public MazeModel GetMazeModel(int index)
        {
            if (index > mazeModels.Count - 1)
                return null;

            return mazeModels[index];
        }

        public MazeModels GetMazeModels()
        {
            return mazeModels;
        }

        private byte[,] GetMazeData()
        {
            return mazedata;
        }

        private static byte[,] ConvertArray(byte[][] ibytes)
        {
            int length = ibytes.Length;
            int width = ibytes[0].Length;

            byte[,] obytes = new byte[width,length];

            for (int y = 0; y < length; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    obytes[x,y] = ibytes[y][x];
                }
            }

            return obytes;
        }

        private static byte[][] ConvertArray(byte[,] ibytes)
        {
            int length = ibytes.GetLength(1);
            int width = ibytes.GetLength(0);

            byte[][] obytes = new byte[length][];

            for (int y = 0; y < length; y++)
            {
                obytes[y] = new byte[width];

                for (int x = 0; x < width; x++)
                {
                    obytes[y][x] = ibytes[x, y];
                }
            }
            return obytes;
        }

        private static char GetObjectChar(MazeObject mo)
        {
            if (mo.object_type == OBJECT_TYPE.BLOCK)
                return BLOCK;

            // ToDd: Scan Debug
            if (mo.object_state == OBJECT_STATE.MOUSE)
                return MOUSE;

            if (mo.isDeadEnd)
                return DEADEND;

            if (mo.isPath)
                return PATH;

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

        private byte GetObjectByte(int x, int y)
        {
            return (maze[XYToIndex(x, y)] == SPACE) ? WHITE : BLACK;
        }

        public OBJECT_TYPE GetObjectDataType(int x, int y)
        {
            return (mazedata[x, y] == WHITE) ? OBJECT_TYPE.SPACE : OBJECT_TYPE.BLOCK;
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
