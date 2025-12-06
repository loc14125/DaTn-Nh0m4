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
    public EnemyUI healthUI;

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private Transform player;

    private bool isIdle = false;
    private int moveDir = 1; // 1=right, -1=left
    private Vector2 startPos;
    private bool lastHitByThunder = false;
    [SerializeField] private GameObject damagePopupPrefab;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 1;
        rb.freezeRotation = true;

        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        currentHealth = maxHealth;
        if (healthUI == null)
            healthUI = FindObjectOfType<EnemyUI>();

        player = GameObject.FindGameObjectWithTag("Player").transform;
        startPos = transform.position;
    }

    void Update()
    {
        if (isDead) return;
        if (healthUI != null)
        {
            healthUI.Init(maxHealth);           // Set max slider
            healthUI.UpdateHealth(currentHealth); // Set value hiện tại
        }
        else
        {
            Debug.LogWarning("⚠ Không tìm thấy PlayerHealthUI!");
        }

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
        PlayerMovement player = hitPlayer.GetComponent<PlayerMovement>();

        if (player != null)
        {
            // ⭐ TRUYỀN VỊ TRÍ CỦA ENEMY LÀM attackerPos
            player.TakeDamage(damage, transform.position);
        }
    }
}
    public void MarkHitByThunder()
{
    lastHitByThunder = true;
}

    public void TakeDamage(int dmg)
    {
        if (isDead) return;
        

        currentHealth -= dmg;
        animator.SetTrigger("Hurt");
        if (damagePopupPrefab != null)
    {
        Vector3 pos = transform.position + Vector3.up * 1f;
        Instantiate(damagePopupPrefab, pos, Quaternion.identity)
            .GetComponent<DamagePopUp>()?
            .Setup(dmg);
    }
        if (healthUI != null)
            healthUI.UpdateHealth(currentHealth);
        if (currentHealth <= 0)
            Die();
    }

    void Die()
    {
        if (isDead) return;
        isDead = true;

        rb.velocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Kinematic;
        GetComponent<Collider2D>().enabled = false;

        // Reset các trigger/bool để không state nào đè lên Die
        animator.ResetTrigger("Hurt");
        animator.SetBool("isWalking", false);

        animator.SetTrigger("Die");
        if (lastHitByThunder && BuffManager.Instance != null)
    {
        BuffManager.Instance.OnThunderKill();
    }

        // destroy sau 1.1s — dư 0.1 giây để ensure animation play đủ
        Destroy(gameObject, 1f);
        ScoreManager.Instance.AddScore(100);
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
