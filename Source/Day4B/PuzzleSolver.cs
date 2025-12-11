
using System.Runtime.CompilerServices;
using Aoc25.Common;

namespace Aoc25.Day4B
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
        /// data structure that keeps track how many occupied neighbor cells each
        /// grid cell has. 
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
        /// Creates a 2-dimensional array that keeps track of how many occupied neighbor cells
        /// each occupied grid cell has, and a list that stores the coordinates of each occupied 
        /// cell in the grid.
        /// </summary>
        /// <param name="puzzleInput"> The puzzle input containing the grid. </param>
        /// <param name="gridColumnCount"> The number of columns in the puzzle grid. </param>
        /// <param name="gridRowCount">  The number of rows in the puzzle grid. </param>
        /// <returns> Data structures that store information about occupied grid cells. </returns>
        private static (Grid<byte>, FastList<(int Row, int Column)>) CreateGridCellOccupancyIndexes(
            string puzzleInput, 
            int gridColumnCount, 
            int gridRowCount)
        {
            // Stores how many occupied neighbor cells each occupied grid cell has. 
            var gridOccupiedCellNeighborCountLookup = new Grid<byte>(
                gridRowCount + (OccupancyGridMargin + OccupancyGridMargin),
                gridColumnCount + (OccupancyGridMargin + OccupancyGridMargin));

            // Stores the coordinates of each occupied cell in the grid.
            var gridOccupiedCellCoordinates = new FastList<(int Row, int Column)>((gridColumnCount * gridRowCount));

            int currentRow = OccupancyGridMargin; 
            int currentColumn = 0;

            // Process the puzzle input and store the coordinates of each occupied grid cell.
            // Also mark each occupied cell in our grid data structure. We will use these marks
            // later when determining how many occupied neighbor cells each occupied grid cell has. 
            for(int i = 0; i < puzzleInput.Length; i++)
            {
                char currentCharacter = puzzleInput[i];

                currentColumn++;

                if(currentCharacter == OccupiedCellCharacter)
                {
                    gridOccupiedCellNeighborCountLookup[currentRow, currentColumn] = 1;
                    gridOccupiedCellCoordinates.Add((currentRow, currentColumn));
                }
                else if(currentCharacter != EmptyCellCharacter)
                {
                    currentRow++;
                    currentColumn = 0;
                }
            }

            // Scans through each occupied grid cell and store how many occupied neighbor each has.
            for(int i = 0; i < gridOccupiedCellCoordinates.Count; i++)
            {
                int row = gridOccupiedCellCoordinates[i].Row;
                int column = gridOccupiedCellCoordinates[i].Column;

                gridOccupiedCellNeighborCountLookup[row, column] = (byte)(
                    ((gridOccupiedCellNeighborCountLookup[row-1, column-1] == 0) ? 0 : 1) +
                    ((gridOccupiedCellNeighborCountLookup[row-1, column] == 0) ? 0 : 1) +
                    ((gridOccupiedCellNeighborCountLookup[row-1, column+1] == 0) ? 0 : 1) +
                    ((gridOccupiedCellNeighborCountLookup[row, column-1] == 0) ? 0 : 1) +
                    gridOccupiedCellNeighborCountLookup[row, column+1] +
                    gridOccupiedCellNeighborCountLookup[row+1, column-1] +
                    gridOccupiedCellNeighborCountLookup[row+1, column] +
                    gridOccupiedCellNeighborCountLookup[row+1, column+1]
                );
            }

            return (gridOccupiedCellNeighborCountLookup, gridOccupiedCellCoordinates);
        }

        /// <summary>
        /// Count how many grid cells that can be removed by forklift.
        /// </summary>
        /// <param name="gridOccupiedCellNeighborCountLookup"> 
        /// 2-dimensional array that stores how many occupied neighbor cells each occupied grid cell has. 
        /// </param>
        /// <param name="gridOccupiedCellCoordinates"> 
        /// List that stores the coordinates of each occupied cell in the grid.
        /// </param>
        /// <returns> The number of cells that can be removed by forklift. </returns>
        private static uint CountForkliftClearableGridCells(
            Grid<byte> gridOccupiedCellNeighborCountLookup, 
            FastList<(int Row, int Column)> gridOccupiedCellCoordinates)
        {
            uint clearedCellCount = 0;
            int previousOccupiedCount;

            // Keep trying to clear cells as long as at least one cell was removed last iteration.
            do
            {
                previousOccupiedCount = gridOccupiedCellCoordinates.Count;

                // Check the occupied neighbor count of every occupied grid cell.
                for(int i = 0; i < gridOccupiedCellCoordinates.Count; i++)
                {
                    int row = gridOccupiedCellCoordinates[i].Row;
                    int column = gridOccupiedCellCoordinates[i].Column;

                    // Check how many occupied neighbors the current grid cell has, and 
                    // remove it if it has less than 4. If removed, we must decrement
                    // the occupied neighbor count of each adjacent grid cell.
                    if(gridOccupiedCellNeighborCountLookup[row, column] < 4)
                    {
                        gridOccupiedCellNeighborCountLookup[row-1, column-1]--;
                        gridOccupiedCellNeighborCountLookup[row-1, column]--;
                        gridOccupiedCellNeighborCountLookup[row-1, column+1]--;
                        gridOccupiedCellNeighborCountLookup[row, column-1]--;
                        gridOccupiedCellNeighborCountLookup[row, column] = 0;
                        gridOccupiedCellNeighborCountLookup[row, column+1]--;
                        gridOccupiedCellNeighborCountLookup[row+1, column-1]--;
                        gridOccupiedCellNeighborCountLookup[row+1, column]--;
                        gridOccupiedCellNeighborCountLookup[row+1, column+1]--;

                        gridOccupiedCellCoordinates.RemoveAt(i);
                        clearedCellCount++;
                        i--;
                    }
                }
                
            } while(gridOccupiedCellCoordinates.Count != previousOccupiedCount);
           
            return clearedCellCount;
        }

        /// <summary>
        /// Solve the puzzle using the passed puzzle input.
        /// </summary>
        /// <param name="puzzleInput"> The puzzle input to solve for. </param>
        /// <returns> The solution to the puzzle, solving for the passed puzzle input. </returns>
        public static ulong Solve(string puzzleInput)
        {
            var (gridWidth, gridHeight) = DetermineGridDimensions(puzzleInput);

            var (gridOccupiedCellNeighborCountLookup, gridOccupiedCellCoordinates) = CreateGridCellOccupancyIndexes(
                puzzleInput, 
                gridWidth,
                gridHeight);
                
            var forkliftClearableGridCellCount = CountForkliftClearableGridCells(
                gridOccupiedCellNeighborCountLookup, 
                gridOccupiedCellCoordinates);

            return (ulong)forkliftClearableGridCellCount;
        }
    }
}