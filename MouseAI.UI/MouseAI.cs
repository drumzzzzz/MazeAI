#region Using Statements

using System;
using System.Drawing;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows.Forms;
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
        private Maze maze;
        private MazePath ai;
        private bool isExit;
        private bool isFound;

        private const int MAZE_WIDTH = 51;
        private const int MAZE_HEIGHT = 25;
        private const int MAZE_SCALE_WIDTH_PX = 16;
        private const int MAZE_SCALE_HEIGHT_PX = 32;
        private const int MAZE_WIDTH_PX = MAZE_WIDTH * MAZE_SCALE_WIDTH_PX;
        private const int MAZE_HEIGHT_PX = MAZE_HEIGHT * MAZE_SCALE_HEIGHT_PX;
        private const int MAZE_MARGIN_PX = 25;
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
        private const float LINE_WIDTH = 1;

        #endregion

        #region Initialization

        public MouseAI()
        {
            InitializeComponent();
            Console.BackgroundColor = ConsoleColor.Blue;
            Console.ForegroundColor = ConsoleColor.White;
            Console.CursorVisible = false;
            Console.OutputEncoding = new UnicodeEncoding();
            Console.WindowHeight = 50;
            Console.WindowWidth = 75;
            ConsoleHelper.SetCurrentFont("Consolas", 25);
            LoadSettings();
            
            InitMaze();
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

        #region Graphics Rendering

        private void InitMaze()
        {
            isExit = false;
            isFound = false;

            pbxMaze.Width = MAZE_WIDTH_PX;
            pbxMaze.Height = MAZE_HEIGHT_PX;
            Width = MAZE_WIDTH_PX + (MAZE_MARGIN_PX * 2);
            Height = MAZE_HEIGHT_PX + (MAZE_MARGIN_PX * 3);
            pbxMaze.Left = (MAZE_MARGIN_PX / 2);
            pbxMaze.Top = (MAZE_MARGIN_PX / 2);
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

            SKImageInfo resizeInfo = new SKImageInfo(90, 190);
            SKBitmap c = Resources.cheese.ToSKBitmap();
            Cheese_Bitmap = c.Resize(resizeInfo, SKFilterQuality.Medium);
            resizeInfo.Height = 60;
            resizeInfo.Width = 30;

            Mouse_Bitmaps = new SKBitmap[4];

            SKBitmap[] mbmps = 
            {
                Resources.mouse_north.ToSKBitmap(),
                Resources.mouse_east.ToSKBitmap(),
                Resources.mouse_south.ToSKBitmap(), 
                Resources.mouse_west.ToSKBitmap()
            }; 

            Mouse_Bitmaps[(int) DIRECTION.NORTH] = Resources.mouse_north.ToSKBitmap();
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

                    ot = maze.GetObjectType(x_idx, y_idx);

                    offscreen.DrawRect(x_pos,y_pos,MAZE_SCALE_WIDTH_PX,MAZE_SCALE_HEIGHT_PX,
                            (ot == OBJECT_TYPE.BLOCK) ? BlockPaint : SpacePaint);

                    if (maze.GetObjectState(x_idx, y_idx) == OBJECT_STATE.CHEESE)
                        offscreen.DrawBitmap(Cheese_Bitmap,x_pos,y_pos);
                }
            }
            offscreen.Save();
            backimage = buffer.Snapshot();
            UpdateMaze();
        }

        private void UpdateMaze()
        {
            canvas.Clear(SKColor.Parse("#003366"));
            canvas.DrawImage(backimage,0,0);

            Point p = maze.GetMousePosition();
            int direction = maze.GetMouseDirection();

            canvas.DrawBitmap(Mouse_Bitmaps[direction], p.X * MAZE_SCALE_WIDTH_PX, p.Y * MAZE_SCALE_HEIGHT_PX);
            
            SKImage image = surface.Snapshot();
            MazeData = image.Encode();

            using (MemoryStream mStream = new MemoryStream(MazeData.ToArray()))
            {
                pbxMaze.Image?.Dispose();
                pbxMaze.Image = new Bitmap(mStream, false);
            }
        }

        #endregion

        #region Processing

        private void MazeAI_Shown(object sender, EventArgs e)
        {
            RunProcess();
        }

        private void RunProcess()
        {
            searchThread = new Thread(AISearch);
            ai = new MazePath();
            maze = new Maze(51, 25, ai.Paths);
            maze.Reset();
            maze.Generate();
            maze.Update();
            maze.AddMouse();
            maze.AddCheese(1, 50, 1, 24);

            DrawMaze();

            //DisplayMessage("Searching for cheese ...");

            searchThread.Start();

            while (!isExit && !isFound)
            {
                UpdateMaze();
                Application.DoEvents();
            }

            if (isFound)
            {
                //DisplayMessage("Found the cheese!");
            }
        }

        private void AISearch()
        {
            while (!isFound)
            {
                if (maze.ProcessMouseMove())
                {
                    maze.Display();
                    Console.WriteLine("Cheese found via path!");
                    isFound = true;
                    break;
                }

                maze.Display(); 
                Thread.Sleep(30);
            }
        }

        #endregion

        #region Controls

        private void SetMenuItems()
        {
            debugToolStripMenuItem.Checked = oSettings.isDebugConsole;
            autorunToolStripMenuItem.Checked = oSettings.isAutoRun;
            loadLastToolStripMenuItem.Checked = oSettings.isLoadLast;
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

        private void DisplayMessage(string message)
        {
            MessageBox.Show(message);
        }

        private void DisplayError(string message, bool isAppExit)
        {
            MessageBox.Show(string.Format("{0}", message));
            if (isAppExit)
                Application.Exit();
        }

        private void DisplayError(string message, Exception e, bool isAppExit)
        {
            MessageBox.Show(string.Format("{0}:{1}  ", message, e.Message));
            if (isAppExit)
                Application.Exit();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            isExit = true;
        }

        #endregion
    }
}
