using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Boss : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public Animator animator;
    public GameObject bossModel;
    private Rigidbody2D rb;
    private BossHealthUI bossUI;

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

    // ✅ tracking player bị hit mỗi hitbox
    private HashSet<GameObject> damagedPlayersThisHit = new HashSet<GameObject>();

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        DisableAllHitboxes();
        currentHealth = maxHealth;
        bossUI = GetComponent<BossHealthUI>();
        bossUI.InitHealth(maxHealth);
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
        bossUI.ActivateBossHealth();
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
            StartCoroutine(HitboxRoutine(hitboxNormal, 0.25f));
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
        StartCoroutine(HitboxRoutine(comboHit1, 0.25f));

        yield return new WaitForSeconds(0.55f);
        StartCoroutine(HitboxRoutine(comboHit2, 0.25f));
    }

    IEnumerator HitboxRoutine(GameObject hitbox, float time)
    {
        damagedPlayersThisHit.Clear(); // ✅ reset cho hitbox mới
        hitbox.SetActive(true);

        float elapsed = 0f;
        Collider2D hitboxCollider = hitbox.GetComponent<Collider2D>();

        while (elapsed < time)
        {
            Collider2D[] hits = Physics2D.OverlapBoxAll(hitboxCollider.bounds.center, hitboxCollider.bounds.size, 0f);
            foreach (Collider2D hit in hits)
            {
                if (!hit.CompareTag("Player")) continue;

                GameObject playerRoot = hit.transform.root.gameObject;
                if (damagedPlayersThisHit.Contains(playerRoot)) continue;

                PlayerMovement p = playerRoot.GetComponent<PlayerMovement>();
                if (p != null)
                {
                    p.TakeDamage(damage);
                    damagedPlayersThisHit.Add(playerRoot); // ✅ 1 hit / player / hitbox
                }
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

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
        // ✅ nếu player ở xa hơn meleeRange thì không nhận damage
        if (Vector2.Distance(transform.position, player.position) > meleeRange)
        {
            return;
        }

        currentHealth -= dmg;
        bossUI.UpdateHealth(currentHealth);
        Debug.Log($"🔥 Boss trúng đòn! Còn {currentHealth} máu");

        if (currentHealth <= 0)
            Die();
    }


    void Die()
    {
        isDead = true;
        rb.velocity = Vector2.zero;
        animator.SetTrigger("Die");
        bossUI.HideUI();
        Debug.Log("💀 Boss đã chết!");
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectRange);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, meleeRange);
    }
}
