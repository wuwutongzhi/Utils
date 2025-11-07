using UnityEngine;

namespace UnityUtils
{
    public static class Vector3Extensions
    {
        /// <summary>
        /// 设置Vector3的任何x y z值
        /// </summary>
        public static Vector3 With(this Vector3 vector, float? x = null, float? y = null, float? z = null)
        {
            return new Vector3(x ?? vector.x, y ?? vector.y, z ?? vector.z);
        }

        /// <summary>
        /// 向Vector3的任何x y z值添加值
        /// </summary>
        public static Vector3 Add(this Vector3 vector, float x = 0, float y = 0, float z = 0)
        {
            return new Vector3(vector.x + x, vector.y + y, vector.z + z);
        }

        /// <summary>
        /// 返回一个布尔值，指示当前Vector3是否在另一个Vector3的给定范围内
        /// </summary>
        /// <param name="current">当前Vector3位置</param>
        /// <param name="target">要比较的Vector3位置</param>
        /// <param name="range">要比较的范围值</param>
        /// <returns>如果当前Vector3在目标Vector3的给定范围内则为true，否则为false</returns>
        public static bool InRangeOf(this Vector3 current, Vector3 target, float range)
        {
            return (current - target).sqrMagnitude <= range * range;
        }

        /// <summary>
        /// 对两个Vector3对象进行分量-wise的除法
        /// </summary>
        /// <remarks>
        /// 对于v0中的每个分量（x, y, z），如果v1中的对应分量不为零，则用v1中的对应分量除v0中的分量。
        /// 否则，v0中的分量保持不变。
        /// </remarks>
        /// <example>
        /// 使用'ComponentDivide'按比例缩放游戏对象：
        /// <code>
        /// myObject.transform.localScale = originalScale.ComponentDivide(targetDimensions);
        /// </code>
        /// 这将对象大小缩放到适合目标尺寸，同时保持其原始比例。
        ///</example>
        /// <param name="v0">此方法扩展的Vector3对象</param>
        /// <param name="v1">v0要除以的Vector3对象</param>
        /// <returns>分量-wise除法产生的新Vector3对象</returns>
        public static Vector3 ComponentDivide(this Vector3 v0, Vector3 v1)
        {
            return new Vector3(
                v1.x != 0 ? v0.x / v1.x : v0.x,
                v1.y != 0 ? v0.y / v1.y : v0.y,
                v1.z != 0 ? v0.z / v1.z : v0.z);
        }

        /// <summary>
        /// 将Vector2转换为y值为0的Vector3
        /// </summary>
        /// <param name="v2">要转换的Vector2</param>
        /// <returns>具有Vector2的x和z值且y值为0的Vector3</returns>
        public static Vector3 ToVector3(this Vector2 v2)
        {
            return new Vector3(v2.x, 0, v2.y);
        }

        /// <summary>
        /// 在指定范围内向<see cref="Vector3"/>的分量添加随机偏移
        /// </summary>
        /// <param name="vector">将应用随机偏移的原始向量</param>
        /// <param name="range">可以添加到向量每个分量或从每个分量减去的随机偏移的最大绝对值</param>
        /// <returns>一个新的<see cref="Vector3"/>，其X、Y和Z分量已应用随机偏移。
        /// 每个偏移在范围[-<paramref name="range"/>, <paramref name="range"/>]内</returns>
        public static Vector3 RandomOffset(this Vector3 vector, float range)
        {
            return vector + new Vector3(
                Random.Range(-range, range),
                Random.Range(-range, range),
                Random.Range(-range, range)
            );
        }

        /// <summary>
        /// 基于中心Vector3点（原点）周围的最小和最大半径值计算圆环（环形区域）中的随机点
        /// </summary>
        /// <param name="origin">圆环的中心Vector3点</param>
        /// <param name="minRadius">圆环的最小半径</param>
        /// <param name="maxRadius">圆环的最大半径</param>
        /// <returns>指定圆环内的随机Vector3点</returns>
        public static Vector3 RandomPointInAnnulus(this Vector3 origin, float minRadius, float maxRadius)
        {
            float angle = Random.value * Mathf.PI * 2f;
            Vector2 direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));

            // 对半径进行平方和开方运算，确保在圆环内均匀分布
            float minRadiusSquared = minRadius * minRadius;
            float maxRadiusSquared = maxRadius * maxRadius;
            float distance = Mathf.Sqrt(Random.value * (maxRadiusSquared - minRadiusSquared) + minRadiusSquared);

            // 将2D方向向量转换为3D位置向量
            Vector3 position = new Vector3(direction.x, 0, direction.y) * distance;
            return origin + position;
        }

        /// <summary>
        /// 将Vector3的分量向下舍入到给定量化步长的最近倍数。
        /// 这对于降低精度或将位置对齐到网格很有用，
        /// 例如限制NavMesh重建或离散化移动更新。
        /// <param name="position">要量化的原始Vector3位置</param>
        /// <param name="quantization">每个分量（x, y, z）的量化步长</param>
        /// <returns>一个新的Vector3，每个分量向下舍入到对应量化步长的最近倍数</returns>
        /// </summary>
        public static Vector3 Quantize(this Vector3 position, Vector3 quantization)
        {
            return Vector3.Scale(
                quantization,
                new Vector3(
                    Mathf.Floor(position.x / quantization.x),
                    Mathf.Floor(position.y / quantization.y),
                    Mathf.Floor(position.z / quantization.z)
                ));
        }
    }
}