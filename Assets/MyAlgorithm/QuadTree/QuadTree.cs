using System.Collections.Generic;
using UnityEngine;
namespace Quadtree
{
    public class Quadtree
    {
        private Rect bounds;
        private int maxObjects;
        private int maxLevels;
        private int level;

        private List<GameObject> objects;
        private Quadtree[] children;

        // 四个象限的索引
        private const int TOP_LEFT = 0;
        private const int TOP_RIGHT = 1;
        private const int BOTTOM_LEFT = 2;
        private const int BOTTOM_RIGHT = 3;

        public Quadtree(int level, int maxObjects, int maxLevels, Rect bounds)
        {
            this.level = level;
            this.maxObjects = maxObjects;
            this.maxLevels = maxLevels;
            this.bounds = bounds;

            objects = new List<GameObject>();
            children = new Quadtree[4];
        }

        // 清空四叉树
        public void Clear()
        {
            objects.Clear();

            for (int i = 0; i < children.Length; i++)
            {
                if (children[i] != null)
                {
                    children[i].Clear();
                    children[i] = null;
                }
            }
        }

        // 分割节点
        private void Split()
        {
            float subWidth = bounds.width / 2f;
            float subHeight = bounds.height / 2f;
            float x = bounds.x;
            float y = bounds.y;

            // 创建四个子节点
            children[TOP_LEFT] = new Quadtree(level + 1, maxObjects, maxLevels,
                new Rect(x, y + subHeight, subWidth, subHeight));
            children[TOP_RIGHT] = new Quadtree(level + 1, maxObjects, maxLevels,
                new Rect(x + subWidth, y + subHeight, subWidth, subHeight));
            children[BOTTOM_LEFT] = new Quadtree(level + 1, maxObjects, maxLevels,
                new Rect(x, y, subWidth, subHeight));
            children[BOTTOM_RIGHT] = new Quadtree(level + 1, maxObjects, maxLevels,
                new Rect(x + subWidth, y, subWidth, subHeight));
        }

        // 获取物体属于哪个象限
        private int GetIndex(Rect objectBounds)
        {
            int index = -1;

            float verticalMidpoint = bounds.x + (bounds.width / 2f);
            float horizontalMidpoint = bounds.y + (bounds.height / 2f);

            // 物体是否可以完全放入某个象限
            bool topQuadrant = objectBounds.y >= horizontalMidpoint;
            bool bottomQuadrant = objectBounds.y + objectBounds.height <= horizontalMidpoint;
            bool leftQuadrant = objectBounds.x + objectBounds.width <= verticalMidpoint;
            bool rightQuadrant = objectBounds.x >= verticalMidpoint;

            if (leftQuadrant)
            {
                if (topQuadrant) index = TOP_LEFT;
                else if (bottomQuadrant) index = BOTTOM_LEFT;
            }
            else if (rightQuadrant)
            {
                if (topQuadrant) index = TOP_RIGHT;
                else if (bottomQuadrant) index = BOTTOM_RIGHT;
            }

            return index;
        }

        // 插入物体
        public void Insert(GameObject obj)
        {
            Rect objBounds = GetObjectBounds(obj);

            // 如果有子节点，尝试将物体插入子节点
            if (children[0] != null)
            {
                int index = GetIndex(objBounds);

                if (index != -1)
                {
                    children[index].Insert(obj);
                    return;
                }
            }

            // 否则加入当前节点
            objects.Add(obj);

            // 如果物体数量超过限制且未达到最大深度，进行分割
            if (objects.Count > maxObjects && level < maxLevels)
            {
                if (children[0] == null)
                    Split();

                // 将当前节点的物体重新分配到子节点
                int i = 0;
                while (i < objects.Count)
                {
                    GameObject currentObj = objects[i];
                    Rect currentBounds = GetObjectBounds(currentObj);
                    int index = GetIndex(currentBounds);

                    if (index != -1)
                    {
                        children[index].Insert(currentObj);
                        objects.RemoveAt(i);
                    }
                    else
                    {
                        i++;
                    }
                }
            }
        }

        // 检索可能发生碰撞的物体
        public List<GameObject> Retrieve(List<GameObject> returnObjects, Rect checkBounds)
        {
            int index = GetIndex(checkBounds);

            // 如果有子节点，递归检索相关子节点
            if (index != -1 && children[0] != null)
            {
                children[index].Retrieve(returnObjects, checkBounds);
            }

            // 添加当前节点的所有物体
            returnObjects.AddRange(objects);

            return returnObjects;
        }

        // 获取物体的边界（实际项目中可能需要从碰撞器获取）
        private Rect GetObjectBounds(GameObject obj)
        {
            // 这里简化处理，实际应该从碰撞器组件获取
            Vector3 position = obj.transform.position;
            return new Rect(position.x - 0.5f, position.y - 0.5f, 1f, 1f);
        }
    }
}
