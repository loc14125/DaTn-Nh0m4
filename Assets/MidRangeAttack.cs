using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MidRangeAttack : MonoBehaviour
{
    public float speed = 8f;
    public int damage = 10;
    public float lifetime = 3f;

    private Vector3 dir;

    public void Init(Vector3 target)
    {
        dir = (target - transform.position).normalized;
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        transform.position += dir * speed * Time.deltaTime;
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            PlayerMovement p = col.transform.root.GetComponent<PlayerMovement>();
            if (p != null)
                p.TakeDamage(damage, transform.position);

            Destroy(gameObject);
        }
    }
}

