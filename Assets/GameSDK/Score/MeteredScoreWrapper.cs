using UnityEngine;
namespace IACGGames
{
    public class MeteredScoreWrapper : MonoBehaviour
    {
        public MeteredScoreUI scoreUI;
        private MeteredScore scoreSystem;

        public void Initialize(int basePoints = 50, int meterTarget = 4)
        {
            scoreSystem = new MeteredScore(basePoints, meterTarget);
            scoreUI.Initialize(scoreSystem);
        }

        public MeteredScore Score => scoreSystem;
    }
}