using UnityEngine;

namespace UnityUtils
{
    public static class Vector2Extensions
    {
        /// <summary>
        /// 向Vector2的任何x y值添加值
        /// </summary>
        public static Vector2 Add(this Vector2 vector2, float x = 0, float y = 0)
        {
            return new Vector2(vector2.x + x, vector2.y + y);
        }

        /// <summary>
        /// 设置Vector2的任何x y值
        /// </summary>
        public static Vector2 With(this Vector2 vector2, float? x = null, float? y = null)
        {
            return new Vector2(x ?? vector2.x, y ?? vector2.y);
        }

        /// <summary>
        /// 返回一个布尔值，指示当前Vector2是否在另一个Vector2的给定范围内
        /// </summary>
        /// <param name="current">当前Vector2位置</param>
        /// <param name="target">要比较的Vector2位置</param>
        /// <param name="range">要比较的范围值</param>
        /// <returns>如果当前Vector2在目标Vector2的给定范围内则为true，否则为false</returns>
        public static bool InRangeOf(this Vector2 current, Vector2 target, float range)
        {
            return (current - target).sqrMagnitude <= range * range;
        }

        /// <summary>
        /// 基于中心Vector2点（原点）周围的最小和最大半径值计算圆环（环形区域）中的随机点
        /// </summary>
        /// <param name="origin">圆环的中心Vector2点</param>
        /// <param name="minRadius">圆环的最小半径</param>
        /// <param name="maxRadius">圆环的最大半径</param>
        /// <returns>指定圆环内的随机Vector2点</returns>
        public static Vector2 RandomPointInAnnulus(this Vector2 origin, float minRadius, float maxRadius)
        {
            float angle = Random.value * Mathf.PI * 2f;
            Vector2 direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));

            // 对半径进行平方和开方运算，确保在圆环内均匀分布
            float minRadiusSquared = minRadius * minRadius;
            float maxRadiusSquared = maxRadius * maxRadius;
            float distance = Mathf.Sqrt(Random.value * (maxRadiusSquared - minRadiusSquared) + minRadiusSquared);

            // 计算位置向量
            Vector2 position = direction * distance;
            return origin + position;
        }
    }
}