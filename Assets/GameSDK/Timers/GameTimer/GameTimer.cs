using System;
using System.Collections;
using UnityEngine;

public class GameTimer
{
    public bool IsRunning { get; private set; }
    public bool IsPaused { get; private set; }

    public float ElapsedTime { get; private set; } // seconds

    private readonly MonoBehaviour runner;
    private Coroutine routine;

    public event Action<int, int> OnTimeChanged; // minutes, seconds

    public GameTimer(MonoBehaviour runner)
    {
        this.runner = runner;
    }

    public void Start()
    {
        Stop();

        ElapsedTime = 0f;
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
        int lastSecond = -1;

        while (IsRunning)
        {
            if (!IsPaused)
            {
                ElapsedTime += Time.deltaTime;

                int totalSeconds = Mathf.FloorToInt(ElapsedTime);
                if (totalSeconds != lastSecond)
                {
                    lastSecond = totalSeconds;

                    int minutes = totalSeconds / 60;
                    int seconds = totalSeconds % 60;

                    OnTimeChanged?.Invoke(minutes, seconds);
                }
            }

            yield return null;
        }
    }
}
