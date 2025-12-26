using UnityEngine;
namespace IACGGames
{
    public class ScoreDemo : MonoBehaviour
    {
        [SerializeField] NormalScoreWrapper normalScoreWrapper;
        [SerializeField] MeteredScoreWrapper meteredScoreWrapper;

        private void Start()
        {
            normalScoreWrapper.Initialize();
            meteredScoreWrapper.Initialize();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                if (normalScoreWrapper != null)
                {
                    normalScoreWrapper.Score.Add(10);
                }
                if (meteredScoreWrapper != null)
                {
                    meteredScoreWrapper.Score.Correct();
                }
            }

            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                if (normalScoreWrapper != null)
                {
                    normalScoreWrapper.Score.Add(-10);
                }
                if (meteredScoreWrapper != null)
                {
                    meteredScoreWrapper.Score.Wrong();
                }
            }
        }
    }
}