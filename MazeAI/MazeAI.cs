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

            Maze maze = new Maze(51, 25);

            maze.Reset();
            maze.Generate();
            maze.Display();
        }
    }
}
