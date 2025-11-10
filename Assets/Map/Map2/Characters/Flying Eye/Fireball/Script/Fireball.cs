using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireball : MonoBehaviour
{
    public float speed = 5f;
    public float lifetime = 3f;
    private Vector2 direction = Vector2.right;
    private Animator animator;
    private bool exploded = false;
    private int damage = 30;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        if (!exploded)
        {
            transform.Translate(direction * speed * Time.deltaTime);
        }
    }

    public void SetDirection(Vector2 dir)
    {
        direction = dir.normalized;

        // Lật sprite nếu cần
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.flipX = direction.x < 0;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (exploded) return;

        if (other.CompareTag("Player"))
        {
            // Gây dmg nếu player có script nhận sát thương
            PlayerMovement health = other.GetComponent<PlayerMovement>();
            if (health != null)
            {
                health.TakeDamage(damage); // <= đổi số dmg tùy bạn
            }

            Explode();
        }
        else if (other.CompareTag("Ground"))
        {
            Explode();
        }
    }

    void Explode()
    {
        exploded = true;
        animator.SetTrigger("Explode");
        Destroy(gameObject, 0.5f); // thời gian để animation nổ phát xong
    }
}
