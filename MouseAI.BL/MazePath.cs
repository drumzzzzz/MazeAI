using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;

namespace MouseAI
{
    public class MazePath
    {
        public List<Path> Paths { get; set; }

        public MazePath()
        {
            Paths = new List<Path>();
        }

        public class Path
        {
            public int x { get; set; }
            public int y { get; set; }
            public DIRECTION direction { get; set; }

            public Path(int x, int y, DIRECTION direction)
            {
                this.x = x;
                this.y = y;
                this.direction = direction;
            }
        }
    }
}
