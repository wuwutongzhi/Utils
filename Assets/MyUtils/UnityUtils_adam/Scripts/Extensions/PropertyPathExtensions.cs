using System;
using Unity.Properties;

public static class PropertyPathExtensions
{
    /// <summary>
    /// 将属性路径的字符串表示形式转换为PropertyPath对象
    /// </summary>
    /// <param name="pathString">表示属性路径的字符串。可以包含点分隔的属性名称和方括号中的数组索引</param>
    /// <returns>表示解析后路径的PropertyPath对象</returns>
    /// <exception cref="ArgumentException">当输入字符串为null、空或仅包含空白字符时抛出</exception>
    /// <exception cref="FormatException">当路径包含无效的数组索引或未匹配的方括号时抛出</exception>
    /// <example>
    /// 有效的路径字符串：
    /// "propertyName"
    /// "parent.child"
    /// "array[0]"
    /// "parent.children[2].name"
    /// "matrix[0][1]"
    /// </example>
    public static PropertyPath ToPropertyPath(this string pathString)
    {
        if (string.IsNullOrWhiteSpace(pathString))
            throw new ArgumentException("路径字符串为null或空");

        var path = default(PropertyPath);
        foreach (var part in pathString.Split('.'))
        {
            int bracketStart = part.IndexOf('[');
            if (bracketStart < 0)
            {
                path = PropertyPath.AppendName(path, part);
                continue;
            }

            path = PropertyPath.AppendName(path, part[..bracketStart]);
            int bracketEnd;
            while ((bracketEnd = part.IndexOf(']', bracketStart)) >= 0)
            {
                if (!int.TryParse(part[(bracketStart + 1)..bracketEnd], out var index))
                    throw new FormatException($"路径中的索引无效: {part[(bracketStart + 1)..bracketEnd]}");

                path = PropertyPath.AppendIndex(path, index);
                bracketStart = part.IndexOf('[', bracketEnd);
                if (bracketStart < 0) break;
            }

            if (bracketStart >= 0)
                throw new FormatException($"路径中方括号不匹配: {part}");
        }
        return path;
    }
}