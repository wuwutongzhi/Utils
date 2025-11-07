using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UIElements;

namespace UnityUtils
{
    public static class UQueryBuilderExtensions
    {
        /// <summary>
        /// 根据键对序列中的元素进行升序排序，并返回有序序列
        /// </summary>
        /// <param name="query">要排序的元素</param>
        /// <param name="keySelector">从元素中提取排序键的函数</param>
        /// <param name="default">用于比较键的比较器</param>
        public static IEnumerable<T> OrderBy<T, TKey>(this UQueryBuilder<T> query, Func<T, TKey> keySelector,
            Comparer<TKey> @default)
            where T : VisualElement
        {
            return query.ToList().OrderBy(keySelector, @default);
        }

        /// <summary>
        /// 根据数值键对序列中的元素进行升序排序，并返回有序序列
        /// </summary>
        /// <param name="query">要排序的元素</param>
        /// <param name="keySelector">从元素中提取数值键的函数</param>
        public static IEnumerable<T> SortByNumericValue<T>(this UQueryBuilder<T> query, Func<T, float> keySelector)
            where T : VisualElement
        {
            return query.OrderBy(keySelector, Comparer<float>.Default);
        }

        /// <summary>
        /// 返回序列中的第一个元素，如果找不到元素则返回默认值
        /// </summary>
        /// <param name="query">要搜索的元素</param>
        public static T FirstOrDefault<T>(this UQueryBuilder<T> query)
            where T : VisualElement
        {
            return query.ToList().FirstOrDefault();
        }

        /// <summary>
        /// 计算序列中满足谓词函数指定条件的元素数量
        /// </summary>
        /// <param name="query">要处理的元素序列</param>
        /// <param name="predicate">测试每个元素条件的函数</param>
        public static int CountWhere<T>(this UQueryBuilder<T> query, Func<T, bool> predicate)
            where T : VisualElement
        {
            return query.ToList().Count(predicate);
        }
    }
}