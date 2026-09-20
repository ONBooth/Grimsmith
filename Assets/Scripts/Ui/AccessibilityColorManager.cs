using System.Collections.Generic;
using UnityEngine;

namespace Games.UI
{
    public class AccessibilityColorManager : MonoBehaviour
    {
        public enum ColorblindMode { None, Protanopia, Deuteranopia, Tritanopia }

        public static AccessibilityColorManager Instance { get; private set; }

        [Header("Colorblind Correction")]
        [SerializeField] private ColorblindMode currentMode = ColorblindMode.None;
        [SerializeField, Range(0f, 1f)] private float colorblindStrength = 1f;

        [Header("Rainbow Effect")]
        [SerializeField] private bool rainbowEnabled = false;
        [SerializeField] private float rainbowSpeed = 0.5f;
        [SerializeField, Range(0f, 1f)] private float rainbowSaturation = 1f;
        [SerializeField, Range(0f, 1f)] private float rainbowValue = 1f;

        [Header("Epilepsy Safe Mode")]
        [SerializeField] private bool epilepsySafeMode = false;
        [SerializeField] private float maxSafeRainbowSpeed = 1.5f;

        static readonly List<ColorAccessibilityTarget> targets = new List<ColorAccessibilityTarget>();
        float hue;

        const string ColorblindModeKey = "Accessibility_ColorblindMode";
        const string ColorblindStrengthKey = "Accessibility_ColorblindStrength";

        public static ColorblindMode CurrentMode => Instance != null ? Instance.currentMode : ColorblindMode.None;
        public static bool RainbowEnabled => Instance != null && Instance.rainbowEnabled && !Instance.epilepsySafeMode;

        void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;

            LoadSaved();
        }

        void Update()
        {
            if (!rainbowEnabled || epilepsySafeMode) return;

            float speed = Mathf.Min(rainbowSpeed, maxSafeRainbowSpeed);
            hue += Time.unscaledDeltaTime * speed;
            if (hue > 1f) hue -= 1f;

            RefreshAll();
        }

        public static void Register(ColorAccessibilityTarget target)
        {
            if (!targets.Contains(target)) targets.Add(target);
        }

        public static void Unregister(ColorAccessibilityTarget target)
        {
            targets.Remove(target);
        }

        public void SetColorblindMode(int modeIndex) => SetColorblindMode((ColorblindMode)modeIndex);

        public void SetColorblindMode(ColorblindMode mode)
        {
            currentMode = mode;
            PlayerPrefs.SetInt(ColorblindModeKey, (int)mode);
            RefreshAll();
        }

        public void SetColorblindStrength(float value)
        {
            colorblindStrength = value;
            PlayerPrefs.SetFloat(ColorblindStrengthKey, value);
            RefreshAll();
        }

        public void SetRainbowEnabled(bool value)
        {
            rainbowEnabled = value;
            RefreshAll();
        }

        public void SetEpilepsySafeMode(bool value)
        {
            epilepsySafeMode = value;
            RefreshAll();
        }

        // Read-only getters for restoring UI state elsewhere (e.g. GraphicsSettingsManager
        // setting its dropdown/slider to match on load) without mutating anything.
        public int GetSavedMode() => PlayerPrefs.GetInt(ColorblindModeKey, (int)ColorblindMode.None);
        public float GetSavedStrength() => PlayerPrefs.GetFloat(ColorblindStrengthKey, 1f);

        static void RefreshAll()
        {
            foreach (var t in targets)
                if (t != null) t.RefreshColor();
        }

        public static Color GetRainbowColor(Color baseColor)
        {
            if (Instance == null) return baseColor;
            Color rainbow = Color.HSVToRGB(Instance.hue, Instance.rainbowSaturation, Instance.rainbowValue);
            rainbow.a = baseColor.a;
            return rainbow;
        }

        public static Color ApplyColorblindCorrection(Color baseColor)
        {
            if (Instance == null) return baseColor;
            return Daltonize(baseColor, Instance.currentMode, Instance.colorblindStrength);
        }

        static Color Daltonize(Color c, ColorblindMode mode, float strength)
        {
            if (mode == ColorblindMode.None || strength <= 0f) return c;

            Vector3 rgb = new Vector3(c.r, c.g, c.b);
            Vector3 simulated = Simulate(rgb, mode);
            Vector3 error = rgb - simulated;

            Vector3 correctedError = new Vector3(
                0f,
                0.7f * error.x + error.y,
                0.7f * error.x + error.z);

            Vector3 result = rgb + correctedError;
            result = Vector3.Lerp(rgb, ClampVec(result), strength);

            return new Color(result.x, result.y, result.z, c.a);
        }

        static Vector3 Simulate(Vector3 rgb, ColorblindMode mode)
        {
            switch (mode)
            {
                case ColorblindMode.Protanopia:
                    return new Vector3(
                        0.567f * rgb.x + 0.433f * rgb.y,
                        0.558f * rgb.x + 0.442f * rgb.y,
                        0.242f * rgb.y + 0.758f * rgb.z);

                case ColorblindMode.Deuteranopia:
                    return new Vector3(
                        0.625f * rgb.x + 0.375f * rgb.y,
                        0.700f * rgb.x + 0.300f * rgb.y,
                        0.300f * rgb.y + 0.700f * rgb.z);

                case ColorblindMode.Tritanopia:
                    return new Vector3(
                        0.950f * rgb.x + 0.050f * rgb.y,
                        0.433f * rgb.y + 0.567f * rgb.z,
                        0.475f * rgb.y + 0.525f * rgb.z);

                default:
                    return rgb;
            }
        }

        static Vector3 ClampVec(Vector3 v) =>
            new Vector3(Mathf.Clamp01(v.x), Mathf.Clamp01(v.y), Mathf.Clamp01(v.z));

        void LoadSaved()
        {
            currentMode = (ColorblindMode)GetSavedMode();
            colorblindStrength = GetSavedStrength();
        }
    }
}