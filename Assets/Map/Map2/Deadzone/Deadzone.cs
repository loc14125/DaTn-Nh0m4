using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deadzone : MonoBehaviour
{
    public int damage;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerMovement player = other.GetComponent<PlayerMovement>();
            if (player != null)
            {
                // ⭐ TRUYỀN VỊ TRÍ CỦA DEADZONE (attackerPos)
                player.TakeDamage(damage, transform.position);
            }
        }
    }
}