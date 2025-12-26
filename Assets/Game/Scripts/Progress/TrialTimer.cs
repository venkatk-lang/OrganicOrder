using TMPro;
using UnityEngine;
public class TrialTimer : MonoBehaviour
{
    float elapsed;
    bool running;
    public float ElapsedTime => elapsed;
    [SerializeField]TextMeshProUGUI testText; //only for debug
    public void StartTimer()
    {
        elapsed = 0f;
        running = true;
    }

    public void StopTimer()
    {
        running = false;
    }
    public void PauseTimer()
    {
        running = false;
    }

    public void ResumeTimer()
    {
        running = true;
    }
    void Update()
    {
        if (!running) return;
        elapsed += Time.deltaTime;
        testText.text = elapsed.ToString();
    }

    public float NormalizedTime(float maxTime)
    {
        return Mathf.Clamp01(elapsed / maxTime);
    }
}
