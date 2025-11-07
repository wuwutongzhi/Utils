using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace UnityUtils
{
    public static class TransformExtensions
    {
        /// <summary>
        /// 检查变换是否在目标变换的特定距离内，并可选择是否在特定角度（视野）内
        /// </summary>
        /// <param name="source">要检查的变换</param>
        /// <param name="target">要比较距离和可选角度的目标变换</param>
        /// <param name="maxDistance">两个变换之间允许的最大距离</param>
        /// <param name="maxAngle">变换的前向向量与指向目标方向之间允许的最大角度（默认为360）</param>
        /// <returns>如果变换在目标的范围内和角度内（如果提供）则返回true，否则返回false</returns>
        public static bool InRangeOf(this Transform source, Transform target, float maxDistance, float maxAngle = 360f)
        {
            Vector3 directionToTarget = (target.position - source.position).With(y: 0);
            return directionToTarget.magnitude <= maxDistance && Vector3.Angle(source.forward, directionToTarget) <= maxAngle / 2;
        }

        /// <summary>
        /// 检索给定Transform的所有子对象
        /// </summary>
        /// <remarks>
        /// 此方法可与LINQ一起使用，对所有子Transform执行操作。例如，
        /// 您可以使用它来查找具有特定标签的所有子对象，禁用所有子对象等。
        /// Transform实现了IEnumerable和GetEnumerator方法，该方法返回其所有子对象的IEnumerator。
        /// </remarks>
        /// <param name="parent">要从中检索子对象的Transform</param>
        /// <returns>包含父对象所有子Transform的IEnumerable&lt;Transform&gt;</returns>    
        public static IEnumerable<Transform> Children(this Transform parent)
        {
            foreach (Transform child in parent)
            {
                yield return child;
            }
        }

        /// <summary>
        /// 重置变换的位置、缩放和旋转
        /// </summary>
        /// <param name="transform">要使用的变换</param>
        public static void Reset(this Transform transform)
        {
            transform.position = Vector3.zero;
            transform.localRotation = Quaternion.identity;
            transform.localScale = Vector3.one;
        }

        /// <summary>
        /// 销毁给定变换的所有子游戏对象
        /// </summary>
        /// <param name="parent">要销毁其子游戏对象的Transform</param>
        public static void DestroyChildren(this Transform parent)
        {
            parent.ForEveryChild(child => Object.Destroy(child.gameObject));
        }

        /// <summary>
        /// 立即销毁给定变换的所有子游戏对象
        /// </summary>
        /// <param name="parent">要立即销毁其子游戏对象的Transform</param>
        public static void DestroyChildrenImmediate(this Transform parent)
        {
            parent.ForEveryChild(child => Object.DestroyImmediate(child.gameObject));
        }

        /// <summary>
        /// 启用给定变换的所有子游戏对象
        /// </summary>
        /// <param name="parent">要启用其子游戏对象的Transform</param>
        public static void EnableChildren(this Transform parent)
        {
            parent.ForEveryChild(child => child.gameObject.SetActive(true));
        }

        /// <summary>
        /// 禁用给定变换的所有子游戏对象
        /// </summary>
        /// <param name="parent">要禁用其子游戏对象的Transform</param>
        public static void DisableChildren(this Transform parent)
        {
            parent.ForEveryChild(child => child.gameObject.SetActive(false));
        }

        /// <summary>
        /// 对给定变换的每个子对象执行指定操作
        /// </summary>
        /// <param name="parent">父变换</param>
        /// <param name="action">要对每个子对象执行的操作</param>
        /// <remarks>
        /// 此方法以反向顺序遍历所有子变换，并对它们执行给定的操作。
        /// 该操作是一个以Transform作为参数的委托。
        /// </remarks>
        public static void ForEveryChild(this Transform parent, System.Action<Transform> action)
        {
            for (var i = parent.childCount - 1; i >= 0; i--)
            {
                action(parent.GetChild(i));
            }
        }

        [Obsolete("已重命名为ForEveryChild")]
        static void PerformActionOnChildren(this Transform parent, System.Action<Transform> action)
        {
            parent.ForEveryChild(action);
        }
    }
}