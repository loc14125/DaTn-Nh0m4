using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class playerHealth : MonoBehaviour
{
    [SerializeField] int maxHealth;
    int currentHealth;
    public thanhmaunv healthBar;
    public UnityEvent OnDeath;


    private void OnEnable()
    {
        OnDeath.AddListener(Death);
    }

    private void Start()
    {
        currentHealth = maxHealth;

        healthBar.UpdateBar(currentHealth, maxHealth);
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        if (currentHealth < 0)
        {
            currentHealth = 0;
            OnDeath.Invoke();
        }

        healthBar.UpdateBar(currentHealth, maxHealth);
    }

    public void Death()
    {
        Destroy(gameObject);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            TakeDamage(6);
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("heo")) 
        {
            TakeDamage(10);
        }
    }
}