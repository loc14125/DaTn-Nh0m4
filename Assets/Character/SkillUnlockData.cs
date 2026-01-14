using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillUnlockData : MonoBehaviour
{
    public static SkillUnlockData Instance;

    public bool thunderUnlocked = false;
    public bool boltUnlocked = false;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
}
