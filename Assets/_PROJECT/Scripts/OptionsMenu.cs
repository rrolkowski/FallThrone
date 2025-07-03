using System;
using UnityEngine;
using UnityEngine.Audio;
using static UnityEngine.Rendering.DebugUI;

public class OptionsMenu : MonoBehaviour
{
    [Header("Audio Mixer")]
    [SerializeField] private AudioMixer _audioMixer;

    [Header("FPS Limit Values")]
    private readonly int[] _fpsOptions = { 60, 120, 144, 180, 240, -1 };

    private void Start()
    {
        InitializeDefaultsIfNeeded();
        ApplySavedSettings();
    }

    private void InitializeDefaultsIfNeeded()
    {
        if(!PlayerPrefs.HasKey("Initialized"))
        {
            PlayerPrefs.SetInt("FPSLimitIndex", 0);
            PlayerPrefs.SetInt("WindowMode", 0);
            PlayerPrefs.SetFloat("MusicVolume", 1f);
            PlayerPrefs.SetFloat("EffectsVolume", 1f);

            PlayerPrefs.SetInt("Initialized", 1);
            PlayerPrefs.Save();
        }

    }

    public void SetLimitFPS(int index)
    {
        int targetFPS = _fpsOptions[index];
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

        Screen.fullScreenMode = mode;
        PlayerPrefs.SetInt("WindowMode", index);
    }

    public void SetMusicVolume(float value)
    {
        _audioMixer.SetFloat("MusicVolume", Mathf.Log10(Mathf.Clamp(value, 0.0001f, 1f)) * 20f);
        PlayerPrefs.SetFloat("MusicVolume", value);
    }

    public void SetEffectsVolume(float value)
    {
        _audioMixer.SetFloat("EffectsVolume", Mathf.Log10(Mathf.Clamp(value, 0.0001f, 1f)) * 20f);
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
    }
}
