using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ENEMYAI4 : EnemyBase, ITimeStopable
{
    [Header("Hover Settings")]
    public float hoverAmplitude = 0.25f;
    public float hoverFrequency = 2f;
    private bool isTimeStopped = false;

    [Header("Attack Settings")]
    public GameObject fireballPrefab;
    public Transform firePoint;          // Điểm bắn cầu lửa
    public float attackCooldown = 1f;    // 1 giây / lần
    public float attackDelay = 0.5f;     // thời điểm bắn trong animation (giữa animation 1s)
    public float fireballSpeed = 5f;

    [Header("Health")]
    public int maxHealth = 60;
    private int currentHealth;
    public EnemyHealthUIs healthUI;

    [Header("Animation")]
    public Animator animator;
    private SpriteRenderer sr;

    private Vector3 basePosition;
    private bool isAttacking = false;
    private bool isDead = false;
    private float attackTimer;
    private bool lastHitByThunder = false;
    [SerializeField] private GameObject damagePopupPrefab;

    void Start()
    {
        basePosition = transform.position;
        currentHealth = maxHealth;
        if (healthUI == null)
            healthUI = FindObjectOfType<EnemyHealthUIs>();
        sr = GetComponent<SpriteRenderer>();
        attackTimer = attackCooldown;
    }

   void Update()
{
    if (isDead || isTimeStopped) return;

    if (healthUI != null)
    {
        healthUI.Init(maxHealth);
        healthUI.UpdateHealth(currentHealth);
    }

    // Hover effect
    float y = Mathf.Sin(Time.time * hoverFrequency) * hoverAmplitude;
    transform.position = basePosition + Vector3.up * y;

    // Attack logic
    attackTimer -= Time.deltaTime;
    if (attackTimer <= 0f && !isAttacking)
    {
        StartCoroutine(Attack());
        attackTimer = attackCooldown;
    }
}
IEnumerator Attack()
{
    isAttacking = true;
    animator.SetTrigger("Attack");

    float t = 0f;
    while (t < attackDelay)
    {
        if (!isTimeStopped)
            t += Time.deltaTime;
        yield return null;
    }

    if (!isTimeStopped)
        ShootFireball();

    t = 0f;
    while (t < attackCooldown - attackDelay)
    {
        if (!isTimeStopped)
            t += Time.deltaTime;
        yield return null;
    }

    isAttacking = false;
}

    void ShootFireball()
    {
        if (fireballPrefab == null || firePoint == null) return;

        GameObject fireball = Instantiate(fireballPrefab, firePoint.position, Quaternion.identity);

        // Hướng bắn theo hướng nhìn của dơi
        Vector2 dir = sr.flipX ? Vector2.left : Vector2.right;

        // Gửi hướng cho fireball
        Fireball fb = fireball.GetComponent<Fireball>();
        if (fb != null)
        {
            fb.SetDirection(dir);
            fb.speed = fireballSpeed;
        }
    }
    public void MarkHitByThunder()
    {
        lastHitByThunder = true;
    }

    public void TakeDamage(int amount)
    {
        if (isDead) return;

        currentHealth -= amount;
        if (healthUI != null)
            healthUI.UpdateHealth(currentHealth);

        // ⭐ Hiển thị Damage Popup
        if (damagePopupPrefab != null)
        {
            Vector3 pos = transform.position + Vector3.up * 1f;
            GameObject pop = Instantiate(damagePopupPrefab, pos, Quaternion.identity);
            pop.GetComponent<DamagePopUp>()?.Setup(amount);
        }

        // ⭐ Thunder kill bonus
        if (lastHitByThunder && BuffManager.Instance != null)
        {
            BuffManager.Instance.OnThunderKill();
        }

        // ⭐ Nếu còn sống → phát animation Hurt
        if (currentHealth > 0)
        {
            animator.SetTrigger("Hurt");
        }
        else
        {
            Die();
        }
    }

    protected override void Die()
    {
        isDead = true;
        animator.SetTrigger("Die");
        GetComponent<Collider2D>().enabled = false; // ngừng va chạm
        StopAllCoroutines();
        Destroy(gameObject, 1.5f); // chờ animation chết
        ScoreManager.Instance.AddScore(100);
base.Die();}

    // Khi bị chém (ví dụ weapon có tag "Weapon" và collider trigger)
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            TakeDamage(3); // chém chết ngay
        }
    }
    public void OnTimeStop(bool stop)
{
    isTimeStopped = stop;

    // Đóng băng animation
    if (animator != null)
        animator.speed = stop ? 0f : 1f;
}
}
