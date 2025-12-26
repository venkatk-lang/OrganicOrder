
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
namespace IACGGames.UISystem
{
    public class LevelCompletePanel : UIPanelBase
    {

        [SerializeField] Button continueButton;
   
        [SerializeField] TextMeshProUGUI levelText;
        [SerializeField] TextMeshProUGUI trialText;
        [SerializeField] TextMeshProUGUI scoreText;


        protected override void OnEnable()
        {
            base.OnEnable();
            continueButton.onClick.AddListener(ContinueClick);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            continueButton.onClick.RemoveAllListeners();
          
        }

        public override void Show(float animTime = 0)
        {
            base.Show(animTime);
        }
        public override void Hide(float animTime = 0)
        {
            base.Hide(animTime);
        }

        public void UpdateLevelText(int level,int trial,int maxTrial,int score)
        {
            levelText.text = level.ToString();
            scoreText.text = score.ToString();
            trialText.text = $"{trial} of {maxTrial}".ToString();
        }
      
        private void ContinueClick()
        {
            GameManager.Instance.StartGame();
        }
        
    }
}
