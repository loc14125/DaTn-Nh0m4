using UnityEngine;
using System.Collections;

public class BatEnemy : MonoBehaviour
{
    [Header("Detection")]
    public float detectRange = 5f;
    public LayerMask playerLayer;

    [Header("Movement")]
    public float hoverAmplitude = 0.25f;
    public float hoverFrequency = 2f;
    public float chargeSpeed = 6f;
    public float returnSpeed = 3f;
    public float stopDistance = 0.5f;

    [Header("Charge Effect")]
    public float preChargeShakeTime = 0.3f;
    public float shakeIntensity = 0.05f;

    [Header("Attack Settings")]
    public GameObject attackTrigger;
    public float attackDuration = 0.30f;

    [Header("Health")]
    public int maxHealth = 50;
    private int currentHealth;
    public EnemyHealUI healthUI;

    private Animator anim;
    private Rigidbody2D rb;
    private Transform player;
    private Vector2 startPos;

    private bool isCharging = false;
    private bool isReturning = false;
    private bool isDead = false;

    private bool lastHitByThunder = false;
    [SerializeField] private GameObject damagePopupPrefab;


    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();

        currentHealth = maxHealth;

        if (healthUI == null)
            healthUI = FindObjectOfType<EnemyHealUI>();

        startPos = rb.position;
        rb.gravityScale = 0;

        if (attackTrigger != null)
            attackTrigger.SetActive(false);

        SetRun(true);
    }


    void Update()
    {
        if (isDead) return;
        if (healthUI != null)
        {
            healthUI.Init(maxHealth);           // Set max slider
            healthUI.UpdateHealth(currentHealth); // Set value hiện tại
        }
        // idle hover
        if (!isCharging && !isReturning)
            HoverEffect();

        // detect player
        if (!isCharging && !isReturning)
        {
            Collider2D hit = Physics2D.OverlapCircle(transform.position, detectRange, playerLayer);
            if (hit)
            {
                player = hit.transform;
                StartCoroutine(ChargeAttack());
            }
        }

        // look at player
        if (!isDead && player != null)
            LookAtPlayer();
    }


    void LookAtPlayer()
    {
        if (player == null) return;

        if (player.position.x > transform.position.x)
            transform.localScale = new Vector3(-1, 1, 1);
        else
            transform.localScale = new Vector3(1, 1, 1);
    }


    void HoverEffect()
    {
        float newY = startPos.y + Mathf.Sin(Time.time * hoverFrequency) * hoverAmplitude;
        rb.MovePosition(new Vector2(transform.position.x, newY));
    }


    IEnumerator ChargeAttack()
    {
        isCharging = true;

        SetRun(false);
        yield return StartCoroutine(PreChargeShake());

        SetRun(true);
        while (Vector2.Distance(rb.position, player.position) > stopDistance)
        {
            Vector2 dir = ((Vector2)player.position - rb.position).normalized;
            rb.MovePosition(rb.position + dir * chargeSpeed * Time.deltaTime);
            yield return null;
        }

        rb.velocity = Vector2.zero;

        // Attack
        SetRun(false);
        PlayAttack();
        yield return StartCoroutine(AttackRoutine());

        isCharging = false;
        isReturning = true;

        StartCoroutine(ReturnToStart());
    }


    IEnumerator PreChargeShake()
    {
        Vector2 originalPos = rb.position;
        float t = 0f;

        while (t < preChargeShakeTime)
        {
            float ox = Random.Range(-shakeIntensity, shakeIntensity);
            float oy = Random.Range(-shakeIntensity, shakeIntensity);

            rb.MovePosition(originalPos + new Vector2(ox, oy));
            t += Time.deltaTime;
            yield return null;
        }

        rb.MovePosition(originalPos);
    }


    IEnumerator AttackRoutine()
    {
        attackTrigger.SetActive(true);
        yield return new WaitForSeconds(attackDuration);
        attackTrigger.SetActive(false);
    }


    IEnumerator ReturnToStart()
    {
        SetRun(true);

        while (Vector2.Distance(rb.position, startPos) > 0.05f)
        {
            rb.MovePosition(Vector2.MoveTowards(rb.position, startPos, returnSpeed * Time.deltaTime));
            yield return null;
        }

        rb.MovePosition(startPos);

        SetRun(true);
        isReturning = false;
    }


    // ----------- Anim -----------
    void SetRun(bool value) => anim.SetBool("isRun", value);
    void PlayAttack() => anim.SetTrigger("Attack");
    void PlayHurt() => anim.SetTrigger("Hurt");
    void PlayDie() => anim.SetTrigger("Die");


    // ----------- Damage -----------
    public void TakeDamage(int amount)
    {
        if (isDead) return;

        currentHealth -= amount;

        // Update UI HP
        if (healthUI != null)
            healthUI.UpdateHealth(currentHealth);

        PlayHurt();

        if (damagePopupPrefab != null)
        {
            GameObject pop = Instantiate(damagePopupPrefab, transform.position + Vector3.up * 1f, Quaternion.identity);
            pop.GetComponent<DamagePopUp>()?.Setup(amount);
        }

        if (currentHealth <= 0)
            Die();
    }


    public void MarkHitByThunder()
    {
        lastHitByThunder = true;
    }


    void Die()
    {
        if (isDead) return;
        isDead = true;
        rb.velocity = Vector2.zero;

        if (lastHitByThunder && BuffManager.Instance != null)
        {
            BuffManager.Instance.OnThunderKill();
        }

        PlayDie();
        ScoreManager.Instance.AddScore(100);

        Destroy(gameObject, 1f);
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectRange);
    }
}
