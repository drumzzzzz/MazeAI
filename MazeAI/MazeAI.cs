using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Reflection;
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
        private SKPaint BlockPaint;
        private SKPaint SpacePaint;
        private SKData MazeData;
        private SKCanvas canvas;
        private SKSurface surface;
        private SKBitmap Cheese_Bitmap;
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

        public enum BitmapAlignment
        {
            Start,
            Center,
            End
        }

        static SKRect CalculateDisplayRect(SKRect dest, float bmpWidth, float bmpHeight,
            BitmapAlignment horizontal, BitmapAlignment vertical)
        {
            float x = 0;
            float y = 0;

            switch (horizontal)
            {
                case BitmapAlignment.Center:
                    x = (dest.Width - bmpWidth) / 2;
                    break;

                case BitmapAlignment.Start:
                    break;

                case BitmapAlignment.End:
                    x = dest.Width - bmpWidth;
                    break;
            }

            switch (vertical)
            {
                case BitmapAlignment.Center:
                    y = (dest.Height - bmpHeight) / 2;
                    break;

                case BitmapAlignment.Start:
                    break;

                case BitmapAlignment.End:
                    y = dest.Height - bmpHeight;
                    break;
            }

            x += dest.Left;
            y += dest.Top;

            return new SKRect(x, y, x + bmpWidth, y + bmpHeight);
        }

        private void DrawMaze()
        {
            var imageInfo = new SKImageInfo(
                width: pbxMaze.Width,
                height: pbxMaze.Height,
                colorType: SKColorType.Rgba8888,
                alphaType: SKAlphaType.Premul);

            surface = SKSurface.Create(imageInfo);
            canvas = surface.Canvas;
            canvas.Clear(SKColor.Parse("#003366"));

            SKBitmap b = Resources.cheese.ToSKBitmap();

            SKImageInfo resizeInfo = new SKImageInfo(MAZE_SCALE_WIDTH_PX, MAZE_SCALE_HEIGHT_PX);
            Cheese_Bitmap = b.Resize(resizeInfo, SKFilterQuality.High);

            float y_pos;
            float x_pos;

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

                    if (maze.GetObjectState(x_idx, y_idx) == OBJECT_STATE.CHEESE)
                    { 
                        canvas.DrawBitmap(Cheese_Bitmap,x_pos,y_pos);
                    }
                    
                }
            }
            canvas.SaveLayer();
            //canvas.Clear(SKColor.Parse("#003366"));
            //SKImage image = surface.Snapshot();
            //MazeData = image.Encode();
            //UpdateMaze();
            //using (SKImage image = surface.Snapshot())
            //using (SKData data = image.Encode())
            //using (System.IO.MemoryStream mStream = new System.IO.MemoryStream(data.ToArray()))
            //{
            //    pbxMaze.Image?.Dispose();
            //    pbxMaze.Image = new Bitmap(mStream, false);
            //}
        }

        private void UpdateMaze()
        {
            canvas.Clear(SKColor.Parse("#003366"));
            SKImage image = surface.Snapshot();
            MazeData = image.Encode();

            using (System.IO.MemoryStream mStream = new System.IO.MemoryStream(MazeData.ToArray()))
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

            DisplayMessage("Searching for cheese ...");

            searchThread.Start();

            while (!isExit && !isFound)
            {
                UpdateMaze();
                Application.DoEvents();
            }

            if (isFound)
                DisplayMessage("Found the cheese!");
        }

        private void MazeAI_Shown(object sender, EventArgs e)
        {
            RunProcess();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            isExit = true;
        }
    }
}
