using System.Collections;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Aoc25.Common
{
    /// <summary>
    /// A fast, fixed-size queue of items that is intended to be faster than the normal queue 
    /// that comes with .NET. The extra speeds comes from omission of error checking (correct 
    /// usage is assumed) and not allowing the queue to be automatically resized when inserting
    /// new items. It also requires the fixed capacity to be a power of two.
    /// </summary>
    /// <typeparam name="T"> The type of items stored in the queue. </typeparam>
    [DebuggerDisplay("Count = {count}")]
    public sealed class FastQueue<T> : IEnumerable<T>
    {
        /// <summary>
        /// The array that stores the content of the queue.
        /// </summary>
        private readonly T[] items;

        /// <summary>
        /// The number of items stored in the queue.
        /// </summary>
        private int count;

        /// <summary>
        /// The index of the item stored at the back of the queue.
        /// </summary>
        private uint tailIndex;

        /// <summary>
        /// The index of the item stored at the front of the queue.
        /// </summary>
        private uint headIndex;

        /// <summary>
        /// The capacity of the queue minus one. 
        /// </summary>
        /// <remarks> Used in  </remarks>
        private uint capacityMinusOne;

        /// <summary>
        /// Gets the number of items stored in the queue.
        /// </summary>
        public int Count
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return count; }
        }

        /// <summary>
        /// Gets the capacity of the queue.
        /// </summary>
        public int Capacity
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return items.Length; }
        }

        /// <summary>
        /// Creates a new <see cref="FastQueue{T}"/> with the specified fixed, power-of-two capacity.
        /// </summary>
        /// <param name="capacity"> The fixed, power-of-two capacity of the queue. </param>
        public FastQueue(int capacity)
        {
            Debug.Assert(SpecializedMath.IsPowerOfTwo(capacity));

            items = new T[capacity];
            capacityMinusOne = (uint)(capacity - 1U);
            headIndex = capacityMinusOne;
        }

        /// <summary>
        /// Adds an item to the back of the queue.
        /// </summary>
        /// <param name="item"> The item to add. </param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void EnqueueBack(T item)
        {         
            tailIndex = (tailIndex == 0) ? capacityMinusOne : (tailIndex - 1U);
            items[tailIndex] = item; 
            count++;
        }

        /// <summary>
        /// Adds an item to the front of the queue.
        /// </summary>
        /// <param name="item"> The item to add. </param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void EnqueueFront(T item)
        {
            headIndex = (headIndex + 1U) & capacityMinusOne;
            items[headIndex] = item;
            count++;
        }

        /// <summary>
        /// Retrieves the item stored at the back of the queue (without removing it).
        /// </summary>
        /// <returns> The item stored at the back of the queue. </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T PeekBack()
        {
            return items[tailIndex];
        }

        /// <summary>
        /// Retrieves the item stored at the front of the queue (without removing it).
        /// </summary>
        /// <returns> The item stored at the front of the queue. </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T PeekFront()
        {
            return items[headIndex];
        }

        /// <summary>
        /// Attempts to retrieve the item stored at the back of the queue (without removing it).
        /// </summary>
        /// <param name="item" The retrieved item, if the operation is successful. </param>
        /// <returns> True if an item could be retrieved from the queue, false if the queue was empty. </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryPeekBack([MaybeNullWhen(false)] out T? item)
        {
            if(count == 0)
            {
                item = default(T);
                return false;
            }

            item = PeekBack();
            return true;
        }
    
        /// <summary>
        /// Attempts to retrieve the item stored at the front of the queue (without removing it).
        /// </summary>
        /// <param name="item" The retrieved item, if the operation is successful. </param>
        /// <returns> True if an item could be retrieved from the queue, false if the queue was empty. </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryPeekFront([MaybeNullWhen(false)] out T? item)
        {
            if(count == 0)
            {
                item = default(T);
                return false;
            }
            
            item = PeekFront();
            return true;
        }

        /// <summary>
        /// Removes the item stored at the back of the queue and returns it.
        /// </summary>
        /// <returns> The removed item. </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T DequeueBack()
        {
            uint removedIndex = tailIndex;

            count--;
            tailIndex = (tailIndex + 1U) & capacityMinusOne;

            return items[removedIndex];
        }
   
        /// <summary>
        /// Removes the item stored at the front of the queue and returns it.
        /// </summary>
        /// <returns> The removed item. </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T DequeueFront()
        {
            uint removedIndex = headIndex;
 
            count--;
            headIndex = (headIndex == 0) ? capacityMinusOne : (headIndex - 1); 

            return items[removedIndex];
        }    
  
        /// <summary>
        /// Attempts to remove the item stored at the front of the queue and return it.
        /// </summary>
        /// <param name="item" The removed item, if the operation is successful. </param>
        /// <returns> True if an item was removed from the queue, false if the queue was empty. </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryDequeueBack([MaybeNullWhen(false)] out T? item)
        {
            if(count == 0)
            {
                item = default(T);
                return false;
            }
            
            item = DequeueBack();
            return true;
        }
  
        /// <summary>
        /// Attempts to remove the item stored at the back of the queue and return it.
        /// </summary>
        /// <param name="item" The removed item, if the operation is successful. </param>
        /// <returns> True if an item was removed from the queue, false if the queue was empty. </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryDequeueFront([MaybeNullWhen(false)] out T? item)
        {
            if(count == 0)
            { 
                item = default(T);
                return false;
            }

            item = DequeueFront();
            return true;
        }   
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void RemoveBack()
        {
            count--;
            tailIndex = (tailIndex + 1U) & capacityMinusOne;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void RemoveFront()
        {
            count--;
            headIndex = (headIndex == 0) ? capacityMinusOne : (headIndex - 1); 
        }

        /// <summary>
        /// Gets whether or not the queue is filled to capacity.
        /// </summary>
        /// <returns> True if the queue is filled to capacity, otherwise false. </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsFull() 
        {
            return (count == items.Length);
        }

        /// <summary>
        /// Gets whether or not the queue is empty (e.g. contains zero items).
        /// </summary>
        /// <returns> True if the queue is empty, otherwise false. </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsEmpty()
        {
            return (count == 0);
        }

        /// <summary>
        /// Removes all items stored in the queue. 
        /// </summary>
        /// <remarks>
        /// Items are removed by simply setting queue item count to zero.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Clear()
        {
            count = 0;
        }

        /// <summary>
        /// Gets an enumerator for enumerating the content of the queue.
        /// </summary>
        /// <returns> An enumerator for enumerating the content of the queue. </returns>
        public Enumerator GetEnumerator()
        {
            return new Enumerator(this);
        }

        /// <summary>
        /// Gets an enumerator for enumerating the content of the queue.
        /// </summary>
        /// <returns> An enumerator for enumerating the content of the queue. </returns>
        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Gets an enumerator for enumerating the content of the queue.
        /// </summary>
        /// <returns> An enumerator for enumerating the content of the queue. </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// An enumerator for enumerating the content of a <see cref="FastQueue{T}"/>.
        /// </summary>
        public struct Enumerator : IEnumerator<T>
        {
            /// <summary>
            /// The <see cref="FastQueue{T}"/> instance being enumerated.
            /// </summary>
            private readonly FastQueue<T> queue;

            /// <summary>
            /// Gets the current item in the enumeration.
            /// </summary>
            public T Current
            {
                get { return queue.PeekFront(); }
            }

            /// <summary>
            /// Gets the current item in the enumeration.
            /// </summary>
            object IEnumerator.Current
            {
                get { return Current!; }
            }

            /// <summary>
            /// Creates a new enumerator for the specified queue.
            /// </summary>
            /// <param name="queue"> The queue to enumerate. </param>
            public Enumerator(FastQueue<T> queue)
            {
                this.queue = queue;
            }

            /// <summary>
            /// Moves to the next item in the enumeration.
            /// </summary>
            /// <returns> True if the enumeration had another item. </returns>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool MoveNext()
            {
                return false;
            }

            /// <summary>
            /// Resets the enumerator.
            /// </summary>
            public void Reset()
            {
                
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
