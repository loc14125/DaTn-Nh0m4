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
    public float detectRange = 15f;
    public float midRange = 6f;
    public float meleeRange = 2f;
    public float moveSpeed = 2f;
    public float attackCooldown = 3f;
    public int maxHealth = 200;
    public int damage = 15;

    [Header("Phase 2 Settings")]
    public bool isPhase2 = false; 
    public bool isTransforming = false; // khóa di chuyển + attack + tele
    public float hpDrainPerSecond = 3f; 
    public float phase2MoveSpeed = 4f;  
    public int phase2Damage = 25;       

    [Header("Flying Enemy")]
    public GameObject flyingEnemyPrefab;

    [Header("Popup Damage")]
    [SerializeField] private GameObject damagePopupPrefab;

    // --- HEALTH ---
    // currentHealthInt dùng cho các API cũ (TakeDamage, UI expects int)
    private int currentHealthInt;
    // currentHealthF để giảm mượt theo deltaTime
    private float currentHealthF;

    private bool isActivated;
    private bool isDead;
    private bool isAttacking;
    private bool playerInMeleeRange;
    private HashSet<GameObject> damagedPlayersThisHit = new HashSet<GameObject>();

    // --- ⭐ MidRange Enemy Spawn ---
    private bool isPlayerInsideMidRange = true;
    private bool isCountingDown = false;
    private float spawnTimer = 0f;
    private Coroutine countdownCoroutine;

    // ===============================
    //       ⭐ TELEPORT SYSTEM ⭐
    // ===============================
    [Header("Teleport Settings")]
    public float teleportDistance = 18f;
    public float verticalLimit = 7f;
    public float teleportDelay = 5f;    // Thoát vùng → đếm 5 giây để dịch chuyển
    public float teleportCooldown = 30f; // Sau khi tele xong → chờ 30s mới được tele tiếp

    float teleTimer = 0f;
    bool isTeleporting = false;
    bool canTeleportAgain = true;
    public bool lastHitByThunder = false;
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        DisableAllHitboxes();

        // khởi tạo health: dùng float để xử lý giảm mượt
        currentHealthInt = maxHealth;
        currentHealthF = maxHealth;

        bossUI = GetComponent<BossHealthUI>();
        if (bossUI != null)
        {
            bossUI.InitHealth(maxHealth);
        }
        else
        {
            Debug.LogWarning("BossHealthUI not found on Boss GameObject!");
        }

        rb.freezeRotation = true;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
    }

    void ActivateBoss()
    {
        isActivated = true;

        // reset timer/cho phép teleport (nếu muốn boss có thể tele ngay sau khi active)
        teleTimer = 0f;
        canTeleportAgain = true;
        isTeleporting = false;

        // bật model, animation, UI (giữ nguyên như cũ)
        if (bossModel != null) bossModel.SetActive(true);
        if (animator != null) animator.SetTrigger("Spam"); // hoặc "WakeUp" nếu bạn dùng trigger khác
        if (bossUI != null) bossUI.ActivateBossHealth();

        Debug.Log("BOSS ĐÃ ĐƯỢC KÍCH HOẠT");
    }

    void Update()
    {
        if (isDead) return;

        // ⛔ Khi đang biến hình phase 2 → đứng yên, khóa logic
        if (isTransforming)
        {
            rb.velocity = Vector2.zero;
            if (animator != null) animator.SetBool("isRunning", false);
            return;
        }

        // Tự giảm máu khi phase 2 (DÙNG float để giảm mượt, rồi cập nhật UI bằng int)
        if (isPhase2 && !isDead)
        {
            // giảm theo deltaTime -> dùng float để tránh RoundToInt(thấp) thành 0
            currentHealthF -= hpDrainPerSecond * Time.deltaTime;
            if (currentHealthF < 0f) currentHealthF = 0f;

            // cập nhật biến int và UI từng frame nếu giảm đủ 1 HP (hoặc giảm liên tục)
            int newInt = Mathf.CeilToInt(currentHealthF); // Ceil để UI giảm ngay khi float xuống dưới ngưỡng
            if (newInt != currentHealthInt)
            {
                currentHealthInt = newInt;
                if (bossUI != null) bossUI.UpdateHealth(currentHealthInt);
            }

            // nếu cạn máu thì chết
            if (currentHealthF <= 0f)
            {
                Die();
                return;
            }
        }

        if (!isActivated)
        {
            if (player && Vector2.Distance(transform.position, player.position) <= detectRange)
                ActivateBoss();
            return;
        }

        // ⭐ Kích hoạt Phase 2 khi còn 1/3 máu (so sánh float cho chính xác)
        if (!isPhase2 && currentHealthF <= (maxHealth / 3f))
        {
            StartCoroutine(EnterPhase2());
        }

        float distance = Vector2.Distance(transform.position, player.position);
        HandleBehavior(distance);
        HandleMidRangeLogic(distance);
        HandleTeleport(distance); // <-- ⭐ thêm vào đúng yêu cầu
    }

    // ==========================================================
    // 🔥 TELEPORT CONTROLLER
    // ==========================================================
    void HandleTeleport(float distance)
    {
        float heightDiff = Mathf.Abs(transform.position.y - player.position.y);

        bool playerTooFar = distance > teleportDistance || heightDiff > verticalLimit;

        // nếu player trở lại gần → reset chờ đếm lại
        if (!playerTooFar)
        {
            teleTimer = 0;
            isTeleporting = false;
            return;
        }

        if (canTeleportAgain && !isTeleporting)
        {
            teleTimer += Time.deltaTime;

            if (teleTimer >= teleportDelay)
            {
                TeleportToPlayer();
                StartCoroutine(TeleportCooldownWait());
            }
        }
    }

    void TeleportToPlayer()
    {
        float offset = (player.position.x > transform.position.x) ? -1.8f : 1.8f;

        transform.position = new Vector2(
            player.position.x + offset,
            player.position.y
        );

        Debug.Log("BOSS TELEPORTED 🔥");
    }

    IEnumerator TeleportCooldownWait()
    {
        isTeleporting = true;
        canTeleportAgain = false;

        yield return new WaitForSeconds(teleportCooldown);

        canTeleportAgain = true;
        teleTimer = 0;
    }

    // ==========================================================
    // ======== ⬇ CÁC HÀM BÊN DƯỚI GIỮ NGUYÊN KHÔNG CHỈNH! ========
    // ==========================================================

    void HandleMidRangeLogic(float distance)
    {
        if (distance <= midRange || distance <= meleeRange || distance > detectRange)
        {
            isPlayerInsideMidRange = true;
            spawnTimer = 0f;
            if (isCountingDown && countdownCoroutine != null)
            {
                StopCoroutine(countdownCoroutine);
                isCountingDown = false;
            }
            return;
        }

        isPlayerInsideMidRange = false;

        if (!isCountingDown)
            countdownCoroutine = StartCoroutine(MidRangeCountdown());

        if (spawnTimer > 0)
        {
            spawnTimer -= Time.deltaTime;
            if (spawnTimer <= 0)
            {
                SpawnFlyingEnemy();
                spawnTimer = 10f;
            }
        }
    }

    IEnumerator MidRangeCountdown()
    {
        isCountingDown = true;
        float t = 5f;

        while (t > 0)
        {
            if (isPlayerInsideMidRange)
            {
                isCountingDown = false;
                yield break;
            }
            t -= Time.deltaTime;
            yield return null;
        }

        if (!isPlayerInsideMidRange)
        {
            SpawnFlyingEnemy();
            spawnTimer = 10f;
        }
        isCountingDown = false;
    }

    void SpawnFlyingEnemy()
    {
        GameObject e = Instantiate(flyingEnemyPrefab, transform.position + Vector3.up * 1.5f, Quaternion.identity);
        FlyingEnemyAI ai = e.GetComponent<FlyingEnemyAI>();
        if (ai != null) ai.target = player;
        Destroy(e, 7f);
    }

    void HandleBehavior(float distance)
    {
        if (distance <= meleeRange)
        {
            if (!playerInMeleeRange)
            {
                playerInMeleeRange = true;
                rb.velocity = Vector2.zero;
                if (animator != null) animator.SetBool("isRunning", false);
            }
            if (!isAttacking)
                StartCoroutine(AttackPlayer());
            return;
        }
        
        playerInMeleeRange = false;
        MoveToPlayer();
    }

    void MoveToPlayer()
    {
        if (!player) return;

        float dirX = Mathf.Sign(player.position.x - transform.position.x);
        transform.localScale = new Vector3(dirX, 1, 1);
        if (animator != null) animator.SetBool("isRunning", true);
        rb.velocity = new Vector2(dirX * moveSpeed, rb.velocity.y);
    }

    IEnumerator AttackPlayer()
    {
        isAttacking = true;

        int rand = Random.Range(0, 2);
        if (rand == 0)
        {
            if (animator != null) animator.SetTrigger("ATK1");
            yield return new WaitForSeconds(0.3f);
            StartCoroutine(HitboxRoutine(hitboxNormal, 0.25f));
        }
        else
        {
            if (animator != null) animator.SetTrigger("ATK2");
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
        damagedPlayersThisHit.Clear();
        hitbox.SetActive(true);

        float elapsed = 0f;
        Collider2D hitboxCollider = hitbox.GetComponent<Collider2D>();

        while (elapsed < time)
        {
            Collider2D[] hits = Physics2D.OverlapBoxAll(hitboxCollider.bounds.center, hitboxCollider.bounds.size, 0f);
            foreach (Collider2D hit in hits)
            {
                if (hit.CompareTag("Player"))
                {
                    GameObject playerRoot = hit.transform.root.gameObject;

                    if (!damagedPlayersThisHit.Contains(playerRoot))
                    {
                        PlayerMovement p = playerRoot.GetComponent<PlayerMovement>();
                        if (p != null) p.TakeDamage(damage, transform.position);

                        damagedPlayersThisHit.Add(playerRoot);
                    }
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

    // public TakeDamage - đồng bộ int/float và cập nhật UI
    public void TakeDamage(int dmg, bool isProjectile = false, bool isThunder = false)
    {
        if (!isProjectile && !isThunder && Vector2.Distance(transform.position, player.position) > meleeRange)
            return;

        // trừ cả float và int
        currentHealthF -= dmg;
        if (currentHealthF < 0f) currentHealthF = 0f;

        currentHealthInt = Mathf.CeilToInt(currentHealthF);

        if (isThunder)
        {
            lastHitByThunder = true; 
        }

        if (bossUI != null) bossUI.UpdateHealth(currentHealthInt);

        if (damagePopupPrefab != null)
        {
            Vector3 popupPos = transform.position + Vector3.up * 1f;
            GameObject popup = Instantiate(damagePopupPrefab, popupPos, Quaternion.identity);
            popup.GetComponent<DamagePopUp>()?.Setup(dmg);
        }

        if (currentHealthF <= 0f) Die();
    }

    IEnumerator EnterPhase2()
    {
        isPhase2 = true;
        isTransforming = true; // khóa toàn bộ

        rb.velocity = Vector2.zero;
        if (animator != null) animator.SetBool("isRunning", false);

        // Trigger animation biến hình
        if (animator != null) animator.SetTrigger("Phase2");

        Debug.Log("🔥 BOSS START TRANSFORM PHASE 2");

        // Đợi animation kết thúc (chỉnh đúng thời gian animation)
        yield return new WaitForSeconds(2.5f);

        // ⭐ Buff stats sau animation
        moveSpeed = phase2MoveSpeed;
        damage = phase2Damage;

        isTransforming = false;

        Debug.Log("🔥 BOSS COMPLETE PHASE 2");
    }

    void Die()
    {
        if (isDead) return;

        isDead = true;
        rb.velocity = Vector2.zero;
        if (animator != null) animator.SetTrigger("Die");
        if (bossUI != null) bossUI.HideUI();
        Destroy(gameObject, 2f);
        ScoreManager.Instance.AddScore(500);
    }

    void OnDrawGizmosSelected()
    {
        // Detect Range
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectRange);

        // Melee Attack Range
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, meleeRange);

        // Mid Range
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, midRange);

        // OPTIONAL: Teleport condition range
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, teleportDistance);
    }

    public void MarkHitByThunder()
    {
        lastHitByThunder = true;
    }
}
