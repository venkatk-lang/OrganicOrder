using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace IACGGames
{
    public class AudioManager : Singleton<AudioManager>
    {
        [Header("Audio Library")]
        public AudioLibrary audioLibrary;

        private Dictionary<SFXAudioID, SFXAudio> sfxLookup;
        private Dictionary<SFXAudioID, AudioSource> sfxSourceLookup;

        [Header("BGM Settings")]
        public AudioSource bgmSourcePrefab;
        private AudioSource bGMAudioSource;
        public float fadeDuration = 1f;
        private bool isPlayingQueue = false;
        private int currentBGMIndex = 0;
        private List<BGMAudio> BGMPlaylist = new List<BGMAudio>();
        private Dictionary<BGMAudioID, BGMAudio> bgmLookup;

        [Header("SFX Settings")]
        public AudioSource sfxSourcePrefab;


        protected override void Awake()
        {
            base.Awake();

            // Build lookups from the ScriptableObject library
            sfxLookup = new Dictionary<SFXAudioID, SFXAudio>();
            sfxSourceLookup = new Dictionary<SFXAudioID, AudioSource>();
            bgmLookup = new Dictionary<BGMAudioID, BGMAudio>();

            BuildSFXMap();
            BuildBGMMap();
            SetupBGMPlayer();

        }
        public void Initialize()
        {
            //setup sound volume as per save data 
            ApplySavedVolumes();


        }
        // ---------------------------------------------------------
        // INITIALIZATION HELPERS
        // ---------------------------------------------------------
        private void BuildSFXMap()
        {
            foreach (var e in audioLibrary.sfxEntries)
            {
                sfxLookup[e.id] = e;

                // Create dedicated AudioSource
                AudioSource src = Instantiate(sfxSourcePrefab, transform);
                src.clip = e.clip;
                src.volume = e.volume;
                src.outputAudioMixerGroup = audioLibrary.sfxAudioMixerGroup;
                src.spatialBlend = 0f; // SFX = 2D

                src.gameObject.name = "SFX_" + e.id;
                sfxSourceLookup[e.id] = src;
            }
        }

        private void BuildBGMMap()
        {
            foreach (var e in audioLibrary.bgmEntries)
                bgmLookup[e.id] = e;
        }

        private void SetupBGMPlayer()
        {
            bGMAudioSource = Instantiate(bgmSourcePrefab, transform);
            bGMAudioSource.loop = true;
            bGMAudioSource.spatialBlend = 0f;
            bGMAudioSource.outputAudioMixerGroup = audioLibrary.bgmAudioMixerGroup;

            bGMAudioSource.gameObject.name = "BGM_Player";
        }
        // -----------------------------
        // PLAY AUDIO BY ID
        // -----------------------------
        public void PlaySFX(SFXAudioID id)
        {
            if (!sfxSourceLookup.TryGetValue(id, out var src)) return;
            src.Play();
        }

        public void PlaySFX(SFXAudioID id, float pitchMin, float pitchMax)
        {
            if (!sfxSourceLookup.TryGetValue(id, out var src)) return;

            src.pitch = Random.Range(pitchMin, pitchMax);
            src.Play();
        }

        #region BGM
        //Play Single BGM and Crossfade if fade = true
        public void PlayBGM(BGMAudioID id, bool fade = true)
        {
            if (!bgmLookup.TryGetValue(id, out var data)) return;

            StopBGMQueue(false);
            StartCoroutine(PlayBGMAsync(data, fade));
        }

        // Play the playlist in loop
        public void PlayBGMQueue(bool fade = true)
        {
            if (BGMPlaylist.Count == 0 || isPlayingQueue) return;

            isPlayingQueue = true;
            currentBGMIndex = 0;

            StartCoroutine(PlayBGMQueueCoroutine(fade));
        }
        //Stop the playlist
        public void StopBGMQueue(bool fade = true)
        {
            if (!isPlayingQueue) return;

            isPlayingQueue = false;
            StartCoroutine(StopBGMAsync(fade));
        }

        private IEnumerator PlayBGMQueueCoroutine(bool fade)
        {
            while (isPlayingQueue && BGMPlaylist.Count > 0)
            {
                BGMAudio next = BGMPlaylist[currentBGMIndex];
                yield return PlayBGMAsync(next, fade);

                // Wait until clip fully finishes
                yield return new WaitUntil(() => !bGMAudioSource.isPlaying || !isPlayingQueue);

                if (!isPlayingQueue) yield break;

                currentBGMIndex = (currentBGMIndex + 1) % BGMPlaylist.Count;
            }
        }

        private IEnumerator PlayBGMAsync(BGMAudio data, bool fade)
        {
            if (fade)
                yield return bGMAudioSource.DOFade(0f, fadeDuration).WaitForCompletion();

            bGMAudioSource.clip = data.clip;
  
            bGMAudioSource.Play();
            if (fade)
                yield return bGMAudioSource.DOFade(data.volume, fadeDuration).WaitForCompletion();
            else
            {
                bGMAudioSource.volume = data.volume;
            }

        }
        private IEnumerator StopBGMAsync(bool fade)
        {
            if (fade)
                yield return bGMAudioSource.DOFade(0f, fadeDuration).WaitForCompletion();

            bGMAudioSource.Stop();
            bGMAudioSource.clip = null;
        }
        #endregion

        #region Settings
        public void SetSFXVolume(float vol)
        {
            float db = VolumeToDecibels(vol);
            audioLibrary.sfxAudioMixerGroup.audioMixer.SetFloat("SFX_Val", db);
            SaveDataHandler.Instance.InGameSoundFXValue = vol;
            SaveDataHandler.Instance.WriteDataToSaveFile(SaveDataFiles.SaveData);
        }

        public void SetBGMVolume(float vol)
        {
            float db = VolumeToDecibels(vol);
            audioLibrary.bgmAudioMixerGroup.audioMixer.SetFloat("BGM_Val", db);
            SaveDataHandler.Instance.BgSoundValue = vol;
            SaveDataHandler.Instance.WriteDataToSaveFile(SaveDataFiles.SaveData);
        }

        private BGMAudio FindCurrentBGMEntry()
        {
            foreach (var e in audioLibrary.bgmEntries)
                if (bGMAudioSource.clip == e.clip)
                    return e;

            return null;
        }

        // Convert 0–1 to decibels
        private float VolumeToDecibels(float value)
        {
            return Mathf.Log10(Mathf.Clamp(value, 0.0001f, 1f)) * 20f;
        }
        private void ApplySavedVolumes()
        {
            SetSFXVolume(SaveDataHandler.Instance.InGameSoundFXValue);
            SetBGMVolume(SaveDataHandler.Instance.BgSoundValue);
        }
        #endregion
    }
}