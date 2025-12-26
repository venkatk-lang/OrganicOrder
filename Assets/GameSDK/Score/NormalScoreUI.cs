using UnityEngine;
using TMPro;
namespace IACGGames
{
    public class NormalScoreUI : MonoBehaviour
    {

        public TextMeshProUGUI scoreText;

        private NormalScore score;

        public void Initialize(NormalScore score)
        {
            if (this.score != null)
                this.score.OnScoreChanged -= UpdateUI;

            this.score = score;

            if (this.score != null)
            {
                this.score.OnScoreChanged += UpdateUI;
                UpdateUI(this.score.Score);
            }

        }

        private void OnDestroy()
        {
            if (score != null)
                score.OnScoreChanged -= UpdateUI;
        }

        private void UpdateUI(int value)
        {

            if (scoreText != null)
                scoreText.text = value.ToString();
        }
    }
}