using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatEnemy2D : MonoBehaviour
{
    [Header("Hover Settings")]
    public float hoverAmplitude = 0.25f;
    public float hoverFrequency = 2f;

    [Header("Attack Settings")]
    public GameObject fireballPrefab;
    public Transform firePoint;          // Điểm bắn cầu lửa
    public float attackCooldown = 1f;    // 1 giây / lần
    public float attackDelay = 0.5f;     // thời điểm bắn trong animation (giữa animation 1s)
    public float fireballSpeed = 5f;

    [Header("Health")]
    public int maxHP = 3;
    private int currentHP;

    [Header("Animation")]
    public Animator animator;
    private SpriteRenderer sr;

    private Vector3 basePosition;
    private bool isAttacking = false;
    private bool isDead = false;
    private float attackTimer;

    void Start()
    {
        basePosition = transform.position;
        currentHP = maxHP;
        sr = GetComponent<SpriteRenderer>();
        attackTimer = attackCooldown;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.X))
        {
            FindObjectOfType<BatEnemy2D>().TakeDamage(1);
        }

        if (isDead) return;

        // Hover effect
        float y = Mathf.Sin(Time.time * hoverFrequency) * hoverAmplitude;
        transform.position = basePosition + Vector3.up * y;

        // Attack logic
        attackTimer -= Time.deltaTime;
        if (attackTimer <= 0f && !isAttacking)
        {
            StartCoroutine(Attack());
            attackTimer = attackCooldown;
        }
    }

    IEnumerator Attack()
    {
        isAttacking = true;
        animator.SetTrigger("Attack");

        // Đợi đến giữa animation để bắn
        yield return new WaitForSeconds(attackDelay);

        ShootFireball();

        // Đợi cho hết animation Attack (1 giây)
        yield return new WaitForSeconds(attackCooldown - attackDelay);
        isAttacking = false;
    }

    void ShootFireball()
    {
        if (fireballPrefab == null || firePoint == null) return;

        GameObject fireball = Instantiate(fireballPrefab, firePoint.position, Quaternion.identity);

        // Hướng bắn theo hướng nhìn của dơi
        Vector2 dir = sr.flipX ? Vector2.left : Vector2.right;

        // Gửi hướng cho fireball
        Fireball fb = fireball.GetComponent<Fireball>();
        if (fb != null)
        {
            fb.SetDirection(dir);
            fb.speed = fireballSpeed;
        }
    }

    public void TakeDamage(int amount)
    {
        if (isDead) return;

        currentHP -= amount;

        if (currentHP > 0)
        {
            animator.SetTrigger("Hurt");
        }
        else
        {
            Die();
        }
    }

    void Die()
    {
        isDead = true;
        animator.SetTrigger("Die");
        GetComponent<Collider2D>().enabled = false; // ngừng va chạm
        StopAllCoroutines();
        Destroy(gameObject, 1.5f); // chờ animation chết
    }

    // Khi bị chém (ví dụ weapon có tag "Weapon" và collider trigger)
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            TakeDamage(3); // chém chết ngay
        }
    }
}
