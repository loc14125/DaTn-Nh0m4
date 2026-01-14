using System.Collections;
using UnityEngine;

public class EnemyCode : EnemyBase
{
    [Header("Movement Settings")]
    public float speed = 2f;
    public float moveDistance = 2f;   // đi qua trái/phải 1 đoạn
    public float idleTime = 5f;       // đứng yên bao lâu
    private Vector2 startPos;
    private int moveDir = 1;          // 1 = phải, -1 = trái
    private bool isIdle = false;

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
    private bool lastHitByThunder = false;

    [SerializeField] private GameObject damagePopupPrefab;
    private Rigidbody2D rb;



    void Start()
    {
        startPos = transform.position;

        player = GameObject.FindGameObjectWithTag("Player").transform;

        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        if (rb == null)
    Debug.LogError("⚠ EnemyCode: Thiếu Rigidbody2D!");

if (col == null)
    Debug.LogError("⚠ EnemyCode: Thiếu Collider2D!");

        currentHealth = maxHealth;
        
    }

    void Update()
{
    if (isDead) return;

    // ⭐ Tính khoảng cách theo attackPoint (CHUẨN)
    float distanceToPlayer = Vector2.Distance(attackPoint.position, player.position);

    // Ưu tiên attack
    if (distanceToPlayer <= attackRange)
    {
        Attack();
        return;
    }

    // Chase
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

    //==========================//
    //         PATROL           //
    //==========================//
    void Patrol()
    {
        animator.SetBool("isWalking", true);

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
        isIdle = true;
        animator.SetBool("isWalking", false);
        yield return new WaitForSeconds(idleTime);

        moveDir *= -1;                     // đổi hướng
        startPos = transform.position;     // reset vị trí trung tâm
        isIdle = false;
    }

    //==========================//
    //        CHASE PLAYER      //
    //==========================//
    void ChasePlayer()
    {
        animator.SetBool("isWalking", true);

        moveDir = player.position.x < transform.position.x ? -1 : 1;

        Flip(moveDir);

        transform.position = Vector2.MoveTowards(
            transform.position,
            new Vector2(player.position.x, transform.position.y),
            speed * Time.deltaTime
        );
    }

    //==========================//
    //         ATTACK           //
    //==========================//
    void Attack()
    {
        animator.SetBool("isWalking", false);

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

        Collider2D hitPlayer = Physics2D.OverlapCircle(
            attackPoint.position, attackRange,
            LayerMask.GetMask("Player")
        );

        if (hitPlayer != null)
        {
            PlayerMovement playerScript = hitPlayer.GetComponent<PlayerMovement>();
            if (playerScript != null)
                playerScript.TakeDamage(damage, transform.position);
        }
    }

    //==========================//
    //      HIT BY THUNDER      //
    //==========================//
    public void MarkHitByThunder()
    {
        lastHitByThunder = true;
    }

    //==========================//
    //         TAKEDAMAGE       //
    //==========================//
    public void TakeDamage(int dmg)
    {
        if (isDead) return;

        currentHealth -= dmg;

        animator.SetTrigger("Hurt");

        // Damage popup
        if (damagePopupPrefab != null)
        {
            GameObject pop = Instantiate(damagePopupPrefab,
                transform.position + Vector3.up * 1f,
                Quaternion.identity);

            pop.GetComponent<DamagePopUp>()?.Setup(dmg);
        }

        // Thunder buff if kill
        if (lastHitByThunder && BuffManager.Instance != null)
        {
            BuffManager.Instance.OnThunderKill();
        }

        if (currentHealth <= 0)
            Die();
    }

    protected override void Die()
{
    if (isDead) return;
    isDead = true;

    rb.velocity = Vector2.zero;
    rb.bodyType = RigidbodyType2D.Dynamic; 
    rb.gravityScale = 1;

    col.enabled = true;  // ĐỂ QUÁI KHÔNG XUYÊN ĐẤT

    animator.ResetTrigger("Hurt");
    animator.SetBool("isWalking", false);
    animator.SetTrigger("Die");

    // Tắt AI nhưng giữ vật lý
    this.enabled = false;

    Destroy(gameObject, 2f);
    ScoreManager.Instance.AddScore(100);
base.Die();
}

    void Flip(int dir)
{
    spriteRenderer.flipX = (dir == -1);

    // ⭐ AttackPoint flip đúng 100%
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
