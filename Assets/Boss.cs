using UnityEngine;

public class Boss : MonoBehaviour
{
    [Header("Settings")]
    public Transform player;           // Tham chiếu tới player
    public float detectRange = 10f;    // Phạm vi kích hoạt boss
    public float moveSpeed = 3f;       // Tốc độ di chuyển khi đuổi player
    public float attackRange = 2f;     // Phạm vi tấn công player
    public float attackCooldown = 2f;  // Thời gian giữa các đòn tấn công

    [Header("References")]
    public GameObject bossModel;       // Thân boss (ẩn/hiện)
    public Animator animator;          // Animator nếu có

    private bool isActivated = false;  // Đã spawn hay chưa
    private bool isDead = false;       // Boss đã chết chưa
    private float nextAttackTime = 0f;

    void Start()
    {
        // Ẩn boss lúc đầu
        bossModel.SetActive(false);
    }

    void Update()
    {
        if (isDead) return;

        float distance = Vector3.Distance(transform.position, player.position);

        // Nếu chưa kích hoạt và player vào phạm vi -> xuất hiện + spam
        if (!isActivated && distance <= detectRange)
        {
            ActivateBoss();
        }

        // Khi boss đã kích hoạt -> đuổi và đánh
        if (isActivated)
        {
            FollowAndAttackPlayer(distance);
        }
    }

    void ActivateBoss()
    {
        isActivated = true;
        bossModel.SetActive(true);
        Debug.Log("🔥 Boss xuất hiện! Bắt đầu spam tấn công!");
        SpamAttack();
    }

    void FollowAndAttackPlayer(float distance)
    {
        if (distance > attackRange)
        {
            // Đuổi theo player
            transform.position = Vector3.MoveTowards(transform.position, player.position, moveSpeed * Time.deltaTime);
            if (animator) animator.SetBool("isRunning", true);
        }
        else
        {
            // Trong phạm vi tấn công
            if (Time.time >= nextAttackTime)
            {
                nextAttackTime = Time.time + attackCooldown;
                Attack();
            }
        }
    }

    void SpamAttack()
    {
        if (animator) animator.SetTrigger("Spam");
        Debug.Log("💥 Boss spam kỹ năng xuất hiện!");
        // ví dụ: Instantiate(skillPrefab, transform.position, Quaternion.identity);
    }

    void Attack()
    {
        if (animator) animator.SetTrigger("Attack");
        Debug.Log("⚔️ Boss tấn công player!");
        // Gọi animation hoặc gây sát thương cho player ở đây
    }

    public void Die()
    {
        if (isDead) return;
        isDead = true;
        if (animator) animator.SetTrigger("Die");
        Debug.Log("☠️ Boss đã chết!");
        // Có thể thêm: Destroy(gameObject, 2f);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
