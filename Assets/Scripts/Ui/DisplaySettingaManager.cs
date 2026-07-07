using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Games.UI
{
    /// <summary>
    /// Persistent display settings manager. Brightness has no built-in Unity
    /// concept without a post-processing stack, so this fakes it with a
    /// full-screen black Image on a top-of-everything Canvas whose alpha we
    /// lerp — put that Canvas + Image on the SAME persistent object as this
    /// script (DontDestroyOnLoad) so it stays on top across every scene.
    /// </summary>
    public class DisplaySettingsManager : MonoBehaviour
    {
        public static DisplaySettingsManager Instance { get; private set; }

        [SerializeField] private Image brightnessOverlay;
        [Range(0f, 1f)] public float brightness = 1f; // 1 = full bright, 0 = black

        private Resolution[] _resolutions;

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            DontDestroyOnLoad(gameObject);

            _resolutions = Screen.resolutions
                .GroupBy(r => (r.width, r.height))
                .Select(g => g.First())
                .ToArray();

            LoadSettings();
        }

        public List<string> GetResolutionOptions()
        {
            return _resolutions.Select(r => $"{r.width} x {r.height}").ToList();
        }

        public int GetCurrentResolutionIndex()
        {
            for (int i = 0; i < _resolutions.Length; i++)
                if (_resolutions[i].width == Screen.currentResolution.width &&
                    _resolutions[i].height == Screen.currentResolution.height)
                    return i;
            return Mathf.Max(0, _resolutions.Length - 1);
        }

        public void SetResolution(int index)
        {
            if (index < 0 || index >= _resolutions.Length) return;
            var r = _resolutions[index];
            Screen.SetResolution(r.width, r.height, Screen.fullScreenMode);
            PlayerPrefs.SetInt("display_resolution_index", index);
        }

        public void SetFullscreen(bool isFullscreen)
        {
            Screen.fullScreen = isFullscreen;
            PlayerPrefs.SetInt("display_fullscreen", isFullscreen ? 1 : 0);
        }

        public void SetBrightness(float value)
        {
            brightness = Mathf.Clamp01(value);
            if (brightnessOverlay != null)
            {
                Color c = brightnessOverlay.color;
                c.a = 1f - brightness;
                brightnessOverlay.color = c;
            }
            PlayerPrefs.SetFloat("display_brightness", brightness);
        }

        private void LoadSettings()
        {
            bool fullscreen = PlayerPrefs.GetInt("display_fullscreen", Screen.fullScreen ? 1 : 0) == 1;
            SetFullscreen(fullscreen);

            int resIndex = PlayerPrefs.GetInt("display_resolution_index", GetCurrentResolutionIndex());
            SetResolution(resIndex);

            SetBrightness(PlayerPrefs.GetFloat("display_brightness", 1f));
        }
    }
}