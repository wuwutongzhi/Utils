using System;
using System.Collections.Generic;

namespace UnityUtils
{
    public static class EnumerableExtensions
    {
        /// <summary>
        /// 对序列中的每个元素执行指定操作
        /// </summary>
        /// <typeparam name="T">序列中元素的类型</typeparam>
        /// <param name="sequence">要遍历的序列</param>
        /// <param name="action">要对每个元素执行的操作</param>
        public static void ForEach<T>(this IEnumerable<T> sequence, Action<T> action)
        {
            foreach (var item in sequence)
            {
                action(item);
            }
        }

        /// <summary>
        /// 从序列中返回一个随机元素
        /// </summary>
        /// <typeparam name="T">序列中元素的类型</typeparam>
        /// <param name="sequence">要从中选择随机元素的序列</param>
        /// <returns>序列中的一个随机元素</returns>
        public static T Random<T>(this IEnumerable<T> sequence)
        {
            if (sequence == null)
                throw new ArgumentNullException(nameof(sequence));

            if (sequence is IList<T> list)
            {
                if (list.Count == 0)
                    throw new InvalidOperationException("无法从空集合中获取随机元素。");
                return list[UnityEngine.Random.Range(0, list.Count)];
            }

            // 当输入不是IList<T>时（如流或延迟序列），使用水库抽样算法
            using var enumerator = sequence.GetEnumerator();
            if (!enumerator.MoveNext())
                throw new InvalidOperationException("无法从空集合中获取随机元素。");

            T result = enumerator.Current;
            int count = 1;
            while (enumerator.MoveNext())
            {
                if (UnityEngine.Random.Range(0, ++count) == 0)
                {
                    result = enumerator.Current;
                }
            }
            return result;
        }
    }
}