using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
namespace IACGGames
{
    [CreateAssetMenu(fileName = "AudioLibrary", menuName = "Audio/Library")]
    public class AudioLibrary : ScriptableObject
    {

        public AudioMixerGroup sfxAudioMixerGroup;

        public AudioMixerGroup bgmAudioMixerGroup;

        [Header("SFX Entries (auto-generated)")]
        public List<SFXAudio> sfxEntries = new List<SFXAudio>();

        [Header("BGM Entries (auto-generated)")]
        public List<BGMAudio> bgmEntries = new List<BGMAudio>();

    }

    [System.Serializable]
    public class SFXAudio
    {
        public SFXAudioID id;
        [Header("Clip Settings")]
        public AudioClip clip;
        [Range(0f, 1f)] public float volume = 1f;
    }

    public enum SFXAudioID
    {
        Click,
        Correct,
        Wrong,
        Lock,
        Slide,
        Pick,
        Put,
        Bell
    }
    [System.Serializable]
    public class BGMAudio
    {
        public BGMAudioID id;
        [Header("Clip Settings")]
        public AudioClip clip;
        [Range(0f, 1f)] public float volume = 1f;
    }
    public enum BGMAudioID
    {
        MainMenu,
        Gameplay
    }
}