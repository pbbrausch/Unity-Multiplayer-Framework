using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using TMPro;

public class SoundSettings : MonoBehaviour
{
    public AudioMixer audioMixer; 
    public Slider masterSlider; 
    public Slider musicSlider; 
    public Slider sfxSlider; 
    public TextMeshProUGUI masterVolumeText; 
    public TextMeshProUGUI sfxVolumeText; 
    public TextMeshProUGUI musicVolumeText; 

    // Method to set up the initial volume values and slider positions
    private void Start()
    {
        float masterVolume = FBPP.GetFloat("MasterVolume", 1f);
        float musicVolume = FBPP.GetFloat("MusicVolume", 0.5f);
        float sfxVolume = FBPP.GetFloat("SFXVolume", 0.5f);

        masterSlider.value = masterVolume;
        sfxSlider.value = sfxVolume;
        musicSlider.value = musicVolume;

        SetMasterVolume();
        SetSFXVolume();
        SetMusicVolume();
    }

    // Method to set the master volume in the Audio Mixer
    public void SetMasterVolume()
    {
        float volume = masterSlider.value;
        audioMixer.SetFloat("MasterVolume", Mathf.Log10(volume) * 20);
        masterVolumeText.text = "Master (" + Mathf.RoundToInt(volume * 100) + "%)";
    }

    // Method to set the music volume in the Audio Mixer
    public void SetMusicVolume()
    {
        float volume = musicSlider.value;
        audioMixer.SetFloat("MusicVolume", Mathf.Log10(volume) * 20);
        musicVolumeText.text = "Music (" + Mathf.RoundToInt(volume * 100) + "%)";
    }

    // Method to set the SFX volume in the Audio Mixer
    public void SetSFXVolume()
    {
        float volume = sfxSlider.value;
        audioMixer.SetFloat("SFXVolume", Mathf.Log10(volume) * 20);
        sfxVolumeText.text = "SFX (" + Mathf.RoundToInt(volume * 100) + "%)";
    }

    public void Save()
    {
        FBPP.SetFloat("MasterVolume", masterSlider.value);
        FBPP.SetFloat("MusicVolume", musicSlider.value);
        FBPP.SetFloat("SFXVolume", sfxSlider.value);
    }
}
