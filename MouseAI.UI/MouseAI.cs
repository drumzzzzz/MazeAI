#region Using Statements
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using MouseAI.BL;
using MouseAI.ML;
using SkiaSharp;
using SkiaSharp.Views.Desktop;
using ThreadState = System.Threading.ThreadState;
#endregion

namespace MouseAI.UI
{
    public partial class MouseAI : Form
    {
        #region Declarations

        // UI forms and objects
        private ModelLoad modelLoad;
        private ModelRun modelRun;
        private ModelPredict modelPredict;
        private TrainSettings trainSettings;
        private Progress progress;
        private MazeNew mazeNew;
        private Settings settings;
        private Maze maze;
        private readonly MazeSegments mazeSegments;
        private readonly MazeStatistics mazeStatistics;

        // Graphics
        private const int MAZE_WIDTH = 31;
        private const int MAZE_HEIGHT = 31;
        private const int MAZE_SCALE_WIDTH_PX = 28;
        private const int MAZE_SCALE_HEIGHT_PX = 28;
        private const int MAZE_WIDTH_PX = MAZE_WIDTH * MAZE_SCALE_WIDTH_PX;
        private const int MAZE_HEIGHT_PX = MAZE_HEIGHT * MAZE_SCALE_HEIGHT_PX;
        private const int MAZE_MARGIN_PX = 25;
        private const int BITMAP_WIDTH = 533;
        private const int BITMAP_HEIGHT = 533;
        private const int MOUSE_BITMAPS = 4;
        private const float LINE_WIDTH = 1;
        private const int SHADOW_ALPHA = 225;
        private const string TITLE = "MOUSE AI";
        private SKColor BlockColor;
        private SKColor SpaceColor;
        private SKColor ClearColor;
        private SKPaint BlockPaint;
        private SKPaint SpacePaint;
        private SKCanvas offscreen;
        private SKCanvas canvas;
        private SKSurface buffer;
        private SKSurface surface;
        private SKImage backimage;
        private SKBitmap Cheese_Bitmap;
        private SKBitmap[] Mouse_Bitmap;
        private SKBitmap Visible_Bitmap;
        private SKBitmap DeadEnd_Bitmap;
        private SKBitmap Smell_Bitmap;
        private SKPaint DropShadow;
        private SKColor DropShadowColor;
        private Point mouse_last;
        private Point mouse_current;
        private readonly Color BLOCK_COLOR = Color.DarkBlue;
        private readonly Color SPACE_COLOR = Color.DarkSlateGray;
        private readonly Color SHADOW_COLOR = Color.DeepSkyBlue;
        private readonly Color RUN_COLOR = Color.Yellow;
        
        // Threading and iteration control
        private Thread trainThread;
        private int runDelay;
        private bool isThreadDone;
        private bool isThreadCancel;
        private const int SEARCH_DELAY = 1;
        private const int RUN_DELAY = 50;
        private const int RUN_DELAY_MIN = 5;
        private const int PLOT_TIME = 500;
        private const int RENDER_TIME = 5;
        private DateTime dtRenderTime;
        private DateTime dtPlotTime;

        // Misc
        private bool isStep;
        private bool isRunAll;
        private bool isRandomSearch;
        private bool isDebug = true;
        private int last_selected;
        private bool isVisible;
        private int maze_count;
        private Maze.RUN_VISIBLE run_visible;

        private enum RUN_MODE
        {
            READY,
            RUN,
            STOP,
            PAUSE,
            STEP,
            EXIT,
            BACK
        }
        private RUN_MODE run_mode;

        #endregion

        #region Initialization

        // Constructor
        public MouseAI()
        {
            InitializeComponent();
            Console.BackgroundColor = ConsoleColor.DarkBlue;
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WindowHeight = 40;
            Console.WindowWidth = 140;

            try
            {
                maze = new Maze(MAZE_WIDTH, MAZE_HEIGHT);
            }
            catch (Exception e)
            {
                DisplayError("Maze Initialization Error", e, true);
            }

            mazeSegments = new MazeSegments(MAZE_WIDTH, MAZE_HEIGHT, maze, SPACE_COLOR)
            {
                Visible = false
            };

            mazeStatistics = new MazeStatistics();
            LoadSettings();
            InitControls();
            InitGraphics();
            DisplayTitleMessage(string.Empty);
        }

        // On form shown: Python check and project loading
        private void MazeAI_Shown(object sender, EventArgs e)
        {
            try
            {
                maze.CheckPythonPath();
            }
            catch (Exception ex)
            {
                DisplayError(string.Format("{0}\nThis program requires Python and library packages.\nSee Http:\\ for more information", ex.Message), true);
            }

            if (settings.isAutoRun && !string.IsNullOrEmpty(settings.LastFileName) && LoadMazes(settings.LastFileName, false))
            {
                SetMenuItems(true);
            }
            else
            {
                SetMenuItems(false);
            }
        }

        #endregion

        #region Path Solving

        // Iterate projects mazes: call path solving and image segment generation 
        private async void SolvePaths(bool isNewMazes)
        {
            if (!maze.isMazeModels())
                return;

            if (MessageBox.Show(string.Format("Calculate and solve maze paths?\n{0}", 
                        (isNewMazes) ? "" : "this will clear any current build")
                    , "Build Maze Paths", MessageBoxButtons.OKCancel) != DialogResult.OK)
                return;

            Console.Clear();
            isThreadCancel = false;
            isThreadDone = false;
            DisplayProgress("Solving Paths ...", true);

            try
            {
                for (int i = 0; i < lvwMazes.Items.Count; i++)
                {
                    if (SelectItem(i) && SelectMaze(i))
                    {
                        await RunSearch();
                    }
                    if (isThreadCancel)
                        break;
                }

                if (!isThreadCancel)
                {
                    maze.UpdateMazeModelPaths();
                    maze.SaveUpdatedMazeModels(settings.LastFileName);
                }
                string message = string.Format("Path Solving {0}", (isThreadCancel) ? "Cancelled." : "Completed.");
                if (isDebug)
                    Console.WriteLine(message);

                if (!isThreadCancel)
                {
                    DisplayDialog(message);
                    SetMenuItems(true);
                }
            }
            catch (Exception e)
            {
                DisplayError("Path solving error", e, false);
            }

            CloseProgress();
        }

        // Async path tree search: generate image segments on success
        private async Task RunSearch()
        {
            maze.MazeReset();
            mouse_last = new Point(-1, -1);
            
            bool isCheese = await Task.Run(PathSearch);

            DrawMaze();

            if (isCheese)
            {
                maze.GeneratePathImage();
                maze.CalculateSegments();
                mazeSegments.ShowImages();
                DrawPath();
            }
            else
                DisplayError("Error Calculating Path!", false);
        }

        // Path search thread: iterate while mouse moves are available
        private bool PathSearch()
        {
            try
            {
                while (!maze.ProcessMouseMove())
                {
                    Thread.Sleep(SEARCH_DELAY);
                }
                Console.WriteLine("Path solved for {0}", maze.GetMazeModelGUID());
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine("Search Error:" + e.Message);
                return false;
            }
        }

        #endregion

        #region Neural Network Training

        // Show train settings dialog: if ok start the training thread
        private void RunTrain()
        {
            if (!maze.CheckMazeModel() || trainThread != null || string.IsNullOrEmpty(maze.GetModelProjectGuid()))
                return;
     
            trainSettings = new TrainSettings(maze.GetConfig());
            trainSettings.Closing += TrainSettings_Closing;

            if (trainSettings.ShowDialog() != DialogResult.OK)
                return;

            trainThread = new Thread(AITrain);
            trainThread.Start();

            Console.Clear();
            isThreadCancel = false;
            isThreadDone = false;
            DisplayProgress("Training Neural Network ...", true);
        }

        // Training thread
        private void AITrain()
        {
            try
            {
                Console.Clear();
                maze.Train(settings.Guid);
                isThreadDone = true;
            }
            catch (Exception e)
            {
                // On cancel - handle any Keras HResult 
                Console.WriteLine("Training Cancelled");
                if (!isThreadCancel && e.HResult != -2146233040)
                    DisplayError("Training Error", e, false);

                isThreadCancel = true;
            }
            // Thread safe invocation
            if (InvokeRequired)
                Invoke(new MethodInvoker(TrainDone));
            else
                TrainDone();
        }

        // Display save dialog if not cancelled
        private void TrainDone()
        {
            CloseProgress();
            trainThread = null;

            if (!isThreadCancel && DisplayDialog("Log file saved: " + maze.GetLogName() + Environment.NewLine +
                              "Save Model and Results?", "Save Files", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                maze.SaveResults();
                maze.SaveUpdatedMazeModels(settings.LastFileName);
                DisplayDialog("Files Saved", "Save Models");
            }
            SetMenuItems(true);
            maze.CleanNetwork();

            // The invoked Python via .NET libraries occasionally enters an unpredictable state with repeated training calls
            // Restart the debugger/application just in case! 
            if (Debugger.IsAttached)
                Debugger.Launch();
            else
            {
                Process.Start(Assembly.GetExecutingAssembly().Location);
                Application.Exit();
            }
        }

        // On thread cancel
        private void StopTraining()
        {
            try
            {
                trainThread.Interrupt();
                if (!trainThread.Join(2000))
                    trainThread.Abort();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }

            trainThread = null;
            CloseProgress();
        }

        // On settings close
        private void TrainSettings_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (trainSettings.DialogResult == DialogResult.OK)
                maze.SetConfig(trainSettings.GetConfig());
        }

        #endregion

        #region Neural Network Model Loading and Testing

        // Display neural model selection and loading dialog
        private void RunTest()
        {
            if (maze == null || !maze.isMazeModels())
                return;

            List<string> starttimes = maze.GetProjectModels();
            if (starttimes == null || starttimes.Count == 0)
            {
                msMain.Enabled = true;
                return;
            }

            msMain.Enabled = false;
            modelLoad = new ModelLoad(maze, starttimes);
            modelLoad.btnPredict.Click += btnPredict_Click;
            modelLoad.btnRun.Click += btnRun_Click;
            modelLoad.Closed += modelLoad_Closed;
            modelLoad.btnDelete.Click += btnDelete_Click;
            modelLoad.Show();
        }

        private void modelLoad_Closed(object sender, EventArgs e)
        {
            msMain.Enabled = true;
        }

        // Model record deletion  
        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (!modelLoad.isSelectedModel())
                return;

            string starttime = modelLoad.GetSelectedModel();
            if (string.IsNullOrEmpty(starttime))
                return;

            DialogResult result = DisplayDialog(string.Format("Delete model record '{0}'?" , starttime), 
                "Confirm Deletion", MessageBoxButtons.OKCancel);

            if (result != DialogResult.OK)
                return;

            modelLoad.btnDelete.Enabled = false;
            RemoveModelRecord(starttime);
        }

        // Perform record deletion, relocate any associated files and reload control
        private void RemoveModelRecord(string starttime)
        {
            try
            {
                if (string.IsNullOrEmpty(starttime))
                    throw new Exception("Selection Error");

                modelLoad.pbxPlot.Image?.Dispose();

                if (!Maze.RemoveProjectRecord(starttime))
                    throw new Exception("Error Removing Record");

                DisplayDialog(string.Format("Record '{0}' Removed", starttime));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                DisplayError("Error Removing Record", e, false);
            }

            modelLoad.Close();
            RunTest();
        }

        // On neural model loaded - call run method
        private void btnRun_Click(object sender, EventArgs e)
        {
            DisplayProgress("Loading Model", false);
            bool result = LoadNeuralModel();
            CloseProgress();

            if (result)
                ModelRun();
        }

        // On neural model loaded - call predict method
        private void btnPredict_Click(object sender, EventArgs e)
        {
            DisplayProgress("Loading Model", false);
            bool result = LoadNeuralModel();
            CloseProgress();

            if (result)
                PredictModel();
        }

        // Load neural model selection
        private bool LoadNeuralModel()
        {
            if (modelLoad.lbxModels.SelectedItem == null)
                return false;

            string starttime = modelLoad.lbxModels.SelectedItem.ToString();
            modelLoad.Enabled = false;

            try
            {
                maze.LoadModel(starttime);
                maze.SetProjectLast(maze.GetModelProjectGuid(), starttime);
                modelLoad.Close();
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                DisplayError(string.Format("Error loading model '{0}'", starttime), e, false);
            }
            modelLoad.Enabled = true;
            return false;
        }

        #endregion

        #region Neural Network Model Running

        #region Model Run Form Controls

        // Init and display model run form
        private void ModelRun()
        {
            if (!maze.isMazeModels())
            {
                DisplayError("Error starting model run", false);
                return;
            }

            modelRun = new ModelRun(maze, BLOCK_COLOR, SPACE_COLOR);

            foreach (Control ctl in modelRun.Controls)
            {
                if (ctl is Button)
                    (ctl as Button).Click += modelRun_Click;
                else if (ctl is RadioButton)
                    (ctl as RadioButton).CheckedChanged += radiobutton_CheckedChanged;
            }

            NumericUpDown nu = modelRun.nudRate;
            nu.ValueChanged += nudRate_ValueChanged;

            if (runDelay == 0 || runDelay < nu.Minimum || runDelay > nu.Maximum)
            {
                runDelay = RUN_DELAY;
            }

            nu.Value = runDelay;
            modelRun.chkRandomWander.CheckedChanged += chkRandomWander_CheckedChanged;
            modelRun.chkRandomWander.Checked = isRandomSearch;
            modelRun.chkDebug.CheckedChanged += chkDebug_CheckedChanged;
            modelRun.chkDebug.Checked = isDebug;
            modelRun.Show();
            UpdateModelRun();
        }

        // Set form text value and run mode
        private void UpdateModelRun()
        {
            if (modelRun == null || !modelRun.Visible)
                return;

            string selected = GetSelectedItem();
            bool isReady = (!string.IsNullOrEmpty(selected));
            modelRun.Text = string.Format("Maze: {0} Model: {1}", selected, maze.GetModelName());

            if (!isRunAll)
                SetRunMode((isReady) ? RUN_MODE.READY : RUN_MODE.STOP);
        }

        // On form button click: set run mode, call associated methods
        private void modelRun_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;

            if (btn == null)
                return;

            switch (btn.Name)
            {
                case "btnExit":
                    if (!isRunMode())
                        RunExit();
                    SetRunMode(RUN_MODE.EXIT);
                    return;
                case "btnBack":
                    if (!isRunMode())
                        RunBack();
                    SetRunMode(RUN_MODE.BACK);
                    return;
                case "btnRun":
                    lvwMazes.Enabled = false;
                    SetRunMode(RUN_MODE.RUN);
                    if (!isRunAll)
                        modelRun.ClearPlots();
                    RunModel();
                    return;
                case "btnRunAll":
                    lvwMazes.Enabled = false;
                    isRunAll = true;
                    SetRunMode(RUN_MODE.RUN);
                    RunModel();
                    return;
                case "btnStop":
                    isStep = false;
                    isRunAll = false;
                    SetRunMode(RUN_MODE.STOP);
                    return;
                case "btnPause":
                    SetRunMode(RUN_MODE.PAUSE);
                    return;
                case "btnStep":
                    isStep = true;
                    SetRunMode(RUN_MODE.STEP);
                    if (lvwMazes.Enabled)
                    {
                        lvwMazes.Enabled = false;
                        RunModel();
                    }
                    return;
            }
        }

        // Set form button control states on run mode type
        private void SetRunMode(RUN_MODE _run_mode)
        {
            if (modelRun == null)
                return;

            ModelRun mdl = modelRun;
            run_mode = _run_mode;

            mdl.ResetButtons();

            if (run_mode == RUN_MODE.READY || run_mode == RUN_MODE.STOP)
            {
                mdl.btnRun.Enabled = true;
                mdl.btnRunAll.Enabled = true;
                mdl.btnExit.Enabled = true;
                mdl.btnBack.Enabled = true;
                modelRun.chkRandomWander.Enabled = true;
                return;
            }

            modelRun.chkRandomWander.Enabled = false;

            switch (run_mode)
            {
                case RUN_MODE.EXIT:
                    return;
                case RUN_MODE.BACK:
                    return;
                case RUN_MODE.RUN:
                    mdl.btnPause.Enabled = true;
                    mdl.btnStop.Enabled = true;
                    mdl.btnStep.Enabled = true;
                    isStep = false;
                    return;
                case RUN_MODE.STEP:
                    if (!isStep)
                    {
                        mdl.btnStep.Enabled = true;
                        mdl.btnRun.Enabled = true;
                        mdl.btnStop.Enabled = true;
                    }
                    return;
                case RUN_MODE.PAUSE:
                    mdl.btnRun.Enabled = true;
                    mdl.btnStep.Enabled = true;
                    return;
                default:
                    return;
            }
        }

        // Misc control event delegates and helpers

        private void chkDebug_CheckedChanged(object sender, EventArgs e)
        {
            isDebug = modelRun.chkDebug.Checked;
        }

        private void radiobutton_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton button = (RadioButton)sender;

            if (button.Checked)
            {
                if (button.Name.Equals("rdoVisible"))
                    run_visible = Maze.RUN_VISIBLE.VISIBLE;
                else if (button.Name.Equals("rdoNeural"))
                    run_visible = Maze.RUN_VISIBLE.NEURAL;
                else if (button.Name.Equals("rdoPaths"))
                    run_visible = Maze.RUN_VISIBLE.PATHS;
                else
                    run_visible = Maze.RUN_VISIBLE.NONE;
            }
        }

        private void chkRandomWander_CheckedChanged(object sender, EventArgs e)
        {
            isRandomSearch = modelRun.chkRandomWander.Checked;
        }

        private void nudRate_ValueChanged(object sender, EventArgs e)
        {
            runDelay = (int)modelRun.nudRate.Value;
        }

        private bool isRunMode()
        {
            return (run_mode == RUN_MODE.STEP || run_mode == RUN_MODE.RUN || run_mode == RUN_MODE.PAUSE);
        }

        private void RunExit()
        {
            modelRun.Close();
            msMain.Enabled = true;
        }

        private void RunBack()
        {
            modelRun.Close();
            RunTest();
        }

        #endregion

        #region Processing

        // Performs mouse maze movement iteration for a loaded model:
        // Asynchronous maze AI movement processing and vision image generation
        // Synchronous neural net predicitions of supplied vision images
        // Statistic plot generation
        private async void RunModel()
        {
            mouse_last = new Point(-1, -1);
            bool isProcess = false;

            if (maze.isMouseAtCheese())
            {
                SelectCurrent();
            }

            try
            {
                maze.ResetRun();
                maze.InitRunMove(isRandomSearch);
                Console.WriteLine("Mouse processing started ...");

                while (isRunMode())
                {
                    if (run_mode == RUN_MODE.STEP)
                    {
                        if (isStep)
                        {
                            Console.WriteLine("Step");
                            isStep = false;
                            SetRunMode(RUN_MODE.STEP);
                            isProcess = true;
                        }
                    }
                    else
                        isProcess = (run_mode != RUN_MODE.PAUSE);

                    if (isProcess)
                    {
                        // Mouse movement decision processing thread
                        if (await Task.Run(AIRun))
                            SetRunMode(RUN_MODE.STOP);
                        // Neural net processing
                        else
                            maze.ProcessVisionImage();

                        isProcess = false;
                    }

                    if (UpdateMaze(run_mode != RUN_MODE.RUN, true) && run_mode != RUN_MODE.PAUSE)
                    {
                        modelRun.UpdateTimer();
                    }

                    UpdateStatistic(false);
                    Application.DoEvents();
                    
                    if (run_mode == RUN_MODE.RUN)
                    {
                        Thread.Sleep(runDelay <= 0 ? RUN_DELAY : runDelay);
                    }
                }

                UpdateStatistics();
                UpdateStatistic(true);
                SetSelectedTextItalic();
            }
            catch (Exception e)
            {
                DisplayError("Run model error", e, false);
            }

            // Post processing control

            Console.WriteLine("Mouse processing ended.");

            lvwMazes.Enabled = true;
            maze.EndStatus();

            if (run_mode == RUN_MODE.EXIT)
                RunExit();
            else if (run_mode == RUN_MODE.BACK)
                RunBack();
            else
            {
                SetRunMode(RUN_MODE.STOP);
                if (isRunAll)
                {
                    if (SelectNext())
                        modelRun.btnRun.PerformClick();
                }
                ResetSelectedItem();
            }
        }

        // Maze movement processing thread
        private bool AIRun()
        {
            return maze.ProcessRunMove(isDebug);
        }

        #endregion

        #region Statistic Plotting

        private void UpdateStatistic(bool isImmediate)
        {
            DateTime dtCurrent = DateTime.UtcNow;

            if (dtCurrent >= dtPlotTime || isImmediate)
            {
                modelRun.UpdatePlot(maze.GetMazeStatisticData(), false);
                modelRun.tbxMouseStatus.Text = maze.GetMouseStatus();
                dtPlotTime = dtCurrent.AddMilliseconds(PLOT_TIME);
            }
        }

        private void UpdateStatistics()
        {
            MazeStatistic ms = maze.GetMazeStatistic();

            if (ms == null || mazeStatistics == null)
            {
                DisplayError("Error updating maze statistics!", false);
                return;
            }

            int index = mazeStatistics.FindIndex(o =>
                o.maze_guid.Equals(ms.maze_guid, StringComparison.OrdinalIgnoreCase));
            if (index != -1)
            {
                mazeStatistics.RemoveAt(index);
            }

            mazeStatistics.Add(maze.GetMazeStatistic());
            modelRun.UpdatePlot(mazeStatistics.GetData(), true);
        }

        private void ClearStatistics()
        {
            mazeStatistics.Clear();
        }

        #endregion

        #endregion

        #region Neural Network Model Predicting

        // Init and display model prediction form
        private void PredictModel()
        {
            modelPredict = new ModelPredict(SPACE_COLOR);

            foreach (Control ctl in modelPredict.Controls)
            {
                if (ctl is Button)
                    (ctl as Button).Click += btnPredictModel;
            }
            modelPredict.lvwSegments.SelectedIndexChanged += lvwSegments_SelectedIndexChanged;

            modelPredict.Show();
        }

        // Model image prediction processing:
        // Perform neural predictions of the image segment data set with the selected trained model
        // Display the results and any image detection failures
        private void AIPredict()
        {
            Console.WriteLine("Predictions Started ...");
            try
            {
                modelPredict.txtResults.Text = string.Empty;
                ImageDatas ids = maze.Predict(modelLoad.GetSelectedModel());
                
                if (ids == null)
                    throw new Exception("Prediction result error!");
                if (!maze.CalculateAccuracies(ids))
                    throw new Exception("Error Calculating Accuracies!");

                modelPredict.txtResults.Text = ids.GetResults();
                modelPredict.SetImages(ids);
            }
            catch (Exception e)
            {
                DisplayError("Prediction Error:", e, false);
                Console.WriteLine(e);
            }
            Console.WriteLine("Predicitions Ended ...");
        }

        // On listview image selection change: display associated maze
        private void lvwSegments_SelectedIndexChanged(object sender, EventArgs e)
        {
            ListView lv = modelPredict.lvwSegments;
            if (lv.FocusedItem == null || lv.SelectedItems.Count == 0 || lv.SelectedItems[0] == null)
                return;

            ListViewItem li = lv.SelectedItems[0];

            int.TryParse(li.Text, out int index);
            index -= 1;

            if (index >= 0 && index != GetSelectedIndex())
            {
                SelectItem(index);
                SelectMaze(index);
            }
        }

        // On form button click
        private void btnPredictModel(object sender, EventArgs e)
        {
            Button btn = (Button)sender;

            if (btn == null)
                return;

            if (btn.Name == "btnExit")
            {
                modelPredict.Close();
                msMain.Enabled = true;
                Focus();
            }
            else if (btn.Name == "btnBack")
            {
                modelPredict.Close();
                RunTest();
            }
            else if (btn.Name == "btnPredict")
            {
                modelPredict.Enabled = false;
                AIPredict();
                modelPredict.Enabled = true;
            }
        }

        #endregion

        #region Graphics Rendering

        // Init UI controls and canvas
        private void InitControls()
        {
            skgMaze.PaintSurface += skgMaze_PaintSurface;
            pbxPath.Width = MAZE_WIDTH * 4;
            pbxPath.Height = MAZE_HEIGHT * 4;
            skgMaze.Width = MAZE_WIDTH_PX;
            skgMaze.Height = MAZE_HEIGHT_PX;
            lvwMazes.Height = skgMaze.Height - pbxPath.Height - 10;
            lvwMazes.Width = pbxPath.Width;
            lvwMazes.Location = new Point(skgMaze.Width + 20, msMain.Height + 5);
            lvwMazes.BackColor = SPACE_COLOR;
            lvwMazes.ForeColor = Color.White;
            skgMaze.Location = new Point(MAZE_MARGIN_PX / 2, msMain.Height + 5);
            Width = MAZE_WIDTH_PX + (MAZE_MARGIN_PX * 2) + pbxPath.Width;
            Height = MAZE_HEIGHT_PX + (MAZE_MARGIN_PX * 5);
            pbxPath.Location = new Point(lvwMazes.Location.X, lvwMazes.Location.Y + lvwMazes.Height + 10);
            MinimumSize = new Size(Width, Height);
            MaximumSize = new Size(Width, Height);
        }

        // Init character bitmaps and colors
        private void InitGraphics()
        {
            SKImageInfo resizeInfo = new SKImageInfo(1, 1)
            {
                Width = (BITMAP_WIDTH / (MAZE_SCALE_WIDTH_PX + 2)) * 3,
                Height = (BITMAP_HEIGHT / (MAZE_SCALE_HEIGHT_PX + 2)) * 3
            };

            Cheese_Bitmap = GetBitmap(Resources.cheese, resizeInfo);
            Visible_Bitmap = GetBitmap(Resources.visible, resizeInfo);
            DeadEnd_Bitmap = GetBitmap(Resources.deadend, resizeInfo);
            Smell_Bitmap = GetBitmap(Resources.smell, resizeInfo);
            Mouse_Bitmap = new SKBitmap[MOUSE_BITMAPS];
            Mouse_Bitmap[(int)DIRECTION.SOUTH] = GetBitmap(Resources.mouse_south, resizeInfo);
            Mouse_Bitmap[(int)DIRECTION.NORTH] = GetBitmap(Resources.mouse_north, resizeInfo);
            Mouse_Bitmap[(int)DIRECTION.WEST] = GetBitmap(Resources.mouse_west, resizeInfo);
            Mouse_Bitmap[(int)DIRECTION.EAST] = GetBitmap(Resources.mouse_east, resizeInfo);

            ClearColor = SKColor.Parse("#003366");

            DropShadowColor = new SKColor(
                red: SHADOW_COLOR.R,
                green: SHADOW_COLOR.G,
                blue: SHADOW_COLOR.B,
                alpha: SHADOW_ALPHA
                );

            DropShadow = new SKPaint
            {
                ImageFilter = SKImageFilter.CreateDropShadow(0, 0, 2, 2, DropShadowColor)
            };

            BlockColor = new SKColor(
                    red: BLOCK_COLOR.R,
                    green: BLOCK_COLOR.G,
                    blue: BLOCK_COLOR.B,
                    alpha: BLOCK_COLOR.A);

            SpaceColor = new SKColor(
                red: SPACE_COLOR.R,
                green: SPACE_COLOR.G,
                blue: SPACE_COLOR.B,
                alpha: SPACE_COLOR.A);

            BlockPaint = new SKPaint
            {
                Color = BlockColor,
                StrokeWidth = LINE_WIDTH,
                IsAntialias = true,
                Style = SKPaintStyle.Fill,
                IsStroke = true,
                StrokeJoin = SKStrokeJoin.Miter
            };

            SpacePaint = new SKPaint
            {
                Color = SpaceColor,
                StrokeWidth = LINE_WIDTH,
                IsAntialias = true,
                Style = SKPaintStyle.Fill,
            };
        }

        private static SKBitmap GetBitmap(Bitmap resource, SKImageInfo resizeInfo)
        {
            SKBitmap bmp = resource.ToSKBitmap();
            return bmp.Resize(resizeInfo, SKFilterQuality.Medium);
        }

        // Offscreen buffer rendering:
        // Create canvas; iterate and render objects
        private void DrawMaze()
        {
            var imageInfo = new SKImageInfo(
                width: skgMaze.Width,
                height: skgMaze.Height,
                colorType: SKColorType.Rgba8888,
                alphaType: SKAlphaType.Premul);

            buffer = SKSurface.Create(imageInfo);
            surface = SKSurface.Create(imageInfo);
            canvas = surface.Canvas;
            offscreen = buffer.Canvas;
            offscreen.Clear(ClearColor);

            float x_pos, y_pos;
            OBJECT_TYPE ot;

            // Iterate height x width and render rectangles/character per object type
            for (int y_idx = 0; y_idx < MAZE_HEIGHT; y_idx++)
            {
                y_pos = y_idx * MAZE_SCALE_HEIGHT_PX;

                for (int x_idx = 0; x_idx < MAZE_WIDTH; x_idx++)
                {
                    x_pos = x_idx * MAZE_SCALE_WIDTH_PX;
                    ot = Maze.GetObjectDataType(x_idx, y_idx);

                    offscreen.DrawRect(x_pos, y_pos, MAZE_SCALE_WIDTH_PX, MAZE_SCALE_HEIGHT_PX,
                            (ot == OBJECT_TYPE.BLOCK) ? BlockPaint : SpacePaint);

                    if (Maze.GetObjectState(x_idx, y_idx) == OBJECT_STATE.CHEESE)
                        offscreen.DrawBitmap(Cheese_Bitmap, x_pos, y_pos, DropShadow);
                }
            }
            offscreen.Save();
            backimage = buffer.Snapshot();
            UpdateMaze(true, false);
        }

        // Timed or immediate maze character object rendering
        private bool UpdateMaze(bool isImmediate, bool isvisible)
        {
            if (!isImmediate)
            {
                DateTime dtCurrent = DateTime.UtcNow;

                if (dtCurrent < dtRenderTime)
                    return false;

                dtRenderTime = dtCurrent.AddMilliseconds(RENDER_TIME);
            }

            mouse_current = maze.GetMousePosition();
            if (mouse_current == mouse_last)
                return false;

            isVisible = isvisible;
            skgMaze.Invalidate();
            return true;
        }

        // Renders character types based on selection states
        private void skgMaze_PaintSurface(object sender, SKPaintGLSurfaceEventArgs e)
        {
            e.Surface.Canvas.Clear(ClearColor);
            if (!maze.isMazeModels())
                return;

            e.Surface.Canvas.DrawImage(backimage, 0, 0);

            if (isVisible && run_visible != Maze.RUN_VISIBLE.NONE)
            {
                maze.UpdatePointLists(mouse_current, run_visible);

                foreach (Point p in maze.GetDeadEndPoints())
                {
                    e.Surface.Canvas.DrawBitmap(DeadEnd_Bitmap, (p.X * MAZE_SCALE_WIDTH_PX),
                        (p.Y * MAZE_SCALE_HEIGHT_PX), DropShadow);
                }

                foreach (Point p in maze.GetVisiblePoints())
                {
                    e.Surface.Canvas.DrawBitmap(Visible_Bitmap, (p.X * MAZE_SCALE_WIDTH_PX),
                        (p.Y * MAZE_SCALE_HEIGHT_PX), DropShadow);
                }

                foreach (Point p in maze.GetSmellPoints())
                {
                    e.Surface.Canvas.DrawBitmap(Smell_Bitmap, (p.X * MAZE_SCALE_WIDTH_PX),
                        (p.Y * MAZE_SCALE_HEIGHT_PX), DropShadow);
                }
            }

            e.Surface.Canvas.DrawBitmap(Mouse_Bitmap[maze.GetMouseDirection()],
                (mouse_current.X * MAZE_SCALE_WIDTH_PX), (mouse_current.Y * MAZE_SCALE_HEIGHT_PX), DropShadow);
        }

        // Render maze path picturebox 
        private void DrawPath()
        {
            string guid = maze.GetMazeModelGUID();

            if (pbxPath.Image != null)
            {
                pbxPath.Image.Dispose();
                pbxPath.Image = null;
            }

            if (string.IsNullOrEmpty(guid))
            {
                return;
            }

            Bitmap bmp = maze.GetPathBMP(guid);

            if (bmp != null)
            {
                pbxPath.Image = new Bitmap(bmp);
                maze.SetTested(true);
                UpdateItemState();
            }
            else
            {
                pbxPath.Image = Resources.nopaths;
            }
        }

        // Coordinate display helper
        private void skgMaze_MouseClick(object sender, MouseEventArgs e)
        {
            tsCoords.Text = string.Format("(X:{0} Y:{1})", e.X / MAZE_SCALE_WIDTH_PX, e.Y / MAZE_SCALE_HEIGHT_PX);
        }

        #endregion

        #region File Related

        // New maze dialog and creation:
        // Iterate and generate mazes by selected amount, create records and save to project file
        private bool NewMazes()
        {
            mazeNew = null;
            mazeNew = new MazeNew();
            mazeNew.Closing += Mazenew_Closing;

            DialogResult dlr = mazeNew.ShowDialog();

            if (dlr != DialogResult.OK)
                return false;

            if (maze_count <= 0)
                return false;

            string filename = maze.GetSaveName();

            if (string.IsNullOrEmpty(filename))
                return false;

            maze = null;
            maze = new Maze(MAZE_WIDTH, MAZE_HEIGHT);

            Console.WriteLine("Creating {0} Mazes", maze_count);
            try
            {
                for (int i = 0; i < maze_count; i++)
                {
                    if (!CreateMaze(maze)) // Retry on character placement conflict
                        i--;

                    DisplayTsMessage(string.Format("Generating Maze {0} of {1}", i + 1, maze_count));
                }

                if (!maze.isMazeModels())
                    throw new Exception("Error creating maze models");

                maze.SetMazeModelsGuid(Guid.NewGuid().ToString());
                maze.SaveMazeModels(filename);
                settings.LastFileName = maze.GetFileName();
                settings.Guid = maze.GetModelProjectGuid();
                UpdateSettings();
                ClearStatistics();
                return true;
            }
            catch (Exception e)
            {
                DisplayError("Error Creating DB:", e, false);
                DisplayTitleMessage(string.Empty);
                return false;
            }
        }

        // Call generation routine, attempt to place characters and add to model list
        private static bool CreateMaze(Maze m)
        {
            m.MazeReset();
            m.GenerateMazes();
            if (!m.AddCharacters_Random())
                return false;
            m.AddMazeModel();
            return true;
        }

        // Update count on form close
        private void Mazenew_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            maze_count = (int)mazeNew.nudMazes.Value;
        }

        // Load an existing project by filename; exit on fatal
        private bool LoadMazes(string filename, bool isExit)
        {
            try
            {
                last_selected = -1;
                maze = null;
                maze = new Maze(MAZE_WIDTH, MAZE_HEIGHT);
                if (!maze.LoadMazeModels(filename))
                    return false;

                if (!maze.isMazeModels())
                {
                    throw new Exception("Failed to load maze models!");
                }

                maze.UpdateMazePaths();
                AddMazeItems();
                settings.LastFileName = maze.GetFileName();
                settings.Guid = maze.GetModelProjectGuid();
                UpdateSettings();
                DisplayTitleMessage(settings.LastFileName);
                ClearStatistics();
                return true;
            }
            catch (Exception e)
            {
                DisplayError("Error Loading Project", e, isExit);
                return false;
            }
        }

        // Close an active project
        private bool CloseProject()
        {
            if (canvas == null)
                return true;

            try
            {
                settings.LastFileName = string.Empty;
                settings.Guid = string.Empty;
                UpdateSettings();

                settings = Settings.Load();
                if (settings == null)
                    DisplayError(Settings.Error, true);

                lvwMazes.Items.Clear();
                maze = null;
                maze = new Maze(MAZE_WIDTH, MAZE_HEIGHT);

                skgMaze.Invalidate();
                DrawPath();
                ClearStatistics();
                DisplayTitleMessage(string.Empty);
                return true;
            }
            catch (Exception e)
            {
                DisplayError("Error closing project", e, false);
                return false;
            }
        }

        // User settings loading
        private void LoadSettings()
        {
            if (!Settings.isSettings())
            {
                settings = Settings.Create();
                if (settings == null)
                    DisplayError(Settings.Error, true);
            }

            settings = Settings.Load();
            if (settings == null)
                DisplayError(Settings.Error, true);

            SetOptionsItems();
        }

        // User settings updating
        private void UpdateSettings()
        {
            settings = Settings.Update(settings);

            if (settings == null)
                DisplayError(Settings.Error, true);
        }

        #endregion

        #region Maze Listview

        // On selection and if valid select the maze and update model run form (if active)
        private void lvwMazes_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lvwMazes.FocusedItem != null && lvwMazes.FocusedItem.Index != last_selected)
            {
                if(SelectMaze(lvwMazes.FocusedItem.Index)) 
                    UpdateModelRun();
            }
        }

        // Performs selected maze rendering and model run plotting updates (if active)
        private bool SelectMaze(int index)
        {
            try
            {
                maze.SelectMazeModel(index);
                if (!maze.AddCharacters())
                    throw new Exception("Could not add characters");

                maze.InitSmell();
                DrawMaze();
                DrawPath();

                if (settings.isMazeSegments)
                    mazeSegments.ShowImages();

                if (modelRun != null && run_mode == RUN_MODE.READY)
                {
                    maze.SetMazeStatistic(mazeStatistics.FirstOrDefault(o => o.maze_guid == maze.GetMazeModelGUID()));
                    if (maze.CheckMazeStatistic())
                    {
                        dtPlotTime = DateTime.UtcNow;
                        UpdateStatistic(true);
                    }
                    else
                    {
                        modelRun.ClearMazePlot();
                    }
                }

                DisplayTsMessage(string.Format("Maze: {0} GUID:{1}", index + 1, maze.GetMazeModelGUID()));
                last_selected = index;
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine("Maze selection error:{0}", e.Message);
                return false;
            }
        }

        #region Selection Helpers

        private void SetSelectedTextItalic()
        {
            ListViewItem item = GetSelectedListViewItem();
            if (item != null)
                item.ForeColor = RUN_COLOR;
        }

        private bool SelectNext()
        {
            int selectedIndex = lvwMazes.SelectedIndices[0];
            selectedIndex++;

            if (selectedIndex < lvwMazes.Items.Count)
            {
                lvwMazes.FocusedItem = lvwMazes.Items[selectedIndex];
                lvwMazes.Items[selectedIndex].Selected = true;
                lvwMazes.Items[selectedIndex].Focused = true;
                lvwMazes.Refresh();
                return true;
            }

            return false;
        }

        private void SelectCurrent()
        {
            if (!isMazeSelected())
                return;

            int selectedIndex = lvwMazes.SelectedIndices[0];

            if (selectedIndex < lvwMazes.Items.Count)
            {
                SelectMaze(selectedIndex);
            }
        }

        private void UpdateItemState()
        {
            if (!isMazeSelected())
                return;

            lvwMazes.Refresh();
        }

        private void AddMazeItems()
        {
            lvwMazes.Items.Clear();

            for (int i = 0; i < maze.GetMazeModelSize(); i++)
            {
                ListViewItem item = new ListViewItem((i + 1).ToString());
                lvwMazes.Items.Add(item);
            }

            if (lvwMazes.Items.Count > 0)
            {
                if (lvwMazes.Items[0] != null)
                {
                    lvwMazes.FocusedItem = lvwMazes.Items[0];
                    lvwMazes.Items[0].Selected = true;
                    lvwMazes.Select();
                }
            }
        }

        private bool isMazeSelected()
        {
            return (lvwMazes.SelectedItems.Count != 0 && lvwMazes.SelectedItems[0] != null);
        }

        private int GetSelectedIndex()
        {
            if (!isMazeSelected())
                return -1;

            return lvwMazes.SelectedItems[0].Index;
        }

        private void ResetSelectedItem()
        {
            if (!isMazeSelected())
                return;

            ListViewItem item = lvwMazes.SelectedItems[0];

            int index = item.Index;

            for (int i = 0; i < 2; i++)
            {
                lvwMazes.Items[index].Selected = !lvwMazes.Items[index].Selected;
                lvwMazes.Refresh();
            }
        }

        private bool SelectItem(int index)
        {
            if (index >= 0 && index < lvwMazes.Items.Count)
            {
                lvwMazes.Items[index].Selected = true;
                lvwMazes.Select();
                return true;
            }

            return false;
        }

        private string GetSelectedItem()
        {
            if (!isMazeSelected())
                return string.Empty;

            ListViewItem item = lvwMazes.SelectedItems[0];
            return item.SubItems[0].Text;
        }


        private ListViewItem GetSelectedListViewItem()
        {
            return !isMazeSelected() ? null : lvwMazes.SelectedItems[0];
        }

        #endregion

        #endregion

        #region Menustrip

        // UI menu item states and method calls

        private void SetMenuItems(bool isOpened)
        {
            closeToolStripMenuItem.Enabled = isOpened;
            pathsToolStripMenuItem.Enabled = isOpened;
            trainToolStripMenuItem.Enabled = isOpened && maze.CheckMazeModel();
            testToolStripMenuItem.Enabled = isOpened && maze.CheckProjectModels();
            mazeSegments.Visible = (isOpened && settings.isMazeSegments);
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (NewMazes() && LoadMazes(settings.LastFileName, false))
            {
                SolvePaths(true);
                SetMenuItems(true);
            }
        }

        private void loadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (LoadMazes(null, true))
            {
                SetMenuItems(true);
            }
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (CloseProject())
            {
                SetMenuItems(false);
            }
        }

        private void SetOptionsItems()
        {
            debugToolStripMenuItem.Checked = settings.isDebugConsole;
            autorunToolStripMenuItem.Checked = settings.isAutoRun;
            loadLastToolStripMenuItem.Checked = settings.isLoadLast;
            mazeSegmentsToolStripMenuItem.Checked = settings.isMazeSegments;
        }

        private void debugToolStripMenuItem_Click(object sender, EventArgs e)
        {
            debugToolStripMenuItem.Checked = !debugToolStripMenuItem.Checked;
            settings.isDebugConsole = debugToolStripMenuItem.Checked;
            UpdateSettings();
        }

        private void autorunToolStripMenuItem_Click(object sender, EventArgs e)
        {
            autorunToolStripMenuItem.Checked = !autorunToolStripMenuItem.Checked;
            settings.isAutoRun = autorunToolStripMenuItem.Checked;
            UpdateSettings();
        }

        private void loadLastToolStripMenuItem_Click(object sender, EventArgs e)
        {
            loadLastToolStripMenuItem.Checked = !loadLastToolStripMenuItem.Checked;
            settings.isLoadLast = loadLastToolStripMenuItem.Checked;
            UpdateSettings();
        }

        private void trainToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RunTrain();
        }

        private void pathsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SolvePaths(false);
        }

        private void testToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RunTest();
        }

        private void mazeSegmentsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mazeSegmentsToolStripMenuItem.Checked = !mazeSegmentsToolStripMenuItem.Checked;
            settings.isMazeSegments = mazeSegmentsToolStripMenuItem.Checked;
            UpdateSettings();

            if (!maze.isMazeModels())
                return;

            mazeSegments.Visible = settings.isMazeSegments;
        }

        private void viewHelpToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        #endregion

        #region Progress Bar

        // Progress bar display for long operations

        private void DisplayProgress(string message, bool isCancel)
        {
            Enabled = false;

            progress = new Progress(message, isCancel);
            
            if (isCancel)
                progress.btnProgressCancel.Click += btnProgressCancel_Click;

            progress.Show();
            progress.Location = new Point((Width / 2) - (progress.Width / 2), (Height / 2) - (progress.Height / 2));
            progress.Focus();
        }

        private void btnProgressCancel_Click(object sender, EventArgs e)
        {
            if (!isThreadDone)
            {
                progress.btnProgressCancel.Enabled = false;
                isThreadCancel = true;

                // If training
                if (trainThread != null && trainThread.ThreadState != ThreadState.Stopped)
                {
                    StopTraining();
                }
            }
        }

        private void CloseProgress()
        {
            progress?.Close();
            Enabled = true;
            Focus();
        }

        #endregion

        #region Messages

        // UI message display helpers

        private static void DisplayError(string message, bool isAppExit)
        {
            MessageBox.Show(string.Format("Error: {0}", message));
            if (isAppExit)
                Application.Exit();
        }

        private static void DisplayError(string message, Exception e, bool isAppExit)
        {
            MessageBox.Show(string.Format("{0}:{1}  ", message, e.Message));
            if (isAppExit)
                Application.Exit();
        }

        private static DialogResult DisplayDialog(string message, string title, MessageBoxButtons mb)
        {
            return MessageBox.Show(message, title, mb);
        }

        private static void DisplayDialog(string message, string title)
        {
            MessageBox.Show(message, title);
        }

        private static void DisplayDialog(string message)
        {
            MessageBox.Show(message);
        }

        private void DisplayTsMessage(string message)
        {
            tsStatus.Text = message;
        }

        private void DisplayTitleMessage(string value)
        {
            Text = (string.IsNullOrEmpty(value)) ? TITLE : string.Format("{0} ({1})", TITLE, value);
        }

        #endregion

    }
}
