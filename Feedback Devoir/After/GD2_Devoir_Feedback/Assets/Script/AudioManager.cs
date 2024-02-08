using UnityEngine.Audio;
using System;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public Sound[] sounds;

    public static AudioManager instance;

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

        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;

            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
            s.source.playOnAwake = s.playonawake;

            // Play sound if playonawake is set to true
            if (s.playonawake)
            {
                s.source.Play();
            }
        }
    }

    void Start()
    {

    }

    public void Play(string name)
    {
       Sound s = Array.Find(sounds, sound => sound.name == name); 
       if (s == null)
       {
        Debug.LogWarning("Sound: " + name + " not found !");
       }
       s.source.Play();
    }
    
    public void PlayOneShot(string name)
    {
       Sound s = Array.Find(sounds, sound => sound.name == name); 
       if (s == null)
       {
        Debug.LogWarning("Sound: " + name + " not found !");
       }
       s.source.PlayOneShot(s.source.clip);
    }

    public void StopSound(string name)
    {
       Sound s = Array.Find(sounds, sound => sound.name == name); 
       if (s == null)
       {
        Debug.LogWarning("Sound: " + name + " not found !");
       }
       s.source.Stop();
    }

    public void EnableSound(string name)
    {
       Sound s = Array.Find(sounds, sound => sound.name == name); 
       if (s == null)
       {
        Debug.LogWarning("Sound: " + name + " not found !");
       }
       s.source.enabled = true;
    }

    public void DisableSound(string name)
    {
       Sound s = Array.Find(sounds, sound => sound.name == name); 
       if (s == null)
       {
        Debug.LogWarning("Sound: " + name + " not found !");
       }
       s.source.enabled = false;
    }

    public bool IsSoundPlaying(string name)
    {
    Sound s = Array.Find(sounds, sound => sound.name == name);
    if (s == null)
    {
        Debug.LogWarning("Sound: " + name + " not found!");
        return false;
    }
    return s.source.isPlaying;
    }
}
