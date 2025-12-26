using UnityEngine;
namespace IACGGames {
    [System.Serializable]
    public class SaveData
    {
        [Header("Settings Save Data")]
        public bool vibrationOn;
        public float bgSoundValue;
        public float inGameSoundFXValue;

        [Header("Level Save Data")]
  
        public bool tutorialCompleted;
        public int level;
        public SaveData(GameConfig gameConfig)
        {
            tutorialCompleted = false;
            vibrationOn = true;
            bgSoundValue = 1f;
            inGameSoundFXValue = 1f;
            level = 1;


        }
       
    }

}
