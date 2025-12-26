using System;

[Serializable]
public class PlayerData
{
    public string playerId;
    public string playerName;
    public int coins;
    public int highScore;

    public PlayerData(string id, string name, int coins = 0, int score = 0)
    {
        this.playerId = id;
        this.playerName = name;
        this.coins = coins;
        this.highScore = score;
    }
}