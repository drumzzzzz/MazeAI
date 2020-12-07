// MazeGenerator Class:
// Recursively generates a random maze via a depth first search algorithm
// Based on the Java implementation from https://algs4.cs.princeton.edu/41graph/Maze.java.html

using System;

namespace MouseAI.BL
{
    public class MazeGenerator
    {
        #region Declarations

        private static byte[] maze;
        private static int maze_width;
        private static int maze_height;
        private readonly MazeObjects.DIRECTION[] dirs;
        private readonly Random r;

        #endregion

        #region Generation Methods
        // Constructor
        public MazeGenerator(int _maze_width, int _maze_height)
        {
            maze_width = _maze_width;
            maze_height = _maze_height;
            r= new Random();

            dirs = new MazeObjects.DIRECTION[4];
            dirs[0] = MazeObjects.DIRECTION.NORTH; 
            dirs[1] = MazeObjects.DIRECTION.EAST; 
            dirs[2] = MazeObjects.DIRECTION.SOUTH; 
            dirs[3] = MazeObjects.DIRECTION.WEST; 

            maze = new byte[maze_width * maze_height];
        }

        // Starting at the given index, recursively visit every direction randomly
        public void Generate(int x = 1, int y = 1)
        {
            // Set current location to empty
            maze[XYToIndex(x, y)] = Maze.WHITE;

            int rand;
            MazeObjects.DIRECTION dir_temp;

            for (int i = 0; i < 4; i++)
            {
                rand = r.Next(0, 4);
                dir_temp = dirs[rand];
                dirs[rand] = dirs[i];
                dirs[i] = dir_temp;
            }

            // Iterate directions and attempt to move
            for (int i = 0; i < 4; ++i)
            {
                // Offset from current location
                int dx = 0;
                int dy = 0;

                switch (dirs[i])
                {
                    case MazeObjects.DIRECTION.NORTH:
                        dy = -1;
                        break;
                    case MazeObjects.DIRECTION.SOUTH:
                        dy = 1;
                        break;
                    case MazeObjects.DIRECTION.EAST:
                        dx = 1;
                        break;
                    case MazeObjects.DIRECTION.WEST:
                        dx = -1;
                        break;
                }

                // Find the coords 2 spaces away
                int x2 = x + (dx << 1);
                int y2 = y + (dy << 1);

                if (Maze.IsInBounds(x2, y2))
                {
                    if (maze[XYToIndex(x2, y2)] == Maze.BLACK)
                    {
                        maze[XYToIndex(x2 - dx, y2 - dy)] = Maze.WHITE;
                        Generate(x2, y2);
                    }
                }
            }
        }

        public void GeneratorReset()
        {
            for (int i = 0; i < maze_width * maze_height; i++)
            {
                maze[i] = Maze.BLACK;
            }
        }

        public static byte GetObjectByte(int x, int y)
        {
            return (maze[XYToIndex(x, y)]);
        }

        private static int XYToIndex(int x, int y)
        {
            return y * maze_width + x;
        }

        #endregion
    }
}
