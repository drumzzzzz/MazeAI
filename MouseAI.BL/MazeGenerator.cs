using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MouseAI.BL
{
    public class MazeGenerator
    {
        private string maze;
        private readonly int maze_width;
        private readonly int maze_height;
        private readonly DIRECTION[] dirs;
        private readonly Random r;

        public MazeGenerator(int maze_width, int maze_height, Random r)
        {
            this.maze_width = maze_width;
            this.maze_height = maze_height;
            this.r = r;

            dirs = new DIRECTION[4];
            dirs[0] = DIRECTION.NORTH; // NORTH;
            dirs[1] = DIRECTION.EAST; // EAST;
            dirs[2] = DIRECTION.SOUTH; // SOUTH;
            dirs[3] = DIRECTION.WEST; // WEST;

            maze = new string(new char[maze_width * maze_height]);
        }

        public void Reset()
        {
            for (int i = 0; i < maze_width * maze_height; i++)
            {
                maze = ChangeCharacter(maze, i, '█');
            }
        }


        // Starting at the given index, recursively visit every direction randomly
        public void Generate(int x = 1, int y = 1)
        {
            // Set current location to empty
            maze = ChangeCharacter(maze, XYToIndex(x, y), Maze.SPACE);

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

                if (IsInBounds(x2, y2))
                {
                    if (maze[XYToIndex(x2, y2)] == Maze.BLOCK)
                    {
                        maze = ChangeCharacter(maze, XYToIndex(x2 - dx, y2 - dy), Maze.SPACE);
                        Generate(x2, y2);
                    }
                }
            }
        }

        public OBJECT_TYPE GetObjectType(int x, int y)
        {
            return (maze[XYToIndex(x, y)] == Maze.SPACE) ? OBJECT_TYPE.SPACE : OBJECT_TYPE.BLOCK;
        }

        public byte GetObjectByte(int x, int y)
        {
            return (maze[XYToIndex(x, y)] == Maze.SPACE) ? Maze.WHITE : Maze.BLACK;
        }

        private bool IsInBounds(int x, int y)
        {
            if (x < 0 || x >= maze_width)
                return false;

            return (y >= 0 && y < maze_height);
        }

        private int XYToIndex(int x, int y)
        {
            return y * maze_width + x;
        }

        public static string ChangeCharacter(string sourceString, int charIndex, char newChar)
        {
            return (charIndex > 0 ? sourceString.Substring(0, charIndex) : "")
                   + newChar +
                   (charIndex < sourceString.Length - 1 ? sourceString.Substring(charIndex + 1) : "");
        }
    }
}
