using UnityEngine;

namespace UnityUtils {
    public static class VectorMath
    {
        /// <summary>
        /// 计算两个向量在由法线向量定义的平面上的有符号角度
        /// </summary>
        /// <param name="vector1">第一个向量</param>
        /// <param name="vector2">第二个向量</param>
        /// <param name="planeNormal">用于计算角度的平面的法线向量</param>
        /// <returns>向量之间的有符号角度（度数）</returns>
        public static float GetAngle(Vector3 vector1, Vector3 vector2, Vector3 planeNormal)
        {
            var angle = Vector3.Angle(vector1, vector2); // 获取无符号角度
            var sign = Mathf.Sign(Vector3.Dot(planeNormal, Vector3.Cross(vector1, vector2))); // 通过叉积和点积确定方向
            return angle * sign; // 返回带方向的角度
        }

        /// <summary>
        /// 计算向量与归一化方向的点积
        /// </summary>
        /// <param name="vector">要投影的向量</param>
        /// <param name="direction">要投影到的方向向量</param>
        /// <returns>向量与方向的点积</returns>
        public static float GetDotProduct(Vector3 vector, Vector3 direction) =>
            Vector3.Dot(vector, direction.normalized);

        /// <summary>
        /// 移除向量在给定方向上的分量
        /// </summary>
        /// <param name="vector">要移除分量的向量</param>
        /// <param name="direction">要移除的方向向量</param>
        /// <returns>移除了指定方向分量后的向量</returns>
        public static Vector3 RemoveDotVector(Vector3 vector, Vector3 direction)
        {
            direction.Normalize(); // 确保方向向量是单位向量
            return vector - direction * Vector3.Dot(vector, direction); // 减去在方向上的投影
        }

        /// <summary>
        /// 提取并返回向量在给定方向上的分量
        /// </summary>
        /// <param name="vector">要提取分量的向量</param>
        /// <param name="direction">要提取的方向向量</param>
        /// <returns>向量在给定方向上的分量</returns>
        public static Vector3 ExtractDotVector(Vector3 vector, Vector3 direction)
        {
            direction.Normalize(); // 确保方向向量是单位向量
            return direction * Vector3.Dot(vector, direction); // 返回在方向上的投影
        }

        /// <summary>
        /// 使用指定的上方向将向量旋转到由法线向量定义的平面上
        /// </summary>
        /// <param name="vector">要旋转到平面上的向量</param>
        /// <param name="planeNormal">目标平面的法线向量</param>
        /// <param name="upDirection">用于确定旋转的当前"上"方向</param>
        /// <returns>旋转到指定平面后的向量</returns>
        public static Vector3 RotateVectorOntoPlane(Vector3 vector, Vector3 planeNormal, Vector3 upDirection)
        {
            // 计算旋转：从上方向到平面法线的旋转
            var rotation = Quaternion.FromToRotation(upDirection, planeNormal);

            // 对向量应用旋转
            vector = rotation * vector;

            return vector;
        }

        /// <summary>
        /// 将给定点投影到由起始位置和方向向量定义的直线上
        /// </summary>
        /// <param name="lineStartPosition">直线的起始位置</param>
        /// <param name="lineDirection">直线的方向向量，应该归一化</param>
        /// <param name="point">要投影到直线上的点</param>
        /// <returns>直线上最接近原点的投影点</returns>
        public static Vector3 ProjectPointOntoLine(Vector3 lineStartPosition, Vector3 lineDirection, Vector3 point)
        {
            var projectLine = point - lineStartPosition; // 从直线起点到点的向量
            var dotProduct = Vector3.Dot(projectLine, lineDirection); // 计算点积

            return lineStartPosition + lineDirection * dotProduct; // 返回投影点
        }

        /// <summary>
        /// 以指定速度在给定时间间隔内将向量向目标向量增量移动
        /// </summary>
        /// <param name="currentVector">要递增的当前向量</param>
        /// <param name="speed">向目标向量移动的速度</param>
        /// <param name="deltaTime">移动的时间间隔</param>
        /// <param name="targetVector">要接近的目标向量</param>
        /// <returns>按指定速度和时间间隔向目标向量递增后的新向量</returns>
        public static Vector3 IncrementVectorTowardTargetVector(Vector3 currentVector, float speed, float deltaTime,
            Vector3 targetVector)
        {
            return Vector3.MoveTowards(currentVector, targetVector, speed * deltaTime);
        }
    }
}
