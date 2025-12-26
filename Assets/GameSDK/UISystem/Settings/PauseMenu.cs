using UnityEngine;
using UnityEngine.UI;
namespace IACGGames
{
    public class PauseMenu : MonoBehaviour
    {
        [Header("Buttons")]
        public Button resumeButton;
        public Button restartButton;
        public Button tutorialButton;
        public Button quitButton;

        [Header("Sliders")]
        public Slider sfxSlider;
        public Slider musicSlider;
        private void OnEnable()
        {
            resumeButton.onClick.AddListener(OnResumeClicked);
            restartButton.onClick.AddListener(OnRestartClicked);
            tutorialButton.onClick.AddListener(OnTutorialClicked);
            quitButton.onClick.AddListener(OnQuitClicked);

            sfxSlider.onValueChanged.AddListener(OnSFXVolumeChanged);
            musicSlider.onValueChanged.AddListener(OnMusicVolumeChanged);


        }
        private void OnDisable()
        {
            resumeButton.onClick.RemoveAllListeners();
            restartButton.onClick.RemoveAllListeners();
            tutorialButton.onClick.RemoveAllListeners();
            quitButton.onClick.RemoveAllListeners();

            sfxSlider.onValueChanged.RemoveAllListeners();
            musicSlider.onValueChanged.RemoveAllListeners();
        }
        private void Start()
        {
            LoadSavedValues();

        }


        private void LoadSavedValues()
        {
            float sfxVal = SaveDataHandler.Instance.InGameSoundFXValue;
            float bgmVal = SaveDataHandler.Instance.BgSoundValue;

            sfxSlider.value = sfxVal;
            musicSlider.value = bgmVal;

            AudioManager.Instance.SetSFXVolume(sfxVal);
            AudioManager.Instance.SetBGMVolume(bgmVal);
        }
    

        private void OnResumeClicked()
        {
            GameSDKSystem.Instance.ResumeGame();
            Show(false);
        }
        private void OnQuitClicked()
        {
            GameSDKSystem.Instance.QuitGame();
        }
        private void OnRestartClicked()
        {
            GameSDKSystem.Instance.RestartGame();
            Show(false);
        }

        private void OnTutorialClicked()
        {
           
            GameSDKSystem.Instance.StartTutorail();
            Show(false);
        }


        private void OnSFXVolumeChanged(float value)
        {
            AudioManager.Instance.SetSFXVolume(value);
            SaveDataHandler.Instance.InGameSoundFXValue = value;
            SaveDataHandler.Instance.WriteDataToSaveFile(SaveDataFiles.SaveData);

        }

        private void OnMusicVolumeChanged(float value)
        {
            AudioManager.Instance.SetBGMVolume(value);
            SaveDataHandler.Instance.BgSoundValue = value;
            SaveDataHandler.Instance.WriteDataToSaveFile(SaveDataFiles.SaveData);

        }


        public void Show(bool show)
        {
            gameObject.SetActive(show);
        }

    }
}