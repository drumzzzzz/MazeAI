using System;
using System.Collections.Generic;
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
        }

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
                width: pictureBox1.Width,
                height: pictureBox1.Height,
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
                pictureBox1.Image?.Dispose();
                pictureBox1.Image = new Bitmap(mStream, false);
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

            //Draw();

            DisplayMessage("Searching for cheese ...");

            searchThread.Start();

            while (!isExit && !isFound)
            {
                Draw();
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
