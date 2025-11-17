using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int health = 100;
    [SerializeField] private GameObject damagePopupPrefab;

    public void TakeDamage(int dmg)
    {
        health -= dmg;
        Debug.Log($"{gameObject.name} took {dmg} damage! HP = {health}");

        if (health <= 0)
            Destroy(gameObject);
    }
}