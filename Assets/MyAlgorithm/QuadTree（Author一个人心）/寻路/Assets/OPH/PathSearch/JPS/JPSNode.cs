using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OPH.PathSearch.JPS {
    /// <summary>
    /// JPS跳点
    /// </summary>
    public class JPSNode {
        public Vector2Int pos;              // 跳点位置
        public Vector2Int parent;           // 跳点的父节点位置
        public Vector2Int[] direactions;    // 跳点继续探索的方向
        /// <summary>
        /// 起点到当前位置的代价
        /// </summary>
        public int gCost;
        /// <summary>
        /// 当前位置到终点的代价
        /// </summary>
        public int hCost;
        /// <summary>
        /// 总代价
        /// </summary>
        public int fCost => gCost + hCost;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="parent">父节点位置</param>
        /// <param name="pos">当前位置</param>
        /// <param name="dirs">跳点探索方向</param>
        /// <param name="gCost">起点到当前位置的代价</param>
        /// <param name="hCost">当前位置到终点的代价</param>
        public JPSNode(Vector2Int parent,Vector2Int pos,Vector2Int[] dirs,int gCost,int hCost) {
            this.pos = pos;
            this.direactions = dirs;
            this.gCost = gCost;
            this.hCost = hCost;
            this.parent = parent;
        }
    }
}
