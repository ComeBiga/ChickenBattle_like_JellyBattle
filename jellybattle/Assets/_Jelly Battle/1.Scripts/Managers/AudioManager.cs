using UnityEngine.Audio;
using UnityEngine;
using System;
using Photon.Pun;

public class AudioManager : MonoBehaviour
{
    public AudioMixer audioMixer;
    public AudioMixerGroup bgGroup;
    public AudioMixerGroup sfxGroup;
    public Sound[] sounds;

    public static AudioManager instance;

    public bool bgMute = false;
    public bool sfxMute = false;

    // Start is called before the first frame update
    void Awake()
    {
        if (instance == null)
            instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);

        foreach(Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;

            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
            
            switch(s.soundType)
            {
                case SoundType.Background:
                    s.source.outputAudioMixerGroup = bgGroup;
                    break;
                case SoundType.Sfx:
                    s.source.outputAudioMixerGroup = sfxGroup;
                    break;
            }
        }
    }

    public void Play(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if(s==null)
        {
            Debug.LogWarning("Sound : " + name + " not found!");
            return;
        }
        s.source.Play();
    }

    public void Stop(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound : " + name + " not found!");
            return;
        }
        s.source.Stop();
    }

    public void SetVolume(float volume)
    {
        audioMixer.SetFloat("volume", volume);
    }

    public void SetMute(SoundType type, bool value)
    {
        switch(type)
        {
            case SoundType.Background:
                bgMute = value;
                break;
            case SoundType.Sfx:
                sfxMute = value;
                break;
        }

        foreach(Sound sound in sounds)
        {
            if(sound.soundType == type)
            {
                sound.source.mute = value;
            }
        }
    }
}
