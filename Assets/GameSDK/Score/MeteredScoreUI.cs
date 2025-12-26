using UnityEngine;
using TMPro;
using DG.Tweening;
namespace IACGGames
{
    public class MeteredScoreUI : MonoBehaviour
    {
        public MeterItemUI meterItemPrefab;
        public Transform meterContainer;

        public TMP_Text scoreText;
        public TMP_Text multiplierText;

        private MeteredScore score;
        private MeterItemUI[] items;

        private int previousMeterValue = 0;
        private int previousMultiplier = 1;
        public void Initialize(MeteredScore score)
        {
            this.score = score;

            GenerateIcons();

            score.OnMeterChanged += UpdateMeter;
            score.OnTotalScoreChanged += UpdateScore;
            score.OnMultiplierChanged += UpdateMultiplier;
            previousMultiplier = score.Multiplier;
            UpdateMeter(score.Meter, score.Multiplier, true);
            UpdateScore(score.TotalScore);
            UpdateMultiplier(score.Multiplier);
        }


        private void GenerateIcons()
        {
            foreach (Transform t in meterContainer)
                Destroy(t.gameObject);

            items = new MeterItemUI[score.MeterTarget];

            for (int i = 0; i < score.MeterTarget; i++)
            {
                items[i] = Instantiate(meterItemPrefab, meterContainer);

            }
        }

        public void UpdateMeter(int meterValue, int currentMultiplier, bool correct)
        {
            bool multiplierIncreased = currentMultiplier > previousMultiplier;
            bool meterDecreased = meterValue < previousMeterValue;
            if (multiplierIncreased)
            {
                InstantResetAllItems();
                previousMeterValue = meterValue;
                previousMultiplier = currentMultiplier;
                return;
            }

            if (meterDecreased && !correct)
            {
                PlayCascadeReset(previousMeterValue, meterValue);
                previousMeterValue = meterValue;
                previousMultiplier = currentMultiplier;
                return;
            }

            FillUpTo(meterValue);

            previousMeterValue = meterValue;
            previousMultiplier = currentMultiplier;
        }
        private void InstantResetAllItems()
        {
            for (int i = 0; i < items.Length; i++)
                items[i].ResetStateInstant();
        }
        private void FillUpTo(int meterValue)
        {
            if (wrongAnimation != null)
            {
                wrongAnimation.Kill();
            }
            for (int i = 0; i < items.Length; i++)
            {
                bool shouldFill = (i < meterValue);
                if (shouldFill)
                {
                    items[i].gameObject.SetActive(true);
                    items[i].SetFilled();
                }

            }
        }
   
        Tween wrongAnimation;
        private void PlayCascadeReset(int lastValue, int targetValue)
        {
            if (wrongAnimation != null)
            {
                wrongAnimation.Kill();
            }
            float delay = 0f;
            float delayStep = 0.08f;
            for (int i = lastValue - 1; i >= targetValue; i--)
            {
                MeterItemUI item = items[i];

                wrongAnimation = DOVirtual.DelayedCall(delay, () =>
                {
                    item.PlayRemoveCascadeTween();
                });

                delay += delayStep;
     
            }
        }
        private void UpdateScore(int val)
        {
            if (scoreText != null)
                scoreText.text = val.ToString();
        }

        private void UpdateMultiplier(int multiplier)
        {
            if (multiplierText != null)
            {
                multiplierText.text = "x" + multiplier;


                multiplierText.transform.DOKill(true);


                multiplierText.transform.localScale = Vector3.one;

                Sequence s = DOTween.Sequence();
                s.Append(multiplierText.transform.DOScale(1.4f, 0.15f).SetEase(Ease.OutBack));
                s.Append(multiplierText.transform.DOScale(1f, 0.12f));
            }
        }
    }
}