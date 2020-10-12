using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MazeAI
{
    class Maze
    {

        private string grid = new string(new char[DefineConstants.GRID_WIDTH * DefineConstants.GRID_HEIGHT]);

        public void ResetGrid()
        {
            // Fills the grid with walls ('#' characters). 

            for (int i = 0; i < DefineConstants.GRID_WIDTH * DefineConstants.GRID_HEIGHT; ++i)
            {
                grid = StringFunctions.ChangeCharacter(grid, i, (char) DefineConstants.BLOCK);
            }
        }


        private int XYToIndex(int x, int y)
        {
            // Converts the two-dimensional index pair (x,y) into a   // single-dimensional index.  The result is y * ROW_WIDTH + x.   
            return y * DefineConstants.GRID_WIDTH + x;
        }


        private bool IsInBounds(int x, int y)
        {
            // Returns "true" if x and y are both in-bounds.   
            if (x < 0 || x >= DefineConstants.GRID_WIDTH)
            {
                return false;
            }

            if (y < 0 || y >= DefineConstants.GRID_HEIGHT)
            {
                return false;
            }

            return true;
        }


        // This is the recursive function we will code in the next project 
        public void Visit(int x, int y)
        {
            // Starting at the given index, recursively visits every direction in a    
            // randomized order. 

            // Set my current location to be an empty passage.   
            grid = StringFunctions.ChangeCharacter(grid, XYToIndex(x, y), ' ');

            // Create an local array containing the 4 directions and shuffle their order.   
            int[] dirs = new int[4];
            dirs[0] = DefineConstants.NORTH;
            dirs[1] = DefineConstants.EAST;
            dirs[2] = DefineConstants.SOUTH;
            dirs[3] = DefineConstants.WEST;

            for (int i = 0; i < 4; ++i)
            {
                int r = RandomNumbers.NextNumber() & 3;
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
                    case DefineConstants.NORTH:
                        dy = -1;
                        break;
                    case DefineConstants.SOUTH:
                        dy = 1;
                        break;
                    case DefineConstants.EAST:
                        dx = 1;
                        break;
                    case DefineConstants.WEST:
                        dx = -1;
                        break;
                }

                // Find the (x,y) coordinates of the grid cell 2 spots 
                // away in the given direction. 
                int x2 = x + (dx << 1);
                int y2 = y + (dy << 1);

                if (IsInBounds(x2, y2))
                {
                    if (grid[XYToIndex(x2, y2)] == (char) DefineConstants.BLOCK)
                    {
                        // (x2,y2) has not been visited yet... knock down the
                        // wall between my current position and that position         
                        grid = StringFunctions.ChangeCharacter(grid, XYToIndex(x2 - dx, y2 - dy), ' ');

                        // Recursively Visit (x2,y2) 
                        Visit(x2, y2);
                    }
                }
            }
        }

        public void PrintGrid()
        {
            // Displays the finished maze to the screen.
            for (int y = 0; y < DefineConstants.GRID_HEIGHT; ++y)
            {
                for (int x = 0; x < DefineConstants.GRID_WIDTH; ++x)
                {
                    Console.Write(grid[XYToIndex(x, y)]);
                }

                Console.Write("\n");
            }
        }

        //private static void Main()
        //{
        //    // Starting point and top-level control. 
        //    RandomNumbers.Seed(time(0)); // seed random number generator.
        //    ResetGrid();
        //    Visit(1, 1);
        //    PrintGrid();
        //}

        internal static class DefineConstants
        {
            public const int GRID_WIDTH = 79;
            public const int GRID_HEIGHT = 23;
            public const int NORTH = 0;
            public const int EAST = 1;
            public const int SOUTH = 2;
            public const int WEST = 3;
            public const int BLOCK = 178;
        }

        //Helper class added by C++ to C# Converter:

        //----------------------------------------------------------------------------------------
        //	Copyright © 2006 - 2020 Tangible Software Solutions, Inc.
        //	This class can be used by anyone provided that the copyright notice remains intact.
        //
        //	This class provides the ability to replicate the behavior of the C/C++ functions for 
        //	generating random numbers, using the .NET Framework System.Random class.
        //	'rand' converts to the parameterless overload of NextNumber
        //	'random' converts to the single-parameter overload of NextNumber
        //	'randomize' converts to the parameterless overload of Seed
        //	'srand' converts to the single-parameter overload of Seed
        //----------------------------------------------------------------------------------------
        internal static class RandomNumbers
        {
            private static System.Random r;

            public static int NextNumber()
            {
                if (r == null)
                    Seed();

                return r.Next();
            }

            public static int NextNumber(int ceiling)
            {
                if (r == null)
                    Seed();

                return r.Next(ceiling);
            }

            public static void Seed()
            {
                r = new System.Random();
            }

            public static void Seed(int seed)
            {
                r = new System.Random(seed);
            }
        }

        //Helper class added by C++ to C# Converter:

        //----------------------------------------------------------------------------------------
        //	Copyright © 2006 - 2020 Tangible Software Solutions, Inc.
        //	This class can be used by anyone provided that the copyright notice remains intact.
        //
        //	This class provides the ability to replicate various classic C string functions
        //	which don't have exact equivalents in the .NET Framework.
        //----------------------------------------------------------------------------------------
        internal static class StringFunctions
        {
            //------------------------------------------------------------------------------------
            //	This method allows replacing a single character in a string, to help convert
            //	C++ code where a single character in a character array is replaced.
            //------------------------------------------------------------------------------------
            public static string ChangeCharacter(string sourceString, int charIndex, char newChar)
            {
                return (charIndex > 0 ? sourceString.Substring(0, charIndex) : "")
                       + newChar.ToString() +
                       (charIndex < sourceString.Length - 1 ? sourceString.Substring(charIndex + 1) : "");
            }

            //------------------------------------------------------------------------------------
            //	This method replicates the classic C string function 'isxdigit' (and 'iswxdigit').
            //------------------------------------------------------------------------------------
            public static bool IsXDigit(char character)
            {
                if (char.IsDigit(character))
                    return true;
                else if ("ABCDEFabcdef".IndexOf(character) > -1)
                    return true;
                else
                    return false;
            }

            //------------------------------------------------------------------------------------
            //	This method replicates the classic C string function 'strchr' (and 'wcschr').
            //------------------------------------------------------------------------------------
            public static string StrChr(string stringToSearch, char charToFind)
            {
                int index = stringToSearch.IndexOf(charToFind);
                if (index > -1)
                    return stringToSearch.Substring(index);
                else
                    return null;
            }

            //------------------------------------------------------------------------------------
            //	This method replicates the classic C string function 'strrchr' (and 'wcsrchr').
            //------------------------------------------------------------------------------------
            public static string StrRChr(string stringToSearch, char charToFind)
            {
                int index = stringToSearch.LastIndexOf(charToFind);
                if (index > -1)
                    return stringToSearch.Substring(index);
                else
                    return null;
            }

            //------------------------------------------------------------------------------------
            //	This method replicates the classic C string function 'strstr' (and 'wcsstr').
            //------------------------------------------------------------------------------------
            public static string StrStr(string stringToSearch, string stringToFind)
            {
                int index = stringToSearch.IndexOf(stringToFind);
                if (index > -1)
                    return stringToSearch.Substring(index);
                else
                    return null;
            }

            //------------------------------------------------------------------------------------
            //	This method replicates the classic C string function 'strtok' (and 'wcstok').
            //	Note that the .NET string 'Split' method cannot be used to replicate 'strtok' since
            //	it doesn't allow changing the delimiters between each token retrieval.
            //------------------------------------------------------------------------------------
            private static string activeString;
            private static int activePosition;

            public static string StrTok(string stringToTokenize, string delimiters)
            {
                if (stringToTokenize != null)
                {
                    activeString = stringToTokenize;
                    activePosition = -1;
                }

                //the stringToTokenize was never set:
                if (activeString == null)
                    return null;

                //all tokens have already been extracted:
                if (activePosition == activeString.Length)
                    return null;

                //bypass delimiters:
                activePosition++;
                while (activePosition < activeString.Length && delimiters.IndexOf(activeString[activePosition]) > -1)
                {
                    activePosition++;
                }

                //only delimiters were left, so return null:
                if (activePosition == activeString.Length)
                    return null;

                //get starting position of string to return:
                int startingPosition = activePosition;

                //read until next delimiter:
                do
                {
                    activePosition++;
                } while (activePosition < activeString.Length &&
                         delimiters.IndexOf(activeString[activePosition]) == -1);

                return activeString.Substring(startingPosition, activePosition - startingPosition);
            }
        }
    }
}
