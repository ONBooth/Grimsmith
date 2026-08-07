using System;
using UnityEngine;
using System.Collections.Generic;
using Games.UI;
using NUnit.Framework;
using UnityEngine.UI;

namespace  Game.UI
{
    /// <summary>
    ///  attach  to settings panel 
    ///   <summary>
}

public class SettingsUI : MonoBehaviour
{
    [Header("Audio")] [SerializeField] private Slider masterVolumeSlider;
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Slider sfxVolumeSlider;

    [Space] [Header("Display")] [SerializeField]
    private Slider brightnessSlider;

    [SerializeField] private Toggle FullscreenToggle;
    [SerializeField] private Dropdown TMP_Dropdown;

    [Space] [Header("Controls")] [SerializeField]
    private List<RebindActionUI> rebindActions = new List<RebindActionUI>();

    private void OnEnable()
    {
        
        InitDisplayControls();
    }

   

    private void InitDisplayControls()
    {
        if (DisplaySettingsManager.Instance == null) return;

        if (brightnessSlider != null)
        {
            brightnessSlider.SetValueWithoutNotify(DisplaySettingsManager.Instance.brightness);
            brightnessSlider.onValueChanged.RemoveAllListeners();
            brightnessSlider.onValueChanged.AddListener(DisplaySettingsManager.Instance.SetBrightness);
        }

        if (FullscreenToggle != null)
        {
            FullscreenToggle.SetIsOnWithoutNotify(Screen.fullScreen);
            FullscreenToggle.onValueChanged.RemoveAllListeners();
            FullscreenToggle.onValueChanged.AddListener(DisplaySettingsManager.Instance.SetFullscreen);
        }

        if (TMP_Dropdown != null)
        {
            List<string> options = DisplaySettingsManager.Instance.GetResolutionOptions();
            TMP_Dropdown.ClearOptions();
            TMP_Dropdown.AddOptions(options);
            TMP_Dropdown.SetValueWithoutNotify(DisplaySettingsManager.Instance.GetCurrentResolutionIndex());
            TMP_Dropdown.onValueChanged.RemoveAllListeners();
            TMP_Dropdown.onValueChanged.AddListener(DisplaySettingsManager.Instance.SetResolution);
        }
    }

    // Reset Button
    public void ResetAudioDisplayDefualts()
    {
       
        DisplaySettingsManager.Instance?.SetBrightness(1f);
        InitDisplayControls();
    }

    public void ResetAllControls()
    {
        foreach (var rebind in rebindActions)
            rebind.ResetToDefault();
    }
}