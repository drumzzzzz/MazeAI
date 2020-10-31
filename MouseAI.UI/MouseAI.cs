#region Using Statements

using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using MouseAI.BL;
using SkiaSharp;
using SkiaSharp.Views.Desktop;

#endregion

namespace MouseAI.UI
{
    public partial class MouseAI : Form
    {
        #region Declarations

        private Settings oSettings;
        private Thread searchThread;
        private Thread testThread;
        private Thread trainThread;
        private readonly MazeText mazeText;
        private Maze maze;
        private bool isExit;
        private bool isFound;
        private bool isStep;
        private bool isTest;
        private bool isDone;
        private bool isValid;

        private const int MAZE_WIDTH = 41;
        private const int MAZE_HEIGHT = 25;
        private const int MAZE_SCALE_WIDTH_PX = 28;
        private const int MAZE_SCALE_HEIGHT_PX = 28;
        private const int MAZE_WIDTH_PX = MAZE_WIDTH * MAZE_SCALE_WIDTH_PX;
        private const int MAZE_HEIGHT_PX = MAZE_HEIGHT * MAZE_SCALE_HEIGHT_PX;
        private const int MAZE_MARGIN_PX = 25;
        private const int MAZE_COUNT = 5;
        private const float LINE_WIDTH = 1;
        private const string TITLE = "MOUSE AI";

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

        private const int PROCESS_DELAY = 1;
        private const int SEARCH_DELAY = 1;

        public enum RUNSTATE
        {
            NONE,
            READY,
            RUN,
            STOP,
            PAUSE,
            STEP,
            RESET,
            PROCESS,
            SELECT,
            BUILD_PATHS,
            TRAIN
        }

        private RUNSTATE RunState;

        #endregion

        #region Initialization

        public MouseAI()
        {
            InitializeComponent();
            //Console.BackgroundColor = ConsoleColor.Blue;
            //Console.ForegroundColor = ConsoleColor.White;
            Console.WindowHeight = 30;
            Console.WindowWidth = 100;
            //ConsoleHelper.SetCurrentFont("Consolas", 25);
            mazeText = new MazeText(MAZE_WIDTH, MAZE_HEIGHT)
            {
                Visible = false
            };

            LoadSettings();
            RunState = RUNSTATE.NONE;
            InitMaze();
            DisplayTitleMessage(string.Empty);
        }

        private void LoadSettings()
        {
            if (!Settings.isSettings())
            {
                oSettings = Settings.Create();
                if (oSettings == null)
                    DisplayError(Settings.Error, true);
            }

            oSettings = Settings.Load();
            if (oSettings == null)
                DisplayError(Settings.Error, true);

            SetMenuItems();
        }

        private void UpdateSettings()
        {
            oSettings = Settings.Update(oSettings);

            if (oSettings == null)
                DisplayError(Settings.Error, true);
        }

        #endregion

        #region Processing

        private void MazeAI_Shown(object sender, EventArgs e)
        {
            if (oSettings.isAutoRun && !string.IsNullOrEmpty(oSettings.LastFileName))
            {
                LoadMazes(oSettings.LastFileName);
                SetMazeTextVisible();
            }

            //RunTrain();
        }

        private static bool CreateMaze(Maze m)
        {
            try
            {
                m.Reset();
                m.Generate();
                m.Update();
                if (!m.AddCharacters_Random())
                    return false;
                m.AddMazeModel();
                return true;
            }
            catch (Exception e)
            {
                DisplayError("Error Creating Maze", e, false);
                return false;
            }
        }

        public void RenderMaze()
        {
            DrawMaze();
            SetRunState(RUNSTATE.READY);
        }

        private void RunProcess()
        {
            maze.Reset();
            searchThread = new Thread(AISearch);
            searchThread.Start();

            mouse_last = new Point(-1,-1);

            while (!isExit && !isFound)
            {
                if (isDone)
                    isDone = false;

                if (RunState == RUNSTATE.STEP && !isStep)
                {
                    RenderMaze();
                    SetRunState(RUNSTATE.PAUSE);
                    DisplayMazeText(true);
                }

                Application.DoEvents();
                Thread.Sleep(PROCESS_DELAY);
            }

            RenderMaze();

            if (isFound && isValid && maze.CalculatePath())
            {
                DrawPath();
                DisplayMazeText(true);
            }
            else
                DisplayError("Error Calculating Path!", false);

            isFound = false;
            searchThread = null;
        }

        private void AISearch()
        {
            isDone = false;

            try
            {
                while (!isFound)
                {
                    if (!isDone)
                    {
                        if (RunState == RUNSTATE.RUN || (RunState == RUNSTATE.STEP && isStep))
                        {
                            if (maze.ProcessMouseMove())
                            {
                                Console.WriteLine("Cheese found via path!");
                                isFound = true;
                                break;
                            }

                            isStep = false;
                        }
                        isDone = true;
                    }
                    Thread.Sleep(SEARCH_DELAY);
                }
                isValid = true;
            }
            catch (Exception e)
            {
                Console.WriteLine("Search Error:" + e.Message);
                isValid = false;
            }

            isFound = true;
            isDone = true;
        }

        private void RunTrain()
        {
            if (string.IsNullOrEmpty(oSettings.Guid) || (trainThread != null && trainThread.ThreadState != ThreadState.Stopped))
                return;

            trainThread = null;
            trainThread = new Thread(AITrain);
            trainThread.Start();
        }

        private void AITrain()
        {
            try
            {
                maze.Train(oSettings.Guid);
            }
            catch (Exception e)
            {
                DisplayError("Error Training", e, false);
            }
        }

        #endregion

        #region Building

        private void BuildPaths()
        {
            if (!maze.isMazeModels())
                return;

            if (MessageBox.Show("Clear and calculate maze paths?", "Build Maze Paths", MessageBoxButtons.OKCancel) != DialogResult.OK)
                return;

            for (int i = 0; i < lvwMazes.Items.Count; i++)
            {
                RunState = RUNSTATE.BUILD_PATHS;

                if (SelectItem(i) && SelectMaze(i))
                {
                    SetRunState(RUNSTATE.RUN);

                    if (searchThread == null)
                        RunProcess();
                }
                if (RunState == RUNSTATE.STOP)
                    break;
            }
            SetRunState(RUNSTATE.READY);
        }

        #endregion

        #region Graphics Rendering

        private void InitMaze()
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
            string guid = maze.GetGUID();
            if (string.IsNullOrEmpty(guid))
            {
                pbxPath.Image.Dispose();
                pbxPath.Image = null;
                return;
            }

            Bitmap bmp = maze.GetPathBMP(guid);

            if (bmp != null)
            {
                pbxPath.Image = bmp;
                if (maze.SetTested(true))
                {
                    lvwMazes.Enabled = true;
                    UpdateItemState(true);
                    lvwMazes.Enabled = false;
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

                    ot = maze.GetObjectDataType(x_idx, y_idx);

                    offscreen.DrawRect(x_pos, y_pos, MAZE_SCALE_WIDTH_PX, MAZE_SCALE_HEIGHT_PX,
                            (ot == OBJECT_TYPE.BLOCK) ? BlockPaint : SpacePaint);

                    if (maze.GetObjectState(x_idx, y_idx) == OBJECT_STATE.CHEESE)
                        offscreen.DrawBitmap(Cheese_Bitmap, x_pos, y_pos);
                }
            }
            offscreen.Save();
            backimage = buffer.Snapshot();
            UpdateMaze();
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

            if (oSettings.isMazeText)
                DisplayMazeText(false);
        }

        #endregion

        #region File Related

        private void NewMazes()
        {
            if (maze == null)
            {
                maze = new Maze(MAZE_WIDTH, MAZE_HEIGHT);
            }

            string filename = maze.GetSaveName();

            if (string.IsNullOrEmpty(filename))
                return;

            maze.ClearMazeModels();

            for (int i = 0; i < MAZE_COUNT; i++)
            {
                if (!CreateMaze(maze)) // Retry on character placement conflict
                    i--;

                DisplayTsMessage(string.Format("Generating Maze {0} of {1}", i + 1, MAZE_COUNT));
            }

            if (maze.isMazeModels())
            {
                string guid = Guid.NewGuid().ToString();
                maze.SetMazeModelsGuid(guid);
                string result = maze.SaveMazeModels(filename);
                if (!string.IsNullOrEmpty(result))
                {
                    DisplayError("Error Creating DB:" + result, false);
                    DisplayTsMessage("Error");
                    DisplayTitleMessage(string.Empty);
                }
                else
                {
                    DisplayTsMessage("Mazes Generated and Saved");
                    AddMazeItems();
                    oSettings.LastFileName = maze.GetFileName();
                    oSettings.Guid = guid;
                    UpdateSettings();
                    DisplayTitleMessage(oSettings.LastFileName);
                }
            }
        }

        private void LoadMazes(string filename)
        {
            if (maze == null)
            {
                maze = new Maze(MAZE_WIDTH, MAZE_HEIGHT);
            }

            string result = maze.LoadMazeModels(filename);

            if (result != string.Empty || !maze.isMazeModels())
            {
                DisplayError(result, false);
                return;
            }

            DisplayTsMessage("Mazes Loaded");

            maze.UpdateMazePaths();
            AddMazeItems();
            oSettings.LastFileName = maze.GetFileName();
            UpdateSettings();
            DisplayTitleMessage(oSettings.LastFileName);
        }

        private void SaveMazes()
        {
            if (!maze.isMazeModels() ||  string.IsNullOrEmpty(oSettings.LastFileName))
                return;

            DisplayTsMessage("Saving Mazes ...");

            try
            {
                maze.UpdateMazeModelPaths();
                string result = maze.SaveUpdatedMazeModels(oSettings.LastFileName);

                if (!string.IsNullOrEmpty(result))
                    throw  new Exception(result);

                DisplayTsMessage("Saved.");
            }
            catch (Exception e)
            {
                DisplayTsMessage("Save Error");
                DisplayError("Error Saving Mazes:", e, false);
            }
        }

        #endregion

        #region Listview

        private void UpdateItemState(bool isPath)
        {
            if (lvwMazes.SelectedItems[0] == null)
                return;

            ListViewItem item = lvwMazes.SelectedItems[0];
            item.SubItems[1].Text = (isPath) ? "YES" : "NO";
            lvwMazes.Refresh();
        }

        private void AddMazeItems()
        {
            lvwMazes.Items.Clear();
            
            for (int i=0;i<maze.GetMazeModelSize();i++)
            {
                ListViewItem item = new ListViewItem((i+1).ToString());
                
                item.SubItems.Add(maze.isModelBMP(i) ? "YES" : "NO");
                lvwMazes.Items.Add(item);
            }

            if (lvwMazes.Items.Count > 0)
            {
                lvwMazes.FocusedItem = lvwMazes.Items[0];
                lvwMazes.Items[0].Selected = true;
                lvwMazes.Select();
            }
        }

        private void lvwMazes_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (RunState == RUNSTATE.BUILD_PATHS)
                return;

            if (lvwMazes.FocusedItem == null)
                return;

            SetRunState(RUNSTATE.PROCESS);
            SelectMaze(lvwMazes.FocusedItem.Index);
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

        private void SelectMaze()
        {
            if (lvwMazes.Items.Count > 0 && lvwMazes.FocusedItem != null)
            {
                SelectMaze(lvwMazes.FocusedItem.Index);
            }
        }

        private bool SelectMaze(int index)
        {
            try
            {
                maze.SelectMazeModel(index);
                if (!maze.AddCharacters())
                    throw new Exception("Could not add characters");

                RenderMaze();
                DrawPath();
                DisplayTsMessage(string.Format("Maze: {0} GUID:{1}", index + 1, maze.GetGUID()));
                SetRunState(RUNSTATE.READY);
                return true;
            }
            catch (Exception e)
            {
                DisplayError("Error Selecting Maze", e, false);
                SetRunState(RUNSTATE.SELECT);
                return false;
            }
        }

        #endregion

        #region Maze Text

        private void SetMazeTextVisible()
        {
            mazeText.Visible = (oSettings.isMazeText);
            mazeText.txtMaze.Clear();
        }

        private void DisplayMazeText(bool isPath)
        {
            if (maze == null)
                return;
           
            if (isPath)
                mazeText.DisplayPaths(maze);
            else
            {
                mazeText.Display(maze);
            }
        }

        #endregion

        #region Button

        private void btnRun_Click(object sender, EventArgs e)
        {
            SetRunState(RUNSTATE.RUN);

            if (searchThread == null)
                RunProcess();
        }

        private void btnPause_Click(object sender, EventArgs e)
        {
            SetRunState(RUNSTATE.PAUSE);
        }

        private void btnStep_Click(object sender, EventArgs e)
        {
            isStep = true;
            SetRunState(RUNSTATE.STEP);
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            SetRunState(RUNSTATE.STOP);
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            SelectMaze();
        }

        #endregion

        #region Menustrip

        private void SetMenuItems()
        {
            debugToolStripMenuItem.Checked = oSettings.isDebugConsole;
            autorunToolStripMenuItem.Checked = oSettings.isAutoRun;
            loadLastToolStripMenuItem.Checked = oSettings.isLoadLast;
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveMazes();
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NewMazes();
        }

        private void loadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LoadMazes(null);
        }

        private void debugToolStripMenuItem_Click(object sender, EventArgs e)
        {
            debugToolStripMenuItem.Checked = !debugToolStripMenuItem.Checked;
            oSettings.isDebugConsole = debugToolStripMenuItem.Checked;
            UpdateSettings();
        }

        private void autorunToolStripMenuItem_Click(object sender, EventArgs e)
        {
            autorunToolStripMenuItem.Checked = !autorunToolStripMenuItem.Checked;
            oSettings.isAutoRun = autorunToolStripMenuItem.Checked;
            UpdateSettings();
        }

        private void loadLastToolStripMenuItem_Click(object sender, EventArgs e)
        {
            loadLastToolStripMenuItem.Checked = !loadLastToolStripMenuItem.Checked;
            oSettings.isLoadLast = loadLastToolStripMenuItem.Checked;
            UpdateSettings();
        }

        private void mazeTextToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mazeTextToolStripMenuItem.Checked = !mazeTextToolStripMenuItem.Checked;
            oSettings.isMazeText = mazeTextToolStripMenuItem.Checked;
            UpdateSettings();
            SetMazeTextVisible();
        }

        private void trainToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RunTrain();
        }

        private void testToolStripMenuItem_Click(object sender, EventArgs e)
        {
            BuildPaths();
        }

        private void testToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            AITrain();
        }

        #endregion

        #region Control States

        private void SetRunState(RUNSTATE r)
        {
            RunState = r;

            if (r == RUNSTATE.BUILD_PATHS)
            {
                btnRun.Enabled = false;
                btnStop.Enabled = true;
                btnPause.Enabled = true;
                btnStep.Enabled = false;
                btnReset.Enabled = false;
                lvwMazes.Enabled = false;
                return;
            }

            switch (r)
            {
                case RUNSTATE.PROCESS:
                    btnRun.Enabled = false;
                    btnStop.Enabled = false;
                    btnPause.Enabled = false;
                    btnStep.Enabled = false;
                    btnReset.Enabled = false;
                    lvwMazes.Enabled = false;
                    return;
                case RUNSTATE.SELECT:
                    btnRun.Enabled = false;
                    btnStop.Enabled = false;
                    btnPause.Enabled = false;
                    btnStep.Enabled = false;
                    btnReset.Enabled = false;
                    lvwMazes.Enabled = true;
                    return;
                case RUNSTATE.NONE:
                    btnRun.Enabled = false;
                    btnStop.Enabled = false;
                    btnPause.Enabled = false;
                    btnStep.Enabled = false;
                    btnReset.Enabled = false;
                    lvwMazes.Enabled = false;
                    return;
                case RUNSTATE.READY:
                    btnRun.Enabled = true;
                    btnStop.Enabled = false;
                    btnPause.Enabled = false;
                    btnStep.Enabled = false;
                    btnReset.Enabled = false;
                    lvwMazes.Enabled = true;
                    return;
                case RUNSTATE.RUN:
                    btnRun.Enabled = false;
                    btnStop.Enabled = true;
                    btnPause.Enabled = true;
                    btnStep.Enabled = false;
                    btnReset.Enabled = false;
                    lvwMazes.Enabled = false;
                    return;
                case RUNSTATE.STOP:
                    btnRun.Enabled = true;
                    btnStop.Enabled = false;
                    btnPause.Enabled = false;
                    btnStep.Enabled = false;
                    btnReset.Enabled = true;
                    lvwMazes.Enabled = true;
                    return;
                case RUNSTATE.PAUSE:
                    btnRun.Enabled = true;
                    btnStop.Enabled = true;
                    btnPause.Enabled = false;
                    btnStep.Enabled = true;
                    return;
                case RUNSTATE.STEP:
                    btnRun.Enabled = false;
                    btnStop.Enabled = false;
                    btnPause.Enabled = false;
                    btnStep.Enabled = false;
                    return;
                case RUNSTATE.RESET:
                    return;
            }
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
