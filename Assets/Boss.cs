using UnityEngine;
using System.Collections;

public class Boss : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public Animator animator;
    public GameObject bossModel;
    private Rigidbody2D rb;

    [Header("Hitboxes")]
    public GameObject hitboxNormal;
    public GameObject comboHit1;
    public GameObject comboHit2;

    [Header("Stats")]
    public float detectRange = 10f;
    public float meleeRange = 2f;
    public float moveSpeed = 2f;
    public float attackCooldown = 3f;
    public int maxHealth = 200;
    public int damage = 15;

    private int currentHealth;
    private bool isActivated;
    private bool isDead;
    private bool isAttacking;
    private bool playerInMeleeRange;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        DisableAllHitboxes();
        currentHealth = maxHealth;
    }

    void Update()
    {
        if (isDead) return;

        if (!isActivated)
        {
            if (player && Vector2.Distance(transform.position, player.position) <= detectRange)
                ActivateBoss();
            return;
        }

        float distance = Vector2.Distance(transform.position, player.position);
        HandleBehavior(distance);
    }

    void ActivateBoss()
    {
        isActivated = true;
        bossModel.SetActive(true);
        animator.SetTrigger("Spam");
        Debug.Log("👹 Boss xuất hiện!");
    }

    void HandleBehavior(float distance)
    {
        if (distance <= meleeRange)
        {
            if (!playerInMeleeRange)
            {
                playerInMeleeRange = true;
                rb.velocity = Vector2.zero;
                animator.SetBool("isRunning", false);
            }

            if (!isAttacking)
                StartCoroutine(AttackPlayer());
        }
        else
        {
            playerInMeleeRange = false;
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
            yield return new WaitForSeconds(0.3f);
            ActivateHitbox(hitboxNormal, 0.25f);
        }
        else
        {
            animator.SetTrigger("ATK2");
            yield return StartCoroutine(ComboAttack());
        }

        yield return new WaitForSeconds(attackCooldown);
        isAttacking = false;
    }

    IEnumerator ComboAttack()
    {
        yield return new WaitForSeconds(0.35f);
        ActivateHitbox(comboHit1, 0.25f);

        yield return new WaitForSeconds(0.55f);
        ActivateHitbox(comboHit2, 0.25f);
    }

    void ActivateHitbox(GameObject hitbox, float time)
    {
        StartCoroutine(HitboxRoutine(hitbox, time));
    }

    IEnumerator HitboxRoutine(GameObject hitbox, float time)
    {
        hitbox.SetActive(true);
        yield return new WaitForSeconds(time);
        hitbox.SetActive(false);
    }

    void DisableAllHitboxes()
    {
        hitboxNormal?.SetActive(false);
        comboHit1?.SetActive(false);
        comboHit2?.SetActive(false);
    }

    void MoveToPlayer()
    {
        Vector2 dir = (player.position - transform.position).normalized;
        rb.velocity = new Vector2(dir.x * moveSpeed, rb.velocity.y);
        transform.localScale = new Vector3(dir.x > 0 ? 1 : -1, 1, 1);
        animator.SetBool("isRunning", true);
    }

    public void TakeDamage(int dmg)
    {
        currentHealth -= dmg;
        Debug.Log($"🔥 Boss trúng đòn! Còn {currentHealth} máu");

        if (currentHealth <= 0)
            Die();
    }

    void Die()
    {
        isDead = true;
        rb.velocity = Vector2.zero;
        animator.SetTrigger("Die");
        Debug.Log("💀 Boss đã chết!");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            player = other.transform;
        }

        // ✅ Boss gây damage cho Player nếu va chạm bằng hitbox
        if (other.CompareTag("Player") && (hitboxNormal.activeSelf || comboHit1.activeSelf || comboHit2.activeSelf))
        {
            PlayerMovement p = other.GetComponent<PlayerMovement>();
            if (p != null)
                p.TakeDamage(damage);
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
