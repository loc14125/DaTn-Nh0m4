using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBase : MonoBehaviour
{
    [Header("EXP")]
    public int expReward = 20;

    protected bool isDeads = false;

    protected virtual void Die()
    {
        if (isDeads) return;
        isDeads = true;

        PlayerLevelSystem levelSystem = FindObjectOfType<PlayerLevelSystem>();
        if (levelSystem != null)
        {
            levelSystem.AddExp(expReward);
        }

        Destroy(gameObject, 1f);
    }
}
