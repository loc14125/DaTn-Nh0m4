using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffManager : MonoBehaviour
{
    public static BuffManager Instance;

    public int bonusMaxHP = 0;
    public int bonusATK = 0;
    public int bonusBoltDamage = 0;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void ResetBuffs()
    {
        bonusMaxHP = 0;
        bonusATK = 0;
        bonusBoltDamage = 0;
    }
}
