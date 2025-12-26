using System;
using System.Collections;
using UnityEngine;

public class CountdownTimer
{
    public bool IsRunning { get; private set; }
    public bool IsPaused { get; private set; }

    private readonly MonoBehaviour runner;
    private Coroutine routine;
    private float remainingTime;

    public event Action<int> OnTick;
    public event Action OnComplete;

    public CountdownTimer(MonoBehaviour runner)
    {
        this.runner = runner;
    }

    public void Start(int seconds)
    {
        Stop();

        remainingTime = seconds;
        IsPaused = false;
        IsRunning = true;

        routine = runner.StartCoroutine(Routine());
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

    private IEnumerator Routine()
    {
        while (remainingTime > 0)
        {
            OnTick?.Invoke(Mathf.CeilToInt(remainingTime));
            yield return WaitWithPause(1f);
            remainingTime--;
        }

        OnTick?.Invoke(0);
        OnComplete?.Invoke();  
        IsRunning = false;
    }

    private IEnumerator WaitWithPause(float seconds)
    {
        float t = 0f;
        while (t < seconds)
        {
            if (!IsPaused)
                t += Time.deltaTime;

            yield return null;
        }
    }
}
