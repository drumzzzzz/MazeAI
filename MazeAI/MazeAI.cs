using System;
using System.Collections.Generic;
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

        private SKSize scaledSize;
        private SKCanvas canvas;

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
        }

        private void skiaView_PaintSurface(object sender, SKPaintSurfaceEventArgs e)
        {
            // the the canvas and properties
            canvas = e.Surface.Canvas;

            // get the screen density for scaling
            var scale = 1f;
            scaledSize = new SKSize(e.Info.Width / scale, e.Info.Height / scale);

            // handle the device screen density
            canvas.Scale(scale);

            // make sure the canvas is blank
            canvas.Clear(SKColors.White);

            // draw some text
            var paint = new SKPaint
            {
                Color = SKColors.Black,
                IsAntialias = true,
                Style = SKPaintStyle.Fill,
                TextAlign = SKTextAlign.Center,
                TextSize = 24
            };
            var coord = new SKPoint(scaledSize.Width / 2, (scaledSize.Height + paint.TextSize) / 2);
            canvas.DrawText("SkiaSharp", coord, paint);
        }

        private void DisplayMessage(string msg)
        {
            txtMaze.Text += msg + Environment.NewLine;
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
            // get the screen density for scaling
            var scale = 1f;
            
            // draw some text
            var paint = new SKPaint
            {
                Color = SKColors.Black,
                IsAntialias = true,
                Style = SKPaintStyle.Fill,
                TextAlign = SKTextAlign.Center,
                TextSize = 24
            };
            var coord = new SKPoint(scaledSize.Width / 2, (scaledSize.Height + paint.TextSize) / 2);
            canvas.DrawText("Hello World!", coord, paint);
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

            Draw();

            DisplayMessage("Searching for cheese ...");

            searchThread.Start();

            while (!isExit && !isFound)
            {
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
