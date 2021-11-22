using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SoundMenu : MonoBehaviour
{
    public AudioMixer audioMixer;

    public Toggle musicToggle;
    public Toggle sfxToggle;

    private void OnEnable()
    {
        musicToggle.isOn = !AudioManager.instance.bgMute;
        sfxToggle.isOn = !AudioManager.instance.sfxMute;
    }

    public void SetVolume(float volume)
    {
        audioMixer.SetFloat("volume", volume);
    }

    public void SetBackground(bool value)
    {
        AudioManager.instance.SetMute(SoundType.Background, !value);
    }

    public void SetSfx(bool value)
    {
        AudioManager.instance.SetMute(SoundType.Sfx, !value);
    }
}
