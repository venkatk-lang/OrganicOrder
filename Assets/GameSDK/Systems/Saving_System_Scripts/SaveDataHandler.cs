using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
namespace IACGGames
{
    public class SaveDataHandler : Singleton<SaveDataHandler>
    {
        [SerializeField] private GameConfig gameConfig;
        [SerializeField] private SaveData saveData;

        //private bool newUser;

        public SaveData SaveData
        {
            get => saveData;
            set => saveData = value;
        }
        public GameConfig GameConfig => gameConfig;

        #region SetUp Data
        public void InitializeSavingSystem()
        {
            IncreamentSessionCount();
            //newUser = false;
            if (File.Exists(SaveFileExtension.saveDataPath))
            {
                ReadDataFromSaveFiles(SaveDataFiles.SaveData);
                LoadData();
            }
            else
            {
                InitializeDataForFirstTime();
                WriteDataToSaveFile(SaveDataFiles.SaveData);
                //newUser = true;
            }
        }
        private void InitializeDataForFirstTime()
        {
            SaveData = new SaveData(gameConfig);
        }
        #endregion
        #region Write Data
        /// <summary>
        /// Write Data To Save Files
        /// </summary>
        private void WriteSaveData()
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(SaveFileExtension.saveDataPath, FileMode.Create);
            stream.Position = 0;
            formatter.Serialize(stream, SaveData);
            stream.Close();
        }

        public void WriteDataToSaveFile(SaveDataFiles saveDataFiles)
        {
            switch (saveDataFiles)
            {
                case SaveDataFiles.SaveData:
                    WriteSaveData();
                    break;
                default:
                    break;
            }
        }
        #endregion

        #region Read Data
        /// <summary>
        /// Reading save Files
        /// </summary>
        private void ReadSaveData()
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(SaveFileExtension.saveDataPath, FileMode.Open);
            stream.Position = 0;
            SaveData = formatter.Deserialize(stream) as SaveData;
            stream.Close();
        }

        public void ReadDataFromSaveFiles(SaveDataFiles saveDataFiles)
        {
            switch (saveDataFiles)
            {
                case SaveDataFiles.SaveData:
                    ReadSaveData();
                    break;
                default:
                    break;
            }
        }
        #endregion

        #region Load Data
        /// <summary>
        /// Loading Save Files
        /// </summary>
        private void LoadData()
        {

        }
        #endregion


        #region Save Properties
        public bool VibrationOn
        {
            get
            {
                return saveData.vibrationOn;
            }
            set
            {
                saveData.vibrationOn = value;
            }
        }
        public float BgSoundValue
        {
            get
            {
                return saveData.bgSoundValue;
            }
            set
            {
                saveData.bgSoundValue = value;
            }
        }
        public float InGameSoundFXValue
        {
            get
            {
                return saveData.inGameSoundFXValue;
            }
            set
            {
                saveData.inGameSoundFXValue = value;
            }
        }

        #endregion

        public int GetSessionCount => PlayerPrefs.GetInt(GameConstants.SESSIONCOUNT, 0);
        private void IncreamentSessionCount()
        {
            int count = GetSessionCount;
            count += 1;
            PlayerPrefs.SetInt(GameConstants.SESSIONCOUNT, count);
        }

        public bool GameSceneLoaded;// in loading page
#if UNITY_IOS || UNITY_EDITOR || UNITY_ANDROID
        private void OnApplicationFocus(bool focus)
        {
            if (!focus)
            {
                if (GameSceneLoaded)
                    WriteDataToSaveFile(SaveDataFiles.SaveData);
            }
        }
#endif

#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
        private void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus)
            {
                if (GameSceneLoaded)
                    WriteDataToSaveFile(SaveDataFiles.SaveData);
            }
            else
            {
            }
        }
#endif

    }

    public static class SaveFileExtension
    {
        public static string saveDataPath = Application.persistentDataPath + "/IACGSaveData.IACGsave";
    }

    public enum SaveDataFiles
    {
        SaveData
    }
}