using UnityEngine;
[CreateAssetMenu(menuName = "ToyHunt/GameConfig")]
public class GameConfig : ScriptableObject
{
    [Tooltip("Train Speed")]
    [SerializeField]private int roundBonusScoreMultiplier = 100;
    public int RoundBonusScoreMultiplier => roundBonusScoreMultiplier; 
    [Header("Score Settings")]
    [SerializeField] private int scoreEachCorrect = 500;
    public int ScoreEachCorrect => scoreEachCorrect;

    [Header("Debug")]
    [SerializeField] private bool showUsedItem = false;
    public bool ShowUsedItem => showUsedItem;


}
