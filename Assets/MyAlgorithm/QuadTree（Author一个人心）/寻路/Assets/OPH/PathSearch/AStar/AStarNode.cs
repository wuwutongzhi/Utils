using UnityEngine;

namespace OPH.PathSearch.AStar {
    /// <summary>
    /// 创建人：一个人心
    /// 功能说明：AStar节点
    /// </summary>
    public class AStarNode {
        /// <summary>
        /// 当前节点位置
        /// </summary>
        public Vector2Int pos;
        /// <summary>
        /// 父节点
        /// </summary>
        public AStarNode parent;
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
        /// <param name="pos">当前节点位置</param>
        /// <param name="parent">父节点位置</param>
        /// <param name="gCost">起点到当前节点代价</param>
        /// <param name="hCost">当前节点到终点代价</param>
        public AStarNode(Vector2Int pos, AStarNode parent, int gCost, int hCost) {
            this.pos = pos;
            this.parent = parent;
            this.gCost = gCost;
            this.hCost = hCost;
        }
    }
}
