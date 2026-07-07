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
        InitAudioControls();
        InitDisplayControls();
    }

    private void InitAudioControls()
    {
        if (AudioManager.Instance == null) return;

        if (masterVolumeSlider != null)
        {
            masterVolumeSlider.SetValueWithoutNotify(AudioManager.Instance.masterVolume);
            masterVolumeSlider.onValueChanged.RemoveAllListeners();
            masterVolumeSlider.onValueChanged.AddListener(AudioManager.Instance.SetMasterVolume);
        }

        if (musicVolumeSlider != null)
        {
            musicVolumeSlider.SetValueWithoutNotify(AudioManager.Instance.musicVolume);
            musicVolumeSlider.onValueChanged.RemoveAllListeners();
            musicVolumeSlider.onValueChanged.AddListener(AudioManager.Instance.SetMusicVolume);
        }

        if (sfxVolumeSlider != null)
        {
            sfxVolumeSlider.SetValueWithoutNotify(AudioManager.Instance.sfxVolume);
            sfxVolumeSlider.onValueChanged.RemoveAllListeners();
            sfxVolumeSlider.onValueChanged.AddListener(AudioManager.Instance.SetSfxVolume);
        }
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
        AudioManager.Instance?.SetMasterVolume(1f);
        AudioManager.Instance?.SetMusicVolume(1f);
        AudioManager.Instance?.SetSfxVolume(1f);
        DisplaySettingsManager.Instance?.SetBrightness(1f);
        InitAudioControls();
        InitDisplayControls();
    }

    public void ResetAllControls()
    {
        foreach (var rebind in rebindActions)
            rebind.ResetToDefault();
    }
}