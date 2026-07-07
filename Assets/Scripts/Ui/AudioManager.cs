using System.Collections.Generic;
using UnityEngine;

namespace Games.UI
{
    /// <summary>
    /// Persistent audio settings manager. No AudioMixer required — master
    /// volume drives AudioListener.volume, and music/sfx volumes are applied
    /// directly to any AudioSource tagged with MusicSource or SfxSource below.
    /// Put this on a persistent "Managers" object (see DisplaySettingsManager
    /// notes for the DontDestroyOnLoad setup).
    /// </summary>
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; private set; }

        [Range(0f, 1f)] public float masterVolume = 1f;
        [Range(0f, 1f)] public float musicVolume = 1f;
        [Range(0f, 1f)] public float sfxVolume = 1f;

        private readonly List<AudioSource> _musicSources = new List<AudioSource>();
        private readonly List<AudioSource> _sfxSources = new List<AudioSource>();

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadSettings();
        }

        public void RegisterMusicSource(AudioSource source)
        {
            if (source != null && !_musicSources.Contains(source)) _musicSources.Add(source);
            source.volume = musicVolume;
        }

        public void RegisterSfxSource(AudioSource source)
        {
            if (source != null && !_sfxSources.Contains(source)) _sfxSources.Add(source);
            source.volume = sfxVolume;
        }

        public void SetMasterVolume(float value)
        {
            masterVolume = value;
            AudioListener.volume = masterVolume;
            PlayerPrefs.SetFloat("audio_master", masterVolume);
        }

        public void SetMusicVolume(float value)
        {
            musicVolume = value;
            foreach (var src in _musicSources)
                if (src != null) src.volume = musicVolume;
            PlayerPrefs.SetFloat("audio_music", musicVolume);
        }

        public void SetSfxVolume(float value)
        {
            sfxVolume = value;
            foreach (var src in _sfxSources)
                if (src != null) src.volume = sfxVolume;
            PlayerPrefs.SetFloat("audio_sfx", sfxVolume);
        }

        private void LoadSettings()
        {
            masterVolume = PlayerPrefs.GetFloat("audio_master", 1f);
            musicVolume = PlayerPrefs.GetFloat("audio_music", 1f);
            sfxVolume = PlayerPrefs.GetFloat("audio_sfx", 1f);
            AudioListener.volume = masterVolume;
        }
    }

    /// <summary>Put on any looping music AudioSource so its volume is managed.</summary>
    [RequireComponent(typeof(AudioSource))]
    public class MusicSource : MonoBehaviour
    {
        private void Start() => AudioManager.Instance?.RegisterMusicSource(GetComponent<AudioSource>());
    }

    /// <summary>Put on any one-shot SFX AudioSource so its volume is managed.</summary>
    [RequireComponent(typeof(AudioSource))]
    public class SfxSource : MonoBehaviour
    {
        private void Start() => AudioManager.Instance?.RegisterSfxSource(GetComponent<AudioSource>());
    }
}