#region Using Statements

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Linq;
using System.Xml.XPath;
using MouseAI.BL;
using MouseAI.ML;
using MouseAI.PL;
using ScottPlot;

#endregion

namespace MouseAI
{
    public class Maze : MazeGenerator
    {
        #region Declarations

        // Objects
        private NeuralNet neuralNet;
        private static byte[,] mazedata;
        private static MazeObject[,] mazeObjects;
        private static MazeModels mazeModels;
        private static MazeModel mazeModel;
        private static List<MazeObject> pathObjects;
        private List<MazeObject> searchObjects;
        private static MazeObject mouseObject;
        private static MazePaths mazePaths;
        private readonly List<MazeObject>[] scanObjects;// = new List<MazeObject>[4];
        private Config config;
        private static MazeDb mazeDb;
        private static DbTable_Projects dbtblProjects;
        private static Random r;
        private static StringBuilder sb;
        private readonly MazeObjects segmentPathObjects;
        private MazeStatistic mazeStatistic;
        private readonly List<PathNode> pathNodes;
        private readonly List<PathNode> badNodes;
        private static List<MazeObject> visionObjects;
        private static Bitmap visualbmp;
        private List<byte[]> imagebytes;
        private List<byte[]> imagebytes_last;

        // Constants
        private const int SMELL_DISTANCE = 10;
        private const string MAZE_DIR = "mazes";
        private const string MAZE_EXT = "mze";
        private const string LOG_DIR = "logs";
        private const string LOG_EXT = "csv";
        private const string MODELS_DIR = "models";
        private const string MODELS_EXT = "h5";
        private const string CONFIG_EXT = "xml";
        private const string BACKUP_DIR = "backup";
        private const string PLOT_EXT = "png";
        private const char LOG_DELIMIT = ',';
        private const int PLOT_COLUMN_X = 0;
        private const int PLOT_WIDTH = 300;
        private const int PLOT_HEIGHT = 300;
        private const string DIR = @"\";
        public const int BLACK = 0x00;
        public const int WHITE = 0xff;
        private const int INVALID = -1;

        // Vars
        private static int maze_width;
        private static int maze_height;
        private static int mouse_x;
        private static int mouse_y;
        private static DIRECTION mouse_direction;
        private static int cheese_x;
        private static int cheese_y;
        private bool isCheesePath;
        private bool isSmellPath;
        private static bool isDebug;
        private static string model_dir;
        private static string log_dir;
        private static string maze_dir;
        private static string backup_dir;
        private string FileName;
        private string modelProjectGuid;

        // Plotting
        private static readonly string[] LOG_COLUMN_VALUES = { "epoch", "accuracy", "loss", "val_accuracy", "val_loss" };
        private static readonly string[] IGNORE_VALUES = { "Config", "Nodes", "DropOut", "int", "double", "Model", "Guid", "StartTime" };
        private static readonly int[] PLOT_COLUMNS_Y = { 1, 3 };
        private static readonly Color[] PLOT_COLORS_Y = { Color.Empty, Color.Green, Color.Blue, Color.Green, Color.Blue };

        // Neural Model Run Related
        private int segment_current;
        private int lastNode;
        private int segmentCountLast;
        private int moveCount;
        private bool isRandomSearch;
        private bool isFirstTime;
        private static string model_info;

        // Graphic object display related
        private readonly List<Point> pnVisible;
        private readonly List<Point> pnDeadends;
        private readonly List<Point> pnSmell;
        public enum RUN_VISIBLE
        {
            NONE,
            VISIBLE,
            NEURAL,
            PATHS
        }

        #endregion

        #region Initialization

        // Constructor
        public Maze(int _maze_width, int _maze_height) : base(_maze_width, _maze_height)
        {
            maze_width = _maze_width;
            maze_height = _maze_height;

            mazedata = new byte[maze_width, maze_height];
            mazeObjects = new MazeObject[maze_width, maze_height];
            mazeModels = new MazeModels();
            mazeModel = new MazeModel();
            mazeDb = new MazeDb();
            r = new Random();
            sb = new StringBuilder();
            pathObjects = new List<MazeObject>();
            visionObjects = new List<MazeObject>();
            mazePaths = new MazePaths(maze_width, maze_height);
            pathNodes = new List<PathNode>();
            badNodes = new List<PathNode>();
            segmentPathObjects = new MazeObjects();
            pnVisible = new List<Point>();
            pnDeadends = new List<Point>();
            pnSmell = new List<Point>();

            scanObjects = new List<MazeObject>[4];
            for (int i = 0; i < scanObjects.Length; i++)
            {
                scanObjects[i] = new List<MazeObject>();
            }

            string appdir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            maze_dir = appdir + DIR + MAZE_DIR;
            log_dir = appdir + DIR + LOG_DIR;
            model_dir = appdir + DIR + MODELS_DIR;
            backup_dir = appdir + DIR + BACKUP_DIR;

            if (!FileIO.CheckCreateDirectory(maze_dir))
                throw new Exception("Could not create maze directory");
            if (!FileIO.CheckCreateDirectory(log_dir))
                throw new Exception("Could not create log directory");
            if (!FileIO.CheckCreateDirectory(model_dir))
                throw new Exception("Could not create log directory");
            if (!FileIO.CheckCreateDirectory(backup_dir))
                throw new Exception("Could not create archive directory");
        }

        // Add maze model instance
        public void AddMazeModel()
        {
            mazeModels.Add(new MazeModel(maze_width, maze_height, mouse_x, mouse_y, cheese_x, cheese_y, mazedata));
        }

        // Insert mouse and cheese characters at maze model coordinates
        public bool AddCharacters()
        {
            if (mazeModel == null)
                return false;

            pathObjects.Clear();

            int mx = mazeModel.mouse_x;
            int my = mazeModel.mouse_y;
            int cx = mazeModel.cheese_x;
            int cy = mazeModel.cheese_y;

            if (!IsInBounds(mx, my) || !IsInBounds(cx, cy))
                return false;

            mouse_x = mx;
            mouse_y = my;
            mouse_direction = DIRECTION.SOUTH;
            mazeObjects[mx, my].object_state = OBJECT_STATE.MOUSE;

            mouseObject = new MazeObject(OBJECT_TYPE.SPACE, mx, my)
            {
                object_state = OBJECT_STATE.MOUSE,
                isVisited = true,
                dtLastVisit = DateTime.UtcNow
            };

            mazeObjects[cx, cy].object_state = OBJECT_STATE.CHEESE;
            mazeObjects[cx, cy].object_type = OBJECT_TYPE.SPACE;
            cheese_x = cx;
            cheese_y = cy;

            return true;
        }

        // Initialize smell nodes extending from cheese coordinates to SMELL_DISTANCE
        public void InitSmell()
        {
            List<MazeObject> nodes = new List<MazeObject>();
            List<MazeObject> newnodes = new List<MazeObject>();
            MazeObject node;
            int[] pan_array;
            int count = 1;
            int lastnode = 0;
            int x, y;

            nodes.Add(mazeObjects[cheese_x, cheese_y]);

            while (count < SMELL_DISTANCE)
            {
                for (int i = lastnode; i < nodes.Count; i++)
                {
                    node = nodes[i];
                    pan_array = GetXYPanArray(node.x, node.y);

                    for (int j = 0; j < pan_array.Length; j += 2)
                    {
                        x = pan_array[j];
                        y = pan_array[j + 1];
                        if (IsInBounds(x, y) && GetObjectDataType(x, y) == OBJECT_TYPE.SPACE &&
                            !nodes.Any(o => o.x == x && o.y == y) && 
                            !newnodes.Any(o => o.x == x && o.y == y))
                        {
                            mazeObjects[x, y].smell_level = count;
                            newnodes.Add(mazeObjects[x, y]);
                        }
                    }
                }
                count++;
                lastnode = nodes.Count - 1;
                nodes.AddRange(newnodes);
                newnodes.Clear();
            }
            nodes.RemoveAt(0);
            pnSmell.Clear();

            foreach (MazeObject mo in nodes)
            {
                pnSmell.Add(new Point(mo.x, mo.y));
            }
        }

        // Insert the mouse and cheese characters at random coordinates and opposing sides of a given maze 
        public bool AddCharacters_Random()
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
            mouse_direction = DIRECTION.SOUTH;
            mazeObjects[x, y].object_state = OBJECT_STATE.MOUSE;

            mouseObject = new MazeObject(OBJECT_TYPE.BLOCK, x, y)
            {
                object_state = OBJECT_STATE.MOUSE,
                object_type = OBJECT_TYPE.SPACE,
                isVisited = true,
                dtLastVisit = DateTime.UtcNow
            };

            x = mo_cheese.x;
            y = mo_cheese.y;

            mazeObjects[x, y].object_state = OBJECT_STATE.CHEESE;
            mazeObjects[x, y].object_type = OBJECT_TYPE.SPACE;
            cheese_x = x;
            cheese_y = y;

            return true;
        }

        // Random maze generation function
        public void GenerateMazes()
        {
            GeneratorReset();
            Generate();

            for (int y = 0; y < maze_height; y++)
            {
                for (int x = 0; x < maze_width; x++)
                {
                    mazedata[x, y] = GetObjectByte(x, y);
                    mazeObjects[x, y] = new MazeObject(GetObjectDataType(x, y), x, y);
                }
            }

            isCheesePath = false;
            isSmellPath = false;
        }

        #endregion

        #region Neural Network Training

        // Perform neural net model training:
        // Create a neural model with the given config parameters
        // Train with a data set initialized from the current projects maze model image segments
        public void Train(string guid)
        {
            if (config == null)
                throw new Exception("Invalid Config!");

            if (mazeModels == null || mazeModels.Count() == 0)
                throw new Exception("No MazeModels Found");

            if (!mazeModels.CheckPaths() || mazeModels.GetSegmentCount() == 0)
            {
                throw new Exception("The path/segment data for model was not found");
            }

            config.Guid = guid;
            neuralNet = new NeuralNet(maze_width, maze_height, log_dir, LOG_EXT, model_dir, MODELS_EXT, CONFIG_EXT);
            neuralNet.InitDataSets(mazeModels.GetImageDatas(), config.Split, config.Seed);
            neuralNet.Process(config, mazeModels.Count());
        }

        // Load a trained neural net model from file 
        public void LoadModel(string modelName)
        {
            if (string.IsNullOrEmpty(modelName))
                throw new Exception("Invalid Model Name!");

            neuralNet = new NeuralNet(maze_width, maze_height, log_dir, LOG_EXT, model_dir, MODELS_EXT, CONFIG_EXT);
            neuralNet.LoadModel(modelName);
        }

        // Save a trained neural model:
        // Create and insert SQLite db record
        // Save model files and plot image
        public void SaveResults()
        {
            dbtblProjects = new DbTable_Projects
            {
                Guid = GetModelProjectGuid(),
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

        // Misc Get/Sets and helpers

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

        // Perform neural net predictions using a model config from file:
        // Initialize a dataset from the current projects image segments
        // Perform predictions with loaded model
        public ImageDatas Predict(string starttime)
        {
            if (string.IsNullOrEmpty(starttime))
                throw new Exception("Invalid Prediction Model");

            Config cfg = (Config) FileIO.DeSerializeXml(typeof(Config),
                Utils.GetFileWithExtension(model_dir, starttime, CONFIG_EXT));

            if (cfg == null || string.IsNullOrEmpty(cfg.Model))
                throw new Exception("Could not open config file");

            ImageDatas imageSegments = mazeModels.GetImageSegments();
            neuralNet.InitDataSets(imageSegments);

            return neuralNet.Predict(cfg.isCNN);
        }

        // Iterate and calculate the prediction errors per maze model:
        // Increment errors if Guid label is incorrect
        public bool CalculateAccuracies(ImageDatas imageSegments)
        {
            if (imageSegments == null || imageSegments.Count == 0 || mazeModels == null || mazeModels.Count() == 0)
                return false;

            foreach (MazeModel mm in mazeModels.GetMazeModels())
            {
                mm.errors = imageSegments.Count(o => o.Label.Equals(mm.guid, StringComparison.OrdinalIgnoreCase));
            }

            return true;
        }

        #endregion

        #region Neural Network Run Movement

        // Initialize for a mouse to cheese run session:
        // Clear lists, init values and statistics
        // Init dataset for neural net from current projects image segments
        public void InitRunMove(bool isRandom)
        {
            if (!mazeModels.CheckPathNodes())
                throw new Exception("Invalid maze model path nodes!");

            Console.Clear();
            pathNodes.Clear();
            pathObjects.Clear();
            badNodes.Clear();
            segmentPathObjects.Clear();
            segment_current = INVALID;
            segmentCountLast = lastNode = moveCount = 0;
            isRandomSearch = isRandom;
            isFirstTime = true;
            mazeStatistic = null;
            mazeStatistic = new MazeStatistic(mazeModel.guid, neuralNet.GetLogName());
            neuralNet.InitDataSets(mazeModels.GetImageSegments());
        }

        // Calculate a single move action within a maze traversal session
        public bool ProcessRunMove(bool isdebug)
        {
            isDebug = isdebug;

            if (isDebug)
                Console.WriteLine("*** Processing Run Move ***");

            int x = mouseObject.x;
            int y = mouseObject.y;
            moveCount++;

            // Update maze statistics
            mazeStatistic.SetPredictedLabels(neuralNet.GetPredicted());
            mazeStatistic.SetPredictedErrors(neuralNet.GetPredictedErrors());

            // Object scan: Scan if mouse can see the cheese (if it hasn't been seen yet)
            // If seen: override smell mode - set cheese path mode
            if (!isCheesePath && ScanObjects(x, y, scanObjects))
            {
                isSmellPath = false;
                isCheesePath = true;
            }

            // Store visible deadend and non deadend objects from the mouse location
            // Store current mouse position object from list
            List<MazeObject> mazeobjects_de = CheckNode(x, y, true);
            List<MazeObject> mazeobjects = mazeobjects_de.Where(o => !o.isDeadEnd).ToList();
            MazeObject mouse = mazeobjects.FirstOrDefault(o => o.object_state == OBJECT_STATE.MOUSE);

            // Validate the mouse is present!
            if (mouse == null)
            {
                throw new Exception("Mouse Object Null!");
            }

            // Check if the mouse has reached the cheese or can see or smell the cheese
            // Return true on reached
            if (CheckForCheese(mouse, mazeobjects))
                return true;

            // Override neural move processing if seen or smelled 
            if (isCheesePath || isSmellPath)
            {
                UpdateMouseDirection(x, y, mouseObject.x, mouseObject.y);
                return false;
            }

            mouse.count++;

            if (isDebug)
                Console.WriteLine("LastNode:{0} pathNodes: {1}", lastNode, pathNodes.Count);

            if (!isFirstTime) // Skip first iteration to ensure an image for neural prediction is generated
            {
                // Process the mouses neural vision state:
                // On false: Process a tree search move
                // return if also false as the mouse is reverting a dead end    
                if (!ProcessVisionState(mouse, mazeobjects) &&
                    !ProcessSearchMove(mazeobjects, mazeobjects_de, mouse))
                {
                    UpdateMouseDirection(x, y, mouseObject.x, mouseObject.y);
                    return false;
                }

                // We have a valid neural vision state:
                // Process the mouses recalled neural vision path
                // return if false as the mouse is still following a path memory 
                if (ProcessNeuralVisionPath(mazeobjects, mazeobjects_de, mouse))
                {
                    mazeStatistic.IncrementNeuralMoves();
                    MazeStatistics.SetMouseStatus(MazeStatistics.MOUSE_STATUS.RECALLING);
                    UpdateMouseDirection(x, y, mouseObject.x, mouseObject.y);
                    return false;
                }
            }
            else
                isFirstTime = false;
            
            // Create an image for neural prediction which represents the mouses visible path   
            isFirstTime = false;
            CreateVisionImage(mouse);
            InvalidateVisionState();
            UpdateMouseDirection(x, y, mouseObject.x, mouseObject.y);
            return false;
        }

        #endregion

        #region Cheese Movement

        // Return true if the mouse has reached the cheese, otherwise process cheese path and smell detection movements
        private bool CheckForCheese(MazeObject mouse, IReadOnlyCollection<MazeObject> mazeobjects)
        {
            if (isDebug)
                Console.WriteLine("Mouse smell cheese: {0} see cheese: {1}", isSmellPath, isCheesePath);

            // If mouse is at cheese: flag for end goal of maze traversal
            if (isMouseAtCheese(mouse))
            {
                MazeStatistics.SetMouseStatus(MazeStatistics.MOUSE_STATUS.DONE);
                return true;
            }

            // If true: move mouse towards the cheese
            if (isCheesePath)
            {
                MazeStatistics.SetMouseStatus(MazeStatistics.MOUSE_STATUS.FOUND);
                if (ProcessCheeseMove(mouse, mazeobjects))
                    CleanPathObjects();
            }
            else
            {
                // If no path yet: check if mouse can smell the cheese
                if (!isSmellPath)
                    isSmellPath = (mouse.smell_level != 0);

                // If true: move mouse towards the cheese
                if (isSmellPath)
                {
                    MazeStatistics.SetMouseStatus(MazeStatistics.MOUSE_STATUS.SMELLED);
                    if (ProcessSmellMove(mouse, mazeobjects))
                        CleanPathObjects();
                }
            }
            return false;
        }

        // Mouse movement follows along a straight line visible path that leads to the cheese
        private bool ProcessCheeseMove(MazeObject mouse, IEnumerable<MazeObject> mazeobjects)
        {
            int curr_x = mouse.x;
            int curr_y = mouse.y;
            int new_x = -1;
            int new_y = -1;

            if (isMouseAtCheese(mouse))
            {
                MazeObject m = mazeObjects[curr_x, curr_y];
                m.object_state = OBJECT_STATE.CHEESE;
                m.dtLastVisit = DateTime.UtcNow;
                pathObjects.Add(m);
                return true;
            }

            if (curr_x == cheese_x) // North or south
            {
                new_x = curr_x;
                new_y = (curr_y > cheese_y) ? curr_y - 1 : curr_y + 1;
            }
            else if (curr_y == cheese_y) // West or East
            {
                new_y = curr_y;
                new_x = (curr_x > cheese_x) ? curr_x - 1 : curr_x + 1;
            }

            if (new_x == -1 || new_y == -1 || !SetPathMove(curr_x, curr_y, new_x, new_y))
                throw new Exception("Invalid Cheese Path Movement!");

            MazeObject mo = mazeobjects.FirstOrDefault(o => o.x == curr_x && o.y == curr_y);
            if (mo != null)
            {
                mo.isCheesePath = true;
                pathObjects.Add(mo);
                segmentPathObjects.Add(mo);
            }

            return false;
        }

        // Mouse movement follows along a smell path of descending levels that leads to the cheese
        private bool ProcessSmellMove(MazeObject mouse, IReadOnlyCollection<MazeObject> mazeobjects)
        {
            int curr_x = mouse.x;
            int curr_y = mouse.y;

            if (isMouseAtCheese(mouse))
            {
                MazeObject m = mazeObjects[curr_x, curr_y];
                m.object_state = OBJECT_STATE.CHEESE;
                m.dtLastVisit = DateTime.UtcNow;
                pathObjects.Add(m);
                return true;
            }

            int current_level = mouse.smell_level;
            MazeObject mo = null;
            int[] pan_array = GetXYPanArray(curr_x, curr_y);

            for (int i = 0; i < pan_array.Length; i += 2)
            {
                mo = mazeobjects.FirstOrDefault(o => o.x == pan_array[i] && o.y == pan_array[i + 1] 
                                                                         && o.smell_level < current_level 
                                                                         && o.smell_level != 0);
                if (mo != null)
                    break;
            }

            if (mo == null || !SetPathMove(curr_x, curr_y, mo.x, mo.y))
                throw new Exception("Invalid Smell Path Movement!");

            mo.isCheesePath = true;
            pathObjects.Add(mo);
            segmentPathObjects.Add(mo);
            
            return false;
        }

        #endregion

        #region Neural Vision Movement

        // Process a mouse path from a given neural vision and the resultant node memory
        private bool ProcessVisionState(MazeObject mouse, IReadOnlyCollection<MazeObject> mazeobjects)
        {
            // Perform any path pruning at a junction
            if (mazeobjects.Count >= 4)
            {
                mouse.isJunction = true;
                CleanPathObjects();
            }

            // If the mouse is currently moving along a path or there is no vision segment to process
            if (lastNode < pathNodes.Count - 1 || segment_current == INVALID) 
                return true;

            // Get a path node memory from the neural prediction 
            List<PathNode> pathnodes = mazeModels.GetPathNodes(segment_current);

            if (pathnodes == null || pathnodes.Count == 0)
            {
                throw new Exception("Invalid Vision Path Nodes!");
            }

            if (isDebug)
                Console.WriteLine("Processing neural vision state from memory index: {0}", segment_current);

            int count = 0;
            int nonodes = 0;
            int start = pathnodes.FindIndex(o => isMouseNode(mouse, o));

            // Get mouse index
            if (start != -1)
            {
                // Iterate nodes >= mouse and valid
                for (int i = start; i < pathnodes.Count; i++)
                {
                    if (CheckNode(pathnodes[i]))
                    {
                        pathNodes.Add(pathnodes[i]);
                        count++;
                    }
                    // Stop the path search threshold
                    else if (nonodes++ > 5) 
                        break;
                }
            }
            // Return if the mouse was found and any neural vision nodes have been added to the list
            return (start != -1 && count != 0);
        }

        // Validate nodes: check bad, duplicate and within vision sight
        private bool CheckNode(PathNode p)
        {
            return !badNodes.Any(o => o.x == p.x && o.y == p.y) &&
                   !pathNodes.Any(o => o.x == p.x && o.y == p.y) &&
                   visionObjects.Any(o => o.x == p.x && o.y == p.y);
        }

        // 
        private bool ProcessNeuralVisionPath(IReadOnlyCollection<MazeObject> mazeobjects,
                        IReadOnlyCollection<MazeObject> mazeobjects_de, MazeObject mouse)
        {
            // Return if no moves remain
            if (lastNode >= pathNodes.Count - 1)
                return false;

            if (isDebug)
                Console.WriteLine("Processing path node");

            MazeObject mo;

            // Iterate path nodes
            for (int i = 0; i < pathNodes.Count; i++) 
            {
                // If at the mouse and any nodes exist past the mouse
                if (isMouseNode(mouse, pathNodes[i]) && (i < pathNodes.Count - 1))
                {
                    PathNode pn = pathNodes[i + 1]; // Get next node
                    
                    // If mouse can move onto the next node: move the mouse and return result
                    if (CheckPathMove(pn.x, pn.y) && CheckNodeNext(pn, mouse) && !isVisited(pn.x, pn.y))
                    {
                        mo = mazeObjects[pn.x, pn.y];
                        UpdatePathObject(mazeobjects, mazeobjects_de, mo, mouse);
                        lastNode = i + 1;
                        InvalidateVisionState();
                        return true;
                    }

                    if (isDebug)
                        Console.WriteLine("Encountered a neural path deadend: pruning path nodes");

                    // Otherwise - invalidate segment and prune bad nodes
                    InvalidateVisionState();
                    for (int idx = pathNodes.Count - 1; idx > -1; idx--)
                    {
                        pn = pathNodes[idx];
                        mo = mazeObjects[pn.x, pn.y];

                        if (!mo.isJunction)
                        {
                            if (!badNodes.Any(o => o.x == mo.x && o.y == mo.y))
                                badNodes.Add(pn);

                            pathNodes.RemoveAt(idx);
                        }
                    }
                    break;
                }
            }
            // Update node counter and return result
            lastNode = (pathNodes.Count - 1 < 0) ? 0 : pathNodes.Count - 1;
            return false;
        }

        // Create a image from the visible path of the mouse 
        private void CreateVisionImage(MazeObject mouse) 
        {
            if (isDebug)
                Console.WriteLine("Creating vision image");

            // If no segments - add the mouse
            if (segmentPathObjects.Count == 0)
            {
                segmentPathObjects.Add(mouse);
            }
            // Otherwise - clear list
            else if (moveCount > 1 && mouse.x == mouse_x && mouse.y == mouse_y)
            {
                segmentPathObjects.Clear();
            }

            // Prepare lists and prune any deadends
            PrepareVisionImage(mouse);

            // If new paths: create an image from the given path
            if (segmentPathObjects.Count != segmentCountLast)
            {
                visionObjects.Clear();
                imagebytes.Clear();
                imagebytes_last.Clear();
                visionObjects.AddRange(segmentPathObjects);
                imagebytes.Add(GenerateVisualImage(segmentPathObjects));
                imagebytes_last.AddRange(imagebytes);
                segmentCountLast = segmentPathObjects.Count;
            }
            // Otherwise: restore from buffer
            else
            {
                imagebytes.Clear();
                imagebytes.AddRange(imagebytes_last);
            }
        }

        // Prepares vision segment object paths and imagebyte lists
        private void PrepareVisionImage(MazeObject mouse)
        {
            searchObjects = SearchObjects(mouse.x, mouse.y).Distinct().ToList();
            segmentPathObjects.AddRange(searchObjects.Except(segmentPathObjects));

            if (imagebytes == null)
            {
                imagebytes = new List<byte[]>();
                imagebytes_last = new List<byte[]>();
            }

            if (mouse.count > 1)
            {
                for (int i = 0; i < segmentPathObjects.Count; i++)
                {
                    if (segmentPathObjects[i].object_state == OBJECT_STATE.MOUSE)
                    {
                        if (isDebug)
                            Console.WriteLine("Pruning a dead end from search tree");

                        segmentPathObjects.RemoveRange(i + 1, segmentPathObjects.Count - 1 - i);
                        break;
                    }
                }
            }
        }

        // Generates and returns a bitmapped segment image of the given path objects
        private static byte[] GenerateVisualImage(MazeObjects mos)
        {
            if (visualbmp == null)
            {
                visualbmp = new Bitmap(maze_width, maze_height);
            }

            Graphics g = Graphics.FromImage(visualbmp);
            g.Clear(GetColor(WHITE));

            foreach (MazeObject mo in mos)
            {
                mo.isVision = true;
                (visualbmp).SetPixel(mo.x, mo.y, GetColor(BLACK));
            }

            MemoryStream memoryStream = new MemoryStream();
            visualbmp.Save(memoryStream, ImageFormat.Bmp);
            byte[] bytes = memoryStream.ToArray();
            memoryStream.Close();

            return bytes;
        }

        // Performs neural prediction on the mouses visible path
        // Stores the resultant segment index within the dataset
        public void ProcessVisionImage()
        {
            if (imagebytes == null || imagebytes.Count == 0)
            {
                return;
            }

            segment_current = neuralNet.Predict(imagebytes, mazeModel.guid, isDebug);
            imagebytes.Clear();

            if (isDebug)
                Console.WriteLine("Processed Vision Result: {0}", segment_current);
        }

        // Reset vision segment state
        private void InvalidateVisionState()
        {
            segment_current = INVALID;

            if (isDebug)
                Console.WriteLine("Processed and Updated Vision");
        }

        #endregion

        #region Tree Search Movement

        // Non neural mouse movement via a tree search algorithm:
        // The mouse is lost in an unrecognized portion of the maze due to neural prediction errors
        // Now the mouse needs to choose an optimal or random move (methods are shared with maze path solving)
        private bool ProcessSearchMove(IReadOnlyCollection<MazeObject> mazeobjects,
                     IReadOnlyCollection<MazeObject> mazeobjects_de, MazeObject mouse)
        {
            // Init array and lists
            int[] pan_array = GetXYPanArray(mouse.x, mouse.y);
            List<MazeObject> mos = mazeobjects.Where(o => o.isVisited == false && o.object_state != OBJECT_STATE.MOUSE)
                .ToList();
            List<MazeObject> mos_selections = new List<MazeObject>();
            MazeObject mo = null;

            // Get a first possible move or random moves 
            if (mos.Count != 0)
            {
                for (int i = 0; i < pan_array.Length; i += 2)
                {
                    mo = mos.FirstOrDefault(o => o.x == pan_array[i] && o.y == pan_array[i + 1]);
                    if (mo != null)
                    {
                        if (isRandomSearch)
                            mos_selections.Add(mo);
                        else
                            break;
                    }
                }
            }
            // If random - select a move
            if (isRandomSearch && mos_selections.Count != 0)
            {
                mo = mos_selections[r.Next(0, mos_selections.Count)];
            }

            // If valid: mouse moves and searches along the path
            if (mo != null)
            {
                UpdatePathObject(mazeobjects, mazeobjects_de, mo, mouse);
                mazeStatistic.IncrementSearchMoves();
                MazeStatistics.SetMouseStatus(MazeStatistics.MOUSE_STATUS.SEARCHING);
                return true;
            }

            // Otherwise: No valid moves as the mouse is at a deadend: mouse reverts along a path
            ProcessOldestPath(mazeobjects, mouse);
            if (mouse.isDeadEnd && !badNodes.Any(o => isMouseNode(mouse, o)))
            {
                badNodes.Add(new PathNode(mouse.x, mouse.y, false));
            }
            MazeStatistics.SetMouseStatus(MazeStatistics.MOUSE_STATUS.REVERTING);
            return false;
        }

        // Mouse movement processing for path solving a new maze
        public bool ProcessMouseMove()
        {
            int x = mouseObject.x;
            int y = mouseObject.y;

            // Check if mouse can see the cheese
            if (!isCheesePath)
                isCheesePath = ScanObjects(x, y, scanObjects);

            // Retrieve visible objects and the mouse
            List<MazeObject> mazeobjects = CheckNode(x, y, false);
            MazeObject mouse = mazeobjects.FirstOrDefault(o => o.object_state == OBJECT_STATE.MOUSE);

            if (mouse == null)
            {
                throw new Exception("Mouse Object Null!");
            }

            // If moving towards the cheese: override search movement
            if (isCheesePath)
            {
                // Return true if cheese reached
                if (ProcessCheeseMove(mouse, mazeobjects))
                {
                    CleanPathObjects();
                    FinalizePathObjects();
                    return true;
                }
                return false;
            }

            // Get possible moves
            MazeObject mo = mazeobjects.FirstOrDefault(o => o.isVisited == false &&
                                                            o.object_state != OBJECT_STATE.MOUSE);
            // If there is a valid move to be made
            if (mo != null)
                UpdatePathObject(mazeobjects, null, mo, mouse);
            // Mouse is in deadend: revert the path
            else
                ProcessOldestPath(mazeobjects, mouse);

            return false;
        }

        // Move mouse along a searched path
        private void UpdatePathObject(IReadOnlyCollection<MazeObject> mazeobjects,
            IReadOnlyCollection<MazeObject> mazeobjects_de, MazeObject mo, MazeObject mouse)
        {
            if (mazeobjects.Count >= 4)
            {
                mouse.isJunction = true;
                CleanPathObjects();
            }

            else if (mazeobjects_de != null && !mouse.isDeadEnd)
            {
                mouse.isDeadEnd = (mazeobjects.Count == 2 &&
                                   (mazeobjects_de.Count - mazeobjects.Count == 1 ||
                                    mazeobjects_de.Count - mazeobjects.Count == 2));
            }

            mouseObject.x = mo.x;
            mouseObject.y = mo.y;
            mo.object_state = OBJECT_STATE.MOUSE;
            mo.dtLastVisit = DateTime.UtcNow;
            mo.isPath = true;
            mouse.isVisited = true;
            mouse.object_state = OBJECT_STATE.VISITED;
            pathObjects.Add(mo);
        }

        // Reverts mouse out of a deadend until a non-visited path is reached
        // Uses a succession of earlier datetimes for path calculations
        private static void ProcessOldestPath(IEnumerable<MazeObject> mazeobjects, MazeObject mouse)
        {
            DateTime dtOldest = DateTime.UtcNow;
            MazeObject mo_oldest = null;

            foreach (MazeObject m in mazeobjects.Where(m => m.object_state != OBJECT_STATE.MOUSE))
            {
                if (m.dtLastVisit < dtOldest)
                {
                    mo_oldest = m;
                    dtOldest = mo_oldest.dtLastVisit;
                }
            }

            if (mo_oldest == null)
                throw new Exception("Maze object was null!");

            mouseObject.x = mo_oldest.x;
            mouseObject.y = mo_oldest.y;

            if (!mouse.isJunction)
                mouse.isDeadEnd = true;

            mazeObjects[mo_oldest.x, mo_oldest.y].object_state = OBJECT_STATE.MOUSE;
            mouse.isVisited = true;
            mouse.object_state = OBJECT_STATE.VISITED;
        }

        #endregion

        #region Scanning

        // Mouse line of sight ray casting from its current position in four directions:
        // If cheese is seen: override for mouse -> cheese movement
        // Otherwise: scan for any ambigous turns/intersections and unambigous deadends
        private static bool ScanObjects(int x, int y, IReadOnlyList<List<MazeObject>> scanobjects)
        {
            bool? result;

            // West
            for (int x_idx = x - 1; x_idx > 0; x_idx--)
            {
                result = ScanDirection(x_idx, y);
                if (result == null)
                    break;
                if (result == true)
                    return true;
                scanobjects[(int)DIRECTION.WEST].Add(mazeObjects[x_idx, y]);
            }

            // East
            for (int x_idx = x + 1; x_idx < maze_width; x_idx++)
            {
                result = ScanDirection(x_idx, y);
                if (result == null)
                    break;
                if (result == true)
                    return true;
                scanobjects[(int)DIRECTION.EAST].Add(mazeObjects[x_idx, y]);
            }

            // North
            for (int y_idx = y - 1; y_idx > 0; y_idx--)
            {
                result = ScanDirection(x, y_idx);
                if (result == null)
                    break;
                if (result == true)
                    return true;
                scanobjects[(int)DIRECTION.NORTH].Add(mazeObjects[x, y_idx]);
            }

            // South
            for (int y_idx = y + 1; y_idx < maze_height; y_idx++)
            {
                result = ScanDirection(x, y_idx);
                if (result == null)
                    break;
                if (result == true)
                    return true;
                scanobjects[(int)DIRECTION.SOUTH].Add(mazeObjects[x, y_idx]);
            }

            // Scan for endpoints and perimeters per visible object
            for (int i = 0; i < scanobjects.Count; i++)
            {
                CheckDeadEnds(scanobjects[i]);
                scanobjects[i].Clear();
            }

            return false;
        }

        // Tristate return: null if not valid or a deadend, true if cheese, false if a space
        private static bool? ScanDirection(int x, int y)
        {
            if (!isScanValid(x, y) || mazeObjects[x, y].isDeadEnd)
                return null;

            return CheckScannedObject(x, y) == OBJECT_STATE.CHEESE;
        }

        // Check a given path object list for a deadend 
        private static void CheckDeadEnds(IList<MazeObject> mazeObjects)
        {
            // Return if none
            if (mazeObjects.Count == 0)
                return;

            // If there are objects and the last point is a dead end
            if (GetPerimiter(mazeObjects.Last()) == 1)
            {
                mazeObjects.Last().isDeadEnd = true;
                mazeObjects.RemoveAt(mazeObjects.Count - 1);

                // Iterate reverse and flag deadends up to any junction
                for (int i = mazeObjects.Count - 1; i > -1; i--)
                {
                    if (GetPerimiter(mazeObjects[i]) == 2)
                        mazeObjects[i].isDeadEnd = true;
                    else
                        break;
                }
            }
        }

        // Returns count of perimeter spaces from a given node 
        private static int GetPerimiter(MazeObject mazeObject)
        {
            int count = 0;
            int[,] panArray = GetXYPan(mazeObject.x, mazeObject.y);

            for (int i = 0; i < panArray.Length / 2; i++)
            {
                count += GetScanValid(panArray[i, 0], panArray[i, 1]);
            }
            return count;
        }

        // Misc scan utilities

        private static OBJECT_STATE CheckScannedObject(int x, int y)
        {
            return mazeObjects[x, y].object_state == OBJECT_STATE.CHEESE ? OBJECT_STATE.CHEESE : OBJECT_STATE.NONE;
        }

        private static bool isScanValid(int x, int y)
        {
            return (IsInBounds(x, y) && mazeObjects[x, y].object_type == OBJECT_TYPE.SPACE);
        }

        private static int GetScanValid(int x, int y)
        {
            return (IsInBounds(x, y) && mazeObjects[x, y].object_type == OBJECT_TYPE.SPACE) ? 1 : 0;
        }

        #endregion

        #region Path Segment Building

        public void CalculateSegments()
        {
            MazeObject mouse = pathObjects.FirstOrDefault(o => o.object_state == OBJECT_STATE.MOUSE);
            MazeObject cheese = pathObjects.FirstOrDefault(o => o.object_state == OBJECT_STATE.MOUSE);

            if (mouse == null || cheese == null)
            {
                throw new Exception("Couldn't find maze object(s)!");
            }

            MazeObjects pathobjects = new MazeObjects(pathObjects.Where(o => o.isPath && !o.isDeadEnd)
                .OrderBy(d => d.dtLastVisit).ToList());

            MazeObjectSegments mazeObjectSegments = new MazeObjectSegments();

            CalculateForwardSegments(mazeObjectSegments, pathobjects);
            ValidateSegments(mazeObjectSegments);
            GenerateSegmentImages(mazeObjectSegments);
            GeneratePathNodes(pathobjects);
        }

        private static void CalculateForwardSegments(MazeObjectSegments mazeObjectSegments, MazeObjects pathobjects)
        {
            MazeObjects segmentObjects = new MazeObjects();
            MazeObject so;
            int index = 0;
            bool isFirstJunction = false;
            bool isBreak = false;
            bool isAddSegments;
            
            int count1;
            while (true)
            {
                isAddSegments = false;
                so = pathobjects[index++];

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
        }

        private static MazeObjects SearchObjects(int x, int y)
        {
            MazeObjects pathobjects = new MazeObjects();

            // Scan West
            for (int x_idx = x - 1; x_idx > 0; x_idx--)
            {
                if (!SearchObject(x_idx, y, pathobjects))
                    break;
            }

            // Scan East
            for (int x_idx = x + 1; x_idx < maze_width; x_idx++)
            {
                if (!SearchObject(x_idx, y, pathobjects))
                    break;
            }

            // Scan North
            for (int y_idx = y - 1; y_idx > 0; y_idx--)
            {
                if (!SearchObject(x, y_idx, pathobjects))
                    break;
            }

            // Scan South
            for (int y_idx = y + 1; y_idx < maze_height; y_idx++)
            {
                if (!SearchObject(x, y_idx, pathobjects))
                    break;
            }

            return pathobjects;
        }

        private static bool SearchObject(int x, int y, MazeObjects pathobjects)
        {
            if (!isScanValid(x, y))
                return false;

            pathobjects.Add(mazeObjects[x, y]);

            int[,] panArray = GetXYPan(x, y);

            // Scan all directions from a given point
            for (int i = 0; i < panArray.Length / 2; i++)
            {
                x = panArray[i, 0];
                y = panArray[i, 1];

                if (isScanValid(x, y))
                {
                    pathobjects.Add(mazeObjects[x, y]);
                }
            }

            return true;
        }

        private static void ValidateSegments(MazeObjectSegments mazeObjectSegments)
        {
            for (int idx = mazeObjectSegments.Count - 1; idx > -1; idx--)
            {
                if (mazeObjectSegments[idx].Count == 0)
                    mazeObjectSegments.RemoveAt(idx);
            }
        }

        private static void GenerateSegmentImages(MazeObjectSegments mazeObjectSegments)
        {
            Bitmap bmp = new Bitmap(maze_width, maze_height);
            Graphics g;
            MazeObjects mos;
            MemoryStream memoryStream;
            mazeModel.segments.Clear();

            for (int idx = 0; idx < mazeObjectSegments.Count; idx++)
            {
                mos = mazeObjectSegments[idx];
                g = Graphics.FromImage(bmp);
                g.Clear(GetColor(WHITE));

                foreach (MazeObject mo in mos)
                {
                    (bmp).SetPixel(mo.x, mo.y, GetColor(BLACK));
                }

                memoryStream = new MemoryStream();
                bmp.Save(memoryStream, ImageFormat.Bmp);
                mazeModel.segments.Add(memoryStream.ToArray());
                memoryStream.Close();
            }
        }

        private static void GeneratePathNodes(MazeObjects mos)
        {
            mazeModel.pathnodes = null;
            mazeModel.pathnodes = new List<PathNode>();

            foreach (MazeObject mo in mos)
            {
                mazeModel.pathnodes.Add(new PathNode(mo.x, mo.y, mo.isJunction));
            }
        }

        public List<byte[]> GetSegments()
        {
            return mazeModel.segments;
        }

        #endregion

        #region Paths

        public void CalculatePath()
        {
            pathObjects = pathObjects.OrderBy(x => x.dtLastVisit).ToList();
            MazePath mp = mazePaths.GetAddPath(mazeModel.guid);

            if (mp == null)
                throw new Exception("Maze path was null!");

            mp.bmp = new Bitmap(maze_width, maze_height);
            Graphics g = Graphics.FromImage(mp.bmp);
            g.Clear(GetColor(WHITE));

            foreach (MazeObject mo in pathObjects)
            {
                mp.mazepath[mo.y][mo.x] = BLACK;
                (mp.bmp).SetPixel(mo.x, mo.y, GetColor(BLACK));
            }
        }

        private static bool SetPathMove(int curr_x, int curr_y, int new_x, int new_y)
        {
            if (CheckPathMove(new_x, new_y))
            {
                MazeObject mo = mazeObjects[curr_x, curr_y];
                mo.object_state = OBJECT_STATE.VISITED;
                mo.dtLastVisit = DateTime.UtcNow;
                mo.isPath = true;
                mo = mazeObjects[new_x, new_y];
                mo.object_state = OBJECT_STATE.MOUSE;
                mo.dtLastVisit = DateTime.UtcNow;
                mo.isPath = true;
                mouseObject.x = new_x;
                mouseObject.y = new_y;
                return true;
            }

            return false;
        }

        private static void FinalizePathObjects()
        {
            List<MazeObject> pathobjects = new List<MazeObject>();
            List<MazeObject> pathsLast = new List<MazeObject>();

            MazeObject m = new MazeObject(OBJECT_TYPE.SPACE, mouse_x, mouse_y)
            {
                dtLastVisit = DateTime.MinValue,
                isPath = true,
                isDeadEnd = false,
                object_state = OBJECT_STATE.MOUSE
            };
            pathObjects.Insert(0, m);

            foreach (MazeObject mo in pathObjects)
            {
                if (mo.x == cheese_x && mo.y == cheese_y)
                    break;

                pathsLast.Clear();
                for (int x = mo.x - 1; x > 0; x--)
                {
                    if (!CheckPathValid(x, mo.y, pathobjects, pathsLast))
                    {
                        CheckPathTurn(pathobjects, pathsLast);
                        break;
                    }
                }

                pathsLast.Clear();
                for (int x = mo.x + 1; x < maze_width; x++)
                {
                    if (!CheckPathValid(x, mo.y, pathobjects, pathsLast))
                    {
                        CheckPathTurn(pathobjects, pathsLast);
                        break;
                    }
                }

                pathsLast.Clear();
                for (int y = mo.y - 1; y > 0; y--)
                {
                    if (!CheckPathValid(mo.x, y, pathobjects, pathsLast))
                    {
                        CheckPathTurn(pathobjects, pathsLast);
                        break;
                    }
                }

                pathsLast.Clear();
                for (int y = mo.y + 1; y < maze_height; y++)
                {
                    if (!CheckPathValid(mo.x, y, pathobjects, pathsLast))
                    {
                        CheckPathTurn(pathobjects, pathsLast);
                        break;
                    }
                }
            }

            pathObjects.AddRange(pathobjects);
            mouseObject.x = mouse_x;
            mouseObject.y = mouse_y;
        }

        private static bool CheckPathValid(int x, int y, ICollection<MazeObject> pathobjects,
            ICollection<MazeObject> pathsLast)
        {
            if (IsInBounds(x, y) && !mazeObjects[x, y].isPath &&
                mazeObjects[x, y].object_type == OBJECT_TYPE.SPACE &&
                !pathObjects.Any(o => o.x == x && o.y == y))
            {
                mazeObjects[x, y].isPath = true;
                mazeObjects[x, y].isDeadEnd = true;
                mazeObjects[x, y].dtLastVisit = DateTime.UtcNow;
                pathobjects.Add(mazeObjects[x, y]);
                pathsLast?.Add(pathobjects.Last());
                return true;
            }

            return false;
        }

        private static void CheckPathTurn(ICollection<MazeObject> pathobjects,
            IReadOnlyCollection<MazeObject> pathsLast)
        {
            if (pathsLast.Count == 0)
                return;

            int[,] panArray;

            foreach (MazeObject pathLast in pathsLast)
            {
                panArray = GetXYPan(pathLast.x, pathLast.y);

                for (int i = 0; i < panArray.Length / 2; i++)
                {
                    CheckPathValid(panArray[i, 0], panArray[i, 1], pathobjects, null);
                }
            }
        }

        // Path Tree Pruning
        private void CleanPathObjects()
        {
            for (int i = pathObjects.Count - 1; i > -1; i--)
            {
                if (pathObjects[i].isDeadEnd)
                {
                    pathObjects[i].isPath = false;
                    pathObjects.RemoveAt(i);
                }
                else if (pathObjects[i].isJunction)
                {
                    List<MazeObject> mo = CheckNode(pathObjects[i].x, pathObjects[i].y, false);

                    if (mo == null)
                        throw new Exception("Objects is null!");

                    if (mo.Count(x => x.isDeadEnd) == mo.Count - 1)
                    {
                        pathObjects[i].isDeadEnd = true;
                        pathObjects[i].isPath = false;
                        pathObjects.RemoveAt(i);
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
                MazePath mp = mazePaths.GetAddPath(mm.guid);

                if (mp?.mazepath != null && mp.bmp != null)
                {
                    mm.mazepath = (byte[][]) mp.mazepath.Clone();

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
                mp = mazePaths.GetAddPath(mm.guid);

                if (mp != null && mm.maze != null && mm.mazepath != null)
                {
                    mp.mazepath = (byte[][]) mm.mazepath.Clone();
                    ms = new MemoryStream(mm.maze);
                    mp.bmp = (Bitmap) Image.FromStream(ms);
                }
            }
        }

        public Bitmap GetPathBMP(string guid)
        {
            return mazePaths.GetAddPath(guid).bmp;
        }

        public string GetMazeModelGUID()
        {
            return mazeModel?.guid;
        }

        public void SetTested(bool isTested)
        {
            if (mazeModel == null)
                return;

            mazeModel.isPath = isTested;
        }

        private static Color GetColor(byte b)
        {
            switch (b)
            {
                case BLACK: return Color.Black;
                case WHITE: return Color.White;
                default: return Color.White;
            }
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

        public bool CheckProjectModels()
        {
            if (string.IsNullOrEmpty(mazeModels.Guid))
                return false;

            IEnumerable<object> oList = mazeDb.ReadProjectGuids(mazeModels.Guid);
            return oList != null && oList.Any();
        }

        public string GetProjectModelName()
        {
            return mazeModels.StartTime;
        }

        public static string GetModelSummary(string starttime)
        {
            string filename = Utils.GetFileWithExtension(model_dir, starttime, CONFIG_EXT);

            try
            {
                XDocument xdoc = FileIO.ReadXml(filename);
                XElement root = xdoc.Root;
                
                if (root == null)
                    throw new Exception("Failed to read XML config");

                int layers = (int) root.Element("Layers");
                int[] Nodes = root.Element("Nodes")?.Elements("int").Select(x => (int) x).ToArray();
                double[] DropOut = root.Element("DropOut")?.Elements("double").Select(x => (double)x).ToArray();
                model_info = (string) root.Element("Model");

                if (Nodes == null || DropOut == null || layers > Nodes.Length + 1 || layers > DropOut.Length + 1)
                {
                    throw new Exception("Invalid XML config values");
                }

                sb.Clear();

                foreach (XElement element in xdoc.Descendants())
                {
                    if (!IGNORE_VALUES.Contains(element.Name.ToString()))
                    {
                        sb.Append(string.Format("{0}:{1}", element.Name, element.Value) + Environment.NewLine);
                    }
                }

                sb.Append("Nodes: ");
                for (int idx = layers - 1; idx > -1; idx--)
                {
                    sb.Append(Nodes[idx] + ((idx > 0) ? ", " : ""));
                }
                sb.Append(Environment.NewLine);

                sb.Append("DropOut: ");
                for (int idx = layers - 1; idx > -1; idx--)
                {
                    sb.Append(DropOut[idx] + ((idx > 0) ? ", " : ""));
                }
                sb.Append(Environment.NewLine);

                return sb.ToString();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error reading summary: {0}", e.Message);
            }

            return null;
        }

        public static string GetModelInfo()
        {
            return FileIO.ParseJson(model_info);
        }

        #endregion

        #region Plotting

        public void SavePlot(string plotname)
        {
            string plotfile = LOG_DIR + @"\" + plotname + "." + LOG_EXT;
            List<string> plotvalues = FileIO.ReadFileAsList(plotfile);

            if (!CheckColumns(plotvalues))
                throw new Exception("Error reading log file");

            Dictionary<int, double[]> values = ProcessLines(plotvalues);

            if (values.Count < PLOT_COLUMNS_Y.Length + 1)
                throw new Exception("Error reading plot columns");

            double[] scores = neuralNet.GetAccuracies();
            if (scores == null || scores.Length != 2)
                throw new Exception("Error reading plot scores");

            var plt = new Plot(PLOT_WIDTH, PLOT_HEIGHT);

            double[] x = values[PLOT_COLUMN_X];

            foreach (var entry in values.Where(entry => PLOT_COLUMNS_Y.Contains(entry.Key)))
            {
                plt.PlotScatter(x, entry.Value, label: LOG_COLUMN_VALUES[entry.Key], color: PLOT_COLORS_Y[entry.Key],
                    markerSize: 0);
            }

            plt.Legend(fontSize: 8);
            plt.XLabel(string.Format("val_loss: {0:0.###} val_accuracy: {1:0.###}", scores[0], scores[1]), fontSize:12);
            plt.Title("Accuracy vs. Epoch");
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

            return !LOG_COLUMN_VALUES.Where((t, idx) =>
                !columns[idx].Trim().Equals(t, StringComparison.InvariantCultureIgnoreCase)).Any();
        }

        public static string GetModelPlot(string starttime)
        {
            string filename = log_dir + DIR + starttime + "." + PLOT_EXT;

            return (FileIO.CheckFileName(filename)) ? filename : string.Empty;
        }

        #endregion

        #region Statistics

        public MazeStatistic GetMazeStatistic()
        {
            return mazeStatistic;
        }

        public bool CheckMazeStatistic()
        {
            return mazeStatistic != null;
        }

        public void SetMazeStatistic(MazeStatistic ms)
        {
            mazeStatistic = ms;
        }

        public string[] GetMazeStatisticPlotColumns()
        {
            return MazeStatistics.GetPlotColumns();
        }

        public double[] GetMazeStatisticData()
        {
            return mazeStatistic?.GetData();
        }

        public string GetMazeStatisticTime()
        {
            return mazeStatistic.GetTime();
        }

        public string GetMouseStatus()
        {
            return MazeStatistics.GetMouseStatus();
        }

        public void EndStatus()
        {
            mazeStatistic.End();
        }

        #endregion

        #region Graphic Point Lists

        public void UpdatePointLists(Point mp, RUN_VISIBLE run_visible)
        {
            pnDeadends.Clear();
            pnVisible.Clear();

            if (run_visible == RUN_VISIBLE.PATHS)
            {
                foreach (MazeObject mo in mazeObjects)
                {
                    UpdatePointList(mp, mo);
                }

                return;
            }
            if (run_visible == RUN_VISIBLE.NEURAL)
            {
                foreach (MazeObject mo in segmentPathObjects)
                {
                    UpdatePointList(mp, mo);
                }
                return;
            }

            Point _mp = new Point();
            if (searchObjects != null && searchObjects.Count != 0)
            {
                foreach (MazeObject mo in searchObjects)
                {
                    _mp.X = mo.x;
                    _mp.Y = mo.y;
                    if (_mp != mp)
                        pnVisible.Add(new Point(mo.x, mo.y));
                }
            }
        }

        private void UpdatePointList(Point mp, MazeObject mo)
        {
            if (mo.isDeadEnd)
                pnDeadends.Add(new Point(mo.x, mo.y));

            else if (mo.isCheesePath || mo.isVision)
            {
                Point _mp = new Point(mo.x, mo.y);
                if (_mp != mp)
                    pnVisible.Add(new Point(mo.x, mo.y));
            }
        }

        public List<Point> GetSmellPoints()
        {
            return pnSmell;
        }

        public List<Point> GetVisiblePoints()
        {
            return pnVisible;
        }

        public List<Point> GetDeadEndPoints()
        {
            return pnDeadends;
        }

        #endregion

        #region File Related

        public string GetModelName()
        {
            return neuralNet.GetLogName();
        }

        public void SaveMazeModels(string filename)
        {
            FileIO.SerializeXml(mazeModels, filename);
            FileName = filename;
        }

        public void SaveUpdatedMazeModels(string filename)
        {
            FileIO.SerializeXml(mazeModels, filename);
        }

        public bool LoadMazeModels(string filename)
        {
            if (string.IsNullOrEmpty(filename))
                filename = FileIO.OpenFile_Dialog(maze_dir, MAZE_EXT);

            if (string.IsNullOrEmpty(filename))
                return false;

            if (!File.Exists(filename))
                throw new Exception("Error Loading File");

            MazeModels mms = (MazeModels) FileIO.DeSerializeXml(typeof(MazeModels), filename);

            if (mms == null)
                throw new Exception("Error Loading File");

            modelProjectGuid = mms.Guid;

            mazeModels.Clear();
            mazeModels = mms;

            FileName = filename;

            return true;
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
            return modelProjectGuid;
        }

        public void SetProjectLast(string guid, string starttime)
        {
            mazeDb.UpdateModelLast(guid, starttime);
        }

        public string GetLogPath(string starttime)
        {
            return string.Format(@"{0}/{1}.{2}",log_dir, starttime, LOG_EXT);
        }

        public string GetLogFileName(string starttime)
        {
            return string.Format(@"{0}.{1}", starttime, LOG_EXT);
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
                Console.WriteLine("Error reading last project model: {0}", e.Message);
                return string.Empty;
            }
        }

        public void CheckPythonPath()
        {
            NeuralNet.CheckPythonPath();
        }

        #endregion

        #region Record Removal

        public static bool RemoveProjectRecord(string starttime)
        {
            int startcount = mazeDb.GetProjectRecordCount(starttime);
            mazeDb.DeleteProjectRecord(starttime);
            int endcount = mazeDb.GetProjectRecordCount(starttime);

            Console.WriteLine("Removed {0} Project Records", startcount - endcount);
            MoveProjectFiles(starttime);

            return (startcount - endcount) == 1;
        }

        private static void MoveProjectFiles(string starttime)
        {
            List<string> files = new List<string>
            {
                Utils.GetFileWithExtension(log_dir, starttime, LOG_EXT),
                Utils.GetFileWithExtension(log_dir, starttime, PLOT_EXT),
                Utils.GetFileWithExtension(model_dir, starttime, CONFIG_EXT),
                Utils.GetFileWithExtension(model_dir, starttime, MODELS_EXT)
            };

            FileIO.MoveFiles(files, backup_dir);
        }

        #endregion

        #region Maze Object Helpers

        public List<MazeObject> CheckNode(int x, int y, bool isDeadends)
        {
            List<MazeObject> mazeobjects = new List<MazeObject>
            {
                mazeObjects[x, y]
            };

            for (int x_idx = x - 1; x_idx < x + 2; x_idx += 2)
            {
                if (isNode(x_idx, y, isDeadends))
                    mazeobjects.Add(mazeObjects[x_idx, y]);
            }

            for (int y_idx = y - 1; y_idx < y + 2; y_idx += 2)
            {
                if (isNode(x, y_idx, isDeadends))
                    mazeobjects.Add(mazeObjects[x, y_idx]);
            }

            return mazeobjects;
        }

        private static MazeObject GetMazeObject(DIRECTION dir)
        {
            List<MazeObject> mos;

            if (dir == DIRECTION.WEST)
                mos = mazeObjects.Cast<MazeObject>().Where(m => m.object_type == OBJECT_TYPE.SPACE && m.x == 1)
                    .ToList();
            else if (dir == DIRECTION.NORTH)
                mos = mazeObjects.Cast<MazeObject>().Where(m => m.object_type == OBJECT_TYPE.SPACE && m.y == 1)
                    .ToList();
            else if (dir == DIRECTION.EAST)
                mos = mazeObjects.Cast<MazeObject>()
                    .Where(m => m.object_type == OBJECT_TYPE.SPACE && m.x == maze_width - 2).ToList();
            else
                mos = mazeObjects.Cast<MazeObject>()
                    .Where(m => m.object_type == OBJECT_TYPE.SPACE && m.y == maze_height - 2).ToList();

            return mos.Count == 0 ? null : mos.ElementAt(r.Next(mos.Count - 1));
        }

        private static void UpdateMouseDirection(int x_last, int y_last, int x_curr, int y_curr)
        {
            if (x_curr < x_last)
                mouse_direction = DIRECTION.WEST;
            else if (x_curr > x_last)
                mouse_direction = DIRECTION.EAST;
            else if (y_curr < y_last)
                mouse_direction = DIRECTION.NORTH;
            else if (y_curr > y_last)
                mouse_direction = DIRECTION.SOUTH;
        }

        private static DIRECTION OppositeDirection(DIRECTION dir)
        {
            if (dir == DIRECTION.EAST)
                return DIRECTION.WEST;
            if (dir == DIRECTION.WEST)
                return DIRECTION.EAST;
            return dir == DIRECTION.NORTH ? DIRECTION.SOUTH : DIRECTION.NORTH;
        }

        public int GetMouseDirection()
        {
            return (int)mouse_direction;
        }

        private static bool isNode(int x, int y, bool isDeadEnds)
        {
            if (!isDeadEnds)
                return (IsInBounds(x, y) && GetObjectDataType(x, y) == OBJECT_TYPE.SPACE && !isDeadEnd(x, y));
            return (IsInBounds(x, y) && GetObjectDataType(x, y) == OBJECT_TYPE.SPACE);
        }

        private static int[,] GetXYPan(int x, int y)
        {
            return new[,] { { x - 1, y }, { x + 1, y }, { x, y - 1 }, { x, y + 1 } };
        }

        private static int[] GetXYPanArray(int x, int y)
        {
            return new[] { x - 1, y, x + 1, y, x, y - 1, x, y + 1 };
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

        private static bool CheckNodeNext(PathNode pn, MazeObject mouse)
        {
            int[] pan_array = GetXYPanArray(mouse.x, mouse.y);

            for (int i = 0; i < pan_array.Length; i += 2)
            {
                if (pn.x == pan_array[i] && pn.y == pan_array[i + 1])
                    return true;
            }

            return false;
        }

        private static bool isMouseNode(MazeObject mouse, PathNode pn)
        {
            return (pn.x == mouse.x && pn.y == mouse.y);
        }

        public static bool isMouseAtCheese(MazeObject mouse)
        {
            return (mouse.x == cheese_x && mouse.y == cheese_y);
        }

        public bool isMouseAtCheese()
        {
            return (mouseObject != null && mouseObject.x == cheese_x && mouseObject.y == cheese_y);
        }

        public Point GetMousePosition()
        {
            return new Point(mouseObject.x, mouseObject.y);
        }

        private static bool isDeadEnd(int x, int y)
        {
            return mazeObjects[x, y].isDeadEnd;
        }

        private static bool isVisited(int x, int y)
        {
            return mazeObjects[x, y].isVisited;
        }

        public static bool IsInBounds(int x, int y)
        {
            if (x < 0 || x >= maze_width)
                return false;

            return (y >= 0 && y < maze_height);
        }

        private static bool CheckPathMove(int x, int y)
        {
            return (IsInBounds(x, y) && GetObjectDataType(x, y) == OBJECT_TYPE.SPACE && !isDeadEnd(x, y));
        }

        public static OBJECT_TYPE GetObjectDataType(int x, int y)
        {
            return (mazedata[x, y] == WHITE) ? OBJECT_TYPE.SPACE : OBJECT_TYPE.BLOCK;
        }

        public static OBJECT_STATE GetObjectState(int x, int y)
        {
            return (mazeObjects[x, y].object_state);
        }

        public string GetFileName()
        {
            return string.IsNullOrEmpty(FileName) ? string.Empty : FileName;
        }

        #endregion

        #region Misc Tools

        public bool SelectMazeModel(int index)
        {
            if (index < 0 || index > mazeModels.Count() - 1)
                return false;

            mazeModel = mazeModels.GetMazeModel(index);

            if (mazeModel == null)
                return false;

            mazedata = ConvertArray(mazeModel.mazedata);

            for (int y = 0; y < maze_height; ++y)
            {
                for (int x = 0; x < maze_width; ++x)
                {
                    mazeObjects[x, y] = null;
                    mazeObjects[x, y] = new MazeObject(GetObjectDataType(x, y), x, y);
                }
            }

            return true;
        }

        public void MazeReset()
        {
            sb.Clear();
            pathObjects.Clear();
            mazePaths.ClearPath(mazeModel.guid);
            isCheesePath = false;
            isSmellPath = false;
        }

        public void ResetRun()
        {
            sb.Clear();
            pathObjects.Clear();
            isCheesePath = false;
            isSmellPath = false;
        }

        public int GetMazeModelSize()
        {
            return (mazeModels == null) ? 0 : mazeModels.Count();
        }

        public bool isMazeModels()
        {
            return (mazeModels != null && mazeModels.Count() > 0);
        }

        public bool CheckMazeModel()
        {
            return mazeModels != null && mazeModels.Count() > 0 && mazeModels.CheckPaths() &&
                   mazeModels.CheckSegments();
        }
    }

    #endregion

}