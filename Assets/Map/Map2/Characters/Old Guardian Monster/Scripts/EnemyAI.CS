using System.Collections;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [Header("Patrol Points")]
    public Transform pointA;
    public Transform pointB;

    [Header("Movement Settings")]
    public float speed = 2f;
    public float idleTime = 2f; // Thời gian nghỉ khi đến A hoặc B

    [Header("Health Settings")]
    public int maxHealth = 3;
    private int currentHealth;
    private bool isDead = false;

    private Transform targetPoint;
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private Collider2D col;
    private bool isIdle = false;

    void Start()
    {
        targetPoint = pointB;
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        col = GetComponent<Collider2D>();
        currentHealth = maxHealth;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            FindObjectOfType<EnemyAI>().TakeDamage(1);
        }

        if (isDead) return;

        if (!isIdle)
        {
            Patrol();
        }
    }

    void Patrol()
    {
        animator.SetBool("isWalking", true);

        transform.position = Vector2.MoveTowards(transform.position, targetPoint.position, speed * Time.deltaTime);

        if (Vector2.Distance(transform.position, targetPoint.position) < 0.1f && !isIdle)
        {
            StartCoroutine(IdleAndSwitch());
        }
    }

    IEnumerator IdleAndSwitch()
    {
        isIdle = true;
        animator.SetBool("isWalking", false);
        yield return new WaitForSeconds(idleTime);

        targetPoint = (targetPoint == pointA) ? pointB : pointA;
        Flip();
        isIdle = false;
    }

    void Flip()
    {
        if (spriteRenderer != null)
            spriteRenderer.flipX = !spriteRenderer.flipX;
    }

    // 🩸 Gọi hàm này khi enemy bị tấn công
    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        animator.SetTrigger("Hurt");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        isDead = true;

        // Tắt di chuyển 
        animator.SetBool("isWalking", false);
        animator.SetTrigger("Die");

        // (Tuỳ chọn) Xoá enemy sau 2 giây
        Destroy(gameObject, 2f);
    }

    private void OnDrawGizmos()
    {
        if (pointA != null && pointB != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(pointA.position, pointB.position);
        }
    }
}
