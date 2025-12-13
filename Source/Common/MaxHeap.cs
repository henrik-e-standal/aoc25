using System.Collections;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Aoc25.Common
{
    /// <summary>
    /// A fast, fixed-size heap data structure implemented as an array-backed, balanced binary tree.
    /// It is designed to efficiently manage and retrieve the largest item in a collection of items. 
    /// This implementation is designed ot be as fast as possible. Any safety checks are omitted 
    /// (correct usage is assumed) and the heap will not resize when available capacity is exhausted.
    /// </summary>
    /// <typeparam name="T"> The type of items stored in the heap. </typeparam>
    [DebuggerDisplay("Count = {count}")]
    public sealed class MaxHeap<T> : IEnumerable<T> where T : IComparable<T>
    {
        /// <summary>
        /// The array that stores the items in the heap.
        /// </summary>
        private T[] data;

        /// <summary>
        /// The number of items stored in the heap.
        /// </summary>
        private int count;

        /// <summary>
        /// Gets the number of items stored in the heap.
        /// </summary>
        public int Count
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return count; }
        }

        /// <summary>
        /// Gets the capacity of the heap.
        /// </summary>
        public int Capacity
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get { return data.Length; }
        }

        /// <summary>
        /// Creates a new <see cref="MaxHeap{T}"/> with the specified fixed capacity.
        /// </summary>
        /// <param name="capacity"> The fixed capacity of the heap. </param>
        public MaxHeap(int capacity) : this((uint)capacity)
        {
            // Intentionally empty.
        }  
         
        /// <summary>
        /// Creates a new <see cref="MaxHeap{T}"/> with the specified fixed capacity.
        /// </summary>
        /// <param name="capacity"> The fixed capacity of the heap. </param>
        public MaxHeap(uint capacity)
        {
            data = new T[capacity];
        }

        /// <summary>
        /// Creates a new <see cref="MaxHeap{T}"/> from a collection of items. 
        /// This constructor will emulate the "move concept" in C++, and will take ownership
        /// of the passed array by nulling the original array via the passed reference.
        /// </summary>
        /// <param name="data"> The collection of items to create the heap from. </param>
        /// <param name="count"> The number of items to create the heap from. </param>
        public MaxHeap(ref T[] data, int count)
        { 
            this.data = data;
            this.count = count;

            data = null!;

            // Construct a max-heap out of the passed data.
            for (int i = (count / 2) - 1; i >= 0; i--)
            {
                Heapify(i);
            }
        }

        /// <summary>
        /// Restores the max-heap property for the subtree rooted at the specified index.
        /// </summary>
        /// <param name="currentIndex"> The start index for the heapify operation. </param>
        private void Heapify(int currentIndex)
        {
            T currentItem = data[currentIndex];
            int heapCount = count;

            while (true)
            {
                int leftIndex = (currentIndex << 1) + 1;
                if (leftIndex >= heapCount)
                    break;

                int rightIndex = leftIndex + 1;
                int largestIndex = leftIndex;

                if ((rightIndex < heapCount) && data[rightIndex].CompareTo(data[leftIndex]) > 0)
                    largestIndex = rightIndex;

                if (currentItem.CompareTo(data[largestIndex]) >= 0)
                    break;

                data[currentIndex] = data[largestIndex];
                currentIndex = largestIndex;
            }

            data[currentIndex] = currentItem;
        }

        /// <summary>
        /// Adds an item to the heap.
        /// </summary>
        /// <param name="item"> The item to add. </param>
        public void Push(T item)
        {
            // Store local copies of field variables - this produce faster code.
            var heapData = data;
            int currentIndex = count++;

            while (currentIndex > 0)
            {
                int parentIndex = (currentIndex - 1) >> 1;
                T parentItem = heapData[parentIndex];
 
                if (item.CompareTo(parentItem) <= 0) {
                   break; 
                }  

                heapData[currentIndex] = parentItem;
                currentIndex = parentIndex;
            }
 
            heapData[currentIndex] = item;
        }

        /// <summary>
        /// Adds a new item to the heap, then removes the largest item in the heap and returns it.
        /// </summary>
        /// <param name="item"> The item to add. </param>
        /// <returns> The largest item in the heap, after adding the new item. </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T PushAndPop(T item)
        {
            T largestItem;

            // If the new item would be the new largest item, simply return it
            // and leave the heap exactly as-is. Otherwise, 
            if(item.CompareTo(data[0]) >= 0) 
            {
                largestItem = item;
            }
            else 
            {
                largestItem = data[0];

                // Overwrite the root (i.e. largest) item in the heap with the new item.
                data[0] = item;

                // Restores the max-heap property for the root subtree of the heap.
                Heapify(0);
            }

            return largestItem;
        }

        /// <summary>
        /// Attempts to first add a new item to the heap, then remove the largest item in the heap and return it.
        /// </summary>
        /// <param name="newItem"> The item to add. </param>
        /// <param name="removedItem" The removed item, if the operation is successful. </param>
        /// <returns> True if an item could be removed from the heap, false if the heap was empty. </returns>
        public bool TryPushAndPop(T newItem, [MaybeNullWhen(false)] out T removedItem)
        {
            if(count == 0)
            {
                removedItem = default!;
                return false;
            }
 
            removedItem = PushAndPop(newItem);
            return true;
        }

        /// <summary>
        /// Removes the largest item in the heap and returns it.
        /// </summary>
        /// <returns> The largest item in the heap. </returns>
        public T Pop()
        {
            T result = data[0];

            RemoveTop();
            
            return result;
        }

        /// <summary>
        /// Attempts to remove the largest item in the heap and return it.
        /// </summary>
        /// <param name="item" The largest item, if the operation is successful. </param>
        /// <returns> True if an item was removed from the heap, false if the heap was empty. </returns>
        public bool TryPop([MaybeNullWhen(false)] out T item)
        {
            if(count == 0)
            {
                item = default(T);
                return false;
            }

            item = Pop();
            return true;
        } 
        
        /// <summary>
        /// Removes largest item in the heap, adds a new item to the heap, and returns the removed item.
        /// </summary>
        /// <param name="item"> The item to add. </param>
        /// <returns> The largest item in the heap. </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T PopAndPush(T item)
        {
            T largestItem = data[0];

            // Overwrite the root (i.e. largest) item in the heap with the new item.
            data[0] = item;

            // Restores the max-heap property for the root subtree of the heap.
            Heapify(0);

            return largestItem;
        }

        /// <summary>
        /// Attempts to remove the largest item in the heap, add a new item, and return the removed item.
        /// </summary>
        /// <param name="newItem"> The item to add. </param>
        /// <param name="item" The removed item, if the operation is successful. </param>
        /// <returns> True if an item could be removed from the heap, false if the heap was empty. </returns>
        public bool TryPopAndPush(T newItem, [MaybeNullWhen(false)] out T removedItem)
        {
            if(count == 0) 
            {
                removedItem = default!;
                return false;
            }

            removedItem = data[0];

            // Overwrite the root (i.e. largest) item in the heap with the new item.
            data[0] = newItem;

            // Restores the max-heap property for the root subtree of the heap.
            Heapify(0);

            return true;
        }

        /// <summary>
        /// Removes the largest item in the heap.
        /// </summary>
        public void RemoveTop()
        {
            // Store local copies of field variables - this produce faster code.
            var heapData = data;
            int lastIndex = --count;

            if (lastIndex != 0)
            {
                T currentItem = heapData[lastIndex];
                int currentIndex = 0;

                while (true)
                {
                    int childIndex = (currentIndex << 1) + 1;
                    if (childIndex >= lastIndex)
                        break;

                    int rightChildIndex = (childIndex + 1);

                    if ((rightChildIndex < lastIndex) && heapData[rightChildIndex].CompareTo(heapData[childIndex]) > 0){
                        childIndex = rightChildIndex;
                    }

                    if (currentItem.CompareTo(heapData[childIndex]) >= 0) {
                        break;
                    }

                    heapData[currentIndex] = heapData[childIndex];
                    currentIndex = childIndex;
                }

                heapData[currentIndex] = currentItem;
            }
        }
        
        /// <summary>
        /// Attempts to remove the largest item in the heap.
        /// </summary>
        /// <param name="item" The retrieved item, if the operation is successful. </param>
        /// <returns> True if an item could be retrieved from the heap, false if the heap was empty. </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryRemoveTop()
        {
            if(count == 0) {
                return false;
            }

            RemoveTop();
            return true;
        }

        /// <summary>
        /// Removes largest item in the heap and then adds an item to the heap.
        /// </summary>
        /// <param name="item"> The item to add. </param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void RemoveTopAndPush(T item)
        {
            // Overwrite the root (i.e. largest) item in the heap with the new item.
            data[0] = item;

            // Restores the max-heap property for the root subtree of the heap.
            Heapify(0);
        }

        /// <summary>
        /// Attempts to remove largest item in the heap and then add an item to the heap.
        /// </summary>
        /// <param name="item"> The item to add. </param>
        /// <returns> True if an item could be removed from the heap, false if the heap was empty. </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool TryRemoveTopAndPush(T item)
        {
            if(count == 0) {
                return false;
            }
            
            // Overwrite the root (i.e. largest) item in the heap with the new item.
            data[0] = item;

            // Restores the max-heap property for the root subtree of the heap.
            Heapify(0);

            return true;
        }

        /// <summary>
        /// Retrieves the largest item in the heap (without removing it).
        /// </summary>
        /// <returns> The item at the front of the heap. </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public T Peek()
        {
            return data[0];
        }
  
        /// <summary>
        /// Attempts to retrieve the largest item in the heap (without removing it).
        /// </summary>
        /// <param name="item" The retrieved item, if the operation is successful. </param>
        /// <returns> True if an item could be retrieved from the heap, false if the heap was empty. </returns>
        public bool TryPeek([MaybeNullWhen(false)] out T item)
        {
            if(count == 0)
            {
                item = default(T);
                return false;
            }

            item = Peek();
            return true;
        }

        /// <summary>
        /// Gets whether or not the heap is filled to capacity.
        /// </summary>
        /// <returns> True if the heap is filled to capacity, otherwise false. </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsFull() 
        {
            return (count == data.Length);
        }

        /// <summary>
        /// Gets whether or not the heap is empty (e.g. contains zero items).
        /// </summary>
        /// <returns> True if the heap is empty, otherwise false. </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsEmpty()
        {
            return (count == 0);
        }

        /// <summary>
        /// Removes all items stored in the heap. 
        /// </summary>
        /// <remarks>
        /// Items are removed by simply setting heap item count to zero.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Clear()
        {
            count = 0;
        }
        
        /// <summary>
        /// Creates a shallow copy of the heap.
        /// </summary>
        /// <returns>
        /// A new <see cref="MaxHeap{T}"/> instance containing the same elements and 
        /// internal structure as the original heap. The cloned heap is independent;
        /// modifications to it will not affect the original heap.
        /// </returns>
        public MaxHeap<T> Clone()
        {
            var newHeap = new MaxHeap<T>(data.Length);

            Array.Copy(data, newHeap.data, count);
            newHeap.count = count;

            return newHeap;
        }

        /// <summary>
        /// Converts the heap to a <see cref="MinHeap{T}"/> by "transferring" ownership of the 
        /// heap's internal data to the new heap.
        /// </summary>
        /// <remarks>
        /// As this operation will transfer the internal data of the current heap to the created 
        /// <see cref="MinHeap{T}"/>, the current heap will be left in an unusable state.
        /// </remarks>
        /// <returns> A new <see cref="MinHeap{T}"/> using this heap's internal data. </returns>
        public MinHeap<T> ConvertToMinHeap()
        {
            return new MinHeap<T>(ref data, count);
        }

        /// <summary>
        /// Gets an enumerator for enumerating the content of the heap in order (from largest to smallest).
        /// </summary>
        /// <remarks>
        /// Creating this enumerator is an O(n) operation, as the entire heap needs to be copied.
        /// </remarks>
        /// <returns> An enumerator for enumerating the content of the heap. </returns>
        public OrderedHeapEnumerator GetEnumerator()
        {
            return new OrderedHeapEnumerator(this);
        }

        /// <summary>
        /// Gets an enumerator for enumerating the content of the heap unordered (not )
        /// </summary>
        /// <remarks>
        /// In contrast to the ordered enumerator, creating this enumerator is fast, as 
        /// no heap data copy needs to be made.
        /// </remarks>
        /// <returns> An enumerator for enumerating the content of the heap. </returns>
        public UnorderedHeapEnumerator GetUnorderedEnumerator()
        {
            return new UnorderedHeapEnumerator(this);
        }

        /// <summary>
        /// Gets an enumerator for enumerating the content of the heap in order (from largest to smallest).
        /// </summary>
        /// <remarks>
        /// Creating this enumerator is an O(n) operation, as the entire heap needs to be copied.
        /// </remarks>
        /// <returns> An enumerator for enumerating the content of the heap. </returns>
        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Gets an enumerator for enumerating the content of the heap in order (from largest to smallest).
        /// </summary>
        /// <remarks>
        /// Creating this enumerator is an O(n) operation, as the entire heap needs to be copied.
        /// </remarks>
        /// <returns> An enumerator for enumerating the content of the heap. </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// An enumerator for enumerating the content of a <see cref="MaxHeap{T}"/> in order.
        /// </summary>
        public struct OrderedHeapEnumerator : IEnumerator<T> 
        {
            /// <summary>
            /// A copy of the <see cref="MaxHeap{T}"/> being enumerated.
            /// </summary>
            private readonly MaxHeap<T> heap;

            /// <summary>
            /// The current item in the enumeration.
            /// </summary>
            private T currentItem;

            /// <summary>
            /// Gets the current item in the enumeration.
            /// </summary>
            public T Current
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get { return currentItem; }
            }

            /// <summary>
            /// Gets the current item in the enumeration.
            /// </summary>
            object IEnumerator.Current
            {
                get { return Current!; }
            }

            /// <summary>
            /// Creates a new enumerator for the specified heap.
            /// </summary>
            /// <param name="heap"> The heap to enumerate. </param>
            public OrderedHeapEnumerator(MaxHeap<T> heap)
            {
                this.heap = heap.Clone();
                this.currentItem = default!;
            }

            /// <summary>
            /// Moves to the next item in the enumeration.
            /// </summary>
            /// <returns> True if the enumeration had another item. </returns>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool MoveNext()
            {
                if (heap.count == 0) {
                    return false;
                }

                currentItem = heap.Pop();
                return true;
            }

            /// <summary>
            /// Resets the enumerator.
            /// </summary>
            public void Reset()
            {
                throw new NotSupportedException("Enumerator reset not supported when enumerating heaps in order.");
            }

            /// <summary>
            /// Disposes of the enumerator.
            /// </summary>
            public void Dispose()
            {
                // Nothing to dispose of.
            }
        }

        /// <summary>
        /// An enumerator for enumerating the content of a <see cref="MaxHeap{T}"/> in an undefined order.
        /// </summary>
        public struct UnorderedHeapEnumerator : IEnumerator<T> 
        {
            /// <summary>
            /// The <see cref="MaxHeap{T}"/> instance being enumerated.
            /// </summary>
            private readonly MaxHeap<T> heap;

            private int currentIndex;

            /// <summary>
            /// Gets the current item in the enumeration.
            /// </summary>
            public T Current
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get { return heap.data[currentIndex]; }
            }

            /// <summary>
            /// Gets the current item in the enumeration.
            /// </summary>
            object IEnumerator.Current
            {
                get { return Current!; }
            }

            /// <summary>
            /// Creates a new enumerator for the specified heap.
            /// </summary>
            /// <param name="heap"> The heap to enumerate. </param>
            public UnorderedHeapEnumerator(MaxHeap<T> heap)
            {
                this.heap = heap;
                this.currentIndex = -1;
            }

            /// <summary>
            /// Moves to the next item in the enumeration.
            /// </summary>
            /// <returns> True if the enumeration had another item. </returns>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool MoveNext()
            {
                int next = (currentIndex + 1);

                if (next >= heap.count) {
                    return false;
                }

                currentIndex = next;
                return true;
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
