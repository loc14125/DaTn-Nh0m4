using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    public AudioSource musicSource;

    private const string MUSIC_KEY = "MusicVolume";

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        musicSource.loop = true;
        musicSource.volume = PlayerPrefs.GetFloat(MUSIC_KEY, 1f);
    }

    public void PlayMusic(AudioClip clip)
    {
        if (musicSource.clip == clip) return;

        musicSource.clip = clip;
        musicSource.volume = PlayerPrefs.GetFloat(MUSIC_KEY, 1f);
        musicSource.Play();
    }

    // ===== PUBLIC API CHO SETTING =====
    public void SetVolume(float volume)
    {
        musicSource.volume = volume;
        PlayerPrefs.SetFloat("MusicVolume", volume);
    }

    public float GetVolume()
    {
        return musicSource.volume;
    }
}
