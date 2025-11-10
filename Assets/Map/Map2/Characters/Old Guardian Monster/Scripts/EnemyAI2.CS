using System.Collections;
using UnityEngine;

public class EnemyAI2 : MonoBehaviour
{
    [Header("Patrol Points")]
    public Transform pointA;
    public Transform pointB;

    [Header("Movement Settings")]
    public float speed = 2f;
    public float idleTime = 2f;

    [Header("Attack Settings")]
    public Transform attackPoint;
    public float attackRange = 0.6f;
    public int damage = 10;
    public float attackCooldown = 1f;
    private float lastAttackTime;

    [Header("Health Settings")]
    public int maxHealth = 3;
    private int currentHealth;
    private bool isDead = false;

    private Transform player;
    private Transform targetPoint;
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private Collider2D col;
    private bool isIdle = false;
    private bool isChasing = false;

    void Start()
    {
        targetPoint = pointB;
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        col = GetComponent<Collider2D>();
        currentHealth = maxHealth;

        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        if (isDead) return;

        float distanceToPlayer = Vector2.Distance(attackPoint.position, player.position);

        // ✅ Player trong phạm vi tấn công → tấn công
        if (distanceToPlayer <= attackRange)
        {
            Attack();
            return;
        }

        // ✅ Player trong phạm vi phát hiện → Chase
        if (distanceToPlayer <= 5.0f)
        {
            isChasing = true;
            ChasePlayer();
        }
        else
        {
            // ✅ Thoát trạng thái chase → quay lại patrol
            if (isChasing)
            {
                isChasing = false;
                if (targetPoint.position.x < transform.position.x)
                    spriteRenderer.flipX = true;
                else
                    spriteRenderer.flipX = false;
            }

            if (!isIdle)
                Patrol();
        }
    }

    void Patrol()
    {
        animator.SetBool("isWalking", true);
        transform.position = Vector2.MoveTowards(transform.position, targetPoint.position, speed * Time.deltaTime);

        if (Vector2.Distance(transform.position, targetPoint.position) < 0.1f && !isIdle)
            StartCoroutine(IdleAndSwitch());
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

    void ChasePlayer()
    {
        if (player.position.x < transform.position.x)
            spriteRenderer.flipX = true;
        else
            spriteRenderer.flipX = false;

        animator.SetBool("isWalking", true);
        transform.position = Vector2.MoveTowards(transform.position, player.position, speed * Time.deltaTime);
    }

    // ✅ Attack
    void Attack()
    {
        animator.SetBool("isWalking", false);

        // Debug xem quái có bước vào hàm attack không
        Debug.Log("Enemy attempting to attack!");

        if (Time.time - lastAttackTime >= attackCooldown)
        {
            animator.SetTrigger("Attack");
            lastAttackTime = Time.time;

            StartCoroutine(DealDamageAfterDelay(0.8f));
        }
    }

    IEnumerator DealDamageAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        // ✅ Kiểm tra xem lúc gây damage có còn player trong phạm vi không
        Collider2D hitPlayer = Physics2D.OverlapCircle(attackPoint.position, attackRange, LayerMask.GetMask("Player"));

        if (hitPlayer != null)
        {
            hitPlayer.GetComponent<PlayerMovement>().TakeDamage(damage);
            Debug.Log("Enemy dealt damage!");
        }
    }


    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        animator.SetTrigger("Hurt");

        if (currentHealth <= 0)
            Die();
    }

    void Die()
    {
        isDead = true;
        animator.SetBool("isWalking", false);
        animator.SetTrigger("Die");
        Destroy(gameObject, 2f);
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(attackPoint.position, attackRange);
        }
    }
}
