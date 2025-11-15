using System.Collections;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [Header("Movement Settings")]
    public float speed = 2f;
    public float moveDistance = 2f;
    public float idleTime = 1f;
    public LayerMask groundLayer;
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;

    [Header("Attack Settings")]
    public Transform attackPoint;
    public float attackRange = 0.6f;
    public int damage = 10;
    public float attackCooldown = 1f;
    private float lastAttackTime;

    [Header("Health")]
    public int maxHealth = 3;
    private int currentHealth;
    private bool isDead = false;

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private Transform player;

    private bool isIdle = false;
    private int moveDir = 1; // 1=right, -1=left
    private Vector2 startPos;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 1;
        rb.freezeRotation = true;

        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        currentHealth = maxHealth;

        player = GameObject.FindGameObjectWithTag("Player").transform;
        startPos = transform.position;
    }

    void Update()
    {
        if (isDead) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer <= attackRange)
        {
            Attack();
            return;
        }

        if (distanceToPlayer <= 5f)
        {
            ChasePlayer();
        }
        else
        {
            if (!isIdle)
                Patrol();
        }
    }

    void Patrol()
    {
        animator.SetBool("isWalking", true);

        // Kiểm tra ground phía trước
        Vector2 checkPos = groundCheck.position + Vector3.right * moveDir * 0.2f;
        bool isGroundAhead = Physics2D.OverlapCircle(checkPos, groundCheckRadius, groundLayer);

        if (isGroundAhead)
        {
            rb.velocity = new Vector2(moveDir * speed, rb.velocity.y);

            float traveled = Mathf.Abs(transform.position.x - startPos.x);
            if (traveled >= moveDistance)
                StartCoroutine(IdleAndTurn());
        }
        else
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
            StartCoroutine(IdleAndTurn());
        }
    }

    IEnumerator IdleAndTurn()
    {
        isIdle = true;
        animator.SetBool("isWalking", false);
        rb.velocity = Vector2.zero;

        yield return new WaitForSeconds(idleTime);

        moveDir *= -1;
        spriteRenderer.flipX = moveDir == -1;
        startPos = transform.position;

        isIdle = false;
    }

    void ChasePlayer()
    {
        animator.SetBool("isWalking", true);

        moveDir = player.position.x < transform.position.x ? -1 : 1;
        spriteRenderer.flipX = moveDir == -1;

        Vector2 checkPos = groundCheck.position + Vector3.right * moveDir * 0.2f;
        bool isGroundAhead = Physics2D.OverlapCircle(checkPos, groundCheckRadius, groundLayer);

        if (isGroundAhead)
        {
            rb.velocity = new Vector2(moveDir * speed, rb.velocity.y);
        }
        else
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
        }
    }

    void Attack()
    {
        animator.SetBool("isWalking", false);
        rb.velocity = new Vector2(0, rb.velocity.y);

        if (Time.time - lastAttackTime >= attackCooldown)
        {
            animator.SetTrigger("Attack");
            lastAttackTime = Time.time;
            StartCoroutine(DealDamageAfterDelay(0.5f));
        }
    }

    IEnumerator DealDamageAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        Collider2D hitPlayer = Physics2D.OverlapCircle(attackPoint.position, attackRange, LayerMask.GetMask("Player"));
        if (hitPlayer != null)
        {
            hitPlayer.GetComponent<PlayerMovement>()?.TakeDamage(damage);
        }
    }

    public void TakeDamage(int dmg)
    {
        if (isDead) return;

        currentHealth -= dmg;
        animator.SetTrigger("Hurt");

        if (currentHealth <= 0)
            Die();
    }

    void Die()
    {
        isDead = true;
        animator.SetBool("isWalking", false);
        animator.SetTrigger("Die");
        rb.velocity = Vector2.zero;
        Destroy(gameObject, 2f);
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(attackPoint.position, attackRange);
        }

        if (groundCheck != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(groundCheck.position + Vector3.right * moveDir * 0.2f, groundCheckRadius);
        }
    }
}
