using System.Collections.Generic;
using OPH.CollectionsEx;
using UnityEngine;

namespace OPH.PathSearch.AStar {
    /// <summary>
    /// 创建人：一个人心
    /// 功能说明：AStar寻路
    /// </summary>
    public class AStar {
        protected IGrid env;            // 环境网格
        protected PriorityQueue<AStarNode, int> pqNode;
        protected Dictionary<Vector2Int, AStarNode> closeDic;
        protected Vector2Int start, end;

        /// <summary>
        /// 构造函数
        /// </summary>
        public AStar() {
            pqNode = new PriorityQueue<AStarNode, int>((a, b) => {
                if (a == b)
                    return 0;
                else if (a < b)
                    return -1;
                else
                    return 1;
            });
            closeDic = new Dictionary<Vector2Int, AStarNode>();
        }

        /// <summary>
        /// 设置环境（网格）
        /// </summary>
        /// <param name="env"></param>
        public void SetEnv(IGrid env) {
            this.env = env;
        }

        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="s"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        public List<Vector2Int> Find(in Vector2Int s, in Vector2Int e) {
            closeDic.Clear();
            pqNode.Clear();
            start = s;
            end = e;

            AStarNode curNode = new AStarNode(start, null, 0, AStarUtilities.ManhattanLen(start, end));
            pqNode.Enqueue(curNode, curNode.fCost);
            while (!pqNode.Empty) {
                // 已放入Close的移除
                while (!pqNode.Empty && closeDic.ContainsKey(pqNode.Peek().pos)) {
                    pqNode.Dequeue();
                }
                if (pqNode.Empty)
                    break;
                curNode = pqNode.Dequeue();
                // 加入Close
                closeDic.Add(curNode.pos, curNode);
                if (curNode.pos == end) {
                    // 构建路径
                    return BuildPath(curNode);
                }
                // 检测当前点
                CheckNode(curNode);
            }
            return null;
        }

        /// <summary>
        /// 构建路径
        /// </summary>
        /// <param name="curNode"></param>
        /// <returns></returns>
        protected List<Vector2Int> BuildPath(AStarNode curNode) {
            List<Vector2Int> ans = new List<Vector2Int>();
            while (curNode != null) {
                ans.Add(curNode.pos);
                curNode = curNode.parent;
            }
            // 反转
            ans.Reverse();
            return ans;
        }

        /// <summary>
        /// 检测节点
        /// </summary>
        /// <param name="parent"></param>
        protected void CheckNode(AStarNode parent) {
            // 获取周围点
            List<Vector2Int> arround = env.GetArroundCell(parent.pos);
            for (int i = 0; i < arround.Count; ++i) {
                // 可以移动 && 非Close列表
                if (env.IsMoveable(arround[i]) && !closeDic.ContainsKey(arround[i])) {
                    AStarNode node = new AStarNode(arround[i], parent, 0, AStarUtilities.ManhattanLen(arround[i], end));
                    // 直线
                    if (AStarUtilities.IsLine(parent.pos, arround[i])) {
                        node.gCost = parent.gCost + 10;
                    }
                    // 斜线
                    else {
                        node.gCost = parent.gCost + 14;
                    }
                    pqNode.Enqueue(node, node.fCost);
                }
            }
        }
    }
}
