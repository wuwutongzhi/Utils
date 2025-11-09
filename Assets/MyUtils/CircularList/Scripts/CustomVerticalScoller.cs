using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace CircularList
{
    public class CustomVerticalScoller : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
    {
        public RectTransform content;
        public List<Item> items = new List<Item>();
        private Vector3 lastMousePosition;
        private float offset;
        private float itemSize;
        private float viewportHeight;

        // 数据源和当前显示索引
        private List<string> dataSource = new List<string>();
        private int currentStartIndex = 0;

        private void Awake()
        {
            // 获取单个item的高度
            itemSize = items[0].GetComponent<RectTransform>().rect.height;
            // 获取视口高度
            viewportHeight = GetComponent<RectTransform>().rect.height;

            // 初始化数据源（示例数据）
            for (int i = 1; i <= 20; i++)
            {
                dataSource.Add("Item " + i);
            }

            // 初始显示前5个数据
            InitializeDisplay();
        }

        private void InitializeDisplay()
        {
            for (int i = 0; i < 5; i++)
            {
                if (i < dataSource.Count)
                {
                    items[i].SetData(dataSource[i]);
                    items[i].transform.localPosition = new Vector3(0, -i * itemSize, 0);
                }
            }

            // 第6个item作为缓冲，初始时隐藏或放在备用位置
            if (dataSource.Count > 5)
            {
                items[5].SetData(dataSource[5]);
                items[5].transform.localPosition = new Vector3(0, -5 * itemSize, 0);
            }
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            lastMousePosition = eventData.position;
        }

        public void OnDrag(PointerEventData eventData)
        {
            offset = eventData.position.y - lastMousePosition.y;
            lastMousePosition = eventData.position;

            // 移动所有可见的item
            for (int i = 0; i < 5; i++)
            {
                items[i].transform.localPosition += new Vector3(0, offset, 0);
            }

            // 根据滚动方向处理循环逻辑
            if (offset > 0) // 向上滚动
            {
                HandleUpwardScroll();
            }
            else // 向下滚动
            {
                HandleDownwardScroll();
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            // 可以在这里添加位置修正，确保item对齐到正确位置
            SnapToNearestPosition();
        }

        private void HandleUpwardScroll()
        {
            // 检查最上面的item是否完全移出视口
            if (items[0].transform.localPosition.y > itemSize)
            {
                // 将最上面的item移动到缓冲位置
                Item topItem = items[0];

                // 更新数据：将当前显示的第一个数据索引后移
                currentStartIndex = (currentStartIndex + 1) % dataSource.Count;

                // 将缓冲item（第6个）的数据设置为新的最后一个数据
                int newDataIndex = (currentStartIndex + 4) % dataSource.Count;
                items[5].SetData(dataSource[newDataIndex]);

                // 重新排列items列表
                items.RemoveAt(0);
                items.Add(topItem);

                // 重新定位所有item
                RepositionAllItems();
            }
        }

        private void HandleDownwardScroll()
        {
            // 检查最下面的item是否完全移出视口
            if (items[4].transform.localPosition.y < -viewportHeight - itemSize)
            {
                // 将最下面的item移动到缓冲位置
                Item bottomItem = items[4];

                // 更新数据：将当前显示的第一个数据索引前移
                currentStartIndex = (currentStartIndex - 1 + dataSource.Count) % dataSource.Count;

                // 将缓冲item（第6个）的数据设置为新的第一个数据
                items[5].SetData(dataSource[currentStartIndex]);

                // 重新排列items列表
                items.RemoveAt(4);
                items.Insert(0, bottomItem);

                // 重新定位所有item
                RepositionAllItems();
            }
        }

        private void RepositionAllItems()
        {
            for (int i = 0; i < 5; i++)
            {
                items[i].transform.localPosition = new Vector3(0, -i * itemSize, 0);
            }
            // 缓冲item放在第5个位置后面
            items[5].transform.localPosition = new Vector3(0, -5 * itemSize, 0);

            // 更新所有item的显示数据
            UpdateAllItemsData();
        }

        private void UpdateAllItemsData()
        {
            for (int i = 0; i < 5; i++)
            {
                int dataIndex = (currentStartIndex + i) % dataSource.Count;
                items[i].SetData(dataSource[dataIndex]);
            }
        }

        private void SnapToNearestPosition()
        {
            // 简单的对齐逻辑，确保item停在正确位置
            // 这里可以添加动画效果
            RepositionAllItems();
        }
    }
}