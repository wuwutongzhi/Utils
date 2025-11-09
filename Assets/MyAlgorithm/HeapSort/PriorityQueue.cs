using System;
using System.Collections.Generic;
namespace HeapSort
{
    public class PriorityQueue<T>
    {
        private List<PriorityItem> heap = new List<PriorityItem>();

        public int Count => heap.Count;
        private class PriorityItem
        {
            public T Item { get; set; }
            public int Priority { get; set; }

            public PriorityItem(T item, int priority)
            {
                Item = item;
                Priority = priority;
            }
        }

        public void Enqueue(T item, int priority)
        {
            heap.Add(new PriorityItem(item, priority));
            HeapifyUp(heap.Count - 1);
        }

        public T Dequeue()
        {
            if (heap.Count == 0)
                throw new InvalidOperationException("Queue is empty");
            T result = heap[0].Item;
            // 将最后一个元素移到堆顶
            heap[0] = heap[heap.Count - 1];
            heap.RemoveAt(heap.Count - 1);
            if (heap.Count > 0)
                HeapifyDown(0);
            return result;
        }

        private void HeapifyUp(int index)
        {
            while (index > 0)
            {
                int parentIndex = (index - 1) / 2;
                if (heap[parentIndex].Priority >= heap[index].Priority) break;
                // 交换位置
                (heap[parentIndex], heap[index]) = (heap[index], heap[parentIndex]);
                index = parentIndex;
            }
        }

        private void HeapifyDown(int index)
        {
            while (true)
            {
                int largest = index;
                int left = 2 * index + 1;
                int right = 2 * index + 2;
                if (left < heap.Count && heap[left].Priority > heap[largest].Priority)
                    largest = left;
                if (right < heap.Count && heap[right].Priority > heap[largest].Priority)
                    largest = right;
                if (largest == index)
                    break;
                (heap[index], heap[largest]) = (heap[largest], heap[index]);
                index = largest;
            }
        }
    }
}