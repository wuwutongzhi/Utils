using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UnityUtils
{
    public static class Helpers
    {
        /// <summary>
        /// 获取一个WaitForSeconds对象，用于协程中的延时等待
        /// </summary>
        /// <param name="seconds">等待的秒数</param>
        /// <returns>WaitForSeconds对象</returns>
        public static WaitForSeconds GetWaitForSeconds(float seconds)
        {
            return WaitFor.Seconds(seconds);
        }

        /// <summary>
        /// 在Unity编辑器中清除控制台日志
        /// </summary>
#if UNITY_EDITOR
        public static void ClearConsole()
        {
            // 通过反射获取UnityEditor内部的LogEntries类型
            var assembly = Assembly.GetAssembly(typeof(SceneView));
            var type = assembly.GetType("UnityEditor.LogEntries");
            var method = type.GetMethod("Clear");

            // 调用Clear方法清空控制台
            method?.Invoke(new object(), null);
        }
#endif        
    }
}