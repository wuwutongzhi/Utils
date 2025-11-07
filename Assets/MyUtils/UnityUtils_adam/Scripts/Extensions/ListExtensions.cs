using System;
using System.Collections.Generic;
using System.Linq;

namespace UnityUtils
{
    public static class ListExtensions
    {
        static Random rng;

        /// <summary>
        /// 判断集合是否为null或没有元素，无需枚举整个集合来获取计数
        ///
        /// 使用LINQ的Any()方法来判断集合是否为空，因此会有一些GC开销
        /// </summary>
        /// <param name="list">要评估的列表</param>
        public static bool IsNullOrEmpty<T>(this IList<T> list)
        {
            return list == null || !list.Any();
        }

        /// <summary>
        /// 创建一个作为原列表副本的新列表
        /// </summary>
        /// <param name="list">要被复制的原列表</param>
        /// <returns>原列表副本的新列表</returns>
        public static List<T> Clone<T>(this IList<T> list)
        {
            List<T> newList = new List<T>();
            foreach (T item in list)
            {
                newList.Add(item);
            }

            return newList;
        }

        /// <summary>
        /// 交换列表中指定索引处的两个元素
        /// </summary>
        /// <param name="list">列表</param>
        /// <param name="indexA">第一个元素的索引</param>
        /// <param name="indexB">第二个元素的索引</param>
        public static void Swap<T>(this IList<T> list, int indexA, int indexB)
        {
            (list[indexA], list[indexB]) = (list[indexB], list[indexA]);
        }

        /// <summary>
        /// 使用Fisher-Yates算法的Durstenfeld实现来洗牌列表中的元素
        /// 此方法会原地修改输入列表，确保每种排列的可能性相等，并返回列表以支持方法链
        /// 参考: http://en.wikipedia.org/wiki/Fisher-Yates_shuffle
        /// </summary>
        /// <param name="list">要被洗牌的列表</param>
        /// <typeparam name="T">列表中元素的类型</typeparam>
        /// <returns>洗牌后的列表</returns>
        public static IList<T> Shuffle<T>(this IList<T> list)
        {
            if (rng == null) rng = new Random();
            int count = list.Count;
            while (count > 1)
            {
                --count;
                int index = rng.Next(count + 1);
                (list[index], list[count]) = (list[count], list[index]);
            }
            return list;
        }

        /// <summary>
        /// 根据谓词过滤集合并返回包含符合指定条件的元素的新列表
        /// </summary>
        /// <param name="source">要过滤的集合</param>
        /// <param name="predicate">测试每个元素的条件</param>
        /// <returns>包含满足谓词的元素的新列表</returns>
        public static IList<T> Filter<T>(this IList<T> source, Predicate<T> predicate)
        {
            List<T> list = new List<T>();
            foreach (T item in source)
            {
                if (predicate(item))
                {
                    list.Add(item);
                }
            }
            return list;
        }
    }
}