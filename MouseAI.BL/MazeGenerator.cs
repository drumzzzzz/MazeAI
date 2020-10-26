using System;

namespace MouseAI.BL
{
    public class MazeGenerator
    {
        private static byte[] maze;
        private static int maze_width;
        private static int maze_height;
        private readonly DIRECTION[] dirs;
        private readonly Random r;

        public MazeGenerator(int _maze_width, int _maze_height, Random r)
        {
            maze_width = _maze_width;
            maze_height = _maze_height;
            this.r = r;

            dirs = new DIRECTION[4];
            dirs[0] = DIRECTION.NORTH; // NORTH;
            dirs[1] = DIRECTION.EAST; // EAST;
            dirs[2] = DIRECTION.SOUTH; // SOUTH;
            dirs[3] = DIRECTION.WEST; // WEST;

            maze = new byte[maze_width * maze_height];
        }

        public static void Reset()
        {
            for (int i = 0; i < maze_width * maze_height; i++)
            {
                maze[i] = Maze.BLACK;
            }
        }

        // Starting at the given index, recursively visit every direction randomly
        public void Generate(int x = 1, int y = 1)
        {
            // Set current location to empty
            maze[XYToIndex(x, y)] = Maze.WHITE;

            int rand;
            DIRECTION dir_temp;

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
                    case DIRECTION.NORTH:
                        dy = -1;
                        break;
                    case DIRECTION.SOUTH:
                        dy = 1;
                        break;
                    case DIRECTION.EAST:
                        dx = 1;
                        break;
                    case DIRECTION.WEST:
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

        public static byte GetObjectByte(int x, int y)
        {
            return (maze[XYToIndex(x, y)]);
        }

        private static int XYToIndex(int x, int y)
        {
            return y * maze_width + x;
        }
    }
}
