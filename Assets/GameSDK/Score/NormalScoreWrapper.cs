using UnityEngine;
namespace IACGGames
{
    public class NormalScoreWrapper : MonoBehaviour
    {
        public NormalScoreUI scoreUI;

        private NormalScore normalScore;

        public void Initialize(int startingScore = 0)
        {
            normalScore = new NormalScore(startingScore);

            // Hook UI to score system
            if (scoreUI != null)
                scoreUI.Initialize(normalScore);
        }

        public NormalScore Score => normalScore;


    }
}
