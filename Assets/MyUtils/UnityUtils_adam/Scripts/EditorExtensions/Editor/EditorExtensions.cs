using System.IO;
using UnityEditor;
using UnityEngine;

namespace UnityUtils {
    /// <summary>
    /// Provides extension methods for various editor functionalities.
    /// </summary>
    public static class EditorExtensions {
        /// <summary>
        /// 检查指定路径的文件是否存在，如果存在则提示用户确认是否覆盖
        /// </summary>
        /// <param name="path">要检查的文件路径</param>
        /// <returns>如果文件不存在或用户确认覆盖则返回true；否则返回false</returns>
        public static bool ConfirmOverwrite(this string path) {
            if (File.Exists(path)) {
                return EditorUtility.DisplayDialog
                (
                    "File Exists",
                    "The file already exists at the specified path. Do you want to overwrite it?",
                    "Yes",
                    "No"
                );
            }
            return true;
        }

        /// <summary>
        /// 打开文件夹浏览对话框并返回选中的文件夹路径
        /// </summary>
        /// <param name="defaultPath">打开文件夹浏览器的默认路径</param>
        /// <returns>选中的文件夹路径</returns>
        public static string BrowseForFolder(this string defaultPath) {
            return EditorUtility.SaveFolderPanel
            (
                "Choose Save Path",
                defaultPath,
                ""
            );
        }

        /// <summary>
        /// 在Unity编辑器中高亮并选中指定的资源
        /// </summary>
        /// <param name="asset">要高亮并选中的资源</param>
        public static void PingAndSelect(this Object asset) {
            EditorGUIUtility.PingObject(asset);
            Selection.activeObject = asset;
        }
    }
}