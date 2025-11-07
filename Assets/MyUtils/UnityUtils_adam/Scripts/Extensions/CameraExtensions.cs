using UnityEngine;

namespace UnityUtils
{
    public static class CameraExtensions
    {
        /// <summary>
        /// 计算并返回带有可选边距的视口范围。用于计算视锥体剔除。
        /// </summary>
        /// <param name="camera">此方法扩展的相机对象</param>
        /// <param name="viewportMargin">应用于视口范围的可选边距。默认为0.2, 0.2</param>
        /// <returns>应用边距后的视口范围，以Vector2形式返回</returns>
        public static Vector2 GetViewportExtentsWithMargin(this Camera camera, Vector2? viewportMargin = null)
        {
            Vector2 margin = viewportMargin ?? new Vector2(0.2f, 0.2f);

            Vector2 result;
            float halfFieldOfView = camera.fieldOfView * 0.5f * Mathf.Deg2Rad;
            result.y = camera.nearClipPlane * Mathf.Tan(halfFieldOfView);
            result.x = result.y * camera.aspect + margin.x;
            result.y += margin.y;
            return result;
        }
    }
}