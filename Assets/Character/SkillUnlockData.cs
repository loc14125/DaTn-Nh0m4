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

    LoadData(); 
}
    public void SaveData()
{
    PlayerPrefs.SetInt("BoltUnlocked", boltUnlocked ? 1 : 0);
    PlayerPrefs.SetInt("ThunderUnlocked", thunderUnlocked ? 1 : 0);
    PlayerPrefs.Save();
}

public void LoadData()
{
    boltUnlocked = PlayerPrefs.GetInt("BoltUnlocked", 0) == 1;
    thunderUnlocked = PlayerPrefs.GetInt("ThunderUnlocked", 0) == 1;
}
}
