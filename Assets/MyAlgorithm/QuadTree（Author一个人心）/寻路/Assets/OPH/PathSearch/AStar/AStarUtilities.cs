using UnityEngine;

namespace OPH.PathSearch.AStar {
    /// <summary>
    /// 创建人：一个人心
    /// 功能说明：AStar工具类
    /// </summary>
    public static class AStarUtilities {
        public static readonly Vector2Int left = Vector2Int.left;
        public static readonly Vector2Int right = Vector2Int.right;
        public static readonly Vector2Int up = Vector2Int.up;
        public static readonly Vector2Int down = Vector2Int.down;
        public static readonly Vector2Int leftUp = new Vector2Int(-1, 1);
        public static readonly Vector2Int leftDown = new Vector2Int(-1, -1);
        public static readonly Vector2Int rightUp = Vector2Int.one;
        public static readonly Vector2Int rightDown = new Vector2Int(1, -1);

        /// <summary>
        /// 是否是水平or垂直
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool IsLine(in Vector2Int a, in Vector2Int b) {
            return (a.x == b.x) || (a.y == b.y);
        }

        /// <summary>
        /// 曼哈顿距离
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        public static int ManhattanLen(in Vector2Int p1, in Vector2Int p2) {
            int diff_x = Mathf.Abs(p1.x - p2.x);
            int diff_y= Mathf.Abs(p1.y - p2.y);
            int min = Mathf.Min(diff_x, diff_y);
            int diff = Mathf.Abs(diff_x - diff_y);
            return min * 14 + diff * 10;
        }
    }
}
