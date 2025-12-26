using UnityEngine;
using UnityEngine.UI;
namespace IACGGames
{
    public class SettingsSystem : MonoBehaviour
    {
        [SerializeField] private PauseMenu pauseMenu;
        [SerializeField] private Button pauseButton;
        private void Start()
        {
            ShowPauseMenu(false);
        }
        private void OnEnable()
        {
            if (pauseButton != null)
                pauseButton.onClick.AddListener(() =>
            {
                GameSDKSystem.Instance.PauseGame();
                ShowPauseMenu(true);
            });

        }
        private void OnDisable()
        {
            if (pauseButton != null)
            pauseButton.onClick.RemoveAllListeners();
        }
        public void ShowPauseMenu(bool show)
        {
            if (pauseMenu != null)
                pauseMenu.Show(show);
        }
    }
}