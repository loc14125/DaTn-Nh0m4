using UnityEngine;

public class Snake : MonoBehaviour
{
    [Header("Snake Stats")]
    public int maxHP = 120;
    public int currentHP;

    [Header("Ranges")]
    public float detectRange = 6f;   // player vào → bắt đầu đuổi
    public float attackRange = 1.8f; // player vào → đánh gần
    public LayerMask playerLayer;

    [Header("Movement")]
    public float moveSpeed = 2.5f;
    Rigidbody2D rb;

    [Header("Attacks")]
    public GameObject attack1Hitbox;
    public GameObject attack2Hitbox;
    public GameObject attack3Hitbox;
    public float attackDuration = 0.35f;
    public float attackCooldown = 1.2f;
    float lastAttack;

    Animator anim;
    Transform player;

    bool isDead = false;
    bool isAttacking = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        currentHP = maxHP;

        // Tắt hết hitbox
        attack1Hitbox.SetActive(false);
        attack2Hitbox.SetActive(false);
        attack3Hitbox.SetActive(false);
    }

    void Update()
    {
        if (isDead) return;

        DetectPlayer();

        if (player != null && !isAttacking)
        {
            float dist = Vector2.Distance(transform.position, player.position);

            if (dist > attackRange)
                MoveTowardPlayer();
            else
                TryAttack();
        }
        else
        {
            anim.SetBool("Run", false);
        }

        FacePlayer();
    }

    // -------------------------
    // PLAYER DETECT
    // -------------------------
    void DetectPlayer()
    {
        Collider2D hit = Physics2D.OverlapCircle(transform.position, detectRange, playerLayer);
        if (hit) player = hit.transform;
    }

    // -------------------------
    // MOVE
    // -------------------------
    void MoveTowardPlayer()
    {
        anim.SetBool("Run", true);

        Vector2 dir = (player.position - transform.position).normalized;
        rb.velocity = new Vector2(dir.x * moveSpeed, rb.velocity.y);
    }

    void FacePlayer()
    {
        if (!player) return;

        float scaleX = (player.position.x > transform.position.x) ? 1 : -1;
        transform.localScale = new Vector3(scaleX, 1, 1);
    }

    // -------------------------
    // ATTACK
    // -------------------------
    void TryAttack()
    {
        if (Time.time < lastAttack + attackCooldown) return;

        lastAttack = Time.time;

        // Random trong 3 đòn
        int atk = Random.Range(1, 3);
        StartCoroutine(AttackRoutine(atk));
    }

    System.Collections.IEnumerator AttackRoutine(int attackType)
{
    isAttacking = true;
    rb.velocity = Vector2.zero;

    // Gọi đúng trigger
    if (attackType == 1) anim.SetTrigger("Atk1");
    if (attackType == 2) anim.SetTrigger("Atk2");
    if (attackType == 3) anim.SetTrigger("Atk3");

    // Delay để khớp animation
    yield return new WaitForSeconds(0.25f);

    // Bật hitbox đúng
    if (attackType == 1) attack1Hitbox.SetActive(true);
    if (attackType == 2) attack2Hitbox.SetActive(true);
    if (attackType == 3) attack3Hitbox.SetActive(true);

    yield return new WaitForSeconds(attackDuration);

    attack1Hitbox.SetActive(false);
    attack2Hitbox.SetActive(false);
    attack3Hitbox.SetActive(false);

    isAttacking = false;
}

    // -------------------------
    // DAMAGE RECEIVE
    // -------------------------
    public void TakeDamage(int dmg)
    {
        if (isDead) return;

        currentHP -= dmg;
        anim.SetTrigger("Hurt");

        if (currentHP <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        isDead = true;
        rb.velocity = Vector2.zero;
        anim.SetTrigger("Die");
        Destroy(gameObject, 1.2f);
    }

    // DEBUG (vẽ range)
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
