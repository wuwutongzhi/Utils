using System.Collections.Generic;
using UnityEngine;

namespace OPH.PathSearch.JPS {
    /// <summary>
    /// JPS工具类
    /// </summary>
    public static class JPSUtilities {
        public static readonly Vector2Int left = Vector2Int.left;
        public static readonly Vector2Int right = Vector2Int.right;
        public static readonly Vector2Int up = Vector2Int.up;
        public static readonly Vector2Int down = Vector2Int.down;
        public static readonly Vector2Int leftUp = new Vector2Int(-1, 1);
        public static readonly Vector2Int leftDown = new Vector2Int(-1, -1);
        public static readonly Vector2Int rightUp = Vector2Int.one;
        public static readonly Vector2Int rightDown = new Vector2Int(1, -1);
        /// <summary>
        /// 方向的垂直方向（lut == LookUp table 查找表）
        /// </summary>
        public static readonly Dictionary<Vector2Int, Vector2Int[]> verticalDirLut = new Dictionary<Vector2Int, Vector2Int[]> {
            {left,new Vector2Int[]{ up, down} },
            {right,new Vector2Int[]{ up, down} },
            {up,new Vector2Int[]{ left, right} },
            {down,new Vector2Int[]{ left, right} },
        };
        /// <summary>
        /// 起点的初始方向
        /// </summary>
        public static readonly Vector2Int[] StartDirs = new Vector2Int[] {
            up,down,left,right,
            leftUp,leftDown,
            rightUp,rightDown
        };

        /// <summary>
        /// 判断当前方向是否是直线方向（水平、垂直）
        /// </summary>
        /// <param name="dir"></param>
        /// <returns></returns>
        public static bool IsLineDireaction(in Vector2Int dir) {
            return dir.x * dir.y == 0;
        }

        /// <summary>
        /// 曼哈顿距离
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        public static int ManhattanLen(in Vector2Int p1, in Vector2Int p2) {
            int diff_x = Mathf.Abs(p1.x - p2.x);
            int diff_y = Mathf.Abs(p1.y - p2.y);
            int min = Mathf.Min(diff_x, diff_y);
            int diff = Mathf.Abs(diff_x - diff_y);
            return min * 14 + diff * 10;
        }

        /// <summary>
        /// 欧拉距离
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        public static int EulerLen(in Vector2Int p1, in Vector2Int p2) {
            int dx = p1.x - p2.x;
            int dy = p1.y - p2.y;
            return dx * dx + dy * dy;
        }
    }
}
