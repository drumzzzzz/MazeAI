using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MazeAI
{
    public class AI
    {
        public List<Path> Paths { get; set; }

        public AI()
        {
            Paths = new List<Path>();
        }

        public class Path
        {
            public int x { get; set; }
            public int y { get; set; }
            public Maze.DIRECTIONS direction { get; set; }

            public Path(int x, int y, Maze.DIRECTIONS direction)
            {
                this.x = x;
                this.y = y;
                this.direction = direction;
            }
        }
    }
}
