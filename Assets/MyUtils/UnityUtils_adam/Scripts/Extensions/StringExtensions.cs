using System;
using System.Collections.Generic;
using System.Linq;

namespace UnityUtils
{
    public static class StringExtensions
    {
        /// <summary>检查字符串是否为Null或空白</summary>
        public static bool IsNullOrWhiteSpace(this string val) => string.IsNullOrWhiteSpace(val);

        /// <summary>检查字符串是否为Null或空</summary>
        public static bool IsNullOrEmpty(this string value) => string.IsNullOrEmpty(value);

        /// <summary>检查字符串是否包含null、空或空白</summary>
        public static bool IsBlank(this string val) => val.IsNullOrWhiteSpace() || val.IsNullOrEmpty();

        /// <summary>检查字符串是否为null，如果是则返回空字符串</summary>
        public static string OrEmpty(this string val) => val ?? string.Empty;

        /// <summary>
        /// 将字符串缩短到指定的最大长度。如果字符串长度小于maxLength，则返回原字符串。
        /// </summary>
        public static string Shorten(this string val, int maxLength)
        {
            if (val.IsBlank()) return val;
            return val.Length <= maxLength ? val : val.Substring(0, maxLength);
        }

        /// <summary>从开始索引到结束索引切片字符串</summary>
        /// <result>切片后的字符串</result>
        public static string Slice(this string val, int startIndex, int endIndex)
        {
            if (val.IsBlank())
            {
                throw new ArgumentNullException(nameof(val), "值不能为null或空");
            }

            if (startIndex < 0 || startIndex > val.Length - 1)
            {
                throw new ArgumentOutOfRangeException(nameof(startIndex));
            }

            // 如果结束索引为负数，将从字符串末尾开始计数
            endIndex = endIndex < 0 ? val.Length + endIndex : endIndex;

            if (endIndex < 0 || endIndex < startIndex || endIndex > val.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(endIndex));
            }

            return val.Substring(startIndex, endIndex - startIndex);
        }

        /// <summary>
        /// 将输入字符串转换为字母数字字符串，可选择允许句点。
        /// </summary>
        /// <param name="input">要转换的输入字符串</param>
        /// <param name="allowPeriods">布尔标志，指示输出字符串中是否应允许句点</param>
        /// <returns>
        /// 仅包含字母数字字符、下划线以及可选句点的新字符串。
        /// 如果输入字符串为null或空，则返回空字符串。
        /// </returns>
        public static string ConvertToAlphanumeric(this string input, bool allowPeriods = false)
        {
            if (string.IsNullOrEmpty(input))
                return string.Empty;

            List<char> filteredChars = new List<char>();
            int lastValidIndex = -1;

            // 遍历输入字符串，过滤并确定有效的开始/结束索引
            foreach (char character in input
                         .Where(character => char
                             .IsLetterOrDigit(character) || character == '_' || (allowPeriods && character == '.'))
                         .Where(character => filteredChars.Count != 0 || (!char.IsDigit(character) && character != '.')))
            {

                filteredChars.Add(character);
                lastValidIndex = filteredChars.Count - 1; // 为有效字符更新lastValidIndex
            }

            // 移除尾部句点
            while (lastValidIndex >= 0 && filteredChars[lastValidIndex] == '.')
            {
                lastValidIndex--;
            }

            // 返回过滤后的字符串
            return lastValidIndex >= 0
                ? new string(filteredChars.ToArray(), 0, lastValidIndex + 1) : string.Empty;
        }

        // 富文本格式化，用于支持富文本的Unity UI元素
        public static string RichColor(this string text, string color) => $"<color={color}>{text}</color>";
        public static string RichSize(this string text, int size) => $"<size={size}>{text}</size>";
        public static string RichBold(this string text) => $"<b>{text}</b>";
        public static string RichItalic(this string text) => $"<i>{text}</i>";
        public static string RichUnderline(this string text) => $"<u>{text}</u>";
        public static string RichStrikethrough(this string text) => $"<s>{text}</s>";
        public static string RichFont(this string text, string font) => $"<font={font}>{text}</font>";
        public static string RichAlign(this string text, string align) => $"<align={align}>{text}</align>";
        public static string RichGradient(this string text, string color1, string color2) => $"<gradient={color1},{color2}>{text}</gradient>";
        public static string RichRotation(this string text, float angle) => $"<rotate={angle}>{text}</rotate>";
        public static string RichSpace(this string text, float space) => $"<space={space}>{text}</space>";
    }
}