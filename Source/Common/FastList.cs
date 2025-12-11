using System.Collections;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Aoc25.Common
{
    /// <summary>
    /// <para> 
    /// A fast, fixed-size list of items that is faster than the normal list that comes 
    /// with .NET. The extra speeds comes from omission of error checking (correct usage 
    /// is assumed), not allowing the list to be automatically resized when inserting
    /// new items and by relaxing certain constraints imposed by normal list implementations.
    /// </para>
    /// <para>
    /// The list will skip nulling / zeroing out deleted items, and the ordering of items in 
    /// the list is unstable, meaning their order may be altered when performing certain list 
    /// operations. By not enforcing a stable ordering of items, some list operations can be 
    /// performed much more efficiently. Most notably, insertion and removal by index can be 
    /// performed without having to forward or backwards shift items in the list. Instead, we can simply 
    /// overwrite the deleted item with the last item in the last, and decrement the list count.
    /// </para>
    /// </summary>
    /// <typeparam name="T"> The type of items stored in the list. </typeparam>
    [DebuggerDisplay("Count = {count}")]
    public sealed class FastList<T> : IEnumerable<T>
    {
        /// <summary>
        /// The array that stores the content of the list.
        /// </summary>
        private readonly T[] items;

        /// <summary>
        /// The number of items stored in the list.
        /// </summary>
        private int count;

        /// <summary>
        /// Gets the number of items stored in the list.
        /// </summary>
        public int Count
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return count; }
        }

        /// <summary>
        /// Gets the capacity of the list.
        /// </summary>
        public int Capacity
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return items.Length; }
        }

        /// <summary>
        /// Gets or sets the item at the specified index in the list.
        /// </summary>
        /// <param name="index"> The index of the item to get or set. </param>
        /// <returns> The item stored at the specified index. </returns>
        public T this[int index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return items[index]; }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set { items[index] = value; }
        }

        /// <summary>
        /// Creates a new <see cref="FastList{T}"/> with the specified fixed capacity.
        /// </summary>
        /// <param name="capacity"> The fixed capacity of the list. </param>
        public FastList(int capacity)
        {
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
        /// Inserts an item into the list at the specified index.
        /// </summary>
        /// <remarks>
        /// The item currently stored at the specified index will be be displaced
        /// to the back of the list, making room to insert the new item.
        /// </remarks>
        /// <param name="index"> The index to insert the item at. </param>
        /// <param name="item"> The item to insert. </param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Insert(int index, T item)
        {
            items[count] = items[index];
            items[index] = item;
            count++;
        }

        /// <summary>
        /// Removes the item at the specified index in the list. 
        /// </summary>
        /// <remarks>
        /// The item stored at the specified index will be overwritten by the last 
        /// item in the list, after which the list item count will be decremented.
        /// to the back of the list, making room to insert the new item.
        /// </remarks>
        /// <param name="index"> The index of the item to remove. </param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void RemoveAt(int index)
        {
            items[index] = items[--count];
        }

        /// <summary>
        /// Removes the first occurrence of a specific item from the list.
        /// </summary>
        /// <param name="item"> The item to remove. </param>
        /// <returns> True if an item was removed, otherwise false. </returns>
        public bool Remove(T item)
        {
            for (int i = 0; i < count; i++)
            {
                if (EqualityComparer<T>.Default.Equals(item, items[i]))
                {
                    RemoveAt(i);
                    return true;
                }
            }

            return false;
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
            return (count == items.Length);
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
        /// Gets the list as a mutable span.
        /// </summary>
        /// <returns> A span over the list data. </returns>
        public Span<T> AsSpan()
        {
            return new Span<T>(items, 0, count);
        }

        /// <summary>
        /// Gets the list as a immutable span.
        /// </summary>
        /// <returns> A span over the list data. </returns>
        public ReadOnlySpan<T> AsReadOnlySpan()
        {
            return new ReadOnlySpan<T>(items, 0, count);
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
        /// An enumerator for enumerating the content of a <see cref="FastList{T}"/>.
        /// </summary>
        public struct Enumerator : IEnumerator<T>
        {
            /// <summary>
            /// The <see cref="FastList{T}"/> instance being enumerated.
            /// </summary>
            private readonly FastList<T> list;

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
