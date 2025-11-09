using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AAStar = OPH.PathSearch.AStar.AStar;
using UnityEngine;
using OPH.PathSearch.AStar;
/// <summary>
/// 创建人：一个人心
/// 功能说明：JPS测试
/// </summary>
namespace AStar {
    public class AStarTest : AAStar {
        public IEnumerable<bool> IFindStep(Vector2Int s, Vector2Int e) {
            closeDic.Clear();
            pqNode.Clear();
            start = s;
            end = e;

            AStarNode curNode = new AStarNode(start, null, 0, AStarUtilities.ManhattanLen(start, end));
            pqNode.Enqueue(curNode, curNode.fCost);
            bool isFind = false;
            while (!pqNode.Empty) {
                // 已放入Close的移除
                while (closeDic.ContainsKey(pqNode.Peek().pos)) {
                    pqNode.Dequeue();
                }
                if (pqNode.Empty) break;
                curNode = pqNode.Dequeue();
                // 加入Close
                closeDic.Add(curNode.pos, curNode);
                MapManager.Instance.SetClose(curNode.pos);
                if (curNode.pos == end) {
                    isFind = true;
                    break;
                }
                MapManager.Instance.SetSelect(curNode.pos);
                CheckNodeTest(curNode);
                yield return false;
                MapManager.Instance.SetUnSelect(curNode.pos);
            }

            if (isFind) {
                List<Vector2Int> path = BuildPath(curNode);
                for (int i = 0; i < path.Count - 1; i++) {
                    DrawPath(path[i], path[i + 1]);
                    yield return false;
                }
            }

            yield return true;
        }

        private void DrawPath(in Vector2Int s, in Vector2Int e) {
            Vector2Int cur = s;
            while (cur != e) {
                MapManager.Instance.SetPath(cur);
                if (cur.x != e.x) {
                    cur.x += cur.x < e.x ? 1 : -1;
                }
                if (cur.y != e.y) {
                    cur.y += cur.y < e.y ? 1 : -1;
                }
            }
            MapManager.Instance.SetPath(cur);
        }

        /// <summary>
        /// 检测节点
        /// </summary>
        /// <param name="curNode"></param>
        protected void CheckNodeTest(AStarNode curNode) {
            List<Vector2Int> arround = env.GetArroundCell(curNode.pos);
            for (int i = 0; i < arround.Count; ++i) {
                // 可以移动
                if (env.IsMoveable(arround[i])) {
                    // 非Close列表
                    if (!closeDic.ContainsKey(arround[i])) {
                        MapManager.Instance.SetOpen(arround[i]);
                        AStarNode node = new AStarNode(arround[i], curNode, 0, AStarUtilities.ManhattanLen(arround[i], end));
                        // 直线
                        if (AStarUtilities.IsLine(curNode.pos, arround[i])) {
                            node.gCost = curNode.gCost + 10;
                        }
                        // 斜线
                        else {
                            node.gCost = curNode.gCost + 14;
                        }
                        pqNode.Enqueue(node, node.fCost);
                    }
                }
            }
        }
    }
}
