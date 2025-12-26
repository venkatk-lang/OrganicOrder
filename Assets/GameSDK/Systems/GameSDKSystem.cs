using UnityEngine;
namespace IACGGames
{
    public class GameSDKSystem : Singleton<GameSDKSystem>
    {
        public PlayerData playerData;
        public bool IsPaused { get; private set; }

        public IGameLifecycle gameLifecycle;
        
        public void StartGame()
        {
            ResumeGame();
            gameLifecycle?.OnGameStarted();
        }
        public void PauseGame()
        {
            if (IsPaused) return;
            IsPaused = true;
            gameLifecycle?.OnPause();
        }

        public void ResumeGame()
        {
            if (!IsPaused) return;
            IsPaused = false;
            gameLifecycle?.OnResume();
        }

        public void RestartGame()
        {
            ResumeGame();
            gameLifecycle?.OnRestart(); 
        }
        public void StartTutorail()
        {
            ResumeGame();
            gameLifecycle?.OnStartTutorial();
        }
        public void QuitGame()
        {
            ResumeGame();
            gameLifecycle?.OnQuit();
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        }

        //IGameLifeCycle - Derive GameManager from IGameLifeCycle , at start of gamemanager - we call this
        public void OnGameLoaded(IGameLifecycle game)
        {
            gameLifecycle = game;
        }

        //public async void OnGameClosed(int score) // take game basis json data here - string
        //{
        //    await APIService.UpdatePlayerScore(playerData.playerId, score);
        //}

     
        //public async Task InitializePlayer(string playerId)
        //{
        //    playerData = await APIService.GetPlayerData(playerId);
        //    if (playerData == null)
        //    {
        //        Debug.LogWarning("Failed to fetch player data. Using defaults.");
        //        playerData = new PlayerData(playerId, "Guest");
        //    }
        //}
    }
}