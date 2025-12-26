
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
namespace IACGGames.UISystem
{
    public class GameHUD : UIPanelBase
    {

        [SerializeField] private GameObject topBarGO;
        [SerializeField] Button submitButton;
        RectTransform submitButtonRT;
        [SerializeField] TextMeshProUGUI submitButtonText;
        [SerializeField] Button impossibleButton;
        [SerializeField] TextMeshProUGUI levelText;
        [SerializeField] TextMeshProUGUI trialText;
        [SerializeField] Transform addedLevelPopup;
        [SerializeField] TextMeshProUGUI addedLevelText;
        [SerializeField] Image feedbackImage;
        Transform feedbackTransform;
        [SerializeField] Sprite[] feedbackSprite;
        Sequence addedLevelSeq;
        Sequence feedbackSeq;

        [SerializeField] GameObject trialIsPossibleUI;
        [SerializeField] GameObject correctAnswerUI;
        [SerializeField] TextMeshProUGUI correctAnswerText;

        protected override void Awake()
        {
            base.Awake();
            feedbackTransform = feedbackImage.transform;
            submitButtonRT = submitButton.GetComponent<RectTransform>();
        }
        protected override void OnEnable()
        {
            base.OnEnable();
         
            submitButton.onClick.AddListener(SubmitClick);
            impossibleButton.onClick.AddListener(ImpossibleClick);
           
 
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            submitButton.onClick.RemoveAllListeners();
            impossibleButton.onClick.RemoveAllListeners();
          
        }

        public override void Show(float animTime = 0)
        {
            base.Show(animTime);

            ShowTopBar(true);
            ResetFeedbacks();
            ResetAddedLevel();
            GameManager.Instance.LevelManager.OnItemPlacedInTarget += SubmitButtonInteractivity;
            GameManager.Instance.LevelManager.OnShowTrial += ResetButtons;
            GameManager.Instance.LevelManager.OnFailTrial += OnFailTrial;
            GameManager.Instance.LevelManager.OnSubmit += ShowFeedbacks;

            if (GameManager.Instance.LevelManager.progress != null)
            {
                
                GameManager.Instance.LevelManager.progress.OnLevelChanged += UpdateLevelText;
            }
            trialIsPossibleUI.SetActive(false);
            correctAnswerUI.SetActive(false);
          
        }
        public override void Hide(float animTime = 0)
        {
            base.Hide(animTime);

            GameManager.Instance.LevelManager.OnItemPlacedInTarget -= SubmitButtonInteractivity;
            GameManager.Instance.LevelManager.OnShowTrial -= ResetButtons;
            GameManager.Instance.LevelManager.OnFailTrial -= OnFailTrial;
            GameManager.Instance.LevelManager.OnSubmit -= ShowFeedbacks;
            if (GameManager.Instance.LevelManager.progress != null)
            {
   
                GameManager.Instance.LevelManager.progress.OnLevelChanged -= UpdateLevelText;
            }
        }


       
        public void ShowTopBar(bool show)
        {
            topBarGO.gameObject.SetActive(show);
           
        }
        
        public void UpdateLevelText(int level,int addedlevel)
        {
            levelText.text = level.ToString();
     
            if (addedlevel > 0)
            {
                ResetAddedLevel();
                addedLevelText.text = $"+{addedlevel}";
                addedLevelPopup.gameObject.SetActive(true);
                addedLevelSeq = DOTween.Sequence();
                addedLevelSeq.Append(addedLevelPopup.DOScale(1f, 0.4f).SetEase(Ease.OutBack));
                addedLevelSeq.AppendInterval(0.5f);
                addedLevelSeq.Append(addedLevelPopup.DOScale(0f, 0.2f).SetEase(Ease.InBack));
                addedLevelSeq.AppendCallback(()=>addedLevelPopup.gameObject.SetActive(false));
                
            }
        }
        public void ShowTrialText(int trial,int maxTrial) 
        {
            trialText.text = $"{trial} of {maxTrial}";
        }

        public void SubmitButtonInteractivity(bool interactive)
        {
            submitButton.interactable = interactive;
        }
        public void ImpossibleButtonInteractivity(bool interactive)
        {
            impossibleButton.interactable = interactive;
        }
        void OnFailTrial()
        {
            ImpossibleButtonInteractivity(false);
            UpdateSubmitButton(false);
            submitButton.interactable = true;
        }

        void UpdateSubmitButton(bool submit)
        {
            submitButtonText.text = submit?"SUBMIT":"NEXT";
        }
        private void SubmitClick()
        {
            
            SubmitButtonInteractivity(false);
            GameManager.Instance.LevelManager.OnSubmitPressed();
        }
        private void ImpossibleClick()
        {
            ImpossibleButtonInteractivity(false);
            GameManager.Instance.LevelManager.OnImpossiblePressed();
        }
        void ResetAddedLevel()
        {
           if(addedLevelSeq!=null) addedLevelSeq.Kill();
            addedLevelPopup.localScale = Vector3.zero;
            addedLevelPopup.gameObject.SetActive(false);
            addedLevelText.text = "";
        }
        void ResetButtons()
        {
            UpdateSubmitButton(true);
            ImpossibleButtonInteractivity(true);
            SubmitButtonInteractivity(false);
        }

        public void ShowFeedbacks(SubmitResult result)
        {
            ResetFeedbacks();

            //Correct
            feedbackTransform.gameObject.SetActive(true);
            feedbackImage.sprite = feedbackSprite[result.isCorrect ? 0 : 1];
            
            feedbackSeq = DOTween.Sequence();
            feedbackSeq.Append(feedbackTransform.DOScale(1f, 0.4f).SetEase(Ease.OutBack));
            feedbackSeq.AppendInterval(0.5f);
            feedbackSeq.Append(feedbackTransform.DOScale(0f, 0.2f).SetEase(Ease.InBack));
            feedbackSeq.AppendCallback(() => feedbackTransform.gameObject.SetActive(false));

           

            if (result.isCorrect)
            {
                ImpossibleButtonInteractivity(false);
                SubmitButtonInteractivity(false);
                return;
            }

            if (result.clickedSubmit)
            {
                if (result.finalAttempt)
                {
                    ShowCorrectAnswerUI(result.isImpossible);
                }
            }
            else
            {
                if (!result.isImpossible)
                {
                    ShowTrialIsPossible();
                }
            }
        }
        private void ResetFeedbacks()
        {
            if(feedbackSeq !=null) feedbackSeq.Kill();
            feedbackTransform.localScale = Vector3.zero;
            feedbackTransform.gameObject.SetActive(false);
        }
        public void ShowTrialIsPossible()
        {
            trialIsPossibleUI.SetActive(true);
            CanvasGroup canvasG = trialIsPossibleUI.GetComponent<CanvasGroup>();
            canvasG.alpha = 1f;
            DOVirtual.DelayedCall(1f, () =>
            {
                canvasG.DOFade(0, 1f).OnComplete(() => trialIsPossibleUI.SetActive(false));
            });
      
        }

        public void ShowCorrectAnswerUI(bool impossible)
        {
            correctAnswerUI.SetActive(true);
            correctAnswerText.text = impossible ? "Tial is Impossible." : "Showing correct answer.";
            CanvasGroup canvasG = correctAnswerUI.GetComponent<CanvasGroup>();
            canvasG.alpha = 1f;
            DOVirtual.DelayedCall(1f, () =>
            {
                canvasG.DOFade(0, 1f).OnComplete(() => correctAnswerUI.SetActive(false));
            });
        }

        public void SetupHUD(bool tutorial)
        {
            submitButton.gameObject.SetActive(true);
            impossibleButton.gameObject.SetActive(!tutorial);
            topBarGO.gameObject.SetActive(!tutorial);   
            submitButtonRT.anchoredPosition = new Vector2(tutorial?0:220, submitButtonRT.anchoredPosition.y);

        }
    }
}
