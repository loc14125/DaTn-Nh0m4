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

        if (other.CompareTag("Player") || other.CompareTag("Ground"))
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
