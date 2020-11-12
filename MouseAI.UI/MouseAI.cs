#region Using Statements

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using MouseAI.ML;
using SkiaSharp;
using SkiaSharp.Views.Desktop;

#endregion

namespace MouseAI.UI
{
    public partial class MouseAI : Form
    {
        #region Declarations

        private Settings settings;
        private Thread trainThread;
        private Thread runThread;
        private readonly MazeText mazeText;
        private ModelLoad modelLoad;
        private ModelRun modelRun;
        private ModelPredict modelPredict;
        private readonly MazeSegments mazeSegments;
        private Maze maze;
        private TrainSettings trainSettings;
        private Progress progress;
        private MazeNew mazeNew;
        private MazeManage mazeManage;

        private bool isValid;
        private bool isThreadDone;
        private bool isThreadCancel;
        private const int MAZE_WIDTH = 41;
        private const int MAZE_HEIGHT = 25;
        private const int MAZE_SCALE_WIDTH_PX = 28;
        private const int MAZE_SCALE_HEIGHT_PX = 28;
        private const int MAZE_WIDTH_PX = MAZE_WIDTH * MAZE_SCALE_WIDTH_PX;
        private const int MAZE_HEIGHT_PX = MAZE_HEIGHT * MAZE_SCALE_HEIGHT_PX;
        private const int MAZE_MARGIN_PX = 25;
        private const float LINE_WIDTH = 1;
        private const string TITLE = "MOUSE AI";
        private const int SEARCH_DELAY = 1;

        private SKColor BlockColor;
        private SKColor SpaceColor;
        private SKPaint BlockPaint;
        private SKPaint SpacePaint;
        private SKData MazeData;
        private SKCanvas offscreen;
        private SKCanvas canvas;
        private SKSurface buffer;
        private SKSurface surface;
        private SKImage backimage;
        private SKBitmap Cheese_Bitmap;
        private SKBitmap[] Mouse_Bitmaps;
        private Point mouse_last;

        private int maze_count;

        enum RUN_MODE
        {
            NONE,
            READY,
            RUNNING,
            STOPPED,
            PAUSED,
            STEP,
            RESET,
            EXIT
        }

        private RUN_MODE run_mode;

        #endregion

        #region Initialization

        public MouseAI()
        {
            InitializeComponent();
            //Console.BackgroundColor = ConsoleColor.Blue;
            //Console.ForegroundColor = ConsoleColor.White;
            Console.WindowHeight = 40;
            Console.WindowWidth = 140;
            ConsoleHelper.SetCurrentFont("Consolas", 18);

            try
            {
                maze = new Maze(MAZE_WIDTH, MAZE_HEIGHT);
            }
            catch (Exception e)
            {
                DisplayError("Maze Initialization Error", e, true);
            }

            mazeText = new MazeText(MAZE_WIDTH, MAZE_HEIGHT, maze)
            {
                Visible = false
            };

            mazeSegments = new MazeSegments(MAZE_WIDTH, MAZE_HEIGHT, maze)
            {
                Visible = false
            };

            LoadSettings();
            InitMazeGraphics();
            DisplayTitleMessage(string.Empty);
        }

        private void MazeAI_Shown(object sender, EventArgs e)
        {
            if (settings.isAutoRun && !string.IsNullOrEmpty(settings.LastFileName) && LoadMazes(settings.LastFileName, false))
            {
                SetMenuItems(true);
                SetMazeTextVisible();
            }
            else
            {
                SetMenuItems(false);
            }

            //RunTest();
        }

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

                ClearMaze();
                DrawPath();
                return true;
            }
            catch (Exception e)
            {
                DisplayError("Error closing project", e, false);
                return false;
            }
        }

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

        private void UpdateSettings()
        {
            settings = Settings.Update(settings);

            if (settings == null)
                DisplayError(Settings.Error, true);
        }

        #endregion

        #region Processing

        private void InitProcessing(string message, bool isCancel)
        {
            Console.Clear();
            Enabled = false;
            isThreadCancel = false;
            isThreadDone = false;

            progress = new Progress(message, isCancel)
            {
                TopMost = true
            };

            if (isCancel)
                progress.btnProgressCancel.Click += btnProgressCancel_Click;

            progress.Show();
            progress.Focus();
        }

        private void DisplayProgress(string message)
        {
            Console.Clear();
            Enabled = false;

            progress = new Progress(message, false)
            {
                TopMost = true
            };

            progress.Show();
            progress.Focus();
        }

        private void CloseProgress()
        {
            progress?.Close();
            Enabled = true;
            Focus();
        }

        private void FinalizeProcessing()
        {
            progress?.Close();
            Enabled = true;
            Focus();
        }

        #endregion

        #region Path Solving

        private async void SolvePaths()
        {
            if (!maze.isMazeModels())
                return;

            if (MessageBox.Show("Calculate and solve maze paths?\nthis will clear any current build"
                    , "Build Maze Paths", MessageBoxButtons.OKCancel) != DialogResult.OK)
                return;

            InitProcessing("Solving Paths ...", true);

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
                Console.WriteLine(message);
                DisplayTsMessage(message);
            }
            catch (Exception e)
            {
                DisplayError("Path solving error", e, false);
            }
            FinalizeProcessing();
        }

        private async Task RunSearch()
        {
            maze.Reset();
            mouse_last = new Point(-1, -1);
            
            await Task.Run(AISearch);

            DrawMaze();

            if (isValid)
            {
                maze.CalculatePath();
                maze.CalculateSegments();
                mazeSegments.ShowImages();

                DrawPath();
                DisplayMazeText();
            }
            else
                DisplayError("Error Calculating Path!", false);
        }

        private void AISearch()
        {
            isValid = false;
            try
            {
                while (!maze.ProcessMouseMove())
                {
                    Thread.Sleep(SEARCH_DELAY);
                }
                Console.WriteLine("Path solved for {0}", maze.GetMazeModelGUID());
                isValid = true;
            }
            catch (Exception e)
            {
                Console.WriteLine("Search Error:" + e.Message);
                isValid = false;
            }
        }

        #endregion

        #region Neural Network Training

        private void RunTrain()
        {
            if (trainThread != null || string.IsNullOrEmpty(maze.GetModelProjectGuid()))
                return;
     
            trainSettings = new TrainSettings(maze.GetConfig());
            trainSettings.Closing += TrainSettings_Closing;
            DialogResult dlr = trainSettings.ShowDialog();

            if (dlr != DialogResult.OK)
                return;

            trainThread = new Thread(AITrain);
            trainThread.Start();

            InitProcessing("Training Neural Network ...", true);
        }

        private void AITrain()
        {
            try
            {
                Console.Clear();
                maze.Train(settings.Guid);
                isThreadDone = true;

                if (DisplayDialog("Log file saved: " + maze.GetLogName() + Environment.NewLine +
                                  "Save Model and Results?", "Save Files", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    maze.SaveResults();
                    maze.SaveUpdatedMazeModels(settings.LastFileName);
                    DisplayDialog("Files Saved", "Save Models");
                }
                maze.CleanNetwork();
            }
            catch (Exception e)
            {
                Console.WriteLine("Training Cancelled");
                if (e.HResult != -2146233040)
                    DisplayError("Training Error", e, false);
            }

            if (InvokeRequired)
            {
                Invoke(new MethodInvoker(TrainDone));
            }
            else
            {
                TrainDone();
            }
        }

        private void TrainDone()
        {
            FinalizeProcessing();

            trainThread = null;
        }

        private void TrainSettings_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (trainSettings.DialogResult == DialogResult.OK)
                maze.SetConfig(trainSettings.GetConfig());
        }

        #endregion

        #region Neural Network Testing

        private void RunTest()
        {
            if (maze == null || !maze.isMazeModels())
                return;

            List<string> starttimes = maze.GetProjectModels();

            if (starttimes == null || starttimes.Count == 0)
            {
                DisplayDialog("No models found for project", "Test Error");
                return;
            }

            msMain.Enabled = false;
            modelLoad = new ModelLoad(starttimes, maze.GetProjectModel());
            modelLoad.Show();
            modelLoad.lbxModels.SelectedIndexChanged += lbxModels_SelectedIndexChanged;
            modelLoad.btnCancel.Click += btnExit_Click;
            modelLoad.btnPredict.Click += btnPredict_Click;
            modelLoad.btnRun.Click += btnRun_Click;
            modelLoad.Shown += ModelLoad_Shown;
        }

        private void btnRun_Click(object sender, EventArgs e)
        {
            if (ModelLoad())
            {
                ModelRun();
            }
        }

        private void btnPredict_Click(object sender, EventArgs e)
        {
            if (ModelLoad())
            {
                PredictModel();
            }
        }

        private void ModelLoad_Shown(object sender, EventArgs e)
        {
            string modelast = maze.GetProjectLast(maze.GetModelProjectGuid());

            if (modelLoad.lbxModels.Items.Count != 0 && modelLoad.lbxModels.Items.Contains(modelast))
            {
                modelLoad.lbxModels.SelectedItem = modelast;
            }
            else
            {
                modelLoad.lbxModels.SelectedItem = modelLoad.lbxModels.Items[0];
            }
        }

        private bool LoadModel(string starttime)
        {
            try
            {
                maze.LoadModel(starttime);
                maze.SetProjectLast(maze.GetModelProjectGuid(), starttime);
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                DisplayError(string.Format("Error loading model '{0}'", starttime), e, false);
                return false;
            }
        }

        private bool ModelLoad()
        {
            if (modelLoad.lbxModels.SelectedItem != null)
            {
                modelLoad.Enabled = false;
                if (LoadModel(modelLoad.lbxModels.SelectedItem.ToString()))
                {
                    modelLoad.Close();
                    return true;
                }
            }
            modelLoad.Enabled = true;
            return false;
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            modelLoad.Close();
            msMain.Enabled = true;
        }

        private void lbxModels_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (isSelectedModel())
            {
                string starttime = GetSelectedModel();
                string summary = Maze.GetModelSummary(starttime);

                if (!string.IsNullOrEmpty(summary))
                {
                    modelLoad.tbxSummary.Text = summary;
                    modelLoad.btnPredict.Enabled = true;
                    modelLoad.btnRun.Enabled = true;

                    string plot = Maze.GetModelPlot(starttime);
                    modelLoad.pbxPlot.Image = (!string.IsNullOrEmpty(plot)) ? Image.FromFile(plot) : null;
                    return;
                }
            }
            modelLoad.btnPredict.Enabled = false;
            modelLoad.btnRun.Enabled = false;
        }

        private bool isSelectedModel()
        {
            return (modelLoad.lbxModels.SelectedItem != null);
        }

        private string GetSelectedModel()
        {
            return modelLoad.lbxModels.SelectedItem.ToString();
        }

        #endregion

        #region Model Running

        private void ModelRun()
        {
            if (runThread != null || !maze.isMazeModels())
            {
                DisplayError("Error starting model run", false);
                return;
            }

            modelRun = new ModelRun();

            foreach (Control ctl in modelRun.Controls)
            {
                if (ctl is Button)
                    (ctl as Button).Click += modelRun_Click;
            }
            modelRun.Show();
            UpdateModelRun();
        }

        private void SetRunMode(RUN_MODE _run_mode)
        {
            if (modelRun == null)
                return;

            ModelRun mdl = modelRun;
            run_mode = _run_mode;

            mdl.ResetButtons();

            switch (run_mode)
            {
                case RUN_MODE.EXIT:
                    if (runThread == null)
                    {
                        modelRun.Close();
                        msMain.Enabled = true;
                        //lvwMazes.Enabled = true;
                    }
                    return;
                case RUN_MODE.READY:
                    mdl.btnRun.Enabled = true;
                    mdl.btnExit.Enabled = true;
                    return;
                case RUN_MODE.STOPPED:
                    mdl.btnRun.Enabled = true;
                    mdl.btnExit.Enabled = true;
                    return;
                case RUN_MODE.RUNNING:
                    mdl.btnPause.Enabled = true;
                    mdl.btnStop.Enabled = true;
                    return;
                case RUN_MODE.STEP:
                    mdl.btnRun.Enabled = true;
                    mdl.btnStep.Enabled = true;
                    return;
                default:
                    return;
            }
        }

        private void UpdateModelRun()
        {
            if (modelRun == null || !modelRun.Visible)
                return;

            string selected = GetSelectedItem();
            string guid = maze.GetMazeModelGUID();

            bool isReady = (!string.IsNullOrEmpty(selected) && !string.IsNullOrEmpty(guid));

            modelRun.tbxMaze.Text = (isReady) 
                ? string.Format("{0} [{1}]", selected, guid)  
                : string.Empty;

            SetRunMode((isReady) ? RUN_MODE.READY : RUN_MODE.NONE);
        }

        private void modelRun_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;

            if (btn == null)
                return;

            if (btn.Name == "btnExit")
            {
                if (runThread != null)
                {
                    isThreadCancel = true;
                }
                SetRunMode(RUN_MODE.EXIT);
            }
            else if (btn.Name == "btnRun")
            {
                lvwMazes.Enabled = false;
                SetRunMode(RUN_MODE.RUNNING);
                RunModel();
            }
            else if (btn.Name == "btnStop")
            {
                if (runThread != null)
                    isThreadCancel = true;

                SetRunMode(RUN_MODE.NONE);
            }
        }

        private void CloseRunModel()
        {
            modelRun.Close();
            msMain.Enabled = true;
            Focus();
        }

        private void RunModel()
        {
            maze.Reset();
            mouse_last = new Point(-1, -1);
            isThreadCancel = false;
            isValid = false;

            runThread = new Thread(AIRun);
            runThread.Start();

            try
            {
                while (!isThreadCancel && !isValid)
                {

                    Application.DoEvents();
                    Thread.Sleep(SEARCH_DELAY);
                }
            }
            catch (Exception e)
            {
                DisplayError("Run model error", e, false);
            }

            runThread = null;

            lvwMazes.Enabled = true;

            if (run_mode == RUN_MODE.EXIT)
                SetRunMode(RUN_MODE.EXIT);
            else
                SetRunMode(RUN_MODE.STOPPED);
        }

        private void AIRun()
        {
            isThreadDone = true;

            try
            {
                while (!isValid)
                {
                    if (isThreadDone)
                    {
                        if (maze.ProcessMouseMove())
                        {
                            isValid = true;
                        }
                        isThreadDone = false;
                    }
                    Thread.Sleep(SEARCH_DELAY);
                }
                Console.WriteLine("Mouse solved path for {0}", maze.GetMazeModelGUID());
            }
            catch (Exception e)
            {
                Console.WriteLine("Search Error:" + e.Message);
                isValid = false;
            }
        }

        #endregion

        #region Model Predict

        private void PredictModel()
        {
            modelPredict = new ModelPredict(MAZE_WIDTH, MAZE_HEIGHT);

            foreach (Control ctl in modelPredict.Controls)
            {
                if (ctl is Button)
                    (ctl as Button).Click += btnPredictModel;
            }
            modelPredict.Show();
        }

        private void AIPredict()
        {
            Console.WriteLine("Predictions Started ...");
            try
            {
                modelPredict.txtResults.Text = string.Empty;

                ImageDatas ids = maze.Predict(GetSelectedModel());
                if (ids == null)
                    throw new Exception("Prediction result error!");

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

        private void InitMazeGraphics()
        {
            pbxPath.Width = MAZE_WIDTH * 3;
            pbxPath.Height = MAZE_HEIGHT * 4;
            pbxMaze.Width = MAZE_WIDTH_PX;
            pbxMaze.Height = MAZE_HEIGHT_PX;
            lvwMazes.Height = pbxMaze.Height - pbxPath.Height - 10;
            lvwMazes.Width = pbxPath.Width;
            lvwMazes.Location = new Point(pbxMaze.Width + 20, msMain.Height + 5);
            pbxMaze.Location = new Point(MAZE_MARGIN_PX / 2, msMain.Height + 5);
            pbxPath.Location = new Point(pbxMaze.Width + 20, lvwMazes.Height - 10);
            Width = MAZE_WIDTH_PX + (MAZE_MARGIN_PX * 2) + pbxPath.Width;
            Height = MAZE_HEIGHT_PX + (MAZE_MARGIN_PX * 5);

            BlockColor = new SKColor(
                    red: 46,
                    green: 37,
                    blue: 217,
                    alpha: 255);

            SpaceColor = new SKColor(
                red: 255,
                green: 255,
                blue: 255,
                alpha: 255);

            BlockPaint = new SKPaint
            {
                Color = BlockColor,
                StrokeWidth = LINE_WIDTH,
                IsAntialias = true,
                Style = SKPaintStyle.Fill
            };

            SpacePaint = new SKPaint
            {
                Color = SpaceColor,
                StrokeWidth = LINE_WIDTH,
                IsAntialias = true,
                Style = SKPaintStyle.Fill
            };
        }

        private void DrawPath()
        {
            string guid = maze.GetMazeModelGUID();
            if (string.IsNullOrEmpty(guid))
            {
                pbxPath.Image.Dispose();
                pbxPath.Image = null;
                return;
            }

            Bitmap bmp = maze.GetPathBMP(guid);

            if (bmp != null)
            {
                pbxPath.Image = new Bitmap(bmp);

                if (maze.SetTested(true))
                {
                    UpdateItemState(true);
                }
            }
        }

        private void DrawMaze()
        {
            var imageInfo = new SKImageInfo(
                width: pbxMaze.Width,
                height: pbxMaze.Height,
                colorType: SKColorType.Rgba8888,
                alphaType: SKAlphaType.Premul);

            buffer = SKSurface.Create(imageInfo);
            surface = SKSurface.Create(imageInfo);
            canvas = surface.Canvas;
            offscreen = buffer.Canvas;
            offscreen.Clear(SKColor.Parse("#003366"));

            SKImageInfo resizeInfo = new SKImageInfo(160, 160);
            SKBitmap c = Resources.cheese.ToSKBitmap();
            Cheese_Bitmap = c.Resize(resizeInfo, SKFilterQuality.Medium);
            resizeInfo.Height = 51;
            resizeInfo.Width = 51;

            Mouse_Bitmaps = new SKBitmap[4];

            SKBitmap[] mbmps =
            {
                Resources.mouse_north.ToSKBitmap(),
                Resources.mouse_east.ToSKBitmap(),
                Resources.mouse_south.ToSKBitmap(),
                Resources.mouse_west.ToSKBitmap()
            };

            Mouse_Bitmaps[(int)DIRECTION.NORTH] = Resources.mouse_north.ToSKBitmap();
            Mouse_Bitmaps[(int)DIRECTION.EAST] = Resources.mouse_east.ToSKBitmap();
            Mouse_Bitmaps[(int)DIRECTION.SOUTH] = Resources.mouse_south.ToSKBitmap();
            Mouse_Bitmaps[(int)DIRECTION.WEST] = Resources.mouse_west.ToSKBitmap();

            for (int i = 0; i < 4; i++)
            {
                Mouse_Bitmaps[i] = mbmps[i].Resize(resizeInfo, SKFilterQuality.Medium);
            }

            float x_pos, y_pos;
            OBJECT_TYPE ot;

            // Height
            for (int y_idx = 0; y_idx < MAZE_HEIGHT; y_idx++)
            {
                y_pos = y_idx * MAZE_SCALE_HEIGHT_PX;

                // Width
                for (int x_idx = 0; x_idx < MAZE_WIDTH; x_idx++)
                {
                    x_pos = x_idx * MAZE_SCALE_WIDTH_PX;

                    ot = Maze.GetObjectDataType(x_idx, y_idx);

                    offscreen.DrawRect(x_pos, y_pos, MAZE_SCALE_WIDTH_PX, MAZE_SCALE_HEIGHT_PX,
                            (ot == OBJECT_TYPE.BLOCK) ? BlockPaint : SpacePaint);

                    if (Maze.GetObjectState(x_idx, y_idx) == OBJECT_STATE.CHEESE)
                        offscreen.DrawBitmap(Cheese_Bitmap, x_pos, y_pos);
                }
            }
            offscreen.Save();
            backimage = buffer.Snapshot();
            UpdateMaze();
        }

        private void ClearMaze()
        {
            if (canvas == null || pbxMaze.Image == null)
                return;

            canvas.Clear(SKColor.Parse("#003366"));
            pbxMaze.Image.Dispose();
            pbxMaze.Image = null;
        }

        private void UpdateMaze()
        {
            Point p = maze.GetMousePosition();
            if (p.X == mouse_last.X && p.Y == mouse_last.Y)
                return;

            canvas.Clear(SKColor.Parse("#003366"));
            canvas.DrawImage(backimage, 0, 0);

            int direction = maze.GetMouseDirection();

            canvas.DrawBitmap(Mouse_Bitmaps[direction], (p.X * MAZE_SCALE_WIDTH_PX), (p.Y * MAZE_SCALE_HEIGHT_PX));

            SKImage image = surface.Snapshot();
            MazeData = image.Encode();

            using (MemoryStream mStream = new MemoryStream(MazeData.ToArray()))
            {
                pbxMaze.Image?.Dispose();
                pbxMaze.Image = new Bitmap(mStream, false);
            }

            if (settings.isMazeText)
                DisplayMazeText();
        }

        #endregion

        #region File Related

        private bool NewMazes()
        {
            string filename = maze.GetSaveName();

            if (string.IsNullOrEmpty(filename))
                return false;

            maze_count = -1;
            mazeNew = null;
            mazeNew = new MazeNew();
            mazeNew.Closing += Mazenew_Closing;

            DialogResult dlr = mazeNew.ShowDialog();

            if (dlr != DialogResult.OK)
                return false;

            if (maze_count <= 0)
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

                string guid = Guid.NewGuid().ToString();
                maze.SetMazeModelsGuid(guid);
                maze.SaveMazeModels(filename);
                DisplayTsMessage("Saved and Generated Mazes ...");
                settings.LastFileName = maze.GetFileName();
                settings.Guid = maze.GetModelProjectGuid();
                UpdateSettings();
                DisplayTitleMessage(settings.LastFileName);
                AddMazeItems();
                return true;
            }
            catch (Exception e)
            {
                DisplayError("Error Creating DB:", e, false);
                DisplayTsMessage("Error");
                DisplayTitleMessage(string.Empty);
                return false;
            }
        }

        private void Mazenew_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            maze_count = (int)mazeNew.nudMazes.Value;
        }

        private static bool CreateMaze(Maze m)
        {
            m.Reset();
            m.Generate();
            m.Update();
            if (!m.AddCharacters_Random())
                return false;
            m.AddMazeModel();
            return true;
        }

        private bool LoadMazes(string filename, bool isExit)
        {
            try
            {
                maze = null;
                maze = new Maze(MAZE_WIDTH, MAZE_HEIGHT);
                maze.LoadMazeModels(filename);

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
                DisplayTsMessage("Mazes Loaded");
                return true;
            }
            catch (Exception e)
            {
                if (!isExit)
                    return false;

                DisplayError("Error Loading Project", e, true);
                return false;
            }
        }

        private void ManageMazes()
        {
            mazeManage = null;
            mazeManage = new MazeManage();
            DisplayProjectFiles();
            mazeManage.btnArchive.Click += btnArchive_Click;
            mazeManage.btnCancel.Click += btnCancel_Click;
            mazeManage.lbxProjects.SelectedIndexChanged += lbxProjects_SelectedIndexChanged;
            mazeManage.ShowDialog();
        }

        private void DisplayProjectFiles()
        {
            mazeManage.SetProjectFiles(maze.GetProjects());
            mazeManage.lbxProjects.SelectedItem = null;
        }

        private void lbxProjects_SelectedIndexChanged(object sender, EventArgs e)
        {
            mazeManage.btnArchive.Enabled = (mazeManage.lbxProjects.SelectedItem != null);
        }

        private static void ArchiveProject(string projectname)
        {
            string message = string.Format(
                    "Archive maze project {0}?\nThe projects files will be archived and removed.\n" +
                    "Any associated database records removed.",
                    projectname);
            if (MessageBox.Show(message, "Archive Project", MessageBoxButtons.OKCancel) != DialogResult.OK)
                return;

            string result = Maze.ArchiveProject(projectname);

            MessageBox.Show(result);
        }

        #endregion

        #region Listview

        private void UpdateItemState(bool isPath)
        {
            if (lvwMazes.SelectedItems.Count == 0 || lvwMazes.SelectedItems[0] == null)
                return;

            ListViewItem item = lvwMazes.SelectedItems[0];
            item.SubItems[1].Text = (isPath) ? "YES" : "NO";
            lvwMazes.Refresh();
        }

        private string GetSelectedItem()
        {
            if (lvwMazes.SelectedItems.Count == 0 || lvwMazes.SelectedItems[0] == null)
                return string.Empty;

            ListViewItem item = lvwMazes.SelectedItems[0];
            return item.SubItems[0].Text;
        }

        private void AddMazeItems()
        {
            lvwMazes.Items.Clear();

            for (int i = 0; i < maze.GetMazeModelSize(); i++)
            {
                ListViewItem item = new ListViewItem((i + 1).ToString());
                item.SubItems.Add(maze.isModelBMP(i) ? "YES" : "NO");
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

        private void lvwMazes_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lvwMazes.FocusedItem == null)
                return;

            if (SelectMaze(lvwMazes.FocusedItem.Index))
            {
                UpdateModelRun();
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

        //private void SelectMaze()
        //{
        //    if (lvwMazes.Items.Count > 0 && lvwMazes.FocusedItem != null && SelectMaze(lvwMazes.FocusedItem.Index))
        //    {
        //        SelectMaze(lvwMazes.FocusedItem.Index);
        //    }
        //}

        private bool SelectMaze(int index)
        {
            try
            {
                maze.SelectMazeModel(index);
                if (!maze.AddCharacters())
                    throw new Exception("Could not add characters");

                DrawMaze();
                DrawPath();

                if (settings.isMazeText)
                    DisplayMazeText();
                if (settings.isMazeSegments)
                    DisplayMazeSegments();

                DisplayTsMessage(string.Format("Maze: {0} GUID:{1}", index + 1, maze.GetMazeModelGUID()));
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine("Maze selection error:{0}", e.Message); ;
                return false;
            }
        }

        #endregion

        #region Maze Text and Segments

        private void SetToolsVisible(bool isOpened)
        {
            if (isOpened)
            {
                SetMazeTextVisible();
                SetMazeSegmentsVisible();
            }
            else
            {
                mazeText.Visible = false;
                mazeSegments.Visible = false;
            }
        }

        private void SetMazeTextVisible()
        {
            mazeText.Visible = (settings.isMazeText);
            mazeText.txtMaze.Clear();
        }

        private void DisplayMazeText()
        {
            mazeText.DisplayMaze();
        }

        private void SetMazeSegmentsVisible()
        {
            mazeSegments.Visible = (settings.isMazeSegments);
        }

        private void DisplayMazeSegments()
        {
            mazeSegments.ShowImages();
        }

        #endregion

        #region Button

        private void btnCancel_Click(object sender, EventArgs e)
        {
            mazeManage.Close();
        }

        private void btnArchive_Click(object sender, EventArgs e)
        {
            object selecteditem = mazeManage.lbxProjects.SelectedItem;
            if (selecteditem != null && !string.IsNullOrEmpty(selecteditem.ToString()))
            {
                mazeManage.btnArchive.Enabled = false;
                mazeManage.btnCancel.Enabled = false;
                ArchiveProject(selecteditem.ToString());
                mazeManage.btnArchive.Enabled = false;
                mazeManage.btnCancel.Enabled = true;
                DisplayProjectFiles();
            }
        }

        private void btnProgressCancel_Click(object sender, EventArgs e)
        {
            if (!isThreadDone)
            {
                progress.btnProgressCancel.Enabled = false;

                if (trainThread != null && trainThread.ThreadState != ThreadState.Stopped)
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

                    FinalizeProcessing();
                }
            }
        }

        #endregion

        #region Menustrip

        private void SetMenuItems(bool isOpened)
        {
            manageToolStripMenuItem.Enabled = !isOpened;
            closeToolStripMenuItem.Enabled = isOpened;
            pathsToolStripMenuItem.Enabled = isOpened;
            trainToolStripMenuItem.Enabled = isOpened;
            testToolStripMenuItem.Enabled = isOpened;

            SetToolsVisible(isOpened);
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (NewMazes())
            {
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

        private void manageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!maze.isMazeModels())
                ManageMazes();
        }

        private void SetOptionsItems()
        {
            debugToolStripMenuItem.Checked = settings.isDebugConsole;
            autorunToolStripMenuItem.Checked = settings.isAutoRun;
            loadLastToolStripMenuItem.Checked = settings.isLoadLast;
            mazeTextToolStripMenuItem.Checked = settings.isMazeText;
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

        private void mazeTextToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mazeTextToolStripMenuItem.Checked = !mazeTextToolStripMenuItem.Checked;
            settings.isMazeText = mazeTextToolStripMenuItem.Checked;
            UpdateSettings();

            if (!maze.isMazeModels())
                return;

            SetMazeTextVisible();
        }

        private void mazeSegmentsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mazeSegmentsToolStripMenuItem.Checked = !mazeSegmentsToolStripMenuItem.Checked;
            settings.isMazeSegments = mazeSegmentsToolStripMenuItem.Checked;
            UpdateSettings();

            if (!maze.isMazeModels())
                return;

            SetMazeSegmentsVisible();
        }

        private void trainToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RunTrain();
        }

        private void pathsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SolvePaths();
        }

        private void testToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RunTest();
        }

        #endregion

        #region Messages

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

        private void DisplayTsMessage(string message)
        {
            tsStatus.Text = message;
        }

        private void DisplayTitleMessage(string value)
        {
            if (string.IsNullOrEmpty(value))
                Text = TITLE;
            else
                Text = string.Format("{0} ({1})", TITLE, value);
        }

        #endregion

    }
}
