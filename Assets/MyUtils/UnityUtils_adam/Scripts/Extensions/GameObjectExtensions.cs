using UnityEngine;
using System.Linq;

namespace UnityUtils
{
    public static class GameObjectExtensions
    {
        /// <summary>
        /// 此方法用于在层级视图中隐藏GameObject
        /// </summary>
        /// <param name="gameObject"></param>
        public static void HideInHierarchy(this GameObject gameObject)
        {
            gameObject.hideFlags = HideFlags.HideInHierarchy;
        }

        /// <summary>
        /// 获取GameObject上指定类型的组件。如果该类型的组件不存在，则添加一个,避免了繁琐的null检查和手动添加
        /// </summary>
        /// <remarks>
        /// 当你不确定GameObject是否具有特定类型的组件，但无论如何都需要使用该组件时，
        /// 此方法非常有用。无需手动检查和添加组件，你可以使用此方法在一行代码中完成两个操作
        /// </remarks>
        /// <typeparam name="T">要获取或添加的组件类型</typeparam>
        /// <param name="gameObject">要从中获取组件或向其添加组件的GameObject</param>
        /// <returns>指定类型的现有组件，如果不存在则返回新添加的组件</returns>    
        public static T GetOrAdd<T>(this GameObject gameObject) where T : Component
        {
            T component = gameObject.GetComponent<T>();
            if (!component) component = gameObject.AddComponent<T>();

            return component;
        }

        /// <summary>
        /// 如果对象存在则返回对象本身，否则返回null
        /// </summary>
        /// <remarks>
        /// 此方法有助于区分空引用和已销毁的Unity对象。Unity的"== null"检查可能对已销毁的对象错误地返回true，
        /// 导致误导性行为。OrNull方法使用Unity的"空检查"，如果对象已被标记为销毁，它确保返回实际的空引用，
        /// 有助于正确链接操作并防止NullReferenceExceptions
        /// </remarks>
        /// <typeparam name="T">对象的类型</typeparam>
        /// <param name="obj">被检查的对象</param>
        /// <returns>如果对象存在且未销毁则返回对象本身，否则返回null</returns>
        public static T OrNull<T>(this T obj) where T : Object => obj ? obj : null;

        /// <summary>
        /// 销毁游戏对象的所有子对象
        /// </summary>
        /// <param name="gameObject">要销毁其子对象的GameObject</param>
        public static void DestroyChildren(this GameObject gameObject)
        {
            gameObject.transform.DestroyChildren();
        }

        /// <summary>
        /// 立即销毁给定GameObject的所有子对象
        /// </summary>
        /// <param name="gameObject">要销毁其子对象的GameObject</param>
        public static void DestroyChildrenImmediate(this GameObject gameObject)
        {
            gameObject.transform.DestroyChildrenImmediate();
        }

        /// <summary>
        /// 启用与给定GameObject关联的所有子GameObject
        /// </summary>
        /// <param name="gameObject">要启用其子GameObject的GameObject</param>
        public static void EnableChildren(this GameObject gameObject)
        {
            gameObject.transform.EnableChildren();
        }

        /// <summary>
        /// 禁用与给定GameObject关联的所有子GameObject
        /// </summary>
        /// <param name="gameObject">要禁用其子GameObject的GameObject</param>
        public static void DisableChildren(this GameObject gameObject)
        {
            gameObject.transform.DisableChildren();
        }

        /// <summary>
        /// 将GameObject的变换的位置、旋转和比例重置为其默认值
        /// </summary>
        /// <param name="gameObject">要重置其变换的GameObject</param>
        public static void ResetTransformation(this GameObject gameObject)
        {
            gameObject.transform.Reset();
        }

        /// <summary>
        /// 返回此GameObject在Unity场景层级中的层级路径
        /// </summary>
        /// <param name="gameObject">要获取路径的GameObject</param>
        /// <returns>表示此GameObject在Unity场景中完整层级路径的字符串。
        /// 这是一个以'/'分隔的字符串，其中每个部分是父级的名称，从根父级开始，
        /// 以指定GameObject的父级名称结束</returns>
        public static string Path(this GameObject gameObject)
        {
            return "/" + string.Join("/",
                gameObject.GetComponentsInParent<Transform>().Select(t => t.name).Reverse().ToArray());
        }

        /// <summary>
        /// 返回此GameObject在Unity场景层级中的完整层级路径
        /// </summary>
        /// <param name="gameObject">要获取路径的GameObject</param>
        /// <returns>表示此GameObject在Unity场景中完整层级路径的字符串。
        /// 这是一个以'/'分隔的字符串，其中每个部分是父级的名称，从根父级开始，
        /// 以指定GameObject本身的名称结束</returns>
        public static string PathFull(this GameObject gameObject)
        {
            return gameObject.Path() + "/" + gameObject.name;
        }

        /// <summary>
        /// 递归地为此GameObject及其在Unity场景层级中的所有后代设置提供的层级
        /// </summary>
        /// <param name="gameObject">要设置层级的GameObject</param>
        /// <param name="layer">要为GameObject及其所有后代设置的层级编号</param>
        public static void SetLayersRecursively(this GameObject gameObject, int layer)
        {
            gameObject.layer = layer;
            gameObject.transform.ForEveryChild(child => child.gameObject.SetLayersRecursively(layer));
        }

        /// <summary>
        /// 激活与MonoBehaviour关联的GameObject并返回实例
        /// </summary>
        /// <typeparam name="T">MonoBehaviour的类型</typeparam>
        /// <param name="obj">其GameObject将被激活的MonoBehaviour</param>
        /// <returns>MonoBehaviour的实例</returns>
        public static T SetActive<T>(this T obj) where T : MonoBehaviour
        {
            obj.gameObject.SetActive(true);
            return obj;
        }

        /// <summary>
        /// 停用与MonoBehaviour关联的GameObject并返回实例
        /// </summary>
        /// <typeparam name="T">MonoBehaviour的类型</typeparam>
        /// <param name="obj">其GameObject将被停用的MonoBehaviour</param>
        /// <returns>MonoBehaviour的实例</returns>
        public static T SetInactive<T>(this T obj) where T : MonoBehaviour
        {
            obj.gameObject.SetActive(false);
            return obj;
        }
    }
}