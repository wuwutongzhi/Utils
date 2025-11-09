using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OPH.PathSearch {
    /// <summary>
    /// 网格接口
    /// </summary>
    public interface IGrid {
        /// <summary>
        /// 是否在地图中
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        bool IsInMap(in Vector2Int pos);

        /// <summary>
        /// 是否在地图中
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        bool IsInMap(int x,int y);

        /// <summary>
        /// 该位置是否可以移动
        /// </summary>
        /// <param name="pos">坐标位置</param>
        /// <returns></returns>
        bool IsMoveable(in Vector2Int pos);

        /// <summary>
        /// 该位置是否是障碍物
        /// </summary>
        /// <param name="pos">坐标位置</param>
        /// <returns></returns>
        bool IsObstacle(in Vector2Int pos);

        /// <summary>
        /// 获取周围格子（9宫格）
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        List<Vector2Int> GetArroundCell(in Vector2Int pos);
    }
}
