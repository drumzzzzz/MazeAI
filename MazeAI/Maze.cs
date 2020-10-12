using System;
using System.Collections.Generic;
using System.Text;

namespace MazeAI
{
    class Maze
    {
        private string maze;
        public readonly int maze_width;
        public readonly int maze_height;

        private enum DIRECTIONS
        {
            NORTH,
            EAST,
            SOUTH,
            WEST
        }

        private readonly DIRECTIONS[] dirs;
        private const char BLOCK = '█';
        private const char VISITED = '×';
        private const char SPACE = '░';
        private const char MOUSE = 'ô';
        private const char CHEESE = 'Δ';

        private static Random r;

        public enum ELEMENT_TYPE
        {
            SPACE,
            BLOCK
        }

        public enum ELEMENT_STATE
        {
            NONE,
            VISITED,
            MOUSE,
            CHEESE
        }

        private class MazeElement
        {
            public ELEMENT_TYPE element_type { get; set; }
            public bool isVisited { get; set; }
            public ELEMENT_STATE element_state { get; set; }

            public MazeElement(ELEMENT_TYPE element_type)
            {
                this.element_type = element_type;
                element_state = ELEMENT_STATE.NONE;
                isVisited = false;
            }
        }

        private readonly MazeElement[,] MazeElements;

        public Maze(int maze_width, int maze_height)
        {
            this.maze_width = maze_width;
            this.maze_height = maze_height;

            maze = new string(new char[maze_width * maze_height]);
            MazeElements = new MazeElement[maze_width,maze_height];

            dirs = new DIRECTIONS[4];
            dirs[0] = DIRECTIONS.NORTH; // NORTH;
            dirs[1] = DIRECTIONS.EAST; // EAST;
            dirs[2] = DIRECTIONS.SOUTH; // SOUTH;
            dirs[3] = DIRECTIONS.WEST; // WEST;

            r = new Random();
        }

        public void AddMouse(int x = 1, int y = 1)
        {
            MazeElements[x, y].element_state = ELEMENT_STATE.MOUSE;
        }

        public void AddCheese(int x_min, int x_max, int y_min, int y_max)
        {
            int x, y;

            while (true)
            {
                x = r.Next(x_min, x_max);
                y = r.Next(y_min, y_max);

                if (MazeElements[x, y].element_type != ELEMENT_TYPE.BLOCK &&
                    MazeElements[x, y].element_state != ELEMENT_STATE.MOUSE)
                {
                    MazeElements[x, y].element_state = ELEMENT_STATE.CHEESE;
                    return;
                }
            }
        }

        public void Reset()
        {
            // Fills the maze with blocks

            for (int i = 0; i < maze_width * maze_height; i++)
            {
                maze = ChangeCharacter(maze, i, '█');
            }
        }


        // Convert linear chars to x/y
        private int XYToIndex(int x, int y)
        {
            return y * maze_width + x;
        }

        private bool IsInBounds(int x, int y)
        {
            // Returns "true" if x and y are both in-bounds.   
            if (x < 0 || x >= maze_width)
                return false;

            return (y >= 0 && y < maze_height);
        }

        public void Generate(int x = 1, int y = 1)
        {
            // Starting at the given index, recursively visits every direction in a    
            // randomized order. 
            if (x == 1 && y == 1)
                Reset();

            // Set my current location to be an empty passage.   
            maze = ChangeCharacter(maze, XYToIndex(x, y), SPACE);

            int rand;
            DIRECTIONS dir_temp;

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
                    case DIRECTIONS.NORTH:
                        dy = -1;
                        break;
                    case DIRECTIONS.SOUTH:
                        dy = 1;
                        break;
                    case DIRECTIONS.EAST:
                        dx = 1;
                        break;
                    case DIRECTIONS.WEST:
                        dx = -1;
                        break;
                }

                // Find coordinates 2 spaces from direction
                int x2 = x + (dx << 1);
                int y2 = y + (dy << 1);

                if (IsInBounds(x2, y2))
                {
                    if (maze[XYToIndex(x2, y2)] == BLOCK)
                    {
                        maze = ChangeCharacter(maze, XYToIndex(x2 - dx, y2 - dy), SPACE);
                        Generate(x2, y2);
                    }
                }
            }
        }

        public void Update()
        {
            char type;

            for (int y = 0; y < maze_height; ++y)
            {
                for (int x = 0; x < maze_width; ++x)
                {
                    type = maze[XYToIndex(x, y)];
                    MazeElements[x, y] = new MazeElement(GetElementType(type));
                }
            }
        }

        public void Display()
        {
            Console.Clear();

            for (int y = 0; y < maze_height; ++y)
            {
                for (int x = 0; x < maze_width; ++x)
                {
                    Console.Write(GetElementChar(MazeElements[x,y]));
                }
                Console.Write(Environment.NewLine);
            }
        }

        public static string ChangeCharacter(string sourceString, int charIndex, char newChar)
        {
            return (charIndex > 0 ? sourceString.Substring(0, charIndex) : "")
                   + newChar +
                   (charIndex < sourceString.Length - 1 ? sourceString.Substring(charIndex + 1) : "");
        }

        private static ELEMENT_TYPE GetElementType(char type)
        {
            return (type == BLOCK) ? ELEMENT_TYPE.BLOCK : ELEMENT_TYPE.SPACE;
        }

        private static char GetElementChar(MazeElement me)
        {
            if (me.element_type == ELEMENT_TYPE.BLOCK)
                return BLOCK;

            switch (me.element_state)
            {
                case ELEMENT_STATE.NONE: return SPACE;
                case ELEMENT_STATE.VISITED: return VISITED;
                case ELEMENT_STATE.CHEESE: return CHEESE;
                case ELEMENT_STATE.MOUSE: return MOUSE;
            }
            
            Console.WriteLine("Invalid Character - type:" + me.element_type + " state:" + me.element_state);
            return SPACE;
        }
    }
}
