using UnityEngine;

namespace Games.UI
{
    /// <summary>
    /// Applies a full-screen colorblind-assist filter to the main camera and
    /// persists the chosen mode/strength. GraphicsSettingsManager drives this
    /// via AccessibilityColorManager.Instance.SetColorblindMode(index) and
    /// SetColorblindStrength(value).
    /// </summary>
    [RequireComponent(typeof(UnityEngine.Camera))]
    public class AccessibilityColorManager : MonoBehaviour
    {
        public static AccessibilityColorManager Instance { get; private set; }

        public enum ColorblindMode
        {
            None = 0,
            Protanopia = 1,
            Deuteranopia = 2,
            Tritanopia = 3
        }
        const string ModeKey = "Accessibility_ColorblindMode";
        const string StrengthKey = "Accessibility_ColorblindStrength";

        int currentMode = 0;
        float currentStrength = 1f;
        // ---------------- Public API used by GraphicsSettingsManager ----------------

        public void SetColorblindMode(int modeIndex)
        {
            currentMode = Mathf.Clamp(modeIndex, 0, System.Enum.GetValues(typeof(ColorblindMode)).Length - 1);
            PlayerPrefs.SetInt(ModeKey, currentMode);
        }

        public void SetColorblindStrength(float strength)
        {
            // Sliders are commonly 0-1 already; if yours is 0-100, divide by 100 before calling this.
            currentStrength = Mathf.Clamp01(strength);
            PlayerPrefs.SetFloat(StrengthKey, currentStrength);
        }

        public int GetSavedMode() => PlayerPrefs.GetInt(ModeKey, 0);
        public float GetSavedStrength() => PlayerPrefs.GetFloat(StrengthKey, 1f);

        void LoadSaved()
        {
            currentMode = GetSavedMode();
            currentStrength = GetSavedStrength();
        }
    }
}