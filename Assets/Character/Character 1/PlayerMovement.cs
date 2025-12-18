using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 7f;
    [SerializeField] private float jumpForce = 12f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private int maxJumps = 2;
    

    [Header("Dash Settings")]
    [SerializeField] private float dashSpeed = 20f;
    [SerializeField] private float dashDuration = 0.2f;
    [SerializeField] public float dashCooldown = 1f;

     //skill bolt
    [SerializeField] private GameObject boltPrefab;
    [SerializeField] private Transform boltSpawnPoint;

    [Header("Health Settings")]
    [SerializeField]  public int maxHealth = 100;
    private int currentHealth;
    private bool isInvincible = false;

    [Header("Combat Flags")]
    public bool hasDealtDamage = false; // ✅ để khóa damage mỗi swing

    [Header("References")]
    [SerializeField] public PlayerAttack playerAttack;
    public PlayerHealthUI healthUI;

    [Header("Bolt Skill")]
    public float boltCooldown = 1.5f;   // thời gian hồi chiêu
    float lastBoltTime = -999f;

    [Header("Hit Effect Settings")]
    [SerializeField] private GameObject bloodEffectPrefab;
    [SerializeField] private Vector2 bloodOffset = new Vector2(0.3f, 0.5f);
    public int boltBonusDamage = 0;

    [Header("Tutorial Lock")]
    public bool canControl = true;


    

    public PlayerMovement player;
    public int boltDamage = 40;
    private Rigidbody2D rb;
    private Animator anim;
    private bool isGrounded;
    private bool isAttacking;
    private bool isDashing;
    private int jumpCount;
    private float inputX;
    private string currentAnim;
    public float lastDashTime;
    private float originalGravity;
    private int facingDirection = 1;

 void Start()
{
    rb = GetComponent<Rigidbody2D>();
    anim = GetComponent<Animator>();
    originalGravity = rb.gravityScale;

    // TÌM UI NẾU CHƯA GÁN TRONG INSPECTOR
    if (healthUI == null)
        healthUI = FindObjectOfType<PlayerHealthUI>();

    // --- LẤY MÁU TỪ GAME MANAGER ---
    // Nếu GameManager chưa tồn tại (KHÔNG NÊN nhưng phòng lỗi)
    if (GameManager.Instance == null)
    {
        GameObject gmObj = new GameObject("GameManager");
        gmObj.AddComponent<GameManager>();
    }

    // Áp buff vào maxHealth (UI sẽ hiểu luôn)
    maxHealth += BuffManager.Instance.bonusMaxHP;

    // Nếu chưa có máu lưu → lần đầu chơi
    if (GameManager.Instance.playerHealth <= 0)
    {
        currentHealth = maxHealth;
        GameManager.Instance.playerHealth = currentHealth;
    }
    else
    {
        // Load máu lưu lại
        currentHealth = GameManager.Instance.playerHealth;

        // Nếu buff làm maxHealth tăng → clamp lại
        if (currentHealth > maxHealth)
            currentHealth = maxHealth;
    }

    // APPLY BUFF (AN TOÀN)
    if (BuffManager.Instance != null)
    {
        maxHealth += BuffManager.Instance.bonusMaxHP;
        BuffManager.Instance.ApplyBuffToPlayer(this);
    }
    else
    {
        Debug.Log("Tutorial scene: không có BuffManager (OK)");
    }


    // CẬP NHẬT UI
    if (healthUI != null)
    {
        healthUI.Init(maxHealth);           // Set max slider
        healthUI.UpdateHealth(currentHealth); // Set value hiện tại
    }
    else
    {
        Debug.LogWarning("⚠ Không tìm thấy PlayerHealthUI!");
    }
}



    void Update()
    {
        if (!canControl)
        {
            rb.velocity = Vector2.zero;
            ChangeAnimation("Idle");
            return;
        }

        inputX = Input.GetAxisRaw("Horizontal");
        bool wasGrounded = isGrounded;
        isGrounded = CheckGround();

        if (isGrounded && !wasGrounded) jumpCount = 0;

        if (!isAttacking && !isDashing)
        {
            Move();
            if (Input.GetKeyDown(KeyCode.Space)) Jump();
        }

        if (Input.GetKeyDown(KeyCode.J) && !isDashing) Attack();
        if (Input.GetKeyDown(KeyCode.L)) TryDash();
        if (Input.GetKeyDown(KeyCode.U)) TryShootBolt();

        HandleAirAnimations();
    }


    private void Move()
{
    if (isAttacking || isDashing) return; // ✅ không cho quay hoặc di chuyển khi đang dash/attack

    rb.velocity = new Vector2(inputX * moveSpeed, rb.velocity.y);

    if (Mathf.Abs(inputX) > 0.1f)
    {
        facingDirection = inputX > 0 ? 1 : -1;
        transform.rotation = Quaternion.Euler(0, facingDirection == 1 ? 0 : 180, 0);
    }

    if (isGrounded)
    {
        ChangeAnimation(Mathf.Abs(inputX) > 0.1f ? "Run" : "Idle");
    }
}

    private void Jump()
    {
        if (jumpCount < maxJumps)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            ChangeAnimation("Jump");
            jumpCount++;
        }
    }

    private void Attack()
    {
        if (isAttacking) return;

        isAttacking = true;
        hasDealtDamage = false; // ✅ reset tại lúc bắt đầu vung kiếm

        rb.velocity = Vector2.zero;
        ChangeAnimation("Attack1");

        if (playerAttack != null) playerAttack.EnableHitbox();

        Invoke(nameof(EndAttack), 0.4f);
    }

    private void EndAttack()
    {
        isAttacking = false;
        if (playerAttack != null) playerAttack.DisableHitbox();
    }

    // ✅ được animation event gọi cuối animation
    public void ResetAttackDamage()
    {
        hasDealtDamage = false;
    }

    private bool CheckGround()
    {
        return Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }

    private void HandleAirAnimations()
    {
        if (!isGrounded && !isAttacking && !isDashing)
        {
            ChangeAnimation(rb.velocity.y > 0.1f ? "Jump" : "Fall");
        }
    }

private void TryDash()
{
    if (Time.time < lastDashTime + dashCooldown) return;
    lastDashTime = Time.time;

    float inputDir = Input.GetAxisRaw("Horizontal");

    if (Mathf.Abs(inputDir) > 0.1f)
        facingDirection = inputDir > 0 ? 1 : -1;

    isDashing = true;
    isInvincible = true; // 🛡️ BẬT BẤT TỬ

Physics2D.IgnoreLayerCollision(
    LayerMask.NameToLayer("Player"),
    LayerMask.NameToLayer("Enemy"),
    true
);

    ChangeAnimation("Dash");
    rb.gravityScale = 0f;
    rb.velocity = new Vector2(facingDirection * dashSpeed, 0);

    Invoke(nameof(EndDash), dashDuration);
}

private void EndDash()
{
    isDashing = false;
    isInvincible = false; // ❌ TẮT BẤT TỬ khi dash kết thúc

    rb.gravityScale = originalGravity;

    Physics2D.IgnoreLayerCollision(
    LayerMask.NameToLayer("Player"),
    LayerMask.NameToLayer("Enemy"),
    false
);

    if (Mathf.Abs(Input.GetAxisRaw("Horizontal")) > 0.1f)
        ChangeAnimation("Run");
    else
        ChangeAnimation("Idle");
}


  public void TakeDamage(int dmg, Vector3 attackerPos)
{
    if (isInvincible) return;

    // spawn máu và hướng
    int hitDir = transform.position.x < attackerPos.x ? -1 : 1;
    SpawnBloodEffect(hitDir);

    currentHealth -= dmg;

    // Lưu máu vào GameManager
    GameManager.Instance.playerHealth = currentHealth;

    // Cập nhật UI
    if (healthUI != null)
        healthUI.UpdateHealth(currentHealth);

    if (currentHealth > 0)
    {
        ChangeAnimation("Hurt");
        Knockback();
    }
    else
    {
        Die();
        return;
    }

    StartCoroutine(InvincibleTime());
}


    private IEnumerator InvincibleTime()
    {
        isInvincible = true;
        yield return new WaitForSeconds(0.5f);
        isInvincible = false;
    }

    private void Die()
    {
        // Dừng di chuyển và gọi animation Die
        rb.velocity = Vector2.zero;
        ChangeAnimation("Die");

        // Ngăn input hoặc di chuyển khi đã chết
        this.enabled = false;

        // Ẩn nhân vật sau khi animation chạy xong
        StartCoroutine(DisappearAfterDeath());
    }

    private IEnumerator DisappearAfterDeath()
    {
        // ⏳ Chờ đúng thời gian bằng độ dài animation Die (hoặc tầm 1 giây)
        yield return new WaitForSeconds(1.0f);

        // 🫥 Ẩn hoặc xóa nhân vật khỏi màn hình
        gameObject.SetActive(false);
    }

    private void ChangeAnimation(string animName)
    {
        if (anim == null || anim.layerCount == 0) return;

        // Luôn cho phép "Hurt" chạy lại kể cả khi đang Hurt
        if (currentAnim == animName && animName != "Hurt") return;

        anim.Play(animName, 0, 0f); // phát từ frame đầu
        currentAnim = animName; if (anim == null || anim.layerCount == 0) return; // tránh lỗi layer -1
        if (currentAnim == animName) return;

        anim.Play(animName, 0, 0f); // luôn phát ở Base Layer
        currentAnim = animName;
    }
    
    private void ShootBolt()
{
    GameObject bolt = Instantiate(boltPrefab, boltSpawnPoint.position, Quaternion.identity);

    // Lấy script projectile và truyền hướng bắn
    BoltScript bp = bolt.GetComponent<BoltScript>();
    bp.SetDirection(facingDirection);
    bp.damage += BuffManager.Instance.bonusBoltDamage;
    bp.damage = BuffManager.Instance.GetBoltDamage();
}

private void TryShootBolt()
{
    // kiểm tra cooldown
    if (Time.time < lastBoltTime + boltCooldown)
    {
        Debug.Log("⚠ Bolt chưa hồi!");
        return;
    }

    ShootBolt();            // bắn đạn
    lastBoltTime = Time.time; // bắt đầu tính hồi chiêu
}

    private void Knockback()
    {
        // Hướng ngược lại hướng đang quay
        int knockDir = -facingDirection;

        // Tạo lực bật lùi nhẹ
        float knockForceX = 9f;  // điều chỉnh độ bật lùi ngang
        float knockForceY = 3f;  // độ nảy lên nhẹ (tùy thích)

        rb.velocity = new Vector2(knockDir * knockForceX, knockForceY);
    }
    public int GetCurrentHealth()
{
    return currentHealth;
}

private void SpawnBloodEffect(int dir)
{
    if (bloodEffectPrefab == null) return;

    // tính vị trí xuất hiện
    Vector3 pos = transform.position + new Vector3(1.2f * dir, 0.5f, 0);

    GameObject fx = Instantiate(bloodEffectPrefab, pos, Quaternion.identity);

    // xoay theo hướng bị đánh
    fx.transform.localScale = new Vector3(dir, 1, 1);

    // cho effect đi theo Player vài frame
    fx.transform.SetParent(transform);

    Destroy(fx, 0.35f);
}
public void Heal(int amount)
{
    currentHealth += amount;
    if (currentHealth > maxHealth)
        currentHealth = maxHealth;

    if (GameManager.Instance != null)
        GameManager.Instance.playerHealth = currentHealth;

    if (healthUI != null)
        healthUI.UpdateHealth(currentHealth);
}


public float GetLastBoltTime()
{
    return lastBoltTime;
}
    

}


