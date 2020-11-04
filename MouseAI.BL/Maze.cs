﻿#region Using Statements

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using MouseAI.BL;
using MouseAI.ML;
using MouseAI.PL;

#endregion

namespace MouseAI
{
    public class Maze
    {
        #region Declarations

        private static MazeDb mazeDb;
        private NeuralNet neuralNet;
        private static byte[,] mazedata;
        private readonly MazeGenerator mazeGenerator;
        private static int maze_width;
        private static int maze_height;
        private int mouse_x;
        private int mouse_y;
        private int cheese_x;
        private int cheese_y;

        private const string FILE_EXT = "mze";
        private const string FILE_DIR = "mazes";
        public const int BLACK = 0x00;
        public const int WHITE = 0xff;
        public const int GREY = 0x80;

        private static Random r;
        private static StringBuilder sb;
        private static MazeObject[,] MazeObjects;
        private static MazeModels mazeModels;
        private static MazeModel mazeModel;
        private static List<MazeObject> PathObjects;
        private MazeObjectSegments mazeObjectSegments;
        private static MazeObject oMouse;
        private static MazePaths mazePaths;
        private readonly string AppDir;
        private string FileName;
        private bool isCheesePath;
        private readonly List<MazeObject>[] scanObjects = new List<MazeObject>[4];
        private static DbTable_Mazes dbtblStats;
        private static DbTable_Projects dbtblProjects;

        #endregion

        #region Initialization

        public Maze(int _maze_width, int _maze_height)
        {
            maze_width = _maze_width;
            maze_height = _maze_height;

            mazedata = new byte[maze_width, maze_height];
            MazeObjects = new MazeObject[maze_width, maze_height];
            mazeModels = new MazeModels();
            mazeModel = new MazeModel();
            r = new Random();
            sb = new StringBuilder();
            PathObjects = new List<MazeObject>();
            mazeGenerator = new MazeGenerator(maze_width, maze_height, r);
            mazePaths = new MazePaths(maze_width, maze_height);

            for (int i = 0; i < scanObjects.Length; i++)
            {
                scanObjects[i] = new List<MazeObject>();
            }

            AppDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\" + FILE_DIR;

            if (mazeDb == null)
                mazeDb = new MazeDb();

            neuralNet = new NeuralNet(maze_width, maze_height);
        }

        public void ClearMazeModels()
        {
            mazeModels.Clear();
        }

        public void AddMazeModel()
        {
            mazeModels.Add(new MazeModel(maze_width, maze_height, mouse_x, mouse_y, cheese_x, cheese_y, mazedata));
        }

        public bool isMazeModels()
        {
            return (mazeModels != null && mazeModels.Count() > 0);
        }

        public bool AddCharacters()
        {
            if (mazeModel == null)
                return false;

            PathObjects.Clear();

            int mx = mazeModel.mouse_x;
            int my = mazeModel.mouse_y;
            int cx = mazeModel.cheese_x;
            int cy = mazeModel.cheese_y;

            if (!IsInBounds(mx, my) || !IsInBounds(cx, cy))
                return false;

            mouse_x = mx;
            mouse_y = my;

            MazeObjects[mx, my].object_state = OBJECT_STATE.MOUSE;

            oMouse = new MazeObject(OBJECT_TYPE.SPACE, mx, my)
            {
                object_state = OBJECT_STATE.MOUSE,
                isVisited = true,
                dtLastVisit = DateTime.UtcNow
            };

            MazeObjects[cx, cy].object_state = OBJECT_STATE.CHEESE;
            MazeObjects[cx, cy].object_type = OBJECT_TYPE.SPACE;
            cheese_x = cx;
            cheese_y = cy;
            return true;
        }

        public bool AddCharacters_Random()
        {
            DIRECTION dir = (DIRECTION)r.Next(1, 4);

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
                isVisited = true,
                dtLastVisit = DateTime.UtcNow
            };

            x = mo_cheese.x;
            y = mo_cheese.y;

            MazeObjects[x, y].object_state = OBJECT_STATE.CHEESE;
            MazeObjects[x, y].object_type = OBJECT_TYPE.SPACE;
            cheese_x = x;
            cheese_y = y;

            // PathObjects.Add(oMouse);

            return true;
        }

        private static MazeObject GetMazeObject(DIRECTION dir)
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
            mazePaths.ClearPath(mazeModel.guid);
            isCheesePath = false;
        }

        public void Generate()
        {
            MazeGenerator.Reset();
            mazeGenerator.Generate();
            isCheesePath = false;
        }

        public void Update()
        {
            for (int y = 0; y < maze_height; ++y)
            {
                for (int x = 0; x < maze_width; ++x)
                {
                    mazedata[x, y] = MazeGenerator.GetObjectByte(x, y);
                    MazeObjects[x, y] = new MazeObject(GetObjectDataType(x, y), x, y);
                }
            }
        }

        #endregion

        #region Neural Net

        public void Train(double split, string guid)
        {
            if (mazeModels == null || mazeModels.Count() == 0)
                throw new Exception("No MazeModels!");

            //neuralNet.TestMnist();
            //return;

            MazeModel _mm = mazeModels.CheckPaths();
            if (_mm != null)
            {
                throw new Exception(string.Format("MazePath data for model not found:{0}\nHas the path been built?", _mm.guid));
            }

            if (mazeModels.Count() == 0|| mazeModels.GetSegmentCount() == 0)
            {
                throw new Exception("Maze test data not found!");
            }

            ImageDatas imageDatas = mazeModels.GetImageDatas();

            neuralNet.InitDataSets(imageDatas, split, guid, r);
            neuralNet.BuildDataSets();
            neuralNet.Process(100, mazeModels.Count(), 10, true, guid, false);
        }

        #endregion

        #region Vision


        #endregion region

        #region Scanning

        private int ScanDirection(int x, int y)
        {
            if (!isScanValid(x, y))
                return 1;

            if (CheckScannedObject(x, y) == OBJECT_STATE.CHEESE)
            {
                isCheesePath = true;
                return 0;
            }

            return MazeObjects[x, y].isDeadEnd ? 1 : -1;
        }

        private void ScanObjects(int x, int y)
        {
            if (isCheesePath)
                return;

            int result;

            // Scan West
            for (int x_idx = x - 1; x_idx > 0; x_idx--)
            {
                result = ScanDirection(x_idx, y);
                if (result == 1)
                    break;
                if (result == 0)
                    return;

                scanObjects[0].Add(MazeObjects[x_idx, y]);
            }

            // Scan East
            for (int x_idx = x + 1; x_idx < maze_width; x_idx++)
            {
                result = ScanDirection(x_idx, y);
                if (result == 1)
                    break;
                if (result == 0)
                    return;

                scanObjects[1].Add(MazeObjects[x_idx, y]);
            }

            // Scan North
            for (int y_idx = y - 1; y_idx > 0; y_idx--)
            {
                result = ScanDirection(x, y_idx);
                if (result == 1)
                    break;
                if (result == 0)
                    return;

                scanObjects[2].Add(MazeObjects[x, y_idx]);
            }

            // Scan South
            for (int y_idx = y + 1; y_idx < maze_height; y_idx++)
            {
                result = ScanDirection(x, y_idx);
                if (result == 1)
                    break;
                if (result == 0)
                    return;

                scanObjects[3].Add(MazeObjects[x, y_idx]);
            }

            for (int i = 0; i < scanObjects.Length; i++)
            {
                CheckEndPoints(scanObjects[i]);
                scanObjects[i].Clear();
            }
        }

        private static OBJECT_STATE CheckScannedObject(int x, int y)
        {
            return MazeObjects[x, y].object_state == OBJECT_STATE.CHEESE ? OBJECT_STATE.CHEESE : OBJECT_STATE.NONE;
        }

        private static bool isScanValid(int x, int y)
        {
            return (IsInBounds(x, y) && MazeObjects[x, y].object_type == OBJECT_TYPE.SPACE);
        }

        private static int GetScanValid(int x, int y)
        {
            return (IsInBounds(x, y) && MazeObjects[x, y].object_type == OBJECT_TYPE.SPACE) ? 1 : 0;
        }

        private static void CheckEndPoints(IList<MazeObject> mos)
        {
            if (mos.Count == 0)
                return;

            // If there are objects and the last point is a dead end
            if (GetPerimiter(mos.Last()) == 1)
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
        }

        private static void SetEndpoint(MazeObject mo)
        {
            mo.isDeadEnd = true;
        }

        private static int GetPerimiter(MazeObject mo)
        {
            int count = 0;
            int[,] panArray = GetXYPan(mo.x, mo.y);

            // Scan all directions from a given point
            for (int i = 0; i < panArray.Length / 2; i++)
            {
                count += GetScanValid(panArray[i, 0], panArray[i, 1]);
            }

            return count;
        }

        #endregion

        #region Movement

        public bool ProcessMouseMove()
        {
            int x = oMouse.x;
            int y = oMouse.y;

            // Check if mouse can see the cheese
            ScanObjects(x, y);

            List<MazeObject> mazeobjects = CheckNode(x, y);

            MazeObject mouse = mazeobjects.FirstOrDefault(o => o.object_state == OBJECT_STATE.MOUSE);

            if (mouse == null)
            {
                throw new Exception("Mouse Object Null!");
            }

            if (isCheesePath)
            {
                if (ProcessCheeseMove(mouse, mazeobjects))
                {
                    CleanPathObjects();
                    FinalizePathObjects();
                    return true;
                }
                return false;
            }

            MazeObject mo = mazeobjects.FirstOrDefault(o => o.isVisited == false &&
                                                            o.object_state != OBJECT_STATE.MOUSE);
            if (mo != null)
            {
                if (mazeobjects.Count >= 4)
                {
                    mouse.isJunction = true;
                    CleanPathObjects();
                }

                // oMouse.direction = GetMouseDirection(oMouse.x, oMouse.y, mo.x, mo.y);
                oMouse.direction = DIRECTION.SOUTH;

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
                    throw new Exception("Maze Object Null");

                oMouse.x = mo_oldest.x;
                oMouse.y = mo_oldest.y;
                oMouse.direction = DIRECTION.SOUTH;

                if (!mouse.isJunction)
                    mouse.isDeadEnd = true;

                MazeObjects[mo_oldest.x, mo_oldest.y].object_state = OBJECT_STATE.MOUSE;
                mouse.isVisited = true;
                mouse.object_state = OBJECT_STATE.VISITED;
            }

            return false;
        }

        private bool ProcessCheeseMove(MazeObject mouse, List<MazeObject> mazeobjects)
        {
            int curr_x = mouse.x;
            int curr_y = mouse.y;
            int new_x = -1;
            int new_y = -1;

            if (curr_x == cheese_x && curr_y == cheese_y)
            {
                MazeObject m = MazeObjects[curr_x, curr_y];
                m.object_state = OBJECT_STATE.CHEESE;
                m.dtLastVisit = DateTime.UtcNow;
                PathObjects.Add(m);
                return true;
            }

            if (curr_x == cheese_x) // Move North or south
            {
                new_x = curr_x;
                if (curr_y > cheese_y)
                    new_y = curr_y - 1;
                else
                    new_y = curr_y + 1;
            }
            else if (curr_y == cheese_y) // Move West or East
            {
                new_y = curr_y;
                if (curr_x > cheese_x)
                    new_x = curr_x - 1;
                else
                    new_x = curr_x + 1;
            }

            if (new_x == -1 || new_y == -1)
            {
                throw new Exception("Invalid Path Direction!");
            }

            if (!SetPathMove(curr_x, curr_y, new_x, new_y))
                throw new Exception("Invalid Path Move!");

            MazeObject mo = mazeobjects.FirstOrDefault(o => o.x == curr_x && o.y == curr_y);
            if (mo != null)
                PathObjects.Add(mo);

            return false;
        }

        public List<MazeObject> CheckNode(int x, int y)
        {
            List<MazeObject> mazeobjects = new List<MazeObject>
            {
                MazeObjects[x, y]
            };

            for (int x_idx = x - 1; x_idx < x + 2; x_idx += 2)
            {
                if (IsInBounds(x_idx, y) && GetObjectDataType(x_idx, y) == OBJECT_TYPE.SPACE)
                {
                    mazeobjects.Add(MazeObjects[x_idx, y]);
                }
            }

            for (int y_idx = y - 1; y_idx < y + 2; y_idx += 2)
            {
                if (IsInBounds(x, y_idx) && GetObjectDataType(x, y_idx) == OBJECT_TYPE.SPACE)
                {
                    mazeobjects.Add(MazeObjects[x, y_idx]);
                }
            }

            return mazeobjects;
        }

        #endregion

        #region Segments

        public void CalculateSegments()
        {
            MazeObject so = PathObjects.FirstOrDefault(o => o.object_state == OBJECT_STATE.MOUSE);

            if (so == null)
            {
                throw new Exception("Couldn't find the mouse!");
            }
            
            MazeObjects pathObjects = new MazeObjects(PathObjects.Where(o => o.isPath && !o.isDeadEnd).OrderBy(d => d.dtLastVisit).ToList());
            MazeObjects segmentObjects = new MazeObjects();
            int index = 0;

            mazeObjectSegments = new MazeObjectSegments();

            while (true)
            {
                so = pathObjects[index++];
                
                segmentObjects.Add(so);
                segmentObjects.AddRange(SearchObjects(so.x, so.y).Distinct());
                
                if (so.isJunction)
                {
                    mazeObjectSegments.Add(new MazeObjects(segmentObjects.Distinct().ToList()));
                }

                if (so.x == cheese_x && so.y == cheese_y)
                {
                    mazeObjectSegments.Add(new MazeObjects(segmentObjects.Distinct().ToList()));
                    break;
                }
            }

            for (int idx = mazeObjectSegments.Count - 1; idx > -1; idx--)
            {
                mazeObjectSegments[idx].RemoveAll(item=>!PathObjects.Contains(item));
                if (mazeObjectSegments[idx].Count == 0)
                    mazeObjectSegments.RemoveAt(idx);
            }

            GenerateSegments();
        }

        private void GenerateSegments()
        {
            byte b;
            Bitmap bmp = new Bitmap(maze_width, maze_height);
            Graphics g;
            MazeObjects mos;

            for (int idx = 0; idx < mazeObjectSegments.Count - 1; idx++)
            {
                mos = mazeObjectSegments[idx];
                g = Graphics.FromImage(bmp);
                g.Clear(GetColorNN(WHITE));

                foreach (MazeObject mo in mos)
                {
                    b = GetByteColor(mo);
                    (bmp).SetPixel(mo.x, mo.y, GetColorNN(b));
                }

                using (var memoryStream = new MemoryStream())
                {
                    bmp.Save(memoryStream, ImageFormat.Bmp);
                    mazeModel.segments.Add(memoryStream.ToArray());
                }
            }

            //foreach (MazeObject mo in mazeSegments.SelectMany(mos => mos))
            //{
            //    b = GetByteColor(mo);
            //    (bmp).SetPixel(mo.x, mo.y, GetColorNN(b));
            //}
        }

        public List<byte[]> GetSegments()
        {
            return mazeModel.segments;
        }

        private static MazeObjects SearchObjects(int x, int y)
        {
            MazeObjects pathObjects = new MazeObjects();
            
            // Scan West
            for (int x_idx = x - 1; x_idx > 0; x_idx--)
            {
                if (!SearchObject(x_idx, y, pathObjects))
                    break;
            }

            // Scan East
            for (int x_idx = x + 1; x_idx < maze_width; x_idx++)
            {
                if (!SearchObject(x_idx, y, pathObjects))
                    break;
            }

            // Scan North
            for (int y_idx = y - 1; y_idx > 0; y_idx--)
            {
                if (!SearchObject(x, y_idx, pathObjects))
                    break;
            }

            // Scan South
            for (int y_idx = y + 1; y_idx < maze_height; y_idx++)
            {
                if (!SearchObject(x, y_idx, pathObjects))
                    break;
            }

            return pathObjects;
        }

        private static bool SearchObject(int x, int y, MazeObjects pathObjects)
        {
            if (!isScanValid(x, y))
                return false;

            pathObjects.Add(MazeObjects[x, y]);

            int[,] panArray = GetXYPan(x, y);
            
            // Scan all directions from a given point
            for (int i = 0; i < panArray.Length / 2; i++)
            {
                x = panArray[i, 0];
                y = panArray[i, 1];

                if (isScanValid(x, y))
                {
                    pathObjects.Add(MazeObjects[x, y]);
                }
            }
            return true;
        }

        public MazeObjectSegments GetMazeSegments()
        {
            return mazeObjectSegments;
        }

        #endregion

        #region Paths

        public void CalculatePath()
        {
            byte b;
            PathObjects = PathObjects.OrderBy(x => x.dtLastVisit).ToList();
            MazePath mp = mazePaths.GetPath(mazeModel.guid);

            if (mp == null)
                throw new Exception("Maze path was null!");

            mp.bmp = new Bitmap(maze_width, maze_height);
            Graphics g = Graphics.FromImage(mp.bmp);
            g.Clear(GetColorNN(WHITE));

            foreach (MazeObject mo in PathObjects)
            {
                b = GetByteColor(mo);
                mp.mazepath[mo.y][mo.x] = b;
                (mp.bmp).SetPixel(mo.x, mo.y, GetColorNN(b));
            }
        }

        private static bool SetPathMove(int curr_x, int curr_y, int new_x, int new_y)
        {
            if (CheckPathMove(new_x, new_y))
            {
                MazeObject mo = MazeObjects[curr_x, curr_y];
                mo.object_state = OBJECT_STATE.VISITED;
                mo.dtLastVisit = DateTime.UtcNow;
                mo.isPath = true;
                mo = MazeObjects[new_x, new_y];
                mo.object_state = OBJECT_STATE.MOUSE;
                mo.dtLastVisit = DateTime.UtcNow;
                mo.isPath = true;
                oMouse.x = new_x;
                oMouse.y = new_y;
                return true;
            }
            return false;
        }

        private static bool CheckPathMove(int x, int y)
        {
            return (IsInBounds(x, y) && GetObjectDataType(x, y) == OBJECT_TYPE.SPACE);
        }

        private void FinalizePathObjects()
        {
            List<MazeObject> pathObjects = new List<MazeObject>();
            List<MazeObject> pathsLast = new List<MazeObject>();

            MazeObject m = new MazeObject(OBJECT_TYPE.SPACE, mouse_x, mouse_y)
            {
                dtLastVisit = DateTime.MinValue,
                isPath = true,
                isDeadEnd = false,
                object_state = OBJECT_STATE.MOUSE
            };
            PathObjects.Insert(0, m);

            foreach (MazeObject mo in PathObjects)
            {
                if (mo.x == cheese_x && mo.y == cheese_y)
                    break;

                pathsLast.Clear();
                for (int x = mo.x - 1; x > 0; x--)
                {
                    if (!CheckPathValid(x, mo.y, pathObjects, pathsLast))
                    {
                        CheckPathTurn(pathObjects, pathsLast);
                        break;
                    }
                }

                pathsLast.Clear();
                for (int x = mo.x + 1; x < maze_width; x++)
                {
                    if (!CheckPathValid(x, mo.y, pathObjects, pathsLast))
                    {
                        CheckPathTurn(pathObjects, pathsLast);
                        break;
                    }
                }

                pathsLast.Clear();
                for (int y = mo.y - 1; y > 0; y--)
                {
                    if (!CheckPathValid(mo.x, y, pathObjects, pathsLast))
                    {
                        CheckPathTurn(pathObjects, pathsLast);
                        break;
                    }
                }

                pathsLast.Clear();
                for (int y = mo.y + 1; y < maze_height; y++)
                {
                    if (!CheckPathValid(mo.x, y, pathObjects, pathsLast))
                    {
                        CheckPathTurn(pathObjects, pathsLast);
                        break;
                    }
                }
            }
            PathObjects.AddRange(pathObjects);
            oMouse.x = mouse_x;
            oMouse.y = mouse_y;
        }

        private static bool CheckPathValid(int x, int y, ICollection<MazeObject> pathObjects, ICollection<MazeObject> pathsLast)
        {
            if (IsInBounds(x, y) && !MazeObjects[x, y].isPath &&
                MazeObjects[x, y].object_type == OBJECT_TYPE.SPACE &&
                !PathObjects.Any(o => o.x == x && o.y == y))
            {
                MazeObjects[x, y].isPath = true;
                MazeObjects[x, y].isDeadEnd = true;
                MazeObjects[x, y].dtLastVisit = DateTime.UtcNow;
                pathObjects.Add(MazeObjects[x, y]);
                pathsLast?.Add(pathObjects.Last());
                return true;
            }
            return false;
        }

        private static void CheckPathTurn(ICollection<MazeObject> pathObjects, IReadOnlyCollection<MazeObject> pathsLast)
        {
            if (pathsLast.Count == 0)
                return;

            int[,] panArray;

            foreach (MazeObject pathLast in pathsLast)
            {
                panArray = GetXYPan(pathLast.x, pathLast.y);

                for (int i = 0; i < panArray.Length / 2; i++)
                {
                    CheckPathValid(panArray[i, 0], panArray[i, 1], pathObjects, null);
                }
            }
        }

        private static int[,] GetXYPan(int x, int y)
        {
            return new[,]{ { x-1, y }, { x+1, y }, { x, y-1 }, { x, y+1 }};
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
                        throw new Exception("Objects is null!");

                    if (mo.Count(x => x.isDeadEnd) == mo.Count - 1)
                    {
                        PathObjects[i].isDeadEnd = true;
                        PathObjects[i].isPath = false;
                        PathObjects.RemoveAt(i);
                    }
                }
            }
        }

        public void UpdateMazeModelPaths()
        {
            if (mazeModels.Count() == 0 || mazePaths.Count == 0)
                return;

            foreach (MazeModel mm in mazeModels.GetMazeModels())
            {
                MazePath mp = mazePaths.GetPath(mm.guid);

                if (mp?.mazepath != null && mp.bmp != null)
                {
                    mm.mazepath = (byte[][])mp.mazepath.Clone();

                    using (var memoryStream = new MemoryStream())
                    {
                        mp.bmp.Save(memoryStream, ImageFormat.Bmp);
                        mm.maze = memoryStream.ToArray();
                    }
                }
            }
        }

        public void UpdateMazePaths()
        {
            if (mazeModels.Count() == 0)
                return;

            MemoryStream ms;
            MazePath mp;

            foreach (MazeModel mm in mazeModels.GetMazeModels())
            {
                mp = mazePaths.GetPath(mm.guid);

                if (mp != null && mm.maze != null && mm.mazepath != null)
                {
                    mp.mazepath = (byte[][])mm.mazepath.Clone();
                    ms = new MemoryStream(mm.maze);
                    mp.bmp = (Bitmap)Image.FromStream(ms);
                }
            }
        }

        public Bitmap GetPathBMP(string guid)
        {
            return mazePaths.GetPath(guid).bmp;
        }

        public bool isModelBMP(int index)
        {
            if (index < 0 || index > mazeModels.Count())
                return false;

            MazeModel mm = mazeModels.GetMazeModel(index);

            if (mm == null)
                return false;

            return mm.maze != null && mm.mazepath != null;
        }

        public string GetGUID()
        {
            return mazeModel?.guid;
        }

        public bool SetTested(bool isTested)
        {
            if (mazeModel == null)
                return false;

            mazeModel.isPath = isTested;
            return true;
        }


        private static Color GetColor(byte b)
        {
            switch (b)
            {
                case BLACK: return Color.Black;
                case GREY: return Color.Gray;
                case WHITE: return Color.White;
                default: return Color.Yellow;
            }
        }

        private static Color GetColorNN(byte b)
        {
            switch (b)
            {
                case BLACK: return Color.Black;
                case GREY: return Color.Black;
                case WHITE: return Color.White;
                default: return Color.Yellow;
            }
        }

        private static byte GetByteColor(MazeObject mo)
        {
            return mo.isDeadEnd ? (byte)GREY : (byte)BLACK;
        }

        #endregion

        #region Saving and Loading

        public string GetSaveName()
        {
            return FileIO.SaveFileAs_Dialog(AppDir, FILE_EXT);
        }

        public void SetMazeModelsGuid(string guid)
        {
            mazeModels.Guid = guid;
        }

        public string GetMazeModelsGuid()
        {
            return mazeModels.Guid;
        }

        public string SaveMazeModels(string filename)
        {
            try
            {
                //dbtblProjects = new DbTable_Projects
                //{
                //    Guid = Guid.NewGuid().ToString(),
                //    Config = string.Empty,
                //    dtStart = string.Empty,
                //    dtEnd = string.Empty
                //};
                //if (!mazeDb.InsertProject(dbtblProjects))
                //    throw new Exception("Failed to create projects table");

                dbtblStats = new DbTable_Mazes();

                foreach (MazeModel mm in mazeModels.GetMazeModels())
                {
                    dbtblStats.Guid = mm.guid;
                    dbtblStats.LastUsed = DateTime.UtcNow.ToString();

                    if (!mazeDb.InsertMaze(dbtblStats))
                        throw new Exception("Failed to create maze table");
                }

                FileIO.SerializeXml(mazeModels, filename);

                FileName = filename;

                return string.Empty;
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        public string SaveUpdatedMazeModels(string filename)
        {
            try
            {
                FileIO.SerializeXml(mazeModels, filename);
                return string.Empty;
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        public string LoadMazeModels(string filename)
        {
            try
            {
                if (string.IsNullOrEmpty(filename))
                    filename = FileIO.OpenFile_Dialog(AppDir, FILE_EXT);

                if (string.IsNullOrEmpty(filename) || !File.Exists(filename))
                    throw new Exception("Error Loading File");


                MazeModels mms = (MazeModels)FileIO.DeSerializeXml(typeof(MazeModels), filename);

                if (mms == null)
                    throw new Exception("Error Loading File");


                foreach (MazeModel mm in mms.GetMazeModels())
                {
                    if (mazeDb.ReadMazes(mm.guid) == null)
                        throw new Exception(string.Format("DB Stats Missing for GUID '{0}'", mm.guid));
                }

                mazeModels.Clear();
                mazeModels = mms;

                FileName = filename;

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
            if (index < 0 || index > mazeModels.Count() - 1)
                return false;

            mazeModel = mazeModels.GetMazeModel(index);

            if (mazeModel == null)
                return false;

            DbTable_Mazes dbTableStats = mazeDb.ReadMazes(mazeModel.guid);

            if (dbTableStats == null)
                return false;

            dbtblStats = dbTableStats;

            mazedata = ConvertArray(mazeModel.mazedata);

            for (int y = 0; y < maze_height; ++y)
            {
                for (int x = 0; x < maze_width; ++x)
                {
                    MazeObjects[x, y] = null;
                    MazeObjects[x, y] = new MazeObject(GetObjectDataType(x, y), x, y);
                }
            }

            return true;
        }

        public int GetMazeModelSize()
        {
            return (mazeModels == null) ? 0 : mazeModels.Count();
        }

        private static byte[,] ConvertArray(byte[][] ibytes)
        {
            int length = ibytes.Length;
            int width = ibytes[0].Length;

            byte[,] obytes = new byte[width, length];

            for (int y = 0; y < length; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    obytes[x, y] = ibytes[y][x];
                }
            }

            return obytes;
        }

        public Point GetMousePosition()
        {
            return new Point(oMouse.x, oMouse.y);
        }

        public int GetMouseDirection()
        {
            return (int)oMouse.direction;
        }

        public static bool IsInBounds(int x, int y)
        {
            if (x < 0 || x >= maze_width)
                return false;

            return (y >= 0 && y < maze_height);
        }

        public static OBJECT_TYPE GetObjectDataType(int x, int y)
        {
            return (mazedata[x, y] == WHITE) ? OBJECT_TYPE.SPACE : OBJECT_TYPE.BLOCK;
        }

        public static OBJECT_STATE GetObjectState(int x, int y)
        {
            return (MazeObjects[x, y].object_state);
        }

        public string GetFileName()
        {
            return string.IsNullOrEmpty(FileName) ? string.Empty : FileName;
        }

        public List<MazeObject> GetPathObjects()
        {
            return PathObjects;
        }

        public MazeObject[,] GetMazeObjects()
        {
            return MazeObjects;
        }
    }

    #endregion
}
