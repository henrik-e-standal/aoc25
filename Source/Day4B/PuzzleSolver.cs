
using System.Runtime.CompilerServices;
using Aoc25.Common;

namespace Aoc25.Day4B
{
    /// <summary>
    /// Contains the puzzle solving code.
    /// </summary>
    internal static class PuzzleSolver
    {
        private const char EmptyCellCharacter = '.';

        private const char OccupiedCellCharacter = '@';

        private const int OccupancyGridMargin = 1;

        /// <summary>
        /// Attempts to get the numeric number of a character.
        /// </summary>
        /// <param name="character"> The character whose numeric value to get. </param>
        /// <param name="numericValue"> The numeric value of the character. </param>
        /// <returns> True if the character was a number, otherwise false. </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool TryGetCharacterNumericValue(char character, out uint numericValue)
        {
            numericValue = (uint)(character - '0');
            return (numericValue <= 9);
        }

        /// <summary>
        /// Determines whether a character represents a grid cell.
        /// </summary>
        /// <param name="character"> The character to check. </param>
        /// <returns> True if the character represents a grid cell, otherwise false. </returns>
        private static bool CharacterIsGridCell(char character)
        {
            return (character >= EmptyCellCharacter) && (character <= OccupiedCellCharacter);
        }

        /// <summary>
        /// Determines the width and height of the grid passed as puzzle input.
        /// </summary>
        /// <param name="puzzleInput"> The puzzle input containing the grid. </param>
        /// <returns> The width and height of the puzzle grid. </returns>
        private static (int gridWidth, int gridHeight) DetermineGridDimensions(string puzzleInput)
        {
            int gridWidth = 0;

            // Determine the width of a single grid row of the input.
            while (CharacterIsGridCell(puzzleInput[gridWidth])) {
                gridWidth++;
            }

            // Determines the height of the grid in the input.
            // Assumption: Each row consists of [gridWidth] cells plus a single newline.
            int gridHeight = (puzzleInput.Length / (gridWidth + 1));

            return(gridWidth, gridHeight);
        }

        /// <summary>
        /// Creates a 2-dimensional grid 
        /// </summary>
        /// <param name="puzzleInput"></param>
        /// <param name="gridWidth"></param>
        /// <param name="gridHeight"></param>
        /// <returns></returns>
        private static (FastGrid<byte> gridOccupancyLookup, FastList<(int row, int column)> X)
            CreateGridCellOccupancyIndexes(string puzzleInput, int gridWidth, int gridHeight)
        {
            // Allocate a two dimensional data structure to store which cells are occupied 
            // and which are empty. Also allocate a margin around the entire grid. This makes 
            // checking occupancy of neighboring cells of a cell easier later.  
            var gridOccupancyLookup = new FastGrid<byte>(
                gridWidth + OccupancyGridMargin + OccupancyGridMargin, 
                gridHeight + OccupancyGridMargin + OccupancyGridMargin);

            // 
            var gridOccupancyList = new FastList<(int, int)>((gridWidth * gridHeight));

            int currentRow = OccupancyGridMargin; 
            int currentColumn = 0;

            for(int i = 0; i < puzzleInput.Length; i++)
            {
                char currentCharacter = puzzleInput[i];

                currentColumn++;

                if(currentCharacter == OccupiedCellCharacter)
                {
                    gridOccupancyLookup[currentRow, currentColumn] = 1;
                    gridOccupancyList.Add((currentRow, currentColumn));
                }
                else if(currentCharacter != EmptyCellCharacter)
                {
                    currentRow++;
                    currentColumn = 0;
                }
            }

            return (gridOccupancyLookup, gridOccupancyList);
        }

        /// <summary>
        /// Count the number of grid cells accessible by forklift (occupied cells with fewer 4 occupied neighbors).
        /// </summary>
        /// <param name="gridOccupancyLookup"></param>
        /// <returns> The </returns>
        private static uint CountForkliftAccessibleGridCells(FastGrid<byte> gridOccupancyLookup, FastList<(int Row, int Column)> gridOccupancyList)
        {
            uint rollsRemoved = 0;
            int previousOccupiedCount;

            // Keeps looking as long as 
            do
            {
                previousOccupiedCount = gridOccupancyList.Count;

                for(int i = 0; i < gridOccupancyList.Count; i++)
                {
                    int row = gridOccupancyList[i].Row;
                    int column = gridOccupancyList[i].Column;

                    var occupiedNeighbourCount = (
                        gridOccupancyLookup[row-1, column-1] +
                        gridOccupancyLookup[row-1, column] +
                        gridOccupancyLookup[row-1, column+1] +
                        gridOccupancyLookup[row, column-1] +
                        gridOccupancyLookup[row, column+1] +
                        gridOccupancyLookup[row+1, column-1] +
                        gridOccupancyLookup[row+1, column] +
                        gridOccupancyLookup[row+1, column+1]);

                    if(occupiedNeighbourCount < 4) 
                    {
                        gridOccupancyList.RemoveAt(i);
                        gridOccupancyLookup[row, column] = 0;
                        rollsRemoved++;
                        i--;
                    }
                }
                
            } while(gridOccupancyList.Count != previousOccupiedCount);
           
            return rollsRemoved;
        }

        /// <summary>
        /// Solve the puzzle using the passed puzzle input.
        /// </summary>
        /// <param name="puzzleInput"> The puzzle input to solve for. </param>
        /// <returns> The solution to the puzzle, solving for the passed puzzle input. </returns>
        public static ulong Solve(string puzzleInput)
        {
            var (gridWidth, gridHeight) = DetermineGridDimensions(puzzleInput);

            var (gridCellOccupancyLookup, gridCellOccupiesCoordinates) = CreateGridCellOccupancyIndexes(
                puzzleInput, 
                gridWidth,
                gridHeight);
                
            var forkliftAccessibleGridCellCount = CountForkliftAccessibleGridCells(
                gridCellOccupancyLookup, 
                gridCellOccupiesCoordinates);

            return (ulong)forkliftAccessibleGridCellCount;
        }
    }
}