using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitboxSnake : MonoBehaviour
{
    public int damage = 10;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        PlayerMovement pm = collision.GetComponent<PlayerMovement>();
        if (pm != null)
            pm.TakeDamage(damage, transform.position);
    }
}

