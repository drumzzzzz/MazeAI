using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;

namespace MouseAI
{
    public class MazePath : List<MazeObject>
    {
        public string guid { get; set; }
        public byte[][] mazepath { get; set; }

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

        public void GeneratePath(List<MazeObject> mazeobjects, string guid)
        {
            MazePath mp = this.FirstOrDefault(x => x.guid == guid) ?? new MazePath(width, height, guid);
        }

        public void ClearPath(string guid)
        {
            MazePath mp = this.FirstOrDefault(x => x.guid == guid);
            if (mp != null)
                Remove(mp);
        }
    }
}
