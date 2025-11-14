using System.Collections;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [Header("Movement Settings")]
    public float speed = 2f;
    public float moveDistance = 2f;   // quãng đường đi tới/lui
    public float idleTime = 1f;       // thời gian đứng chờ

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
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private Collider2D col;

    private bool isIdle = false;
    private bool isChasing = false;
    private Vector2 startPos;
    private int moveDir = 1; // 1 = phải, -1 = trái

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        col = GetComponent<Collider2D>();
        currentHealth = maxHealth;

        player = GameObject.FindGameObjectWithTag("Player").transform;
        startPos = transform.position;
    }

    void Update()
    {
        if (isDead) return;

        float distanceToPlayer = Vector2.Distance(attackPoint.position, player.position);

        // ✅ Attack
        if (distanceToPlayer <= attackRange)
        {
            Attack();
            return;
        }

        // ✅ Chase
        if (distanceToPlayer <= 5.0f)
        {
            isChasing = true;
            ChasePlayer();
        }
        else
        {
            isChasing = false;

            if (!isIdle)
                Patrol();
        }
    }

    void Patrol()
    {
        animator.SetBool("isWalking", true);
        transform.Translate(Vector2.right * moveDir * speed * Time.deltaTime);

        float traveled = Mathf.Abs(transform.position.x - startPos.x);
        if (traveled >= moveDistance)
            StartCoroutine(IdleAndTurn());
    }

    IEnumerator IdleAndTurn()
    {
        isIdle = true;
        animator.SetBool("isWalking", false);
        yield return new WaitForSeconds(idleTime);

        moveDir *= -1;
        spriteRenderer.flipX = !spriteRenderer.flipX;

        // reset vị trí gốc để đo lại quãng đường
        startPos = transform.position;

        isIdle = false;
    }

    void ChasePlayer()
    {
        if (player == null) return;

        if (player.position.x < transform.position.x)
            spriteRenderer.flipX = true;
        else
            spriteRenderer.flipX = false;

        animator.SetBool("isWalking", true);
        transform.position = Vector2.MoveTowards(transform.position, player.position, speed * Time.deltaTime);
    }

    void Attack()
    {
        animator.SetBool("isWalking", false);

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

        Collider2D hitPlayer = Physics2D.OverlapCircle(attackPoint.position, attackRange, LayerMask.GetMask("Player"));
        if (hitPlayer != null)
        {
            hitPlayer.GetComponent<PlayerMovement>().TakeDamage(damage);
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
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(attackPoint.position, attackRange);
        }
    }
}
