using IACGGames;
using System;
using UnityEngine;

public class PlayerProgress
{
    private int level = 1;
    public int Level => level;
    public const int MinLevel = 1;
    public Action<int,int> OnLevelChanged;
    public PlayerProgress()
    {
        level = SaveDataHandler.Instance.SaveData.level;
    
    }
    public void Initialize()
    {
        OnLevelChanged?.Invoke(level,0);
    }
    public void IncreaseLevel(int levelToIncrease)
    {
        level = level + levelToIncrease;
        OnLevelChanged?.Invoke(level, levelToIncrease);
    

    }

   
    public void DecreaseLevel(int amount)
    {
        level = Mathf.Max(level - amount, MinLevel);
        OnLevelChanged?.Invoke(level, -amount);

       
    }

    public void SaveLevelProgress()
    {
        SaveDataHandler.Instance.SaveData.level = level;
        SaveDataHandler.Instance.WriteDataToSaveFile(SaveDataFiles.SaveData);
    }
}

public static class LevelProgressCalculator
{
   
        public static int CalculateLevelGain(
            int itemCount,
            float timeTaken,
            float maxTime)
        {
        float graceTime = 5f;
            if (itemCount <= 0)
                return 0;
        if (timeTaken <= graceTime)
            return itemCount;
        if (timeTaken >= maxTime)
                return 0;

        float decayTime = maxTime - graceTime;
        if (decayTime <= 0f)
            return 0;

        float timeInDecay = timeTaken - graceTime;

        float timePerSeed = decayTime / itemCount;

        int bucket = Mathf.FloorToInt(timeInDecay / timePerSeed);

        int gain = itemCount - bucket;

        return Mathf.Clamp(gain, 0, itemCount);
    }

    public static int CalculateLevelDecrease(
      float timeTaken,
      float fastThreshold = 2f)
    {
        return timeTaken < fastThreshold ? 1 : 2;
    }
}