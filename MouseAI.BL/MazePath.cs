using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;

namespace MouseAI
{
    public class MazePath
    {
        public string guid { get; set; }
        public byte[][] mazepath { get; set; }

        public Bitmap bmp { get; set; }

        public MazePath(int width, int height, string guid)
        {
            mazepath = new byte[height][];
            
            for (int i=0; i<height;i++)
                mazepath[i] = new byte[width];

            this.mazepath = mazepath;
            this.guid = guid;
        }
    }

    public class MazePaths : List<MazePath>
    {
        private readonly int width;
        private readonly int height;

        public MazePaths()
        { }

        public MazePaths(int width, int height)
        {
            this.width = width;
            this.height = height;
        }

        public void ClearPath(string guid)
        {
            if (string.IsNullOrEmpty(guid))
                return;

            MazePath mp = this.FirstOrDefault(x => x.guid == guid);
            if (mp != null)
                Remove(mp);
        }

        public MazePath GetAddPath(string guid)
        {
            MazePath mp = this.FirstOrDefault(x => x.guid == guid);
            if (mp != null)
            {
                return mp;
            }

            mp = new MazePath(width, height, guid);
            Add(mp);
            return mp;
        }

        public MazePath GetPath(string guid)
        {
            MazePath mp = this.FirstOrDefault(x => x.guid == guid);
            return mp;
        }

        public void SetPath(MazePath mp, MazeObjects mos)
        {
            byte[][] bytes = mp.mazepath;
            MazeObject mo;

            for (int y = 0; y < bytes.Length; y++)
            {
                for (int x = 0; x < bytes[0].Length; y++)
                {
                    if (bytes[y][x] == Maze.BLACK)
                    {
                       mo = mos.FirstOrDefault(o => o.x == x && o.y == y);

                       if (mo != null)
                           mo.isPath = true;
                    }
                }
            }
        }

        public bool isPath(string guid)
        {
            return !string.IsNullOrEmpty(guid) && this.Any(x => x.guid == guid);
        }
    }

    public class PathNode
    {
        public int x { get; set; }
        public int y { get; set; }
        public bool isJunction { get; set; }

        public PathNode()
        {
        }

        public PathNode(int x, int y, bool isJunction)
        {
            this.x = x;
            this.y = y;
            this.isJunction = isJunction;
        }
    }

    public class PathNodes : List<PathNode>
    {
        public PathNode GetPath(int x, int y)
        {
            return this.FirstOrDefault(o => o.x == x && o.y == y);
        }
    }
}
