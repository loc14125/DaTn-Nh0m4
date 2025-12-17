using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTimeStop : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator animator;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    public void Freeze(bool v)
    {
        if (rb != null)
            rb.simulated = !v;

        if (animator != null)
            animator.speed = v ? 0 : 1;
    }

    public static void FreezeAllEnemies(bool v)
    {
        EnemyTimeStop[] enemies = FindObjectsOfType<EnemyTimeStop>();
        foreach (var e in enemies)
            e.Freeze(v);
    }
}