using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapMusic : MonoBehaviour
{
    public AudioClip mapMusic;

    void Start()
    {
        if (AudioManager.instance != null && mapMusic != null)
        {
            AudioManager.instance.PlayMusic(mapMusic);
        }
    }
}
