
using System.Runtime.CompilerServices;

namespace Aoc25.Common
{
    /// <summary>
    /// A fixed-size two-dimensional array.
    /// </summary>
    /// <remarks>
    /// Extend the implementation with new functionality as needed.
    /// </remarks>
    /// <typeparam name="T"> The type of items stored in the grid. </typeparam>
    public sealed class FastGrid<T>
    {
        /// <summary>
        /// Stores the contents of the grid.
        /// </summary>
        /// <remarks>
        /// We store the 2-dimensional data in a 1-dimensional array.
        /// To calculate an array index from a (row, column) tuple:
        /// f(row, column) = (row * column_count) + column
        /// </remarks>
        private readonly T[] data;

        /// <summary>
        /// Gets the number of columns in the grid.
        /// </summary>
        public int ColumnCount
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;
            init;
        } 
        
        /// <summary>
        /// Gets the number of rows in the grid.
        /// </summary>
        public int RowCount
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;
            init;
        }

        /// <summary>
        /// Gets or sets the item at the specified position in the grid.
        /// </summary>
        /// <param name="row"> The row of the item to get or set. </param>
        /// <param name="column"> The column of the item to get or set. </param>
        /// <returns> The value stored at the specified position in the grid. </returns>
        public T this[int row, int column]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return data[(row * ColumnCount) + column]; }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set { data[(row * ColumnCount) + column] = value; }
        }

        /// <summary>
        /// Creates a new <see cref="FastGrid{T}"/> with the specified dimensions.
        /// </summary>
        /// <param name="columnCount"> The number of columns in the grid. </param>
        /// <param name="rowCount"> The number of rows in the grid. </param>
        public FastGrid(int columnCount, int rowCount)
        {
            ColumnCount = columnCount;
            RowCount = rowCount;

            data = new T[columnCount * rowCount];
        }
 
        /// <summary>
        /// Removes all items stored in the grid. 
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Clear()
        {
            Array.Clear(data);
        }
    }
}