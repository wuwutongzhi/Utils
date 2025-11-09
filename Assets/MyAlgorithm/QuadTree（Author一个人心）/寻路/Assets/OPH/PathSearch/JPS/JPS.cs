using System.Collections;
using System.Collections.Generic;
using OPH.CollectionsEx;
using UnityEngine;

namespace OPH.PathSearch.JPS {
    /// <summary>
    /// JPS寻路
    /// </summary>
    public class JPS {
        /// <summary>
        /// 节点优先队列
        /// </summary>
        protected PriorityQueue<JPSNode, int> pqList;//open
        /// <summary>
        /// Open跳点查找表
        /// </summary>
        protected Dictionary<Vector2Int, JPSNode> openLut;//open
        /// <summary>
        /// Close跳点查找表
        /// </summary>
        protected HashSet<Vector2Int> closeLut;//close
        /// <summary>
        /// 起点，终点
        /// </summary>
        protected Vector2Int start, end;
        /// <summary>
        /// 环境（网格）
        /// </summary>
        protected IGrid env;

        public JPS() {
            openLut = new Dictionary<Vector2Int, JPSNode>();
            closeLut = new HashSet<Vector2Int>();
            pqList = new PriorityQueue<JPSNode, int>((a, b) => {
                if (a == b) return 0;
                else if (a < b) return -1;
                else return 1;
            });
        }

        /// <summary>
        /// 设置环境（网格）
        /// </summary>
        /// <param name="env"></param>
        public void SetEnv(IGrid env) {
            this.env = env;
        }

        public List<Vector2Int> Find(Vector2Int s, Vector2Int e) {
            openLut.Clear();
            closeLut.Clear();
            pqList.Clear();
            start = s;
            end = e;

            JPSNode curNode = new JPSNode(s, s, JPSUtilities.StartDirs, 0,JPSUtilities.ManhattanLen(start,end));
            pqList.Enqueue(curNode, curNode.fCost);
            openLut.Add(s, curNode);
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
                if (curNode.pos == end) return BuildPath();

                Vector2Int[] nodeDirs = curNode.direactions;
                for (int i = 0; i < nodeDirs.Length; ++i) {
                    if (JPSUtilities.IsLineDireaction(nodeDirs[i])) {
                        CheckLine(curNode.pos, nodeDirs[i], curNode.gCost);
                    }
                    else {
                        CheckDiagonal(curNode.pos, curNode.pos, nodeDirs[i], curNode.gCost);
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// 构建路径
        /// </summary>
        /// <returns></returns>
        protected List<Vector2Int> BuildPath() {
            Stack<Vector2Int> stk = new Stack<Vector2Int>();
            Vector2Int curPos = end;
            JPSNode curNode;
            while (curPos != start) {
                curNode = openLut[curPos];
                stk.Push(curPos);
                curPos = curNode.parent;
            }
            stk.Push(curPos);
            List<Vector2Int> result = new List<Vector2Int>(stk.Count);
            while (stk.Count > 0) {
                result.Add(stk.Pop());
            }
            return result;
        }

        /// <summary>
        /// 添加跳点
        /// </summary>
        /// <param name="parent">跳点的父节点</param>
        /// <param name="pos">跳点位置</param>
        /// <param name="dirs">跳点扫描方向</param>
        /// <param name="gCost">起点到跳点消耗</param>
        protected void AddJumpNode(Vector2Int parent, Vector2Int pos, Vector2Int[] dirs, int gCost) {
            if (!closeLut.Contains(pos)) {
                if (openLut.ContainsKey(pos)) {
                    JPSNode node = openLut[pos];
                    // 当前代价小于之前的则替换
                    if (node.gCost > gCost) {
                        node.gCost = gCost;
                        node.parent = parent;
                        node.direactions = dirs;
                        pqList.Enqueue(node, node.fCost);       // 添加到优先队列
                    }
                }
                else {
                    JPSNode node = new JPSNode(parent, pos, dirs, gCost,JPSUtilities.ManhattanLen(pos, end));
                    openLut.Add(pos, node);                          // 添加到查询表
                    pqList.Enqueue(node, node.fCost);           // 添加到优先队列
                }
            }
        }

        /// <summary>
        /// 检测横纵向的强邻居
        /// </summary>
        /// <param name="pos">当前检测点</param>
        /// <param name="dir">父节点指向检测点的的方向</param>
        protected List<Vector2Int> CheckForceNeighborsInLine(Vector2Int pos, Vector2Int dir) {
            List<Vector2Int> directions = new List<Vector2Int>();
            Vector2Int[] vertical = JPSUtilities.verticalDirLut[dir];
            for (int i = 0; i < vertical.Length; ++i) {
                Vector2Int blockPt = pos + vertical[i];         // 垂直点位置
                if (env.IsObstacle(blockPt) && env.IsMoveable(blockPt + dir))
                    directions.Add(vertical[i] + dir);
            }
            return directions;
        }

        /// <summary>
        /// 横纵向检测
        /// </summary>
        /// <param name="parent">父节点</param>
        /// <param name="dir">父节点的前进方向</param>
        /// <param name="gCost">起点到父节点的代价</param>
        /// <returns>遇到跳点，障碍物，边缘退出函数；跳点返回true，其他为false</returns>
        protected bool CheckLine(Vector2Int parent, Vector2Int dir, int gCost) {
            Vector2Int cur = parent + dir;    // 当前检测点
            while (env.IsMoveable(cur)) {
                gCost += 10;
                // 检测到终点
                if (cur == end) {
                    AddJumpNode(parent, cur, new Vector2Int[0], gCost);
                    return true;
                }

                // 检测当前点是否有强制邻居
                List<Vector2Int> directions = CheckForceNeighborsInLine(cur, dir);
                if (directions.Count > 0) {
                    directions.Add(dir);    // 添加父节点前进方向
                    AddJumpNode(parent, cur, directions.ToArray(), gCost);
                    return true;
                }
                cur += dir;
            }
            return false;
        }

        /// <summary>
        /// 对角分量检测
        /// </summary>
        /// <param name="pos">对角移动后的点</param>
        /// <param name="dir">对角移动方向</param>
        /// <param name="gCost">移动到pos点的总代价</param>
        /// <returns>遇到跳点，障碍物，边缘退出函数；跳点返回true，其他为false</returns>
        protected bool DiagonalMove(Vector2Int pos, Vector2Int dir, int gCost) {
            // 拆分移动向量
            bool _1 = CheckLine(pos, new Vector2Int(dir.x, 0), gCost);
            bool _2 = CheckLine(pos, new Vector2Int(0, dir.y), gCost);
            return _1 || _2;
        }

        /// <summary>
        /// 检测斜向强邻居
        /// </summary>
        /// <param name="pos">当前移动点</param>
        /// <param name="obs">pos侧边的的障碍物</param>
        /// <param name="dir">父节点到当前点的移动方向</param>
        /// <param name="mask">方向遮罩</param>
        /// <returns></returns>
        protected List<Vector2Int> CheckForceNeighborInDiagonal(Vector2Int pos, Vector2Int obs, Vector2Int dir, Vector2Int mask) {
            List<Vector2Int> directions = new List<Vector2Int>();
            obs += dir * mask;  // 提取某方向（叠加得到强邻居）
            if (env.IsMoveable(obs)) {
                directions.Add(obs - pos);
            }
            return directions;
        }

        /// <summary>
        /// 斜向移动
        /// </summary>
        /// <param name="originalParent">斜向移动的原点</param>
        /// <param name="parent">父节点</param>
        /// <param name="dir">斜向移动方向</param>
        /// <param name="gCost">移动到当前点的消耗</param>
        protected void CheckDiagonal(Vector2Int originalParent, Vector2Int parent, Vector2Int dir, int gCost) {
            // 计算需要判断障碍物的位置
            Vector2Int b1 = new Vector2Int(parent.x + dir.x, parent.y);     // 垂直方向的障碍物
            Vector2Int b2 = new Vector2Int(parent.x, parent.y + dir.y);     // 水平方向的障碍物
            parent += dir;
            gCost += 14;
            if (!env.IsMoveable(parent)) return;

            if (env.IsMoveable(b1)) {
                // b1 b2均为空
                if (env.IsMoveable(b2)) {
                    // 先检测可移动后检测是否是终点
                    if (parent == end) {
                        AddJumpNode(originalParent, parent, null, gCost);
                        return;
                    }
                    // 对角分量中有跳点
                    if (DiagonalMove(parent, dir, gCost)) {
                        AddJumpNode(originalParent, parent, new Vector2Int[] { dir }, gCost);
                        return;
                    }
                    // 继续斜向移动
                    CheckDiagonal(originalParent, parent, dir, gCost);
                }
                // b1可移动 b2障碍物
                else {
                    // 先检测可移动后检测是否是终点
                    if (parent == end) {
                        AddJumpNode(originalParent, parent, null, gCost);
                        return;
                    }
                    // 检测斜向强邻居
                    List<Vector2Int> dirs = CheckForceNeighborInDiagonal(parent, b2, dir, JPSUtilities.up);
                    if (DiagonalMove(parent, dir, gCost) || dirs.Count > 0) {
                        dirs.Add(dir);      // 添加之前移动方向
                        AddJumpNode(originalParent, parent, dirs.ToArray(), gCost);
                        return;
                    }
                    CheckDiagonal(originalParent, parent, dir, gCost);
                }
            }
            else {
                // b1障碍物 b2可移动
                if (env.IsMoveable(b2)) {
                    // 先检测可移动后检测是否是终点
                    if (parent == end) {
                        AddJumpNode(originalParent, parent, null, gCost);
                        return;
                    }
                    // 检测斜向强邻居
                    List<Vector2Int> dirs = CheckForceNeighborInDiagonal(parent, b1, dir, JPSUtilities.right);
                    if (DiagonalMove(parent, dir, gCost) || dirs.Count > 0) {
                        dirs.Add(dir);
                        AddJumpNode(originalParent, parent, dirs.ToArray(), gCost);
                        return;
                    }
                    CheckDiagonal(originalParent, parent, dir, gCost);
                }
                // b1、b2不可移动（不做操作
                else {
                    // NoDo
                }
            }
        }
    }
}
