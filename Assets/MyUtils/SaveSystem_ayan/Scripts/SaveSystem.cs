using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;
namespace SaveSystem_ayan
{
    public static class SaveSystem
    {
        #region PlayerPrefs
        public static void SaveByPlayerPrefs(string key, object data)
        {
            var json = JsonUtility.ToJson(data);
            PlayerPrefs.SetString(key, json);
            PlayerPrefs.Save();
        }
        public static string LoadFromPlayerPrefs(string key)
        {
            return PlayerPrefs.GetString(key, null);
        }
        #endregion

        #region JSON
        public static void SaveByJson(string saveFileName, object data)
        {
            var json = JsonUtility.ToJson(data);
            var path = Path.Combine(Application.persistentDataPath, saveFileName);

            try
            {
                File.WriteAllText(path, json);
#if UNITY_EDITOR
                Debug.Log($"Susscessfully saved data to {path}.");
#endif
            }
            catch (System.Exception exception)
            {
#if UNITY_EDITOR
                Debug.LogError($"Failed to save data to {path}.\n{exception}");
#endif
                //什么都不写会，尝试捕获任何异常，不建议这么做
            }
            //catch(FileNotFoundException e)
            //{
            //    //HandleException
            //}
            //catch(DirectoryNotFoundException e)
            //{

            //}
        }
        public static T LoadFromJson<T>(string saveFileName)
        {
            var path = Path.Combine(Application.persistentDataPath, saveFileName);
            try
            {
                var json = File.ReadAllText(path);
                var data = JsonUtility.FromJson<T>(json);
                return data;
            }
            catch (System.Exception exception)
            {
#if UNITY_EDITOR
                Debug.LogError($"Failed to load data from {path}.\n{exception}");
                return default;
#endif
            }


        }
        #endregion

        #region Deleting
        public static void DeleteSaveFile(string saveFileName)
        {
            var path = Path.Combine(Application.persistentDataPath, saveFileName);
            try
            {
                File.Delete(path);
            }
            catch (System.Exception exception)
            {
#if UNITY_EDITOR
                Debug.LogError($"Failed to delete{path}.\n{exception}");

#endif
            }

        }
        #endregion
    }
}