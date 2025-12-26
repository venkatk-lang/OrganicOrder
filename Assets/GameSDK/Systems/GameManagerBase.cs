
using UnityEngine;
namespace IACGGames
{
    public abstract class GameManagerBase<T> : Singleton<T>, IGameLifecycle
    where T : MonoBehaviour
    {
        protected override void Awake()
        {
            base.Awake();
            // GameSDKSystem should be loaded first
            GameSDKSystem.Instance.OnGameLoaded(this);
        }

        public virtual void OnPause()
        {
            Debug.Log($"[{typeof(T).Name}] Paused");
        }

        public virtual void OnResume()
        {
            Debug.Log($"[{typeof(T).Name}] Resumed");
        }

        public virtual void OnRestart()
        {
            Debug.Log($"[{typeof(T).Name}] Restart triggered");
        }

        public virtual void OnQuit()
        {
            Debug.Log($"[{typeof(T).Name}] Quit triggered");
        }

        public virtual void OnStartTutorial()
        {
            Debug.Log($"[{typeof(T).Name}] Start Tutorial");
        }

        public virtual void OnGameStarted()
        {
            Debug.Log($"[{typeof(T).Name}] Start Game");
        }
    }
}