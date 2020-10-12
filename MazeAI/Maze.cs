using System;

namespace MazeAI
{
    class Maze
    {
        private string grid;// = new string(new char[maze_width * maze_height]);
        public readonly int maze_width;
        public readonly int maze_height;
        public const int NORTH = 0;
        public const int EAST = 1;
        public const int SOUTH = 2;
        public const int WEST = 3;
        public char BLOCK = '█';
        public char SPACE = ' ';

        private static Random r;

        public static int NextRandom()
        {
            if (r == null)
                r = new Random();

            return r.Next();
        }

        public Maze(int maze_width, int maze_height)
        {
            this.maze_width = maze_width;
            this.maze_height = maze_height;

            grid = new string(new char[maze_width * maze_height]);
        }

        public void Reset()
        {
            // Fills the grid with walls ('#' characters). 

            for (int i = 0; i < maze_width * maze_height; ++i)
            {
                grid = ChangeCharacter(grid, i, '█');
            }
        }


        private int XYToIndex(int x, int y)
        {
            // Converts the two-dimensional index pair (x,y) into a   // single-dimensional index.  The result is y * ROW_WIDTH + x.   
            return y * maze_width + x;
        }


        private bool IsInBounds(int x, int y)
        {
            // Returns "true" if x and y are both in-bounds.   
            if (x < 0 || x >= maze_width)
            {
                return false;
            }

            if (y < 0 || y >= maze_height)
            {
                return false;
            }

            return true;
        }

        public void Generate(int x = 1, int y = 1)
        {
            // Starting at the given index, recursively visits every direction in a    
            // randomized order. 
            if (x == 1 && y == 1)
                Reset();

            // Set my current location to be an empty passage.   
            grid = ChangeCharacter(grid, XYToIndex(x, y), SPACE);

            // Create an local array containing the 4 directions and shuffle their order.   
            int[] dirs = new int[4];
            dirs[0] = NORTH;
            dirs[1] = EAST;
            dirs[2] = SOUTH;
            dirs[3] = WEST;

            for (int i = 0; i < 4; ++i)
            {
                int r = NextRandom() & 3;
                int temp = dirs[r];
                dirs[r] = dirs[i];
                dirs[i] = temp;
            }

            // Loop through every direction and attempt to Visit that direction.   
            for (int i = 0; i < 4; ++i)
            {
                // dx,dy are offsets from current location.  Set them based 
                // on the next direction I wish to try.     
                int dx = 0;
                int dy = 0;
                switch (dirs[i])
                {
                    case NORTH:
                        dy = -1;
                        break;
                    case SOUTH:
                        dy = 1;
                        break;
                    case EAST:
                        dx = 1;
                        break;
                    case WEST:
                        dx = -1;
                        break;
                }

                // Find the (x,y) coordinates of the grid cell 2 spots 
                // away in the given direction. 
                int x2 = x + (dx << 1);
                int y2 = y + (dy << 1);

                if (IsInBounds(x2, y2))
                {
                    if (grid[XYToIndex(x2, y2)] == (char)BLOCK)
                    {
                        grid = ChangeCharacter(grid, XYToIndex(x2 - dx, y2 - dy), SPACE);
                        Generate(x2, y2);
                    }
                }
            }
        }

        public static string ChangeCharacter(string sourceString, int charIndex, char newChar)
        {
            return (charIndex > 0 ? sourceString.Substring(0, charIndex) : "")
                   + newChar +
                   (charIndex < sourceString.Length - 1 ? sourceString.Substring(charIndex + 1) : "");
        }

        public void Display()
        {
            Console.Clear();

            for (int y = 0; y < maze_height; ++y)
            {
                for (int x = 0; x < maze_width; ++x)
                {
                    Console.Write(grid[XYToIndex(x, y)]);
                }

                Console.Write(Environment.NewLine);
            }
        }
    }
}
