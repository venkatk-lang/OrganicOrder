using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class CountdownUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private Transform bg;
    [SerializeField] private float scaleUpDuration = 0.3f;
    [SerializeField] private float scaleDownDuration = 0.25f;
    [SerializeField] private Ease scaleEase = Ease.OutBack;
    [SerializeField] private Ease scaleDownEase = Ease.InBack;

    private Tween scaleTween;
    private CountdownTimer boundTimer;

    public void Bind(CountdownTimer timer)
    {
        boundTimer = timer;
        timer.OnTick += OnTick;

        bg.transform.localScale = Vector3.zero;
        gameObject.SetActive(true);
    }

    public void Unbind()
    {
        if (boundTimer != null)
            boundTimer.OnTick -= OnTick;

        boundTimer = null;
    }

    private void OnTick(int value)
    {
        scaleTween?.Kill();

       

        if (value > 0)
        {
            bg.transform.localScale = Vector3.zero;
            text.text = value.ToString();

            scaleTween = bg.transform
                .DOScale(1f, scaleUpDuration)
                .SetEase(scaleEase);
        }
        else
        {
            text.text = "GO!";
            scaleTween = bg.transform
                .DOScale(0f, scaleDownDuration)
                .SetEase(scaleDownEase)
                .OnComplete(() =>
                {
                    Unbind();
                    Destroy(gameObject);
                });
        }
    }


    private void OnDestroy()
    {
        scaleTween?.Kill();
        Unbind();
    }
}
