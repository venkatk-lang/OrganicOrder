using IACGGames;
using IACGGames.UISystem;
using UnityEngine;

public class GameManagerDemo : GameManagerBase<GameManagerDemo>
{
    private void Start()
    {
        UIManager.Instance.Init();
    }

    public void StartGame() 
    {     
        UIManager.Instance.Show(UIState.GameHUD, 0.5f);

    } 
    
    public override void OnRestart()
    {
        base.OnRestart();
       
    }
    public override void OnQuit()
    {
        base.OnQuit();

    }
    public override void OnPause()
    {
        base.OnPause();
    }
    public override void OnResume()
    {
        base.OnResume();
    }
    public override void OnStartTutorial()
    {
        base.OnStartTutorial();
    }
    public override void OnGameStarted()
    {
        base.OnGameStarted();
        StartGame();
    }
}
