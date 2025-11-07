using System;

namespace UnityUtils {
    public static class DateTimeExtensions {
        /// <summary>
        /// 感谢Adam重构此代码
        /// 返回具有指定年、月、日的新 DateTime 对象。
        /// 如果任何参数为 null，则使用原始 DateTime 中的对应值。
        /// </summary>
        /// <param name="dt">原始的 DateTime 对象</param>
        /// <param name="year">新的年份值。如果为 null，则使用原始年份</param>
        /// <param name="month">新的月份值。如果为 null，则使用原始月份</param>
        /// <param name="day">新的日期值。如果为 null，则使用原始日期</param>
        /// <returns>具有指定年、月、日的新 DateTime 对象</returns>
        public static DateTime WithDate(this DateTime dt, int? year = null, int? month = null, int? day = null) {
            int newYear = year ?? dt.Year;
            int newMonth = month ?? dt.Month;
            int newDay = day ?? dt.Day;

            // 通过必要时限制日期来确保新日期有效
            int daysInMonth = DateTime.DaysInMonth(newYear, newMonth);
            newDay = Math.Min(newDay, daysInMonth);

            return new DateTime(newYear, newMonth, newDay, dt.Hour, dt.Minute, dt.Second, dt.Millisecond);
        }
    }
}