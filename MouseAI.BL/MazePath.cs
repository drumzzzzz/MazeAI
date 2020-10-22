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

        public MazePath GetPath(string guid)
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

        public bool isPath(string guid)
        {
            return !string.IsNullOrEmpty(guid) && this.Any(x => x.guid == guid);
        }
    }
}
