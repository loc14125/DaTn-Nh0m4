using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossHealth : MonoBehaviour
{
    public int maxHP = 300;
    private int currentHP;
    public Animator anim;

    void Start()
    {
        currentHP = maxHP;
    }

    public void TakeDamage(int dmg)
    {
        currentHP -= dmg;
        Debug.Log($"Boss mất {dmg} máu — còn {currentHP}");

        anim.SetTrigger("Hit");

        if (currentHP <= 0)
            Die();
    }

    void Die()
    {
        anim.SetTrigger("Die");
        Debug.Log("Boss đã chết!");
        // disable AI, drop item, mở cửa, cutscene, v.v.
    }
}