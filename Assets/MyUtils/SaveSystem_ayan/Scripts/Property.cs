using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
namespace SaveSystem_ayan
{
    public class Property : MonoBehaviour
    {
        string Name => playerName;
        int Level => level;
        float Height => height;
        Vector3 Position => position;

        [SerializeField] string playerName = "playername";
        [SerializeField] int level;
        [SerializeField] float height;
        [SerializeField] Vector3 position;
        [SerializeField] Text nameText;
        [SerializeField] Text levelText;
        [SerializeField] Text heightText;
        [SerializeField] Text positionTextX;
        [SerializeField] Text positionTextY;
        [SerializeField] Text positionTextZ;

        private const string PLAY_DATA_KEY = "PlayerData";
        const string PLAYER_DATA_FILE_NAME = "PlayerData.sav";

        //[SerializeField] SaveData saveData;
        [Serializable]
        class SaveData
        {
            public string playerName;
            public int playerLevel;
            public float playerHeight;
            public Vector3 playerPosition;
        }
        private void Awake()
        {
        }
        private void Update()
        {
            nameText.text = playerName;
            levelText.text = level.ToString();
            heightText.text = height.ToString();
            positionTextX.text = position.x.ToString();
            positionTextY.text = position.y.ToString();
            positionTextZ.text = position.z.ToString();

        }
        #region Save And Load
        public void Save()
        {
            //SaveByPlayerPrefs();
            SaveByJson();
        }
        public void Load()
        {
            //LoadFromPlayerPrefs();
            LoadFromJson();
        }
        #endregion

        #region PlayerPrefs 
        //数据安全性低
        //拓展方法，创建一个可序列化类，拥有本类的各种变量。声明一个不继承任何类的SaveSystem,将object比如刚创建的可序列化类用jsonUtility转换为字符串类型存储和读取。
        void SaveByPlayerPrefs()
        {
            //PlayerPrefs.SetString("Name", playerName);
            //PlayerPrefs.SetInt("Level", level);
            //PlayerPrefs.SetFloat("Height", height);
            //PlayerPrefs.SetFloat("PositionX", position.x);
            //PlayerPrefs.SetFloat("PositionY", position.y);
            //PlayerPrefs.SetFloat("PositionZ", position.z);
            //PlayerPrefs.Save();

            SaveSystem.SaveByPlayerPrefs(PLAY_DATA_KEY, SavingData());
        }



        void LoadFromPlayerPrefs()
        {
            //playerName = PlayerPrefs.GetString("Name","Somebody");//参数2为默认值
            //level = PlayerPrefs.GetInt("Level");
            //height = PlayerPrefs.GetFloat("Height");
            //position.x = PlayerPrefs.GetFloat("PositionX");
            //position.y = PlayerPrefs.GetFloat("PositionY");
            //position.z = PlayerPrefs.GetFloat("PositionZ");
            var json = SaveSystem.LoadFromPlayerPrefs(PLAY_DATA_KEY);
            var saveData = JsonUtility.FromJson<SaveData>(json);
            LoadData(saveData);
        }




        #endregion

        #region JSON
        void SaveByJson()
        {
            //SaveSystem.SaveByJson(PLAYER_DATA_FILE_NAME,SavingData());
            SaveSystem.SaveByJson($"{System.DateTime.Now:yyyy.dd.M HH-mm-ss}.sav", SavingData());
            //
        }
        void LoadFromJson()
        {
            var saveData = SaveSystem.LoadFromJson<SaveData>(PLAYER_DATA_FILE_NAME);

            LoadData(saveData);
        }
        #endregion

        #region Help Function
        private SaveData SavingData()
        {
            var saveData = new SaveData();
            saveData.playerName = playerName;
            saveData.playerLevel = level;
            saveData.playerHeight = height;
            saveData.playerPosition = position;
            return saveData;
        }
        private void LoadData(SaveData saveData)
        {
            playerName = saveData.playerName;
            level = saveData.playerLevel;
            height = saveData.playerHeight;
            position = saveData.playerPosition;
        }
        //[MenuItem("Developer/Delete Player Data Prefs")]
        public static void DeletePlayerDataPrefs()
        {
            //PlayerPrefs.DeleteAll();
            //PlayerPrefs.DeleteKey(PLAY_DATA_KEY);
            PlayerPrefs.DeleteKey(PLAY_DATA_KEY);
        }
        //[MenuItem("Developer/Delete Player Data Save Files")]
        public static void DeletePlayerDataSaveFile()
        {
            SaveSystem.DeleteSaveFile(PLAYER_DATA_FILE_NAME);
        }
        #endregion
    }
}