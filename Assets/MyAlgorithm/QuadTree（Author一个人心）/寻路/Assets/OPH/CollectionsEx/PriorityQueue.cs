using System;
using System.Collections.Generic;

namespace OPH.CollectionsEx {
    /// <summary>
    /// 创建人：一个人心
    /// 功能说明：优先队列
    /// </summary>
    /// <typeparam name="TElement">元素类型</typeparam>
    /// <typeparam name="TPriority">优先值类型</typeparam>
    public class PriorityQueue<TElement, TPriority> {
        /// <summary>
        /// 所有节点
        /// </summary>
        private List<(TElement element, TPriority priority)> _nodes;
        /// <summary>
        /// 优先级比较器
        /// </summary>
        private Comparison<TPriority> _comparison;
        /// <summary>
        /// 堆大小
        /// </summary>
        public int Size { get; private set; }
        /// <summary>
        /// 容器是否是空
        /// </summary>
        public bool Empty => Size == 0;
         
        private const int Arity = 4;
        /// <summary>
        /// 使用位运算符，表示左移Log2Arity或右移Log2Arity(效率更高)，即相当于除以1<<Log2Arity
        /// </summary>
        private const int Log2Arity = 2;
        /// <summary>
        /// 获取当前节点的父节点下标
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        private int GetParentIndex(int index) => (index - 1) >> Log2Arity;
        /// <summary>
        /// 获取当前节点的第一个子节点下标
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        private int GetFirsetIndex(int index) => index << Log2Arity | 1;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="comparison">优先级比较方法</param>
        public PriorityQueue(Comparison<TPriority> comparison) {
            _nodes = new List<(TElement, TPriority)>();
            _comparison = comparison;
        }

        /// <summary>
        /// 入队
        /// </summary>
        /// <param name="element">元素</param>
        /// <param name="priority">优先值</param>
        public void Enqueue(TElement element, TPriority priority) {
            int lastIdx = Size++;   // 获取列表最后的位置
            (TElement element, TPriority priority) pair = (element, priority);
            _nodes.Add(pair);
            MoveUpUpdate(pair, lastIdx);
        }

        /// <summary>
        /// 向上更新
        /// </summary>
        /// <param name="node">当前节点</param>
        /// <param name="index">当前下标</param>
        private void MoveUpUpdate((TElement element, TPriority priority) node, int index) {
            // 列表从尾部往上更新
            while (index > 0) {
                int parentIdx = GetParentIndex(index);  // 获取父节点下标
                (TElement element, TPriority priority) parent = _nodes[parentIdx];
                // 当前节点优先级比父节点高，则替换
                if (_comparison(node.priority, parent.priority) < 0) {
                    _nodes[index] = parent;     // 交换父节点
                    index = parentIdx;
                }
                // 无需继续向上更新
                else {
                    break;
                }
            }
            _nodes[index] = node;   // 设置节点位置
        }

        /// <summary>
        /// 出队
        /// </summary>
        /// <returns>当前最优值</returns>
        /// <exception cref="InvalidOperationException">如果队列为空则抛出</exception>
        public TElement Dequeue() {
            if (Size == 0) {
                throw new InvalidOperationException("队列为空");
            }
            //将堆顶元素返回
            TElement element = _nodes[0].element;
            //然后调整堆
            RemoveRootNode();
            return element;
        }

        /// <summary>
        /// 移除根节点
        /// </summary>
        private void RemoveRootNode() {
            int lastIdx = --Size;
            if (lastIdx > 0) {
                // 最后一个节点
                (TElement Element, TPriority Priority) lastNode = _nodes[lastIdx];
                MoveDownUpdate(lastNode, 0);
            }
            _nodes.RemoveAt(lastIdx);
        }

        /// <summary>
        /// 向下更新
        /// </summary>
        /// <param name="node">当前节点</param>
        /// <param name="index">当前下标</param>
        private void MoveDownUpdate((TElement element, TPriority priority) node, int index) {
            int size = Size;
            int i;
            // 获取子节点位置
            while ((i = GetFirsetIndex(index)) < size) {
                (TElement element, TPriority priority) maxChild = _nodes[i];
                int maxIdx = i;
                int childIndexUpperBound = Math.Min(i + Arity, size);       // 限制范围
                while(++i < childIndexUpperBound) {
                    (TElement element, TPriority priority) nextChild = _nodes[i];
                    // 子节点优先级比当前最高的高，则替换
                    if (_comparison(nextChild.priority, maxChild.priority) < 0) {
                        maxChild = nextChild;
                        maxIdx = i;
                    }
                }
                // 子节点中最高优先级的比自身节点优先低，则直接放到父节点
                if (_comparison(node.priority, maxChild.priority) <= 0) {
                    break;
                }
                //将最小的子元素赋值给父节点
                _nodes[index] = maxChild;
                index = maxIdx;
            }
            _nodes[index] = node;
        }

        /// <summary>
        /// 返回最优节点
        /// </summary>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public TElement Peek() {
            if (Size == 0) {
                throw new InvalidOperationException("队列为空");
            }
            return _nodes[0].element;
        }

        /// <summary>
        /// 移除某个元素（On复杂度）
        /// </summary>
        /// <param name="element"></param>
        public bool Remove(TElement element) {
            if (Size == 0) return false;
            int idx = -1;
            for (int i = 0; i < Size; ++i) {
                if (element.Equals(_nodes[i].element)) {
                    idx = i;
                    break;
                }
            }
            if (idx >= 0) {
                int lastNodeIndex = --Size;
                (TElement Element, TPriority Priority) lastNode = _nodes[lastNodeIndex];
                MoveDownUpdate(lastNode, idx);
                _nodes.RemoveAt(lastNodeIndex);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 清除所有
        /// </summary>
        public void Clear() {
            _nodes.Clear();
            Size = 0;
        }
    }
}
