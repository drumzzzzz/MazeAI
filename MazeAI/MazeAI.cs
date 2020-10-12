using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MazeAI
{
    public partial class MazeAI : Form
    {
        private Thread searchThread;
        private Maze maze;
        private AI ai;
        private AI.Path aipath;
        private bool isExit;
        private bool isFound;

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

        private void DisplayMessage(string msg)
        {
            txtMaze.Text += msg + Environment.NewLine;
        }

        private void AISearch()
        {
            foreach (AI.Path aip in ai.Paths)
            {
                if (maze.SetPath(aip.x, aip.y))
                {
                    aipath = aip;
                    isFound = true;
                    break;
                }
                maze.Display();
                Thread.Sleep(100);
            }
        }

        private void MazeAI_Shown(object sender, EventArgs e)
        {
            searchThread = new Thread(AISearch);
            ai = new AI();
            maze = new Maze(51, 25, ai.Paths);
            maze.Reset();
            maze.Generate();
            maze.Update();
            maze.AddMouse();
            maze.AddCheese(1, 50, 1, 24);
            //maze.Display();

            DisplayMessage("Searching for cheese ...");

            searchThread.Start();

            while (!isExit && !isFound)
            {
                Application.DoEvents();
            }

            if (isFound)
                DisplayMessage(string.Format("Found the cheese at {0}, {1}!", aipath.x, aipath.y));

            //foreach (AI.Path aip in ai.Paths)
            //{
            //    if (maze.SetPath(aip.x, aip.y))
            //    {
            //        DisplayMessage(string.Format("Found the cheese at {0}, {1}!", aip.x, aip.y));
            //        break;
            //    }
            //    maze.Display();
            //    Thread.Sleep(10);
            //}

            //maze.Display();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            isExit = true;
        }
    }
}
