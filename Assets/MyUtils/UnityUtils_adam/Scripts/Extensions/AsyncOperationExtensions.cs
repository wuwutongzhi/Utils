using System.Threading.Tasks;
using UnityEngine;

namespace UnityUtils
{
    public static class AsyncOperationExtensions
    {
        /// <summary>
        /// 将AsyncOperation转换为Task的扩展方法 //类似SceneManager.LoadSceneAsync().AsTask()
        /// </summary>
        /// <param name="asyncOperation">要转换的AsyncOperation</param>
        /// <returns>表示AsyncOperation完成状态的Task</returns>
        public static Task AsTask(this AsyncOperation asyncOperation)
        {
            var tcs = new TaskCompletionSource<bool>();
            asyncOperation.completed += _ => tcs.SetResult(true);
            return tcs.Task;
        }
    }
}