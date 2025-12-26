using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
namespace IACGGames
{
    public class Systems : PersistentSingleton<Systems>
    {
        [SerializeField] string gameSceneName;
        [SerializeField] float delayBeforeLoad = 2f;
        public void Start()
        {
            //All System Initialized here.
            SaveDataHandler.Instance.InitializeSavingSystem();
            AudioManager.Instance.Initialize();
            //from sdkSystem initialize playerdata

            //Load GameScene on initialized
            StartCoroutine(LoadGameSceneDelayed());
        }
        private IEnumerator LoadGameSceneDelayed()
        {
            yield return new WaitForSeconds(delayBeforeLoad);
            AsyncOperation op = SceneManager.LoadSceneAsync(gameSceneName);
            op.allowSceneActivation = false;
            while (!op.isDone)
            {
                float progress = Mathf.Clamp01(op.progress / 0.9f);
                if (op.progress >= 0.9f)
                {
                    op.allowSceneActivation = true;
                }
                yield return null;
            }
        }
    }
}