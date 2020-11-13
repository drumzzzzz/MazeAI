#region Using Statements

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using MouseAI.BL;
using MouseAI.ML;
using MouseAI.PL;
using MouseAI.SH;
using ScottPlot;

#endregion

namespace MouseAI
{
    public class Maze
    {
        #region Declarations

        private NeuralNet neuralNet;
        private static byte[,] mazedata;
        private readonly MazeGenerator mazeGenerator;
        private static MazeObject[,] MazeObjects;
        private static MazeModels mazeModels;
        private static MazeModel mazeModel;
        private static List<MazeObject> PathObjects;
        private MazeObjectSegments mazeObjectSegments;
        private static MazeObject oMouse;
        private static MazePaths mazePaths;
        private readonly List<MazeObject>[] scanObjects = new List<MazeObject>[4];
        private Config config;

        // Db
        private static MazeDb mazeDb;
        private static DbTable_Mazes dbtblStats;
        private static DbTable_Projects dbtblProjects;

        private static Random r;
        private static StringBuilder sb;

        private static int maze_width;
        private static int maze_height;
        private int mouse_x;
        private int mouse_y;
        private int cheese_x;
        private int cheese_y;
        private bool isCheesePath;

        private const string MAZE_DIR = "mazes";
        private const string MAZE_EXT = "mze";
        private const string LOG_DIR = "logs";
        private const string LOG_EXT = "csv";
        private const string MODELS_DIR = "models";
        private const string MODELS_EXT = "h5";
        private const string CONFIG_EXT = "xml";
        private const string ARCHIVE_DIR = "archived";
        private const string ARCHIVE_EXT = "zip";
        private const string PLOT_EXT = "png";
        private const char LOG_DELIMIT = ',';
        private static readonly string[] LOG_COLUMN_VALUES = {"epoch", "accuracy", "loss", "val_accuracy", "val_loss"};
        private static readonly int[] PLOT_COLUMNS_Y = { 1, 3 };
        private static readonly Color[] PLOT_COLORS_Y = {Color.Empty, Color.Green, Color.Blue, Color.Green, Color.Blue};
        private const int PLOT_COLUMN_X = 0;
        private const int PLOT_WIDTH = 300;
        private const int PLOT_HEIGHT = 300;
        private const string DIR = @"\";
        public const int BLACK = 0x00;
        public const int WHITE = 0xff;
        public const int GREY = 0x80;

        private static string model_dir;
        private static string log_dir;
        private static string maze_dir;
        private static string archive_dir;
        private string FileName;
        private string ModelProjectGuid;

        private static readonly string[] IGNORE_VALUES = { "Config", "Model", "Guid", "StartTime" };

        private static Bitmap visualbmp;
        // private static byte[] imagebytes;
        private List<byte[]> imagebytes;

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

            string appdir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            maze_dir = appdir + DIR + MAZE_DIR;
            log_dir = appdir + DIR + LOG_DIR;
            model_dir = appdir + DIR + MODELS_DIR;
            archive_dir = appdir + DIR + ARCHIVE_DIR;

            if (!FileIO.CheckCreateDirectory(maze_dir))
                throw new Exception("Could not create maze directory");
            if (!FileIO.CheckCreateDirectory(log_dir))
                throw new Exception("Could not create log directory");
            if (!FileIO.CheckCreateDirectory(model_dir))
                throw new Exception("Could not create log directory");
            if (!FileIO.CheckCreateDirectory(archive_dir))
                throw new Exception("Could not create archive directory");

            mazeDb = new MazeDb();
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

        #region Neural Network Training

        public void Train(string guid)
        {
            if (config == null)
                throw new Exception("Invalid Config!");

            if (mazeModels == null || mazeModels.Count() == 0)
                throw new Exception("No MazeModels!");

            MazeModel _mm = mazeModels.CheckPaths();
            if (_mm != null)
            {
                throw new Exception(string.Format("MazePath data for model not found:{0}\nHas the path been built?", _mm.guid));
            }

            if (mazeModels.Count() == 0 || mazeModels.GetSegmentCount() == 0)
            {
                throw new Exception("Maze test data not found!");
            }

            config.Guid = guid;
            ImageDatas imageDatas = mazeModels.GetImageDatas();

            neuralNet = new NeuralNet(maze_width, maze_height, log_dir, LOG_EXT, model_dir, MODELS_EXT, CONFIG_EXT, PLOT_EXT);
            neuralNet.InitDataSets(imageDatas, config.Split, r);
            neuralNet.Process(config, mazeModels.Count());
        }

        public void LoadModel(string modelName)
        {
            if (string.IsNullOrEmpty(modelName))
                throw new Exception("Invalid Model Name!");

            neuralNet = new NeuralNet(maze_width, maze_height, log_dir, LOG_EXT, model_dir, MODELS_EXT, CONFIG_EXT, PLOT_EXT);
            neuralNet.LoadModel(modelName);
        }

        public void SaveResults()
        {
            dbtblProjects = new DbTable_Projects
            {
                Guid = GetModelProjectGuid(),
                Accuracy = neuralNet.GetAccuracy(),
                Epochs = neuralNet.GetEpochs(),
                Start = neuralNet.GetStartTime(),
                End = neuralNet.GetEndTime(),
                Log = neuralNet.GetLogName(),
                isLast = "0"
            };

            if (!mazeDb.InsertProject(dbtblProjects))
                throw new Exception("Failed to insert db record");

            mazeDb.UpdateModelLast(mazeModels.Guid, dbtblProjects.Log);

            neuralNet.SaveFiles();
            mazeModels.StartTime = config.StartTime;
            SavePlot(config.StartTime);
        }

        public void CleanNetwork()
        {
            neuralNet = null;
        }

        public Config GetConfig()
        {
            return config;
        }

        public void SetConfig(Config _config)
        {
            config = _config;
        }

        public string GetLogName()
        {
            return neuralNet.GetLogName();
        }

        #endregion

        #region Neural Network Prediction

        public ImageDatas Predict(string starttime)
        {
            if (string.IsNullOrEmpty(starttime))
                throw new Exception("Invalid Prediction Model");

            Config cfg = (Config)FileIO.DeSerializeXml(typeof(Config),
                Utils.GetFileWithExtension(model_dir, starttime, CONFIG_EXT));

            if (cfg == null || string.IsNullOrEmpty(cfg.Model))
                throw new Exception("Could not open config file");

            ImageDatas imageSegments = mazeModels.GetImageSegments();
            neuralNet.InitDataSets(imageSegments);
            return neuralNet.Predict(cfg.isCNN);
        }

        #endregion

        #region Neural Network Running

        public bool ProcessRunMove()
        {
            int x = oMouse.x;
            int y = oMouse.y;

            // Check if mouse can see the cheese
            if (!isCheesePath)
                ScanObjects(x, y);

            List<MazeObject> mazeobjects = CheckNode(x, y);

            MazeObject mouse = mazeobjects.FirstOrDefault(o => o.object_state == OBJECT_STATE.MOUSE);

            if (mouse == null)
            {
                throw new Exception("Mouse Object Null!");
            }

            MazeObjects segmentObjects = new MazeObjects {mouse};
            segmentObjects.AddRange(SearchObjects(mouse.x, mouse.y).Distinct());

            if (imagebytes == null)
            {
                imagebytes = new List<byte[]>();
            }
            imagebytes.Clear();
            imagebytes.Add(GenerateVisualImage(segmentObjects));
            return false;
        }

        public void ProcessRunImage()
        {
            if (imagebytes == null || imagebytes.Count == 0)
            {
                Console.WriteLine("Invalid image bytes");
                return;
            }

            int index = neuralNet.Predict(imagebytes);
            Console.WriteLine("Index: {0}", index);
        }

        private static byte[] GenerateVisualImage(MazeObjects mos)
        {
            byte b;
            if (visualbmp == null)
            {
                visualbmp = new Bitmap(maze_width, maze_height);
            }

            Graphics g = Graphics.FromImage(visualbmp);
            g.Clear(GetColorNN(WHITE));

            foreach (MazeObject mo in mos)
            {
                b = GetByteColor(mo);
                (visualbmp).SetPixel(mo.x, mo.y, GetColorNN(b));
            }

            MemoryStream memoryStream = new MemoryStream();
            visualbmp.Save(memoryStream, ImageFormat.Bmp);
            byte[] bytes = memoryStream.ToArray();
            memoryStream.Close();
            
            return bytes;
        }

        #endregion

        #region Movement

        public bool ProcessMouseMove()
        {
            int x = oMouse.x;
            int y = oMouse.y;

            // Check if mouse can see the cheese
            if (!isCheesePath)
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

            ProcessMove(mazeobjects, mouse);

            return false;
        }

        private void ProcessMove(IReadOnlyCollection<MazeObject> mazeobjects, MazeObject mouse)
        {
            MazeObject mo = mazeobjects.FirstOrDefault(o => o.isVisited == false &&
                                                            o.object_state != OBJECT_STATE.MOUSE);
            if (mo != null)
            {
                if (mazeobjects.Count >= 4)
                {
                    mouse.isJunction = true;
                    CleanPathObjects();
                }

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
                new_y = (curr_y > cheese_y) ? curr_y - 1 : curr_y + 1;
            }
            else if (curr_y == cheese_y) // Move West or East
            {
                new_y = curr_y;
                new_x = (curr_x > cheese_x) ? curr_x - 1 : curr_x + 1;
            }

            if (new_x == -1 || new_y == -1)
                throw new Exception("Invalid Path Direction!");

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
                if (isNode(x_idx, y))
                    mazeobjects.Add(MazeObjects[x_idx, y]);
            }

            for (int y_idx = y - 1; y_idx < y + 2; y_idx += 2)
            {
                if(isNode(x, y_idx))
                    mazeobjects.Add(MazeObjects[x, y_idx]);
            }

            return mazeobjects;
        }

        private static bool isNode(int x, int y)
        {
            return (IsInBounds(x, y) && GetObjectDataType(x, y) == OBJECT_TYPE.SPACE);
        }

        #endregion

        #region Scanning

        private void ScanObjects(int x, int y)
        {
            int result;

            // Scan West
            for (int x_idx = x - 1; x_idx > 0; x_idx--)
            {
                if (ScanDirection(x_idx, y, out result))
                    return;
                if (result == 1)
                    break;
                scanObjects[0].Add(MazeObjects[x_idx, y]);
            }

            // Scan East
            for (int x_idx = x + 1; x_idx < maze_width; x_idx++)
            {
                if (ScanDirection(x_idx, y, out result))
                    return;
                if (result == 1)
                    break;
                scanObjects[1].Add(MazeObjects[x_idx, y]);
            }

            // Scan North
            for (int y_idx = y - 1; y_idx > 0; y_idx--)
            {
                if (ScanDirection(x, y_idx, out result))
                    return;
                if (result == 1)
                    break;
                scanObjects[2].Add(MazeObjects[x, y_idx]);
            }

            // Scan South
            for (int y_idx = y + 1; y_idx < maze_height; y_idx++)
            {
                if (ScanDirection(x, y_idx, out result))
                    return;
                if (result == 1)
                    break;
                scanObjects[3].Add(MazeObjects[x, y_idx]);
            }

            for (int i = 0; i < scanObjects.Length; i++)
            {
                CheckEndPoints(scanObjects[i]);
                scanObjects[i].Clear();
            }
        }

        private bool ScanDirection(int x, int y, out int result)
        {
            if (!isScanValid(x, y))
            {
                result = 1;
                return false;
            }
            if (CheckScannedObject(x, y) == OBJECT_STATE.CHEESE)
            {
                isCheesePath = true;
                result = 0;
                return true;
            }
            result = MazeObjects[x, y].isDeadEnd ? 1 : -1;
            return false;
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

        private static void SetEndpoint(MazeObject mo)
        {
            mo.isDeadEnd = true;
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
            bool isFirstJunction = false;
            bool isAddSegments;
            bool isBreak = false;
            int count1;

            mazeObjectSegments = new MazeObjectSegments();

            while (true)
            {
                isAddSegments = false;
                so = pathObjects[index++];

                segmentObjects.Add(so);
                segmentObjects.AddRange(SearchObjects(so.x, so.y).Distinct());

                if (so.x == cheese_x && so.y == cheese_y)
                {
                    isBreak = true;
                }
                else if (!so.isJunction && !isFirstJunction)
                {
                    count1 = mazeObjectSegments.Count;
                    isAddSegments = (count1 == 0 || mazeObjectSegments[count1 - 1].Count !=
                                     segmentObjects.Distinct().ToList().Count);
                }
                else if (so.isJunction)
                {
                    isAddSegments = isFirstJunction = true;
                }

                if (isAddSegments || isBreak)
                {
                    mazeObjectSegments.Add(new MazeObjects(segmentObjects.Distinct().ToList()));
                    if (isBreak)
                        break;
                }
            }
            ValidateSegments(mazeObjectSegments);
            GenerateSegmentImages(mazeObjectSegments);
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

        private static void ValidateSegments(MazeObjectSegments mazeObjectSegments)
        {
            for (int idx = mazeObjectSegments.Count - 1; idx > -1; idx--)
            {
                mazeObjectSegments[idx].RemoveAll(item => !PathObjects.Contains(item));
                if (mazeObjectSegments[idx].Count == 0)
                    mazeObjectSegments.RemoveAt(idx);
            }

            int count1 = mazeObjectSegments[mazeObjectSegments.Count - 1].Count;
            int count2 = PathObjects.Count - 1;

            if (count1 != count2)
                throw new Exception(string.Format("Segment length not equal: {0}, {1}", count1, count2));
        }

        private static void GenerateSegmentImages(MazeObjectSegments mazeObjectSegments)
        {
            byte b;
            Bitmap bmp = new Bitmap(maze_width, maze_height);
            Graphics g;
            MazeObjects mos;
            MemoryStream memoryStream;
            mazeModel.segments.Clear();

            for (int idx = 0; idx < mazeObjectSegments.Count; idx++)
            {
                mos = mazeObjectSegments[idx];
                g = Graphics.FromImage(bmp);
                g.Clear(GetColorNN(WHITE));

                foreach (MazeObject mo in mos)
                {
                    b = GetByteColor(mo);
                    (bmp).SetPixel(mo.x, mo.y, GetColorNN(b));
                }

                memoryStream = new MemoryStream();
                bmp.Save(memoryStream, ImageFormat.Bmp);
                mazeModel.segments.Add(memoryStream.ToArray());
                memoryStream.Close();
            }
        }

        public List<byte[]> GetSegments()
        {
            return mazeModel.segments;
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
            return new[,] { { x - 1, y }, { x + 1, y }, { x, y - 1 }, { x, y + 1 } };
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

        public string GetMazeModelGUID()
        {
            return mazeModel?.guid;
        }

        public string GetMazeModelsGUID()
        {
            return mazeModels?.Guid;
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

        #region Test

        public List<string> GetProjectModels()
        {
            if (string.IsNullOrEmpty(mazeModels.Guid))
                return null;

            IEnumerable<object> oList = mazeDb.ReadProjectGuids(mazeModels.Guid);

            if (oList == null)
                return null;

            DbTable_Projects dbTableProjects;
            List<string> starttimes = new List<string>();

            foreach (DbTable_Projects obj in oList)
            {
                dbTableProjects = obj;
                starttimes.Add(dbTableProjects.Log);
            }

            return starttimes;
        }

        private static List<string> GetProjectModels(string guid)
        {
            IEnumerable<object> oList = mazeDb.ReadProjectGuids(guid);

            if (oList == null)
                return null;

            DbTable_Projects dbTableProjects;
            List<string> starttimes = new List<string>();

            foreach (DbTable_Projects obj in oList)
            {
                dbTableProjects = obj;
                starttimes.Add(dbTableProjects.Log);
            }

            return starttimes;
        }

        public string GetProjectModel()
        {
            return mazeModels.StartTime;
        }

        public static string GetModelSummary(string starttime)
        {
            string filename = model_dir + DIR + starttime + "." + CONFIG_EXT;

            try
            {
                return FileIO.ReadXml(filename, IGNORE_VALUES);
            }
            catch
            {
            }

            return null;
        }

        public static string GetModelPlot(string starttime)
        {
            string filename = log_dir + DIR + starttime + "." + PLOT_EXT;

            return (FileIO.CheckFileName(filename)) ? filename : string.Empty;
        }

        #endregion

        #region Plotting

        public static void SavePlot(string plotname)
        {
            string plotfile = LOG_DIR + @"\" + plotname + "." + LOG_EXT;
            List<string> plotvalues = FileIO.ReadFileAsList(plotfile);

            if (!CheckColumns(plotvalues))
                throw new Exception("Error reading log file");

            Dictionary<int, double[]> values = ProcessLines(plotvalues);

            if (values.Count < PLOT_COLUMNS_Y.Length + 1)
                throw new Exception("Error reading plot columns");

            var plt = new Plot(PLOT_WIDTH, PLOT_HEIGHT);

            double[] x = values[PLOT_COLUMN_X];

            foreach (var entry in values.Where(entry => PLOT_COLUMNS_Y.Contains(entry.Key)))
            {
                plt.PlotScatter(x, entry.Value, label: LOG_COLUMN_VALUES[entry.Key], color: PLOT_COLORS_Y[entry.Key], markerSize: 0);
            }

            plt.Legend(fontSize: 8);
            plt.SaveFig(LOG_DIR + @"\" + plotname + "." + PLOT_EXT);
        }

        private static Dictionary<int, double[]> ProcessLines(IReadOnlyList<string> plotvalues)
        {
            Dictionary<int, double[]> dvalues = new Dictionary<int, double[]>();

            int data_size = plotvalues.Count - 1;

            dvalues.Add(PLOT_COLUMN_X, new double[data_size]);


            foreach (int column in PLOT_COLUMNS_Y)
            {
                dvalues.Add(column, new double[data_size]);
            }

            for (int idx = 1; idx < plotvalues.Count; idx++)
            {
                ProcessValues(plotvalues[idx], dvalues, idx - 1);
            }

            return dvalues;
        }

        private static void ProcessValues(string line, IReadOnlyDictionary<int, double[]> dvalues, int index)
        {
            if (string.IsNullOrEmpty(line))
                throw new Exception("Invalid log data");

            string[] line_values = line.Split(LOG_DELIMIT);

            if (line_values.Length != LOG_COLUMN_VALUES.Length)
                throw new Exception("Invalid log data length");

            for (int idx = 0; idx < LOG_COLUMN_VALUES.Length; idx++)
            {
                if (PLOT_COLUMN_X == idx)
                    dvalues[idx].SetValue(Convert.ToDouble(line_values[idx]), index);
                else if (PLOT_COLUMNS_Y.Contains(idx))
                    dvalues[idx].SetValue(Convert.ToDouble(line_values[idx]), index);
            }
        }

        private static bool CheckColumns(IReadOnlyList<string> plotvalues)
        {
            if (plotvalues.Count < 2)
                return false;

            string[] columns = plotvalues[0].Split(LOG_DELIMIT);

            if (columns.Length != LOG_COLUMN_VALUES.Length)
                return false;

            return !LOG_COLUMN_VALUES.Where((t, idx) => !columns[idx].Trim().Equals(t, StringComparison.InvariantCultureIgnoreCase)).Any();
        }

        #endregion

        #region Saving and Loading

        public void SaveMazeModels(string filename)
        {
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
        }

        public void SaveUpdatedMazeModels(string filename)
        {
            FileIO.SerializeXml(mazeModels, filename);
        }

        public void LoadMazeModels(string filename)
        {
            if (string.IsNullOrEmpty(filename))
                filename = FileIO.OpenFile_Dialog(maze_dir, MAZE_EXT);

            if (string.IsNullOrEmpty(filename) || !File.Exists(filename))
                throw new Exception("Error Loading File");

            MazeModels mms = (MazeModels)FileIO.DeSerializeXml(typeof(MazeModels), filename);

            if (mms == null)
                throw new Exception("Error Loading File");

            ModelProjectGuid = mms.Guid;

            foreach (MazeModel mm in mms.GetMazeModels())
            {
                if (mazeDb.ReadMazes(mm.guid) == null)
                    throw new Exception(string.Format("DB Stats Missing for GUID '{0}'", mm.guid));
            }

            mazeModels.Clear();
            mazeModels = mms;

            FileName = filename;
        }

        public List<string> GetProjects()
        {
            return FileIO.GetFiles(maze_dir,"*." + MAZE_EXT);
        }

        public string GetSaveName()
        {
            return FileIO.SaveFileAs_Dialog(maze_dir, MAZE_EXT);
        }

        public void SetMazeModelsGuid(string guid)
        {
            mazeModels.Guid = guid;
        }

        public string GetModelProjectGuid()
        {
            return ModelProjectGuid;
        }

        public void SetProjectLast(string guid, string starttime)
        {
            mazeDb.UpdateModelLast(guid, starttime);
        }

        public string GetProjectLast(string guid)
        {
            if (string.IsNullOrEmpty(guid))
                return string.Empty;

            try
            {
                return mazeDb.ReadModelLast(guid);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error reading last project model", e);
                return string.Empty;
            }
        }

        public string GetLogDir()
        {
            return log_dir;
        }

        public string GetModelsDir()
        {
            return model_dir;
        }

        #endregion

        #region Archiving

        public static string ArchiveProject(string projectname)
        {
            try
            {
                MazeModels mms = (MazeModels)FileIO.DeSerializeXml(typeof(MazeModels), maze_dir + @"\" + projectname);

                if (mms == null || string.IsNullOrEmpty(mms.Guid))
                    throw new Exception("Failed to retrieve project id");

                List<MazeModel> mazemodels = mms.GetMazeModels();

                if (mazemodels == null || mazemodels.Count == 0)
                {
                    throw new Exception("No maze models found in project");
                }

                RemoveMazeRecords(mms);
                return ArchiveProjectFiles(projectname, mms.Guid);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error Archiving Project:{0}", e.Message);
                return string.Empty;
            }
        }

        private static void RemoveMazeRecords(MazeModels mms)
        {
            List<string> guids = mms.GetGuids();

            int rowcount = 0;

            if (guids.Count != 0)
            {
                foreach (string guid in guids)
                {
                    mazeDb.DeleteMazeRecords(guid);
                }
                rowcount = mazeDb.GetMazeCounts(guids);
            }

            if (rowcount == 0)
            {
                Console.WriteLine("Deleted {0} maze records", guids.Count);
            }
            else
            {
                Console.WriteLine("Error deleting {0} maze records: Returned {1}", guids.Count, rowcount);
            }
        }

        private static string ArchiveProjectFiles(string projectname, string guid)
        {
            List<string> projectmodels = GetProjectModels(guid);

            if (projectmodels.Count != 0)
            {
                List<string> files = new List<string>
                {
                    maze_dir + DIR + projectname
                };

                foreach (string pm in projectmodels)
                {
                    files.Add(Utils.GetFileWithExtension(log_dir, pm, LOG_EXT));
                    files.Add(Utils.GetFileWithExtension(log_dir, pm, PLOT_EXT));
                    files.Add(Utils.GetFileWithExtension(model_dir, pm, CONFIG_EXT));
                    files.Add(Utils.GetFileWithExtension(model_dir, pm, MODELS_EXT));
                }

                string archive_name = Utils.GetFileWithExtension(archive_dir, Utils.GetDateTime_Formatted(), ARCHIVE_EXT);

                Console.WriteLine("Archiving {0} Files to {1}", files.Count, archive_name);

                if (!FileIO.CreateZipArchive(archive_name, files, out string result))
                {
                    throw new Exception(result);
                }

                sb.Clear();
                sb.Append(result);
                sb.Append(Environment.NewLine);
                sb.Append(RemoveProjectFiles(files));
                sb.Append(Environment.NewLine);
                sb.Append(RemoveProjectRecord(guid));

                return sb.ToString();
            }
            return string.Empty;
        }

        private static string RemoveProjectFiles(List<string> projectfiles)
        {
            int removed_count = FileIO.DeleteFiles(projectfiles);
            int count = projectfiles.Count;

            return removed_count != count 
                ? string.Format("Error Removing {0} of {1} Project Files", count - removed_count, count) 
                : string.Format("Removed {0} Project Files", removed_count);
        }

        private static string RemoveProjectRecord(string guid)
        {
            mazeDb.DeleteProjectRecords(guid);
            int removed_count = mazeDb.GetProjectCounts(guid);

            return removed_count != 0
                ? "Error Removing 1 Project Record"
                : "Removed 1 Project Record";
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

        private static byte[,] ConvertArray(IReadOnlyList<byte[]> ibytes)
        {
            int length = ibytes.Count;
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
