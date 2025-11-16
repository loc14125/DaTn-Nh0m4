using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoltScript : MonoBehaviour
{
    public float speed = 10f;
    public int damage = 20;
    public float lifeTime = 3f;

    private int direction = 1;

    private void Start()
    {
        Destroy(gameObject, lifeTime); // Tự hủy sau 3 giây
    }

    public void SetDirection(int dir)
    {
        direction = dir;
        transform.localScale = new Vector3(dir, 1, 1); // xoay sprite theo hướng
    }

    private void Update()
    {
        transform.Translate(Vector2.right * direction * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Enemy")) return;

        // Boss
        Boss boss = collision.transform.root.GetComponent<Boss>();
        if (boss != null)
        {
            boss.TakeDamage(damage, true); 
            Destroy(gameObject);
            return;
        }

        // EnemyAI2
        EnemyAI2 e2 = collision.transform.root.GetComponent<EnemyAI2>();
        if (e2 != null)
        {
            e2.TakeDamage(damage);
            Destroy(gameObject);
            return;
        }

        // Enemy Golem
        EnemyAI golem = collision.transform.root.GetComponent<EnemyAI>();
        if (golem != null)
        {
            golem.TakeDamage(damage);
            Destroy(gameObject);
            return;
        }

        // Bat
        BatEnemy2D bat = collision.transform.root.GetComponent<BatEnemy2D>();
        if (bat != null)
        {
            bat.TakeDamage(damage);
            Destroy(gameObject);
            return;
        }
    }
}
