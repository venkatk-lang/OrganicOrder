using DG.Tweening;
using System;
using System.Collections;
using UnityEngine;

public class CountdownTimerMMSS
{
    public bool IsRunning { get; private set; }
    public bool IsPaused { get; private set; }

    public float RemainingTime { get; private set; } // seconds

    private readonly MonoBehaviour runner;
    private Coroutine routine;

    public event Action<int, int> OnTimeChanged; // minutes, seconds
    public event Action OnFinished;

    public CountdownTimerMMSS(MonoBehaviour runner)
    {
        this.runner = runner;
    }

    /// <summary>
    /// durationSeconds = total time to count down from
    /// </summary>
    public void Start(float durationSeconds)
    {
        Stop();

        RemainingTime = Mathf.Max(0, durationSeconds);
        IsRunning = true;
        IsPaused = false;

        routine = runner.StartCoroutine(TimerRoutine());
    }

    public void Pause() => IsPaused = true;
    public void Resume() => IsPaused = false;

    public void Stop()
    {
        if (routine != null)
            runner.StopCoroutine(routine);

        routine = null;
        IsRunning = false;
        IsPaused = false;
    }

    private IEnumerator TimerRoutine()
    {
        int lastSecond = Mathf.CeilToInt(RemainingTime);

        // Initial push
        EmitTime(lastSecond);

        while (RemainingTime > 0f)
        {
            if (!IsPaused)
            {
                RemainingTime -= Time.deltaTime;
                RemainingTime = Mathf.Max(0f, RemainingTime);

                int currentSecond = Mathf.CeilToInt(RemainingTime);
                if (currentSecond != lastSecond)
                {
                    lastSecond = currentSecond;
                    EmitTime(currentSecond);
                }
            }

            yield return null;
        }

        IsRunning = false;
        OnFinished?.Invoke();

    }

    private void EmitTime(int totalSeconds)
    {
        int minutes = totalSeconds / 60;
        int seconds = totalSeconds % 60;
        OnTimeChanged?.Invoke(minutes, seconds);
    }
}
