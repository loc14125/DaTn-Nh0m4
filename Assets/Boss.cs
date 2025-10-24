using UnityEngine;
using System.Collections;

public class Boss : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public Animator animator;
    public GameObject bossModel;
    private Rigidbody2D rb;

    [Header("Settings")]
    public float detectRange = 10f;  // vùng đỏ
    public float meleeRange = 2f;    // vùng xanh (đánh)
    public float moveSpeed = 2f;
    public float attackCooldown = 5f;

    private bool isActivated = false;
    private bool isDead = false;
    private bool isAttacking = false;
    private bool playerInMeleeRange = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        //bossModel.SetActive(false); // ban đầu ẩn boss
    }

    void Update()
    {
        if (isDead) return;

        if (!isActivated)
        {
            // kiểm tra player có trong vùng phát hiện (vùng đỏ) chưa
            if (player && Vector2.Distance(transform.position, player.position) <= detectRange)
            {
                ActivateBoss();
            }
            return;
        }

        float distance = Vector2.Distance(transform.position, player.position);
        HandleBehavior(distance);
    }

    // 🔥 Kích hoạt boss khi player vào vùng đỏ
    void ActivateBoss()
    {
        if (isActivated) return;
        isActivated = true;
        bossModel.SetActive(true);
        animator.SetTrigger("Spam");
        Debug.Log("Boss xuất hiện và bắt đầu đuổi theo!");
    }

    void HandleBehavior(float distance)
    {
        if (distance <= meleeRange)
        {
            // Trong vùng xanh
            if (!playerInMeleeRange)
            {
                // Mới bước vào vùng xanh
                playerInMeleeRange = true;
                rb.velocity = Vector2.zero;
                animator.SetBool("isRunning", false);
                Debug.Log("Player vào vùng cận chiến.");
            }

            // Tấn công nếu chưa tấn công
            if (!isAttacking)
                StartCoroutine(AttackPlayer());
        }
        else
        {
            // Player ra khỏi vùng xanh
            if (playerInMeleeRange)
            {
                playerInMeleeRange = false;
                Debug.Log("Player rời vùng cận chiến — Boss đuổi tiếp.");
            }

            MoveToPlayer();
        }
    }

    IEnumerator AttackPlayer()
    {
        isAttacking = true;

        int rand = Random.Range(0, 2);
        if (rand == 0)
        {
            animator.SetTrigger("ATK1");
            Debug.Log("Boss đánh ATK1!");
        }
        else
        {
            animator.SetTrigger("ATK2");
            Debug.Log("Boss đánh ATK2!");
        }

        // Chờ cooldown xong mới đánh lại
        yield return new WaitForSeconds(attackCooldown);
        isAttacking = false;
    }

    void MoveToPlayer()
    {
        Vector2 direction = (player.position - transform.position).normalized;
        rb.velocity = new Vector2(direction.x * moveSpeed, rb.velocity.y);

        // Lật mặt boss theo hướng player
        if (direction.x > 0)
            transform.localScale = new Vector3(1, 1, 1);
        else if (direction.x < 0)
            transform.localScale = new Vector3(-1, 1, 1);

        animator.SetBool("isRunning", true);
    }

    public void Die()
    {
        if (isDead) return;
        isDead = true;
        rb.velocity = Vector2.zero;
        animator.SetTrigger("Die");
        Debug.Log("Boss đã chết!");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            player = other.transform;
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectRange);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, meleeRange);
    }
}
