using System;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class OptionsMenu : MonoBehaviour
{
    [Header("Audio Mixer")]
    [SerializeField] private AudioMixer _audioMixer;

    [Header("FPS Limit Values")]
    private readonly int[] _fpsOptions = { 60, 120, 144, 180, 240, -1 };

    [Header("UI")]
    [SerializeField] private TMP_Dropdown _displayModeDropdown;
    [SerializeField] private TMP_Dropdown _fpsLimitDropdown;
    [SerializeField] private Slider _musicVolumeSlider;
    [SerializeField] private Slider _effectsVolumeSlider;


    private void Awake()
    {
        InitializeDefaultsIfNeeded();
        ApplySavedSettings();
    }

    private void InitializeDefaultsIfNeeded()
    {
        if(!PlayerPrefs.HasKey("Initialized"))
        {
            PlayerPrefs.SetInt("FPSLimitIndex", 0);
            PlayerPrefs.SetInt("DisplayMode", 0);
            PlayerPrefs.SetFloat("MusicVolume", 1f);
            PlayerPrefs.SetFloat("EffectsVolume", 1f);

            PlayerPrefs.SetInt("Initialized", 1);
            PlayerPrefs.Save();
        }
    }

    public void SetLimitFPS(int index)
    {
        int targetFPS = _fpsOptions[index];

        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = targetFPS;
        PlayerPrefs.SetInt("FPSLimitIndex", index);
    }

    public void SetDisplayMode(int index)
    {
        FullScreenMode mode = FullScreenMode.ExclusiveFullScreen;
        switch (index)
        {
            case 0: mode = FullScreenMode.ExclusiveFullScreen; break;
            case 1: mode = FullScreenMode.FullScreenWindow; break;
            case 2: mode = FullScreenMode.Windowed; break;
        }

        Resolution currentResolution = Screen.currentResolution;

        Screen.SetResolution(currentResolution.width, currentResolution.height, mode, currentResolution.refreshRateRatio);

        Screen.fullScreenMode = mode;
        PlayerPrefs.SetInt("DisplayMode", index);
    }

    public void SetMusicVolume(float value)
    {
        //_audioMixer.SetFloat("MusicVolume", Mathf.Log10(Mathf.Clamp(value, 0.0001f, 1f)) * 20f);
        float warped = Mathf.Pow(value, 2.2f);
        float volumeDb = Mathf.Log10(Mathf.Clamp(warped, 0.0001f, 1f)) * 20f;
        _audioMixer.SetFloat("MusicVolume", volumeDb);
        PlayerPrefs.SetFloat("MusicVolume", value);
    }

    public void SetEffectsVolume(float value)
    {
        //_audioMixer.SetFloat("EffectsVolume", Mathf.Log10(Mathf.Clamp(value, 0.0001f, 1f)) * 20f);
        float warped = Mathf.Pow(value, 2.2f);
        float volumeDb = Mathf.Log10(Mathf.Clamp(warped, 0.0001f, 1f)) * 20f;
        _audioMixer.SetFloat("EffectsVolume", volumeDb);
        PlayerPrefs.SetFloat("EffectsVolume", value);
    }

    public void ApplySavedSettings()
    {
        int fpsIndex = PlayerPrefs.GetInt("FPSLimitIndex", 0);
        SetLimitFPS(fpsIndex);

        int wmIndex = PlayerPrefs.GetInt("DisplayMode", 0);
        SetDisplayMode(wmIndex);

        float music = PlayerPrefs.GetFloat("MusicVolume", 1f);
        SetMusicVolume(music);

        float effects = PlayerPrefs.GetFloat("EffectsVolume", 1f);
        SetEffectsVolume(effects);

        _displayModeDropdown.value = wmIndex;
        _displayModeDropdown.RefreshShownValue();

        _fpsLimitDropdown.value = fpsIndex;
        _fpsLimitDropdown.RefreshShownValue();

        _musicVolumeSlider.value = music;
        _effectsVolumeSlider.value = effects;
    }
}
