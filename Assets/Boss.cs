using UnityEngine;

public class Boss : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public Animator animator;
    public GameObject bossModel;
    private Rigidbody2D rb;

    [Header("Settings")]
    public float detectRange = 10f;
    public float meleeRange = 2f;
    public float eleckRange = 6f;
    public float moveSpeed = 3f;
    public float attackCooldown = 2f;

    private bool isActivated = false;
    private bool isDead = false;
    private float nextAttackTime = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        bossModel.SetActive(false);
    }

    void Update()
    {
        if (isDead || !isActivated) return;

        float distance = Vector2.Distance(transform.position, player.position);
        HandleBehavior(distance);
    }

    void ActivateBoss()
    {
        if (isActivated) return;
        isActivated = true;

        bossModel.SetActive(true);
        animator.SetTrigger("Spam");
        Debug.Log("🔥 Boss xuất hiện!");
    }

    void HandleBehavior(float distance)
    {
        if (distance > detectRange) return;

        if (distance > meleeRange && distance <= eleckRange && Time.time >= nextAttackTime)
        {
            nextAttackTime = Time.time + attackCooldown;
            CastEleck();
        }
        else if (distance <= meleeRange && Time.time >= nextAttackTime)
        {
            nextAttackTime = Time.time + attackCooldown;
            MeleeAttack();
        }
        else if (distance > eleckRange)
        {
            MoveToPlayer();
        }
    }

    void MoveToPlayer()
    {
        Vector2 direction = (player.position - transform.position).normalized;
        rb.velocity = new Vector2(direction.x * moveSpeed, rb.velocity.y);

        // Lật mặt boss nếu cần
        if (direction.x > 0)
            transform.localScale = new Vector3(1, 1, 1);
        else if (direction.x < 0)
            transform.localScale = new Vector3(-1, 1, 1);

        animator.SetBool("isRunning", true);
    }

    void MeleeAttack()
    {
        rb.velocity = Vector2.zero;
        animator.SetBool("isRunning", false);
        animator.SetTrigger("Attack");
        Debug.Log("⚔️ Boss tấn công cận chiến!");
    }

    void CastEleck()
    {
        rb.velocity = Vector2.zero;
        animator.SetBool("isRunning", false);
        animator.SetTrigger("Eleck");
        Debug.Log("⚡ Boss phóng điện tầm xa!");
    }

    public void Die()
    {
        if (isDead) return;
        isDead = true;
        rb.velocity = Vector2.zero;
        animator.SetTrigger("Die");
        Debug.Log("☠️ Boss đã chết!");
    }

    // 👇 Sử dụng Collider2D thay vì Collider
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            player = other.transform;
            ActivateBoss();
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, eleckRange);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, meleeRange);
    }
}
