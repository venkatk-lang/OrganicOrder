using IACGGames;
using IACGGames.UISystem;
using UnityEngine;
public class GameManager : GameManagerBase<GameManager>
{
    [Header("Score")]
    [SerializeField] NormalScoreWrapper normalScoreWrapper;
    [SerializeField] LevelManager levelManager;
    public LevelManager LevelManager {  get { return levelManager; } }

    private void Start()
    {
        UIManager.Instance.Init();
        AudioManager.Instance.PlayBGM(BGMAudioID.MainMenu, true);
    }

    public void StartGame()
    {
       
        normalScoreWrapper.Initialize();
        levelManager.LevelSetup();
        UIManager.Instance.Show(UIState.GameHUD, 0.5f);
        levelManager.StartGameLevel();
    }

    public override void OnRestart()
    {
        levelManager.ResetLevel();
        
        normalScoreWrapper.Initialize();
        levelManager.LevelSetup();
        UIManager.Instance.Show(UIState.GameHUD, 0.5f);
        levelManager.StartGameLevel();
        base.OnRestart();

    }
    public override void OnQuit()
    {
        base.OnQuit();

    }
    public override void OnPause()
    {
      
        base.OnPause();
        RaycastInputManager.Instance.DisableInput();
        levelManager.PauseLevel();
    }
    public override void OnResume()
    {
        base.OnResume();

        RaycastInputManager.Instance.EnableInput();
        levelManager.ResumeLevel();
    }
    public override void OnStartTutorial()
    {
        base.OnStartTutorial();
        levelManager.ResetLevel();
        UIManager.Instance.Show(UIState.GameHUD, 0.5f);
        TutorialManager.Instance.StartTutorial(levelManager);
    }
    public override void OnGameStarted()
    {
        base.OnGameStarted();
        StartGame();
    }

    public void AddScore(int score)
    {
        normalScoreWrapper.Score.Add(score);
    }
    public int CurrentScore => normalScoreWrapper.Score.Score;
}

