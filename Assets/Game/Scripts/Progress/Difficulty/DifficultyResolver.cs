using UnityEngine;
using System.Collections.Generic;

public static class DifficultyResolver
{
    static readonly List<LevelDifficultyConfig> configs =
        new List<LevelDifficultyConfig>
        {
            new LevelDifficultyConfig
            {
                minLevel = 1,
                maxLevel = 2,
                positions = 2,
                ruleRange = new Vector2Int(1, 1)
            },
            new LevelDifficultyConfig
            {
                minLevel = 3,
                maxLevel = 30,
                positions = 3,
                ruleRange = new Vector2Int(1, 2)
            },
            new LevelDifficultyConfig
            {
                minLevel = 31,
                maxLevel = 130,
                positions = 4,
                ruleRange = new Vector2Int(2, 3)
            },
            new LevelDifficultyConfig
            {
                minLevel = 131,
                maxLevel = 200,
                positions = 5,
                ruleRange = new Vector2Int(3, 4)
            },
            new LevelDifficultyConfig
            {
                minLevel = 201,
                maxLevel = int.MaxValue,
                positions = 6,
                ruleRange = new Vector2Int(4, 5)
            }
        };

    public static int GetPositions(int level)
    {
        return GetConfig(level).positions;
    }

    public static int GetRuleCount(int level)
    {
        var cfg = GetConfig(level);
        return Random.Range(cfg.ruleRange.x, cfg.ruleRange.y + 1);
    }

    static LevelDifficultyConfig GetConfig(int level)
    {
        foreach (var cfg in configs)
        {
            if (cfg.Matches(level))
                return cfg;
        }

        // fallback (should never happen)
        return configs[0];
    }
}
public struct LevelDifficultyConfig
{
    public int minLevel;
    public int maxLevel; // use int.MaxValue for open-ended
    public int positions;
    public Vector2Int ruleRange;

    public bool Matches(int level)
    {
        return level >= minLevel && level <= maxLevel;
    }
}