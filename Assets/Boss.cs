using UnityEngine;

public class Boss : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public Animator animator;
    public GameObject bossModel;

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
        bossModel.SetActive(false);
    }

    void Update()
    {
        if (isDead) return;

        float distance = Vector3.Distance(transform.position, player.position);

        if (!isActivated && distance <= detectRange)
        {
            ActivateBoss();
        }

        if (isActivated)
        {
            HandleBehavior(distance);
        }
    }

    void ActivateBoss()
    {
        isActivated = true;
        bossModel.SetActive(true);
        animator.SetTrigger("Spam");
        Debug.Log("🔥 Boss xuất hiện!");
    }

    void HandleBehavior(float distance)
    {
        if (distance > detectRange) return; // Nếu player chạy quá xa thì boss đứng im hoặc quay lại sau

        if (distance > meleeRange && distance <= eleckRange && Time.time >= nextAttackTime)
        {
            // Player ở tầm trung -> bắn điện
            nextAttackTime = Time.time + attackCooldown;
            CastEleck();
        }
        else if (distance <= meleeRange && Time.time >= nextAttackTime)
        {
            // Player ở gần -> tấn công cận chiến
            nextAttackTime = Time.time + attackCooldown;
            MeleeAttack();
        }
        else if (distance > eleckRange)
        {
            // Đuổi theo player
            MoveToPlayer();
        }
    }

    void MoveToPlayer()
    {
        transform.position = Vector3.MoveTowards(transform.position, player.position, moveSpeed * Time.deltaTime);
        animator.SetBool("isRunning", true);
    }

    void MeleeAttack()
    {
        animator.SetBool("isRunning", false);
        animator.SetTrigger("Attack");
        Debug.Log("⚔️ Boss tấn công cận chiến!");
    }

    void CastEleck()
    {
        animator.SetBool("isRunning", false);
        animator.SetTrigger("Eleck");
        Debug.Log("⚡ Boss phóng điện tầm xa!");
        // ở đây m có thể spawn ra projectile hoặc hiệu ứng điện:
        // Instantiate(eleckPrefab, castPoint.position, Quaternion.identity);
    }

    public void Die()
    {
        if (isDead) return;
        isDead = true;
        animator.SetTrigger("Die");
        Debug.Log("☠️ Boss đã chết!");
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
