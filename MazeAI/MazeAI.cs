using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MazeAI
{
    public partial class MazeAI : Form
    {
        public MazeAI()
        {
            InitializeComponent();

            //Maze.RandomNumbers.Seed(time(0)); // seed random number generator.
            Console.BackgroundColor = ConsoleColor.Blue;
            Console.ForegroundColor = ConsoleColor.White;

            Maze maze = new Maze(51, 25);

            maze.Reset();
            maze.Generate();
            maze.Update();
            maze.AddMouse();
            maze.AddCheese(1, 50, 1, 24);
            maze.Display();
        }
    }
}
