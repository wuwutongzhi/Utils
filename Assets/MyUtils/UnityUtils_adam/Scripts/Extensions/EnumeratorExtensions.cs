using System.Collections.Generic;

namespace UnityUtils
{
    public static class EnumeratorExtensions
    {
        /// <summary>
        /// 将 IEnumerator<T> 转换为 IEnumerable<T>
        /// </summary>
        /// <param name="e">IEnumerator<T> 的实例</param>
        /// <returns>具有与输入实例相同元素的 IEnumerable<T></returns>    
        public static IEnumerable<T> ToEnumerable<T>(this IEnumerator<T> e)
        {
            while (e.MoveNext())
            {
                yield return e.Current;
            }
        }
    }
}