using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Shamelessly grabbed from https://www.daggerhartlab.com/unity-audio-and-sound-manager-singleton-script/
public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    // Audio players components.
    public AudioSource MusicSource;
    public AudioSource EffectsSource;

    [SerializeField] AudioClip[] sfxClips;
	
    // Initialize the singleton instance.
    private void Awake()
    {
        // If there is not already an instance of SoundManager, set it to this.
        if (Instance == null)
        {
            Instance = this;
        }
        //If an instance already exists, destroy whatever this object is to enforce the singleton.
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
        //Set SoundManager to DontDestroyOnLoad so that it won't be destroyed when reloading our scene.
        DontDestroyOnLoad (gameObject);
    }

    // Play a single clip through the music source.
    public void PlayMusic(AudioClip clip)
    {
        MusicSource.clip = clip;
        MusicSource.Play();
    }

    // Play a single clip through the sound effects source.
    public void Play(AudioClip clip)
    {
        EffectsSource.clip = clip;
        EffectsSource.Play();
    }
    
    public void Play(SoundEffects clip)
    {
        if (!sfxClips[(int)clip])
        {
            Debug.Log("Tried to play audio clip <color=blue>" + clip.ToString() + "</color> but index was out of range.");
            return;
        }

        EffectsSource.clip = sfxClips[(int)clip];
        EffectsSource.Play();
    }
}

public enum SoundEffects
{
    Bubbles,
    ButtonPress,
    HeartCollect,
    ShieldBroken,
    ShieldCollect
}
