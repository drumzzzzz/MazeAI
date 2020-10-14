using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using SkiaSharp;
using SkiaSharp.Views.Desktop;


namespace MazeAI
{
    public partial class MazeAI : Form
    {
        private Thread searchThread;
        private Maze maze;
        private MazePath ai;
        private MazePath.Path aipath;
        private bool isExit;
        private bool isFound;
        private frmAISearch oFrmAiSearch;
        private Random rand;

        private const int MAZE_WIDTH = 51;
        private const int MAZE_HEIGHT = 25;
        private const int MAZE_SCALE_WIDTH_PX = 16;
        private const int MAZE_SCALE_HEIGHT_PX = 32;
        private const int MAZE_WIDTH_PX = MAZE_WIDTH * MAZE_SCALE_WIDTH_PX;
        private const int MAZE_HEIGHT_PX = MAZE_HEIGHT * MAZE_SCALE_HEIGHT_PX;
        private const int MAZE_MARGIN_PX = 25;
        private SKColor BlockColor;
        private SKColor SpaceColor;
        private const float LINE_WIDTH = 1;

        public MazeAI()
        {
            InitializeComponent();
            Console.BackgroundColor = ConsoleColor.Blue;
            Console.ForegroundColor = ConsoleColor.White;
            Console.CursorVisible = false;
            Console.OutputEncoding = new UnicodeEncoding();
            Console.WindowHeight = 50;
            Console.WindowWidth = 75;
            isExit = false;
            isFound = false;
            ConsoleHelper.SetCurrentFont("Consolas", 25);
            rand = new Random(0);
            InitMaze();
        }

        #region Graphics Rendering

        private void InitMaze()
        {
            pbxMaze.Width = MAZE_WIDTH_PX;
            pbxMaze.Height = MAZE_HEIGHT_PX;
            Width = MAZE_WIDTH_PX + (MAZE_MARGIN_PX * 2);
            Height = MAZE_HEIGHT_PX + (MAZE_MARGIN_PX * 3);
            pbxMaze.Left = (MAZE_MARGIN_PX / 2);
            pbxMaze.Top = (MAZE_MARGIN_PX / 2);
            BlockColor = new SKColor(
                    red: (byte)46,
                    green: (byte)37,
                    blue: (byte)217,
                    alpha: (byte)255);

            SpaceColor = new SKColor(
                red: (byte)255,
                green: (byte)255,
                blue: (byte)255,
                alpha: (byte)255);
        }

        private void DrawMaze()
        {
            var imageInfo = new SKImageInfo(
                width: pbxMaze.Width,
                height: pbxMaze.Height,
                colorType: SKColorType.Rgba8888,
                alphaType: SKAlphaType.Premul);

            var surface = SKSurface.Create(imageInfo);
            var canvas = surface.Canvas;

            canvas.Clear(SKColor.Parse("#003366"));

            MazeObject[,] MazeObjects = maze.GetMazeObjects();

            float y_pos;
            float x_pos;

            var BlockPaint = new SKPaint
            {
                Color = BlockColor,
                StrokeWidth = LINE_WIDTH,
                IsAntialias = true,
                Style = SKPaintStyle.Fill
            };

            var SpacePaint = new SKPaint
            {
                Color = SpaceColor,
                StrokeWidth = LINE_WIDTH,
                IsAntialias = true,
                Style = SKPaintStyle.Fill
            };

            // Height
            for (int y_idx = 0; y_idx < MAZE_HEIGHT; y_idx++)
            {
                y_pos = y_idx * MAZE_SCALE_HEIGHT_PX;

                // Width
                for (int x_idx = 0; x_idx < MAZE_WIDTH; x_idx++)
                {
                    x_pos = x_idx * MAZE_SCALE_WIDTH_PX;

                    OBJECT_TYPE ot = maze.GetObjectType(x_idx, y_idx);
                    canvas.DrawRect(x_pos,y_pos,(float)MAZE_SCALE_WIDTH_PX,(float) MAZE_SCALE_HEIGHT_PX,
                            (ot == OBJECT_TYPE.BLOCK) ? BlockPaint : SpacePaint);
                }
            }

            //for (int i = 0; i < 100; i++)
            //{
            //    float lineWidth = rand.Next(1, 10);
            //    var lineColor = new SKColor(
            //        red: (byte)rand.Next(255),
            //        green: (byte)rand.Next(255),
            //        blue: (byte)rand.Next(255),
            //        alpha: (byte)rand.Next(255));

            //    var linePaint = new SKPaint
            //    {
            //        Color = lineColor,
            //        StrokeWidth = lineWidth,
            //        IsAntialias = true,
            //        Style = SKPaintStyle.Stroke
            //    };

            //    int x1 = rand.Next(imageInfo.Width);
            //    int y1 = rand.Next(imageInfo.Height);
            //    int x2 = rand.Next(imageInfo.Width);
            //    int y2 = rand.Next(imageInfo.Height);
            //    canvas.DrawLine(x1, y1, x2, y2, linePaint);
            //}

            using (SKImage image = surface.Snapshot())
            using (SKData data = image.Encode())
            using (System.IO.MemoryStream mStream = new System.IO.MemoryStream(data.ToArray()))
            {
                pbxMaze.Image?.Dispose();
                pbxMaze.Image = new Bitmap(mStream, false);
            }
        }

        #endregion

        private void DisplayMessage(string msg)
        {
            //txtMaze.Text += msg + Environment.NewLine;
        }

        private void AISearch()
        {
            List<MazeObject> elements = new List<MazeObject>();

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
                Thread.Sleep(50);
            }
        }

        private void Draw()
        {
            var imageInfo = new SKImageInfo(
                width: pbxMaze.Width,
                height: pbxMaze.Height,
                colorType: SKColorType.Rgba8888,
                alphaType: SKAlphaType.Premul);

            var surface = SKSurface.Create(imageInfo);
            var canvas = surface.Canvas;

            canvas.Clear(SKColor.Parse("#003366"));

            for (int i = 0; i < 100; i++)
            {
                float lineWidth = rand.Next(1, 10);
                var lineColor = new SKColor(
                    red: (byte)rand.Next(255),
                    green: (byte)rand.Next(255),
                    blue: (byte)rand.Next(255),
                    alpha: (byte)rand.Next(255));

                var linePaint = new SKPaint
                {
                    Color = lineColor,
                    StrokeWidth = lineWidth,
                    IsAntialias = true,
                    Style = SKPaintStyle.Stroke
                };

                int x1 = rand.Next(imageInfo.Width);
                int y1 = rand.Next(imageInfo.Height);
                int x2 = rand.Next(imageInfo.Width);
                int y2 = rand.Next(imageInfo.Height);
                canvas.DrawLine(x1, y1, x2, y2, linePaint);
            }

            using (SKImage image = surface.Snapshot())
            using (SKData data = image.Encode())
            using (System.IO.MemoryStream mStream = new System.IO.MemoryStream(data.ToArray()))
            {
                pbxMaze.Image?.Dispose();
                pbxMaze.Image = new Bitmap(mStream, false);
            }

        }

        private void MazeAI_Shown(object sender, EventArgs e)
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

            //Draw();

            DisplayMessage("Searching for cheese ...");

            searchThread.Start();

            while (!isExit && !isFound)
            {
                //Draw();
                Application.DoEvents();
            }

            if (isFound)
                DisplayMessage("Found the cheese!");
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            isExit = true;
        }
    }
}
