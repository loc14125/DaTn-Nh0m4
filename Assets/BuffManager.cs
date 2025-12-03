using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffManager : MonoBehaviour
{
    public static BuffManager Instance;

    public int bonusMaxHP = 0;
    public int bonusATK = 0;
    public int bonusBoltDamage = 0;

    public int baseBoltDamage = 40;

    
    

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

    public void ApplyBuffToPlayer(PlayerMovement player)
{
    if (player == null) return;

    // Tăng máu tối đa và heal
    player.maxHealth += bonusMaxHP;
    player.Heal(bonusMaxHP);

    // Tăng damage đánh thường
    player.playerAttack.damage += bonusATK;

    // Tăng damage bolt
    player.boltBonusDamage = baseBoltDamage + bonusBoltDamage;

}
public int GetBoltDamage()
{
    return baseBoltDamage + bonusBoltDamage;
}
}
