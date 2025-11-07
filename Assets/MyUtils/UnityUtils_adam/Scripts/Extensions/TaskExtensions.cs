using System;
using System.Collections;
using System.Threading.Tasks;

namespace UnityUtils
{
    public static class TaskExtensions
    {
        /// <summary>
        /// 将提供的对象包装到已完成的Task中
        /// </summary>
        /// <param name="obj">要包装在Task中的对象</param>
        /// <typeparam name="T">对象的类型</typeparam>
        /// <returns>包含对象的已完成Task</returns>
        public static Task<T> AsCompletedTask<T>(this T obj) => Task.FromResult(obj);

        /// <summary>
        /// 将Task转换为IEnumerator，用于Unity协程
        /// </summary>
        /// <param name="task">要转换的Task</param>
        /// <returns>Task的IEnumerator表示</returns>
        public static IEnumerator AsCoroutine(this Task task)
        {
            while (!task.IsCompleted) yield return null;
            // 当用于失败的Task时，GetResult()将传播原始异常
            // 参见：https://devblogs.microsoft.com/pfxteam/task-exception-handling-in-net-4-5/
            task.GetAwaiter().GetResult();
        }

        /// <summary>
        /// 标记一个任务为"忘记"，意味着任务抛出的任何异常将被捕获和处理
        /// </summary>
        /// <param name="task">要被忘记的任务</param>
        /// <param name="onException">捕获异常时要执行的可选操作。如果提供，异常将不会重新抛出</param>
        public static async void Forget(this Task task, Action<Exception> onException = null)
        {
            try
            {
                await task;
            }
            catch (Exception exception)
            {
                if (onException == null)
                    throw exception;

                onException(exception);
            }
        }
    }
}