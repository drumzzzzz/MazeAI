﻿using System;
using System.Collections.Generic;
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


        public int index { get; set; }

        public bool isTest { get; set; }

        public MazeModel()
        { }

        public MazeModel(int index, int width, int height, int mouse_x, int mouse_y, int cheese_x, int cheese_y, byte[,] MazeData)
        {
            this.index = index;
            this.width = width;
            this.height = height;
            this.mouse_x = mouse_x;
            this.mouse_y = mouse_y;
            this.cheese_x = cheese_x;
            this.cheese_y = cheese_y;
            guid = Guid.NewGuid().ToString();
            mazedata = new byte[width][];
            isTest = false;

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

    public class MazeModels : List<MazeModel>
    {
    }
}
