using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioSettingsManager : MonoBehaviour
{
    [Header("Mixer")]
    [SerializeField] private AudioMixer mixer;

    [Header("Sliders")]
    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider menuMusicSlider;
    [SerializeField] private Slider collisionsUISlider;

    private const string MasterKey = "MasterVolume";
    private const string MusicKey = "MusicVolume";
    private const string MenuMusicKey = "MenuMusicVolume";
    private const string CollisionsUIKey = "CollisionsUIVolume";

    void Start()
    {
        float master = PlayerPrefs.GetFloat(MasterKey, 0.8f);
        float music = PlayerPrefs.GetFloat(MusicKey, 0.8f);
        float menuMusic = PlayerPrefs.GetFloat(MenuMusicKey, 0.8f);
        float collisionsUI = PlayerPrefs.GetFloat(CollisionsUIKey, 0.8f);

        masterSlider.value = master;
        musicSlider.value = music;
        menuMusicSlider.value = menuMusic;
        collisionsUISlider.value = collisionsUI;

        SetMasterVolume(master);
        SetMusicVolume(music);
        SetMenuMusicVolume(menuMusic);
        SetCollisionsUIVolume(collisionsUI);

        masterSlider.onValueChanged.AddListener(SetMasterVolume);
        musicSlider.onValueChanged.AddListener(SetMusicVolume);
        menuMusicSlider.onValueChanged.AddListener(SetMenuMusicVolume);
        collisionsUISlider.onValueChanged.AddListener(SetCollisionsUIVolume);
    }

    public void SetMasterVolume(float value)
    {
        PlayerPrefs.SetFloat(MasterKey, value);
        PlayerPrefs.Save();
        SetMixerVolume("MasterVolume", value);
    }

    public void SetMusicVolume(float value)
    {
        PlayerPrefs.SetFloat(MusicKey, value);
        PlayerPrefs.Save();
        SetMixerVolume("MusicVolume", value);
    }

    public void SetMenuMusicVolume(float value)
    {
        PlayerPrefs.SetFloat(MenuMusicKey, value);
        PlayerPrefs.Save();
        SetMixerVolume("MenuMusicVolume", value);
    }

    public void SetCollisionsUIVolume(float value)
    {
        PlayerPrefs.SetFloat(CollisionsUIKey, value);
        PlayerPrefs.Save();
        SetMixerVolume("CollisionsUIVolume", value);
    }

    private void SetMixerVolume(string parameterName, float value)
    {
        if (mixer == null) return;

        if (value <= 0.001f)
            mixer.SetFloat(parameterName, -80f);
        else
            mixer.SetFloat(parameterName, Mathf.Log10(value) * 20f);
    }
}