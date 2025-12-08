using System.Collections;
using UnityEngine;

public class Snake : MonoBehaviour
{
    [Header("Movement Settings")]
    public float speed = 2f;
    public float moveDistance = 2f;
    public float idleTime = 5f;
    private Vector2 startPos;
    private int moveDir = 1;
    private bool isIdle = false;
    [Header("Attack Settings")]
    public Transform attackPoint;
    public float attackRange = 0.6f;
    public int damage = 10;
    public float attackCooldown = 1f;
    private float lastAttack;

    [Header("Stop Settings")]
    public float stopDistance = 1.2f;   // đứng lại khi player quá gần


    [Header("Health Settings")]
    public int maxHealth = 3;
    private int currentHealth;
    private bool isDead = false;

    private Transform player;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;
    private Collider2D col;

    void Start()
    {
        startPos = transform.position;
        currentHealth = maxHealth;

        player = GameObject.FindGameObjectWithTag("Player").transform;

        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();

        if (rb != null)
        {
            rb.gravityScale = 1;
            rb.bodyType = RigidbodyType2D.Dynamic;
        }
    }

    void Update()
    {
        if (isDead) return;

        float dist = Vector2.Distance(attackPoint.position, player.position);

        // Attack
        if (dist <= attackRange)
        {
            Attack();
            return;
        }

        // Chase
        if (dist <= 5f)
        {
            ChasePlayer();
        }
        else
        {
            if (!isIdle)
                Patrol();
        }
    }

    // ---------------- PATROL ---------------- //
    void Patrol()
    {
        animator.SetBool("Run", true);

        transform.position += Vector3.right * moveDir * speed * Time.deltaTime;

        float traveled = Mathf.Abs(transform.position.x - startPos.x);
        if (traveled >= moveDistance)
        {
            StartCoroutine(IdleAndTurn());
        }

        Flip(moveDir);
    }

    IEnumerator IdleAndTurn()
    {
        animator.SetBool("Run", false);
        isIdle = true;

        yield return new WaitForSeconds(idleTime);

        moveDir *= -1;
        startPos = transform.position;
        isIdle = false;
    }

    // ---------------- CHASE ---------------- //
    void ChasePlayer()
    {
        float dist = Vector2.Distance(transform.position, player.position);

        // Nếu player quá gần → đứng yên, không đẩy nhau
        if (dist <= stopDistance)
        {
            animator.SetBool("Run", false);

            // quay mặt về player
            moveDir = (player.position.x < transform.position.x) ? -1 : 1;
            Flip(moveDir);

            return; // KHÔNG DI CHUYỂN
        }

        // Nếu player còn xa → rắn chạy tới
        animator.SetBool("Run", true);

        moveDir = (player.position.x < transform.position.x) ? -1 : 1;
        Flip(moveDir);

        transform.position = Vector2.MoveTowards(
            transform.position,
            new Vector2(player.position.x, transform.position.y),
            speed * Time.deltaTime
        );
    }

    // ---------------- ATTACK ---------------- //
    void Attack()
    {
        animator.SetBool("Run", false);

        if (Time.time - lastAttack >= attackCooldown)
        {
            animator.SetTrigger("Atk2");
            lastAttack = Time.time;

            StartCoroutine(DealDamage(0.35f));
        }
    }

    IEnumerator DealDamage(float delay)
    {
        yield return new WaitForSeconds(delay);

        Collider2D hit = Physics2D.OverlapCircle(
            attackPoint.position,
            attackRange,
            LayerMask.GetMask("Player")
        );

        if (hit != null)
        {
            PlayerMovement p = hit.GetComponent<PlayerMovement>();
            if (p != null)
                p.TakeDamage(damage, transform.position);
        }
    }

    // ---------------- DAMAGE ---------------- //
    public void TakeDamage(int dmg)
    {
        if (isDead) return;

        currentHealth -= dmg;

        if (currentHealth <= 0)
            Die();
    }

    // ---------------- DEATH ---------------- //
    void Die()
    {
        isDead = true;

        rb.velocity = Vector2.zero;
        animator.SetBool("Run", false);
        animator.SetTrigger("Die");

        this.enabled = false;

        Destroy(gameObject, 2f);
    }

    // ---------------- FLIP ---------------- //
    void Flip(int dir)
    {
        spriteRenderer.flipX = (dir == -1);

        if (attackPoint != null)
        {
            float x = Mathf.Abs(attackPoint.localPosition.x);
            attackPoint.localPosition = new Vector3(dir == -1 ? -x : x, attackPoint.localPosition.y, 0);
        }
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