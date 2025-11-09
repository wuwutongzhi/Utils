namespace HeapSort
{
    public class HeapSort
    {
        private int[] array;

        // 获取父节点的索引
        private int Parent(int i) => (i - 1) / 2;

        // 获取左子节点的索引
        private int LeftChild(int i) => 2 * i + 1;

        // 获取右子节点的索引
        private int RightChild(int i) => 2 * i + 2;

        private void HeapifyDown(int n, int i)
        {
            int largest = i; // 初始化最大值为根节点
            int left = LeftChild(i); // 左子节点
            int right = RightChild(i); // 右子节点
                                       // 如果左子节点大于最大值
            if (left < n && array[left] > array[largest])
                largest = left;
            // 如果右子节点大于最大值
            if (right < n && array[right] > array[largest])
                largest = right;
            // 如果最大值不是根节点
            if (largest != i)
            {
                // 交换位置
                (array[i], array[largest]) = (array[largest], array[i]);
                // 继续对affected子树进行堆化下沉
                HeapifyDown(n, largest);
            }
        }

        public void Sort()
        {
            int n = array.Length;
            // 构建大根堆
            for (int i = n / 2 - 1; i >= 0; i--)
                HeapifyDown(n, i);
            // 一个个从堆顶取出最大值
            for (int i = n - 1; i > 0; i--)
            {
                // 将堆顶（最大值）与当前最后一个元素交换
                (array[0], array[i]) = (array[i], array[0]);
                // 对剩余元素重新堆化
                HeapifyDown(i, 0);
            }
        }
    }
}