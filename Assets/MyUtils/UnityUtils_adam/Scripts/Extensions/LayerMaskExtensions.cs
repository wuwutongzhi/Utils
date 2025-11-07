using UnityEngine;

namespace UnityUtils
{
    public static class LayerMaskExtensions
    {
        /// <summary>
        /// 检查给定的层级编号是否包含在层级掩码中
        /// </summary>
        /// <param name="mask">要检查的层级掩码</param>
        /// <param name="layerNumber">要检查是否包含在层级掩码中的层级编号</param>
        /// <returns>如果层级编号包含在层级掩码中则返回true，否则返回false</returns>
        public static bool Contains(this LayerMask mask, int layerNumber)
        {
            return mask == (mask | (1 << layerNumber));
        }
    }
//LayerMask在Unity中实际上是32位的位掩码（对应32个层级）

//每个位代表一个层级是否被包含（1=包含，0=不包含）

//例如：如果掩码值为5（二进制101），表示包含第0层和第2层
}