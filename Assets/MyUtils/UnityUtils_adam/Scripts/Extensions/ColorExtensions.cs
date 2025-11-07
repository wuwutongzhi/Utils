using System;
using UnityEngine;

namespace UnityUtils
{
    public static class ColorExtensions
    {
        /// <summary>
        /// 设置颜色的透明度分量
        /// </summary>
        /// <param name="color">原始颜色</param>
        /// <param name="alpha">新的透明度值</param>
        /// <returns>具有指定透明度值的新颜色</returns>
        public static Color SetAlpha(this Color color, float alpha)
            => new(color.r, color.g, color.b, alpha);

        /// <summary>
        /// 将两个颜色的RGBA分量相加并将结果限制在0到1之间
        /// </summary>
        /// <param name="thisColor">第一个颜色</param>
        /// <param name="otherColor">第二个颜色</param>
        /// <returns>两个颜色相加后的新颜色，分量值限制在0到1之间</returns>
        public static Color Add(this Color thisColor, Color otherColor)
            => (thisColor + otherColor).Clamp01();

        /// <summary>
        /// 从一个颜色中减去另一个颜色的RGBA分量并将结果限制在0到1之间
        /// </summary>
        /// <param name="thisColor">第一个颜色</param>
        /// <param name="otherColor">第二个颜色</param>
        /// <returns>两个颜色相减后的新颜色，分量值限制在0到1之间</returns>
        public static Color Subtract(this Color thisColor, Color otherColor)
            => (thisColor - otherColor).Clamp01();

        /// <summary>
        /// 将颜色的RGBA分量限制在0到1之间
        /// </summary>
        /// <param name="color">原始颜色</param>
        /// <returns>每个分量都限制在0到1之间的新颜色</returns>
        static Color Clamp01(this Color color)
        {
            return new Color
            {
                r = Mathf.Clamp01(color.r),
                g = Mathf.Clamp01(color.g),
                b = Mathf.Clamp01(color.b),
                a = Mathf.Clamp01(color.a)
            };
        }

        /// <summary>
        /// 将颜色转换为十六进制字符串
        /// </summary>
        /// <param name="color">要转换的颜色</param>
        /// <returns>颜色的十六进制字符串表示</returns>
        public static string ToHex(this Color color)
            => $"#{ColorUtility.ToHtmlStringRGBA(color)}";

        /// <summary>
        /// 将十六进制字符串转换为颜色
        /// </summary>
        /// <param name="hex">要转换的十六进制字符串</param>
        /// <returns>十六进制字符串表示的颜色</returns>
        public static Color FromHex(this string hex)
        {
            if (ColorUtility.TryParseHtmlString(hex, out Color color))
            {
                return color;
            }

            throw new ArgumentException("无效的十六进制字符串", nameof(hex));
        }

        /// <summary>
        /// 使用指定比例混合两种颜色
        /// </summary>
        /// <param name="color1">第一种颜色</param>
        /// <param name="color2">第二种颜色</param>
        /// <param name="ratio">混合比例（0到1之间）</param>
        /// <returns>混合后的颜色</returns>
        public static Color Blend(this Color color1, Color color2, float ratio)
        {
            ratio = Mathf.Clamp01(ratio);
            return new Color(
                color1.r * (1 - ratio) + color2.r * ratio,
                color1.g * (1 - ratio) + color2.g * ratio,
                color1.b * (1 - ratio) + color2.b * ratio,
                color1.a * (1 - ratio) + color2.a * ratio
            );
        }

        /// <summary>
        /// 反转颜色
        /// </summary>
        /// <param name="color">要反转的颜色</param>
        /// <returns>反转后的颜色</returns>
        public static Color Invert(this Color color)
            => new(1 - color.r, 1 - color.g, 1 - color.b, color.a);
    }
}