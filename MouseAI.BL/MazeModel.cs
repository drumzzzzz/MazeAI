using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MouseAI.BL
{
    public class MazeModel
    {
        private string maze;
        private int width;
        private int height;
        private int mouse_x;
        private int mouse_y;
        private int cheese_x;
        private int cheese_y;

        public MazeModel()
        { }

        public MazeModel(int width, int height, int mouse_x, int mouse_y, int cheese_x, int cheese_y, string maze)
        {
            this.width = width;
            this.height = height;
            this.mouse_x = mouse_x;
            this.mouse_y = mouse_y;
            this.cheese_x = cheese_x;
            this.cheese_y = cheese_y;
            this.maze = new string(new char[width * height]);
            this.maze = maze;
        }
    }

    public class MazeModels :List<MazeModel>
    { }
}
