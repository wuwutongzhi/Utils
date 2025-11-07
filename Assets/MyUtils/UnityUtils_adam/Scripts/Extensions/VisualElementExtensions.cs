using UnityEngine;
using UnityEngine.UIElements;

namespace UnityUtils
{
    public static class VisualElementExtensions
    {
        /// <summary>
        /// 创建一个新的子VisualElement并将其添加到父元素
        /// </summary>
        /// <param name="parent">要添加子元素的父VisualElement</param>
        /// <param name="classes">要添加到子元素的CSS类</param>
        /// <returns>创建的子VisualElement</returns>
        public static VisualElement CreateChild(this VisualElement parent, params string[] classes)
        {
            var child = new VisualElement();
            child.AddClass(classes).AddTo(parent);
            return child;
        }

        /// <summary>
        /// 创建类型为T的新子元素并将其添加到父元素
        /// </summary>
        /// <typeparam name="T">子VisualElement的类型</typeparam>
        /// <param name="parent">要添加子元素的父VisualElement</param>
        /// <param name="classes">要添加到子元素的CSS类</param>
        /// <returns>创建的T类型的子VisualElement</returns>
        public static T CreateChild<T>(this VisualElement parent, params string[] classes)
            where T : VisualElement, new()
        {
            var child = new T();
            child.AddClass(classes).AddTo(parent);
            return child;
        }

        /// <summary>
        /// 将子VisualElement添加到父元素并返回子元素
        /// </summary>
        /// <typeparam name="T">子VisualElement的类型</typeparam>
        /// <param name="child">要添加的子VisualElement</param>
        /// <param name="parent">要添加子元素的父VisualElement</param>
        /// <returns>添加的子VisualElement</returns>
        public static T AddTo<T>(this T child, VisualElement parent) where T : VisualElement
        {
            parent.Add(child);
            return child;
        }

        /// <remarks>
        /// 请参阅 <see cref="AddTo{T}(T, VisualElement)"/> 了解如何将子元素添加到父元素。
        /// </remarks>
        public static void RemoveFrom<T>(this T child, VisualElement parent)
            where T : VisualElement => parent.Remove(child);

        /// <summary>
        /// 将指定的CSS类添加到VisualElement
        /// </summary>
        /// <typeparam name="T">VisualElement的类型</typeparam>
        /// <param name="visualElement">要添加类的VisualElement</param>
        /// <param name="classes">要添加的CSS类</param>
        /// <returns>添加了类的VisualElement</returns>
        public static T AddClass<T>(this T visualElement, params string[] classes) where T : VisualElement
        {
            foreach (string cls in classes)
            {
                if (!string.IsNullOrEmpty(cls))
                {
                    visualElement.AddToClassList(cls);
                }
            }
            return visualElement;
        }

        /// <remarks>
        /// 请参阅 <see cref="AddClass{T}(T, string[])"/> 了解如何添加类。
        /// </remarks>
        public static void RemoveClass<T>(this T visualElement, params string[] classes) where T : VisualElement
        {
            foreach (string cls in classes)
            {
                if (!string.IsNullOrEmpty(cls))
                {
                    visualElement.RemoveFromClassList(cls);
                }
            }
        }

        /// <summary>
        /// 向VisualElement添加操作器
        /// </summary>
        /// <typeparam name="T">VisualElement的类型</typeparam>
        /// <param name="visualElement">要添加操作器的VisualElement</param>
        /// <param name="manipulator">要添加的操作器</param>
        /// <returns>添加了操作器的VisualElement</returns>
        public static T WithManipulator<T>(this T visualElement, IManipulator manipulator) where T : VisualElement
        {
            visualElement.AddManipulator(manipulator);
            return visualElement;
        }

        /// <summary>
        /// 使用给定的Sprite设置VisualElement的背景图像
        /// </summary>
        /// <param name="imageContainer">要设置背景图像的VisualElement</param>
        /// <param name="sprite">要用作背景图像的Sprite</param>
        public static void SetImageFromSprite(this VisualElement imageContainer, Sprite sprite)
        {
            var texture = sprite.texture;
            if (texture)
            {
                imageContainer.style.backgroundImage = new StyleBackground(texture);
            }
        }
    }
}