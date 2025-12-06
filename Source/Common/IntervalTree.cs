using System.Collections;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Aoc25.Common
{
    public struct Interval<T> where T : IBinaryInteger<T>
    {
        T StartIndex;
        T EndIndex;
    }

    /// <summary>
    /// A fixed-size tree structure designed to allow fast .
    /// </summary>
    /// <typeparam name="T"> The type of items stored in the list. </typeparam>
    [DebuggerDisplay("Count = {count}")]
    public sealed class IntervalTree<T> : IEnumerable<Interval<T>> where T : IBinaryInteger<T>
    {

        private struct Node
        {
            Interval<T> interval;
        }

        /// <summary>
        /// The array that stores the nodes of the interval tree.
        /// </summary>
        private readonly Node[] nodes;

        /// <summary>
        /// The number of intervals stored in the interval tree.
        /// </summary>
        private int count;

        /// <summary>
        /// Gets the number of intervals stored in the interval tree.
        /// </summary>
        public int Count
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return count; }
        }

        /// <summary>
        /// Gets the capacity of the interval tree.
        /// </summary>
        public int Capacity
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return items.Length; }
        }

        /// <summary>
        /// Creates a new <see cref="IntervalTree{T}"/> with the specified fixed capacity.
        /// </summary>
        /// <param name="capacity"> The fixed capacity of the internal tree (must be a power of two). </param>
        public IntervalTree(int capacity)
        {
            
            if (!SpecializedMath.IsPowerOfTwo(capacity)) {
                capacity = BitOperations.RoundUpToPowerOf2(capacity);
            }
            items = new T[capacity];
        }

        /// <summary>
        /// Adds an item to the list.
        /// </summary>
        /// <param name="item"> The item to add. </param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add(T item)
        {
            items[count] = item;
            count++;
        }

        /// <summary>
        /// Determines if the list contains the specified item.
        /// </summary>
        /// <param name="item"> The item to check for. </param>
        /// <returns> True if the list contained the item, otherwise false. </returns>
        public bool Contains(T item)
        {
            for (int i = 0; i < count; i++)
            {
                if (EqualityComparer<T>.Default.Equals(item, items[i]))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Gets whether or not the list is filled to capacity.
        /// </summary>
        /// <returns> True if the list is filled to capacity, otherwise false. </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsFull() 
        {
            return (count == nodes.Length);
        }

        /// <summary>
        /// Gets whether or not the list is empty (e.g. contains zero items).
        /// </summary>
        /// <returns> True if the list is empty, otherwise false. </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsEmpty()
        {
            return (count == 0);
        }

        /// <summary>
        /// Removes all items stored in the list. 
        /// </summary>
        /// <remarks>
        /// Items are removed by simply setting list item count to zero.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Clear()
        {
            count = 0;
        }

        /// <summary>
        /// Gets an enumerator for enumerating the content of the list.
        /// </summary>
        /// <returns> An enumerator for enumerating the content of the list. </returns>
        public Enumerator GetEnumerator()
        {
            return new Enumerator(this);
        }

        /// <summary>
        /// Gets an enumerator for enumerating the content of the list.
        /// </summary>
        /// <returns> An enumerator for enumerating the content of the list. </returns>
        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Gets an enumerator for enumerating the content of the list.
        /// </summary>
        /// <returns> An enumerator for enumerating the content of the list. </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// An enumerator for enumerating the content of a <see cref="IntervalTree{T}"/>.
        /// </summary>
        public struct Enumerator : IEnumerator<Interval<T>>
        {
            /// <summary>
            /// The <see cref="IntervalTree{T}"/> instance being enumerated.
            /// </summary>
            private readonly IntervalTree<T> tree;

            /// <summary>
            /// Index of the current item in the enumeration.
            /// </summary>
            private int currentIndex;

            /// <summary>
            /// Gets the current item in the enumeration.
            /// </summary>
            public T Current
            {
                get { return list[currentIndex]; }
            }

            /// <summary>
            /// Gets the current item in the enumeration.
            /// </summary>
            object IEnumerator.Current
            {
                get { return Current!; }
            }

            /// <summary>
            /// Creates a new enumerator for the specified list.
            /// </summary>
            /// <param name="list"> The list to enumerate. </param>
            public Enumerator(FastList<T> list)
            {
                this.list = list;
                currentIndex = -1;
            }

            /// <summary>
            /// Moves to the next item in the enumeration.
            /// </summary>
            /// <returns> True if the enumeration had another item. </returns>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool MoveNext()
            {
                return ++currentIndex < list.count;
            }

            /// <summary>
            /// Resets the enumerator.
            /// </summary>
            public void Reset()
            {
                currentIndex = -1;
            }

            /// <summary>
            /// Disposes of the enumerator.
            /// </summary>
            public void Dispose()
            {
                // Nothing to dispose of.
            }
        }
    }
}
