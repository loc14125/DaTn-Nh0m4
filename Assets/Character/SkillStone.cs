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

            SkillHintUI.Instance.ShowHint(
                "Bạn đã lấy lại sức mạnh sấm sét thần thánh của mình. Nhấn I để dùng"
            );
        }
        else if (skillToUnlock == SkillType.Bolt)
        {
            SkillUnlockData.Instance.boltUnlocked = true;

            SkillHintUI.Instance.ShowHint(
                "Bạn đã lấy lại sức mạnh của thần sấm .Nhấn U để bắn đạn."
            );
        }

        Destroy(gameObject);
    }
}