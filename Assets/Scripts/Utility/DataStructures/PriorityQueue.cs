using System;
using System.Collections.Generic;

namespace DataStructures
{
    /// <summary>
    /// Priority queue.
    /// <br/>
    /// The highest priority has the lowest value.
    /// </summary>
    public class PriorityQueue<T>
        where T : IComparable<T>
    {
        /// <summary>
        /// The unsorted data. Dequeue gives the item with the highest priority.
        /// </summary>
        private readonly List<T> data = new();

        public int Count => data.Count;

        public bool IsEmpty => Count == 0;

        public void Enqueue(T item) => data.Add(item);

        /// <summary>
        /// Find the item with the lowest value and remove it from the list.
        /// </summary>
        public T Dequeue()
        {
            int bestIndex = 0;

            // Find the item with the lowest value
            for (int i = 0; i < data.Count; i++)
            {
                if (data[i].CompareTo(data[bestIndex]) < 0)
                {
                    bestIndex = i;
                }
            }

            T bestItem = data[bestIndex];
            data.RemoveAt(bestIndex);
            return bestItem;
        }

        public bool Contains(T item)
        {
            return data.Contains(item);
        }
    }
}
