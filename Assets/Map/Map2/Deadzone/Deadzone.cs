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
            
            PlayerMovement health = other.GetComponent<PlayerMovement>();
            if (health != null)
            {
                health.TakeDamage(damage); 
            }
        }
    }
}
