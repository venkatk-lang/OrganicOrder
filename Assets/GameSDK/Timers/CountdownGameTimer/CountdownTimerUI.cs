using TMPro;
using UnityEngine;

public class CountdownTimerUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI timeText;

    private CountdownTimerMMSS boundTimer;

    public void Bind(CountdownTimerMMSS timer)
    {
        boundTimer = timer;
        timer.OnTimeChanged += UpdateUI;
    }

    public void Unbind()
    {
        if (boundTimer != null)
            boundTimer.OnTimeChanged -= UpdateUI;

        boundTimer = null;
    }

    private void UpdateUI(int minutes, int seconds)
    {
        timeText.text = $"{minutes:00}:{seconds:00}";
    }

    private void OnDestroy()
    {
        Unbind();
    }
}
