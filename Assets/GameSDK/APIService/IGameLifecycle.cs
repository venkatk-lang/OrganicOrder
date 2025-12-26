namespace IACGGames
{
    public interface IGameLifecycle
    {
    
        void OnGameStarted();
        void OnPause();
        void OnResume();
        void OnRestart();
        void OnQuit();
        void OnStartTutorial();
    }
}