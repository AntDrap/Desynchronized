using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class SettingsBehavior : MonoBehaviour
{
    public Slider musicVolume, effectsVolume;
    public Toggle fpsToggle;
    public AudioMixer mixer;

    void Start()
    {
        refresh();
    }

    public void refresh()
    {
        if (!PlayerPrefs.HasKey("effectsVol"))
        {
            PlayerPrefs.SetFloat("effectsVol", 0);
        }

        if (!PlayerPrefs.HasKey("musicVol"))
        {
            PlayerPrefs.SetFloat("musicVol", 0);
        }

        if (!PlayerPrefs.HasKey("showFPS"))
        {
            PlayerPrefs.SetString("showFPS", false.ToString());
        }

        musicVolume.value = PlayerPrefs.GetFloat("musicVol");
        effectsVolume.value = PlayerPrefs.GetFloat("effectsVol");
        fpsToggle.isOn = bool.Parse(PlayerPrefs.GetString("showFPS"));
        updateSettings();
        print("!!!!!");
    }

    public void updateSettings()
    {
        mixer.SetFloat("musicVol", musicVolume.value);
        mixer.SetFloat("effectsVol", effectsVolume.value);
        PlayerPrefs.SetFloat("musicVol", musicVolume.value);
        PlayerPrefs.SetFloat("effectsVol", effectsVolume.value);
        PlayerPrefs.SetString("showFPS", fpsToggle.isOn.ToString());
    }

    public void IncreaseVolume(Slider slider)
    {
        slider.value = Mathf.Clamp(slider.value + 10, -80, 20);
        updateSettings();
    }

    public void DecreaseVolume(Slider slider)
    {
        slider.value = Mathf.Clamp(slider.value - 10, -80, 20);
        updateSettings();
    }

    public void toggleFPS()
    {
        updateSettings();
    }
}
