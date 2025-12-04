using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeAttackHitbox : MonoBehaviour
{
    public int damage = 10;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerMovement p = collision.GetComponent<PlayerMovement>();
            if (p != null)
            {
                p.TakeDamage(damage, transform.position);
            }
        }
    }
}