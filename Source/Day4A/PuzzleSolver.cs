
using System.Runtime.CompilerServices;
using Aoc25.Common;

namespace Aoc25.Day4A
{
    /// <summary>
    /// Contains the puzzle solving code.
    /// </summary>
    public static class PuzzleSolver
    {
        /// <summary>
        /// The character used to represent an empty grid cell.
        /// </summary>
        private const char EmptyCellCharacter = '.';

        /// <summary>
        /// The character used to represent an occupied grid cell.
        /// </summary>
        private const char OccupiedCellCharacter = '@';

        /// <summary>
        /// The size of the extra margin to allocate for the 2-dimensional grid 
        /// data structure that keeps track of which grid cells are occupied.
        /// </summary>
        /// <remarks>
        /// This margin simplifies lookup of a grid cell's neighbor cells,
        /// as we don't have to worry about going outside of the grid's bounds
        /// when doing grid[currentCellRow - 1, currentCellColumn], for example.
        /// </remarks>
        private const int OccupancyGridMargin = 1;

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
        private static (int gridColumnCount, int gridRowCount) DetermineGridDimensions(string puzzleInput)
        {
            int gridColumnCount = 0;

            // Determine the width of a single grid row of the input.
            while (CharacterIsGridCell(puzzleInput[gridColumnCount])) {
                gridColumnCount++;
            }

            // Determines the height of the grid in the input.
            // Assumption: Each row consists of [gridWidth] cells plus a single newline.
            int gridRowCount = (puzzleInput.Length / (gridColumnCount + 1));

            return(gridColumnCount, gridRowCount);
        }

        /// <summary>
        /// Creates a 2-dimensional array that keeps track of which grid cells are occupied.
        /// </summary>
        /// <param name="puzzleInput"> The puzzle input containing the grid. </param>
        /// <param name="gridColumnCount"> The number of columns in the puzzle grid. </param>
        /// <param name="gridRowCount">  The number of rwos in the puzzle grid. </param>
        /// <returns> A 2-dimensional array that keeps track of which grid cells are occupied. </returns>
        private static Grid<byte> CreateGridOccupancyIndex(string puzzleInput, int gridColumnCount, int gridRowCount)
        {
            // Allocate a 2-dimensional data structure to keep track of which cells are occupied 
            // and which are empty. Also allocate a margin around the entire grid. This makes 
            // checking occupancy of neighboring cells of a cell easier later.  
            var gridOccupancyLookup = new Grid<byte>(
                gridColumnCount + (OccupancyGridMargin + OccupancyGridMargin), 
                gridRowCount + (OccupancyGridMargin + OccupancyGridMargin));

            int currentRow = OccupancyGridMargin; 
            int currentColumn = 0;

            for(int i = 0; i < puzzleInput.Length; i++)
            {
                char currentCharacter = puzzleInput[i];

                currentColumn++;

                if(currentCharacter == OccupiedCellCharacter)
                {
                    gridOccupancyLookup[currentRow, currentColumn] = 1;
                }
                else if(currentCharacter != EmptyCellCharacter)
                {
                    currentRow++;
                    currentColumn = 0;
                }
            }

            return gridOccupancyLookup;
        }

        /// <summary>
        /// Count the number of grid cells accessible by forklift (occupied cells with fewer 4 occupied neighbors).
        /// </summary>
        /// <param name="gridCellOccupancyIndex"> A 2-dimensional array that keeps track of which grid cells are occupied. </param>
        /// <returns> The number of grid cells accessible by forklift. </returns>
        private static uint CountForkliftAccessibleGridCells(Grid<byte> gridCellOccupancyIndex)
        {
            uint accessibleGridCells = 0;

            // Enumerate every row in the grid (skipping the margins added when creating the grid occupancy lookup).
            for(int row = OccupancyGridMargin; row < (gridCellOccupancyIndex.RowCount - OccupancyGridMargin); row++)
            {
                // Enumerate every column in the grid (skipping the margins added when creating the grid occupancy lookup).
                for(int column = OccupancyGridMargin; column < (gridCellOccupancyIndex.ColumnCount - OccupancyGridMargin); column++)
                {
                    if(gridCellOccupancyIndex[row, column] != 0)
                    {
                        var occupiedNeighbourCount = (
                            gridCellOccupancyIndex[row-1, column-1] +
                            gridCellOccupancyIndex[row-1, column] +
                            gridCellOccupancyIndex[row-1, column+1] +
                            gridCellOccupancyIndex[row, column-1] +
                            gridCellOccupancyIndex[row, column+1] +
                            gridCellOccupancyIndex[row+1, column-1] +
                            gridCellOccupancyIndex[row+1, column] +
                            gridCellOccupancyIndex[row+1, column+1]);

                        if(occupiedNeighbourCount < 4) {
                            accessibleGridCells ++;
                        }
                    }
                } 
            } 

            return accessibleGridCells;
        }

        /// <summary>
        /// Solve the puzzle using the passed puzzle input.
        /// </summary>
        /// <param name="puzzleInput"> The puzzle input to solve for. </param>
        /// <returns> The solution to the puzzle, solving for the passed puzzle input. </returns>
        public static ulong Solve(string puzzleInput)
        {
            var (gridColumnCount, gridRowCount) = DetermineGridDimensions(puzzleInput);
            var gridCellOccupancyIndex = CreateGridOccupancyIndex(puzzleInput, gridColumnCount, gridRowCount);
            var forkliftAccessibleGridCellCount = CountForkliftAccessibleGridCells(gridCellOccupancyIndex);

            return (ulong)forkliftAccessibleGridCellCount;
        }
    }
}