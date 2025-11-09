using System.Collections.Generic;
using System.Text;
using OPH.PathSearch.JPS;
using UnityEngine;

namespace JPS {
    /// <summary>
    /// 创建人：一个人心
    /// 功能说明：JPS测试
    /// </summary>
    public class JPSTest : OPH.PathSearch.JPS.JPS {
        public IEnumerable<bool> IFindStep(Vector2Int s, Vector2Int e) {
            openLut.Clear();
            closeLut.Clear();
            pqList.Clear();
            start = s;
            end = e;

            // 起点添加到查找表
            JPSNode curNode = new JPSNode(s, s, JPSUtilities.StartDirs, 0, JPSUtilities.ManhattanLen(start, end));
            pqList.Enqueue(curNode, curNode.fCost);
            openLut.Add(s, curNode);
            bool isFind = false;
            while (!pqList.Empty) {
                // 已放入Close的移除
                while (!pqList.Empty && closeLut.Contains(pqList.Peek().pos)) {
                    pqList.Dequeue();
                }
                if (pqList.Empty) break;
                curNode = pqList.Dequeue();
                // 加入到Close
                closeLut.Add(curNode.pos);
                // 终点
                if (curNode.pos == end) {
                    isFind = true;
                    break;
                }

                MapManager.Instance.SetSelect(curNode.pos);
                Vector2Int[] nodeDirs = curNode.direactions;
                for (int i = 0; i < nodeDirs.Length; ++i) {
                    if (JPSUtilities.IsLineDireaction(nodeDirs[i])) {
                        CheckLineTest(curNode.pos, nodeDirs[i], curNode.gCost);
                    }
                    else {
                        CheckDiagonalTest(curNode.pos, curNode.pos, nodeDirs[i], curNode.gCost);
                    }
                    yield return false;
                }

                MapManager.Instance.SetUnSelect(curNode.pos);
            }
            if (isFind) {
                List<Vector2Int> path = BuildPath();
                for (int i = 0; i < path.Count - 1; ++i) {
                    DrawPath(path[i], path[i + 1]);
                    yield return false;
                }
            }
        }

        /// <summary>
        /// 添加跳点
        /// </summary>
        /// <param name="parent">跳点的父节点</param>
        /// <param name="pos">跳点位置</param>
        /// <param name="dirs">跳点扫描方向</param>
        /// <param name="gCost">起点到跳点消耗</param>
        protected void AddJumpNodeTest(Vector2Int parent, Vector2Int pos, Vector2Int[] dirs, int gCost) {
            if (!closeLut.Contains(pos)) {
                if (openLut.ContainsKey(pos)) {
                    JPSNode node = openLut[pos];
                    // 当前代价小于之前的则替换
                    if (node.gCost > gCost) {
                        StringBuilder sb = new StringBuilder();
                        sb.Append(pos.ToString());
                        sb.Append(":");
                        for (int i  = 0; i < node.direactions.Length; ++i) {
                            sb.Append(node.direactions[i].ToString());
                        }
                        sb.Append("=>");
                        if (dirs != null) {
                            for (int i = 0; i < dirs.Length; ++i) {
                                sb.Append(dirs[i].ToString());
                            }
                        }
                        Debug.Log(sb.ToString());
                        node.gCost = gCost;
                        node.parent = parent;
                        node.direactions = dirs;
                        pqList.Enqueue(node, node.fCost);       // 添加到优先队列
                    }
                }
                else {
                    JPSNode node = new JPSNode(parent, pos, dirs, gCost, JPSUtilities.ManhattanLen(pos, end));
                    openLut.Add(pos, node);                          // 添加到查询表
                    pqList.Enqueue(node, node.fCost);           // 添加到优先队列
                    MapManager.Instance.SetJump(pos);
                }
            }
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
        /// 横纵向检测 测试
        /// </summary>
        /// <param name="parent">父节点</param>
        /// <param name="dir">父节点的前进方向</param>
        /// <param name="gCost">起点到父节点的代价</param>
        /// <returns>遇到跳点，障碍物，边缘退出函数；跳点返回true，其他为false</returns>
        protected bool CheckLineTest(Vector2Int parent, Vector2Int dir, int gCost) {
            Vector2Int cur = parent + dir;    // 当前检测点
            while (env.IsMoveable(cur)) {
                gCost += 10;
                // 检测到终点
                if (cur == end) {
                    AddJumpNodeTest(parent, cur, new Vector2Int[0], gCost);
                    return true;
                }
                MapManager.Instance.SetSearch(cur);
                // 检测当前点是否有强制邻居
                List<Vector2Int> directions = CheckForceNeighborsInLine(cur, dir);
                if (directions.Count > 0) {
                    directions.Add(dir);    // 添加父节点前进方向
                    AddJumpNodeTest(parent, cur, directions.ToArray(), gCost);
                    return true;
                }
                cur += dir;
            }
            return false;
        }

        /// <summary>
        /// 对角分量检测 测试
        /// </summary>
        /// <param name="pos">对角移动后的点</param>
        /// <param name="dir">对角移动方向</param>
        /// <param name="cost">移动到pos点的总代价</param>
        /// <returns>遇到跳点，障碍物，边缘退出函数；跳点返回true，其他为false</returns>
        protected bool DiagonalMoveTest(Vector2Int pos, Vector2Int dir, int cost) {
            // 拆分移动向量
            bool _1 = CheckLineTest(pos, new Vector2Int(dir.x, 0), cost);
            bool _2 = CheckLineTest(pos, new Vector2Int(0, dir.y), cost);
            return _1 || _2;
        }

        /// <summary>
        /// 斜向移动 测试
        /// </summary>
        /// <param name="originalParent">斜向移动的原点</param>
        /// <param name="parent">父节点</param>
        /// <param name="dir">斜向移动方向</param>
        /// <param name="gCost">移动到当前点的消耗</param>
        protected void CheckDiagonalTest(Vector2Int originalParent, Vector2Int parent, Vector2Int dir, int gCost) {
            // 计算需要判断障碍物的位置
            Vector2Int b1 = new Vector2Int(parent.x + dir.x, parent.y);
            Vector2Int b2 = new Vector2Int(parent.x, parent.y + dir.y);
            parent += dir;
            gCost += 14;

            if (env.IsMoveable(b1)) {
                // b1 b2均为空
                if (env.IsMoveable(b2)) {
                    if (env.IsMoveable(parent)) {
                        MapManager.Instance.SetSearch(parent);
                        // 先检测可移动后检测是否是终点
                        if (parent == end) {
                            AddJumpNodeTest(originalParent, parent, null, 0);
                            return;
                        }
                        // 对角分量中有跳点
                        if (DiagonalMoveTest(parent, dir, gCost)) {
                            AddJumpNodeTest(originalParent, parent, new Vector2Int[] { dir }, gCost);
                            return;
                        }
                        // 继续斜向移动
                        CheckDiagonalTest(originalParent, parent, dir, gCost);
                    }
                }
                // b1可移动 b2障碍物
                else {
                    if (env.IsMoveable(parent)) {
                        MapManager.Instance.SetSearch(parent);
                        // 先检测可移动后检测是否是终点
                        if (parent == end) {
                            AddJumpNodeTest(originalParent, parent, null, 0);
                            return;
                        }
                        // 检测斜向强邻居
                        List<Vector2Int> dirs = CheckForceNeighborInDiagonal(parent, b2, dir, JPSUtilities.up);
                        if (DiagonalMoveTest(parent, dir, gCost) || dirs.Count > 0) {
                            dirs.Add(dir);      // 添加之前移动方向
                            AddJumpNodeTest(originalParent, parent, dirs.ToArray(), gCost);
                            return;
                        }
                        CheckDiagonalTest(originalParent, parent, dir, gCost);
                    }
                }
            }
            else {
                // b1障碍物 b2可移动
                if (env.IsMoveable(b2)) {
                    if (env.IsMoveable(parent)) {
                        MapManager.Instance.SetSearch(parent);
                        // 先检测可移动后检测是否是终点
                        if (parent == end) {
                            AddJumpNodeTest(originalParent, parent, null, 0);
                            return;
                        }
                        // 检测斜向强邻居
                        List<Vector2Int> dirs = CheckForceNeighborInDiagonal(parent, b1, dir, JPSUtilities.right);
                        if (DiagonalMoveTest(parent, dir, gCost) || dirs.Count > 0) {
                            dirs.Add(dir);
                            AddJumpNodeTest(originalParent, parent, dirs.ToArray(), gCost);
                            return;
                        }
                        CheckDiagonalTest(originalParent, parent, dir, gCost);
                    }
                }
                // b1、b2不可移动（不做操作
                else {
                    // NoDo
                }
            }
        }
    }
}