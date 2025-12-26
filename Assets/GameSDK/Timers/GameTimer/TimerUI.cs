using TMPro;
using UnityEngine;

public class TimerUI : MonoBehaviour
{
    [SerializeField] private TMP_Text timeText;

    private GameTimer boundTimer;

    public void Bind(GameTimer timer)
    {
        boundTimer = timer;
        timer.OnTimeChanged += UpdateUI;
        UpdateUI(0, 0);
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
