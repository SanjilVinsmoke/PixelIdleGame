using System.Collections.Generic;
using UnityEngine;
using Utils;
[RequireComponent(typeof(AudioSource))]
public  class AudioManager : SingletonMonoBehavior<AudioManager>
{
    [System.Serializable]
    public class Sound
    {
        public string name;
        public AudioClip clip;
        [Range(0f, 1f)]
        public float volume = 1f;
        [Range(0.1f, 3f)]
        public float pitch = 1f;
        public bool loop;
        
        [HideInInspector]
        public AudioSource source;
    }

    [SerializeField] private Sound[] sounds;
    [SerializeField] private float masterVolume = 1f;
    
    private Dictionary<string, Sound> soundDictionary;

    protected override void InitializeSingleton()
    {
        soundDictionary = new Dictionary<string, Sound>();
        
        foreach (Sound s in sounds)
        {
            AudioSource source = gameObject.AddComponent<AudioSource>();
            s.source = source;
            source.clip = s.clip;
            source.volume = s.volume * masterVolume;
            source.pitch = s.pitch;
            source.loop = s.loop;
            
            soundDictionary[s.name] = s;
        }
    }

    public void PlaySound(string name)
    {
        if (soundDictionary.TryGetValue(name, out Sound sound))
        {
            sound.source.Play();
        }
        else
        {
            Debug.LogWarning($"Sound {name} not found!");
        }
    }

    public void StopSound(string name)
    {
        if (soundDictionary.TryGetValue(name, out Sound sound))
        {
            sound.source.Stop();
        }
    }

    public void SetMasterVolume(float volume)
    {
        masterVolume = Mathf.Clamp01(volume);
        foreach (Sound s in sounds)
        {
            s.source.volume = s.volume * masterVolume;
        }
    }
    
    
}