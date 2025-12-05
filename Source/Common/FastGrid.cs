
using System.Runtime.CompilerServices;

namespace Aoc25.Common
{
    /// <summary>
    /// A 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class FastGrid<T>
    {
        /// <summary>
        /// Stores the 2-dimensional data in a 1-dimensional array.
        /// </summary>
        private readonly T[] data;

        /// <summary>
        /// Gets the width of the grid
        /// </summary>
        public int ColumnCount
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;
            init;
        } 
        
        /// <summary>
        /// Gets the height of the grid.
        /// </summary>
        public int RowCount
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;
            init;
        }

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
        /// 
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public FastGrid(int width, int height)
        {
            ColumnCount = width;
            RowCount = height;

            data = new T[width * height];
        }
    }
}