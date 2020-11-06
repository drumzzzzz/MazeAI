using System;
using System.Collections.Generic;
using System.Linq;
using MouseAI.ML;

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
        public byte[] maze { get; set; }
        public List<byte[]> segments { get; set; }

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
            segments = new List<byte[]>();

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

    public class MazeModels
    {
        public string Guid { get; set; }
        public string StartTime { get; set; }
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

        public int GetSegmentCount()
        {
            return mazeModels.Where(mm => mm.segments != null).Sum(mm => mm.segments.Count);
        }

        public int[] GetSegmentArray()
        {
            List<int> segments = new List<int>();

            int index = 0;
            foreach (MazeModel m in mazeModels)
            {
                if (m.segments == null || m.segments.Count == 0)
                    throw  new Exception("Invalid Segment Found");

                for (int i = 0; i < m.segments.Count; i++)
                {
                    segments.Add(index);
                }

                index++;
            }

            return segments.ToArray();
        }

        public int[] GetModelArray()
        {
            List<int> models = new List<int>();

            for (int index = 0; index < mazeModels.Count; index++)
            {
                models.Add(index);
            }

            return models.ToArray();
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

        public ImageDatas GetImageDatas()
        {
            List<string> Guids = new List<string>();
            foreach (MazeModel mm in mazeModels)
            {
                if (string.IsNullOrEmpty(mm.guid))
                    throw new Exception("Invalid Guid found in MazeModel!");

                Guids.Add(mm.guid);
            }

            ImageDatas imageDatas = new ImageDatas(Guid, Guids);

            MazeModel m;
            for (int i = 0; i < mazeModels.Count; i++)
            {
                m = mazeModels[i];
                if (m.maze == null || m.maze.Length == 0 || m.segments == null || m.segments.Count == 0)
                    throw new Exception(string.Format("Invalid Image Data Found at Index {0}", i));

                imageDatas.Add(new ImageData(m.maze, m.guid));
                imageDatas.AddRange(m.segments.Select(t => new ImageData(t, m.guid)));
            }

            return imageDatas;
        }

        public MazeModel CheckPaths()
        {
            foreach (MazeModel mm in mazeModels)
            {
                if (mm.mazepath == null || mm.mazepath.Length == 0 || mm.maze == null)
                {
                    return mm;
                }
            }

            return null;
        }
    }
}
