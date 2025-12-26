using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
namespace IACGGames
{
    public class MeterItemUI : MonoBehaviour
    {
        [Header("References")]
        public GameObject fillObject;

        private Image fillImage;
        private RectTransform fillRT;
        private Vector2 originalPos;

        public bool IsFilled { get; private set; } = false;

        private void Awake()
        {
            fillImage = fillObject.GetComponent<Image>();
            fillRT = fillImage.rectTransform;
            originalPos = fillRT.anchoredPosition;
            IsFilled = false;
            ResetStateInstant();
        }

        public void SetFilled()
        {
            if (IsFilled == true) return;
            IsFilled = true;
            FullResetBeforeAnimation();
            PlayFillPop();

        }

        public void FullResetBeforeAnimation()
        {
            if (dropSeq != null)
            {
                dropSeq.Complete();
                dropSeq.Kill();
            }

            fillRT.DOKill(true);   
            fillImage.DOKill(true);
            fillRT.anchoredPosition = originalPos;
            fillRT.localScale = Vector3.one;
        }


        public void ResetStateInstant()
        {
            fillRT.DOKill(true);
            fillImage.DOKill(true);
            fillRT.anchoredPosition = originalPos;
            fillRT.localScale = Vector3.zero;
            fillImage.color = new Color(1, 1, 1, 1);
            IsFilled = false;
            fillObject.SetActive(false);

        }


        private void PlayFillPop()
        {
            fillObject.SetActive(true);

            fillRT.localScale = Vector3.zero;
            fillImage.color = new Color(1, 1, 1, 1);

            fillRT.DOScale(1f, 0.25f)
                .SetEase(Ease.OutBack);
        }

        Sequence dropSeq;
        public void PlayRemoveCascadeTween()
        {
            fillObject.SetActive(true);

            IsFilled = false;
            dropSeq = DOTween.Sequence();

            dropSeq.Join(fillImage.DOFade(0f, 0.20f));
            dropSeq.Join(fillRT.DOAnchorPos(originalPos + new Vector2(0, -100f), 0.20f));

            dropSeq.OnComplete(() =>
            {
                ResetStateInstant();
            });
        }
    }
}