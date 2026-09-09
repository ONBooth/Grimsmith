using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace  Games.UI
{
    public class GraphicsSettingsManager : MonoBehaviour
    {
        [Header("Resolution")]
        [SerializeField] private TMP_Dropdown resolutionDropdown;
        Resolution[] availableResolutions;

        [Header("Display Mode")] [SerializeField]
        private TMP_Dropdown displayModeDropdown;
        private static readonly FullScreenMode[] displayModes =
        {
            FullScreenMode.ExclusiveFullScreen, // FullScreen
            FullScreenMode.FullScreenWindow, // Borderless
            FullScreenMode.Windowed // Windows
        };
        [Header("Brightness")]
        [SerializeField] private Slider brightnessSlider; // 0-100
        [SerializeField] private Image brightnessOverlay;  // full-screen black Image, alpha = darkness

        [Header("Colorblind")]
        [SerializeField] private TMP_Dropdown colorblindDropdown;
        [SerializeField] private Slider colorblindStrengthSlider; // 0-100

        [Header("Frame Rate")]
        [SerializeField] private TMP_Dropdown fpsDropdown;
        static readonly int[] fpsOptions = { 30, 60, 120, 144, -1 }; // -1 = unlimited
        [SerializeField] private Toggle showFpsCounterToggle;
        [SerializeField] private GameObject fpsCounterDisplay;

        [Header("Confirm / Revert Safety Net")]
        [SerializeField] private GameObject confirmDialog;
        [SerializeField] private TextMeshProUGUI countdownText;
        [SerializeField] private float revertTimeoutSeconds = 8f;
        
        // keeps a snapshot of the display settings BEFORE changes
        Resolution previousResolution;
        FullScreenMode previousDisplayMode;
        Coroutine revertCoroutine;

        const string ResolutionIndexKey = "Graphics_ResolutionIndex";
        const string DisplayModeIndexKey = "Graphics_DisplayModeIndex";
        const string BrightnessKey = "Graphics_Brightness";
        const string FpsIndexKey = "Graphics_FpsIndex";
        const string ShowFpsKey = "Graphics_ShowFps";
        void Start()
        {
            BuildResolutionDropdown();
            BuildDisplayModeDropdown();
            BuildFpsDropdown();
            BuildColorblindDropdown();
            LoadSavedSettings();

            brightnessSlider.onValueChanged.AddListener(SetBrightness);
            colorblindDropdown.onValueChanged.AddListener(i =>
            {
                if (AccessibilityColorManager.Instance == null)
                {
                    Debug.LogWarning("GraphicsSettingsManager: AccessibilityColorManager.Instance is null — " +
                                      "is the component present and active in the scene?");
                    return;
                }
                AccessibilityColorManager.Instance.SetColorblindMode(i);
            });
            colorblindStrengthSlider.onValueChanged.AddListener(v =>
            {
                if (AccessibilityColorManager.Instance == null)
                {
                    Debug.LogWarning("GraphicsSettingsManager: AccessibilityColorManager.Instance is null — " +
                                      "is the component present and active in the scene?");
                    return;
                }
                AccessibilityColorManager.Instance.SetColorblindStrength(v / 100f);
            });
            fpsDropdown.onValueChanged.AddListener(SetFpsLimitByIndex);
            showFpsCounterToggle.onValueChanged.AddListener(SetShowFpsCounter);

            confirmDialog.SetActive(false);
        }

        // ---------------- Resolution ----------------

        void BuildResolutionDropdown()
        {
            availableResolutions = Screen.resolutions;
            resolutionDropdown.ClearOptions();

            var options = new System.Collections.Generic.List<string>();
            int currentIndex = 0;

            for (int i = 0; i < availableResolutions.Length; i++)
            {
                var r = availableResolutions[i];
                options.Add($"{r.width} x {r.height} @ {r.refreshRateRatio.value:0}Hz");

                if (r.width == Screen.currentResolution.width && r.height == Screen.currentResolution.height)
                    currentIndex = i;
            }

            resolutionDropdown.AddOptions(options);
            resolutionDropdown.SetValueWithoutNotify(currentIndex);
            resolutionDropdown.onValueChanged.AddListener(SetResolutionByIndex);
        }

        void SetResolutionByIndex(int index)
        {
            previousResolution = Screen.currentResolution;
            previousDisplayMode = Screen.fullScreenMode;

            var r = availableResolutions[index];
            Screen.SetResolution(r.width, r.height, Screen.fullScreenMode, r.refreshRateRatio);

            PlayerPrefs.SetInt(ResolutionIndexKey, index);
            BeginConfirmCountdown();
        }

        // ---------------- Display Mode ----------------

        void BuildDisplayModeDropdown()
        {
            displayModeDropdown.ClearOptions();
            displayModeDropdown.AddOptions(new System.Collections.Generic.List<string> { "Fullscreen", "Borderless", "Windowed" });

            int currentIndex = System.Array.IndexOf(displayModes, Screen.fullScreenMode);
            displayModeDropdown.SetValueWithoutNotify(currentIndex < 0 ? 0 : currentIndex);
            displayModeDropdown.onValueChanged.AddListener(SetDisplayModeByIndex);
        }

        void SetDisplayModeByIndex(int index)
        {
            previousResolution = Screen.currentResolution;
            previousDisplayMode = Screen.fullScreenMode;

            Screen.fullScreenMode = displayModes[index];
            PlayerPrefs.SetInt(DisplayModeIndexKey, index);
            BeginConfirmCountdown();
        }
        // ---------------- Confirm / Revert safety net ----------------
 // setting change strand the player in an unusable state without an escape hatch.

        void BeginConfirmCountdown()
        {
            if (revertCoroutine != null) StopCoroutine(revertCoroutine);
            revertCoroutine = StartCoroutine(ConfirmCountdownRoutine());
        }

        IEnumerator ConfirmCountdownRoutine()
        {
            confirmDialog.SetActive(true);
            float remaining = revertTimeoutSeconds;

            while (remaining > 0f)
            {
                countdownText.text = $"Reverting in {Mathf.CeilToInt(remaining)}...";
                yield return null;
                remaining -= Time.unscaledDeltaTime; // unscaled — this must work even if the game is paused
            }

            RevertNow();
        }

        public void KeepChanges()
        {
            if (revertCoroutine != null) StopCoroutine(revertCoroutine);
            confirmDialog.SetActive(false);
        }

        public void RevertNow()
        {
            if (revertCoroutine != null) StopCoroutine(revertCoroutine);
            Screen.SetResolution(previousResolution.width, previousResolution.height, previousDisplayMode);
            confirmDialog.SetActive(false);

            // Refresh dropdowns to reflect the restored state.
            BuildResolutionDropdown();
            BuildDisplayModeDropdown();
        }

        // ---------------- Brightness ----------------

        void SetBrightness(float value)
        {
            // value: 0 (darkest) - 100 (brightest). Overlay alpha is inverted —
            // lower brightness = more opaque black overlay on top of the game.
            float alpha = Mathf.Lerp(0.6f, 0f, value / 100f);
            var c = brightnessOverlay.color;
            c.a = alpha;
            brightnessOverlay.color = c;

            PlayerPrefs.SetFloat(BrightnessKey, value);
        }

        // ---------------- Colorblind ----------------

        void BuildColorblindDropdown()
        {
            colorblindDropdown.ClearOptions();
            colorblindDropdown.AddOptions(new System.Collections.Generic.List<string>
            {
                "None", "Protanopia", "Deuteranopia", "Tritanopia"
            });
        }

        // ---------------- FPS ----------------

        void BuildFpsDropdown()
        {
            fpsDropdown.ClearOptions();
            fpsDropdown.AddOptions(new System.Collections.Generic.List<string> { "30", "60", "120", "144", "Unlimited" });
        }

        void SetFpsLimitByIndex(int index)
        {
            Application.targetFrameRate = fpsOptions[index];
            QualitySettings.vSyncCount = 0; // targetFrameRate is ignored while VSync is controlling frame pacing
            PlayerPrefs.SetInt(FpsIndexKey, index);
        }

        void SetShowFpsCounter(bool value)
        {
            fpsCounterDisplay.SetActive(value);
            PlayerPrefs.SetInt(ShowFpsKey, value ? 1 : 0);
        }

        // ---------------- Load saved ----------------

        void LoadSavedSettings()
        {
            float savedBrightness = PlayerPrefs.GetFloat(BrightnessKey, 100f);
            brightnessSlider.SetValueWithoutNotify(savedBrightness);
            SetBrightness(savedBrightness);

            int savedFps = PlayerPrefs.GetInt(FpsIndexKey, 1); // default 60
            fpsDropdown.SetValueWithoutNotify(savedFps);
            SetFpsLimitByIndex(savedFps);

            bool savedShowFps = PlayerPrefs.GetInt(ShowFpsKey, 0) == 1;
            showFpsCounterToggle.SetIsOnWithoutNotify(savedShowFps);
            SetShowFpsCounter(savedShowFps);

            // Colorblind mode/strength live on AccessibilityColorManager, not local PlayerPrefs keys,
            // since that's the single source of truth other systems may also read from.
            if (AccessibilityColorManager.Instance != null)
            {
                int savedColorblindMode = AccessibilityColorManager.Instance.GetSavedMode();
                colorblindDropdown.SetValueWithoutNotify(savedColorblindMode);
                AccessibilityColorManager.Instance.SetColorblindMode(savedColorblindMode);

                float savedColorblindStrength = AccessibilityColorManager.Instance.GetSavedStrength(); // 0-1
                colorblindStrengthSlider.SetValueWithoutNotify(savedColorblindStrength * 100f); // slider is 0-100
                AccessibilityColorManager.Instance.SetColorblindStrength(savedColorblindStrength);
            }
            else
            {
                Debug.LogWarning("GraphicsSettingsManager: AccessibilityColorManager.Instance is null — " +
                                  "skipping colorblind restore. Is the component present and active in the scene?");
            }
        }
    }
}