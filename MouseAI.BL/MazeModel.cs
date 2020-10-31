using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Xml.Serialization;

namespace MouseAI.BL
{
    public class MazeModel
    {
        public byte[][] mazedata { get; set; }
        public string guid { get; set; }
        public int width { get; set; }
        public int height { get; set; }
        public int mouse_x { get; set; }
        public int mouse_y { get; set; }
        public int cheese_x { get; set; }
        public int cheese_y { get; set; }
        public bool isPath { get; set; }
        public byte[][] mazepath { get; set; }

        //[XmlIgnore]
        public byte[] bmp { get; set; }
        // public Bitmap bmp { get; set; }

        public MazeModel()
        { }

        public MazeModel(int width, int height, int mouse_x, int mouse_y, int cheese_x, int cheese_y, byte[,] MazeData)
        {
            this.width = width;
            this.height = height;
            this.mouse_x = mouse_x;
            this.mouse_y = mouse_y;
            this.cheese_x = cheese_x;
            this.cheese_y = cheese_y;
            guid = Guid.NewGuid().ToString();
            mazedata = new byte[height][];
            isPath = false;

            for (int y = 0; y < height; y++)
            {
                mazedata[y] = new byte[width];

                for (int x = 0; x < width; x++)
                {
                    mazedata[y][x] = MazeData[x, y];
                }
            }
        }
    }

    public class MazeModels // : List<MazeModel>
    {
        public string Guid { get; set; }
        public List<MazeModel> mazeModels { get; set; }

        public MazeModels()
        {
            mazeModels = new List<MazeModel>();
        }

        public void Clear()
        {
            mazeModels.Clear();
        }

        public void Add(MazeModel mm)
        {
            mazeModels.Add(mm);
        }

        public int Count()
        {
            return mazeModels.Count();
        }

        public MazeModel GetMazeModel(int index)
        {
            if (index > mazeModels.Count || index < 0)
                return null;
            return mazeModels[index];
        }

        public List<MazeModel> GetMazeModels()
        {
            return mazeModels;
        }

        public MazeModel CheckPaths()
        {
            foreach (MazeModel mm in mazeModels)
            {
                if (mm.mazepath == null || mm.mazepath.Length == 0 || mm.bmp == null)
                {
                    return mm;
                }
            }

            return null;
        }
    }
}
