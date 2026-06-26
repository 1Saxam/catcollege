using UnityEngine;
using UnityEngine.UI;

public class SettingsAudioConnector : MonoBehaviour
{
    public Slider masterSlider;
    public Slider musicSlider;
    public Slider sfxSlider;
    public Toggle muteToggle;

    void Start()
    {
        if (AudioManager.Instance == null)
        {
            Debug.LogError("AudioManager not found!");
            return;
        }

        // اتصال اسلایدرها به AudioManager
        if (masterSlider != null)
            masterSlider.onValueChanged.AddListener(AudioManager.Instance.SetMasterVolume);

        if (musicSlider != null)
            musicSlider.onValueChanged.AddListener(AudioManager.Instance.SetMusicVolume);

        if (sfxSlider != null)
            sfxSlider.onValueChanged.AddListener(AudioManager.Instance.SetSFXVolume);

        // اتصال دکمه Mute
        if (muteToggle != null)
            muteToggle.onValueChanged.AddListener(isOn => AudioManager.Instance.SetMute(isOn));

        // تنظیم مقدار اولیه اسلایدرها (از AudioManager)
        if (masterSlider != null)
            masterSlider.value = AudioManager.Instance.masterVolume;

        if (musicSlider != null)
            musicSlider.value = AudioManager.Instance.musicVolume;

        if (sfxSlider != null)
            sfxSlider.value = AudioManager.Instance.sfxVolume;

        if (muteToggle != null)
            muteToggle.isOn = false; // مقدار اولیه Mute
    }
}