using System;
using UnityEngine;

public class CountdownDemo : MonoBehaviour
{
    public CountdownTimer cd { get; private set; }
    [SerializeField] CountdownUI cdUIPrefab;
    [SerializeField] Transform cdParent;

    //Timer demo
    [SerializeField] TimerUI timerUI;
    GameTimer gameTimer;

    //Countdown MM:SS demo
    [SerializeField] CountdownTimerUI cdTimerUI;
    CountdownTimerMMSS cdTimer;
    void Start()
    {
        cd = new CountdownTimer(this);
        CountdownUI cdUI = Instantiate(cdUIPrefab, cdParent);
        cdUI.Bind(cd);
        cd.Start(3);
        cd.OnComplete += Cd_OnComplete;

    }

    private void Cd_OnComplete()
    {
        //Start Timer
        //gameTimer = new GameTimer(this);
        //timerUI.Bind(gameTimer);
        //gameTimer.Start();

        cdTimer = new CountdownTimerMMSS(this);
        cdTimerUI.Bind(cdTimer);

        cdTimer.Start(180f); // 03:00
        cdTimer.OnFinished += CdTimer_OnFinished;
    }

    private void CdTimer_OnFinished()
    {
        Debug.Log("Countdown MM:SS Finished!");
    }

    //TODO
    //Add audio to cd
    // pause/resume functionality   
}
