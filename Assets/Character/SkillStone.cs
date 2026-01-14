using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillStone : MonoBehaviour
{
    public enum SkillType { Thunder, Bolt }
    public SkillType skillToUnlock;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        if (skillToUnlock == SkillType.Thunder)
        {
            SkillUnlockData.Instance.thunderUnlocked = true;
            Debug.Log("Thunder UNLOCKED");
        }
        else if (skillToUnlock == SkillType.Bolt)
        {
            SkillUnlockData.Instance.boltUnlocked = true;
            Debug.Log("Bolt UNLOCKED");
        }

        Destroy(gameObject); // đá biến mất
    }
}