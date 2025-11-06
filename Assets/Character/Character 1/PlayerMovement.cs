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
    [SerializeField] private float dashCooldown = 1f;

    [Header("Health Settings")]
    [SerializeField] private int maxHealth = 100;
    private int currentHealth;
    private bool isInvincible = false;

    [Header("Combat Flags")]
    public bool hasDealtDamage = false; // ✅ để khóa damage mỗi swing

    [Header("References")]
    [SerializeField] public PlayerAttack playerAttack;
    public PlayerHealthUI healthUI;

    private Rigidbody2D rb;
    private Animator anim;
    private bool isGrounded;
    private bool isAttacking;
    private bool isDashing;
    private int jumpCount;
    private float inputX;
    private string currentAnim;
    private float lastDashTime;
    private float originalGravity;
    private int facingDirection = 1;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        originalGravity = rb.gravityScale;

        currentHealth = maxHealth;
        if (healthUI != null) healthUI.Init(maxHealth);
    }

    void Update()
    {
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

    // ✅ Ưu tiên hướng đang nhấn phím
    float inputDir = Input.GetAxisRaw("Horizontal");

    // Nếu người chơi không nhấn phím thì dash theo hướng đang quay mặt
    if (Mathf.Abs(inputDir) > 0.1f)
        facingDirection = inputDir > 0 ? 1 : -1;

    // Bắt đầu dash
    isDashing = true;

    // ✅ Tạm thời bỏ qua va chạm giữa Player và Enemy
    Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Enemy"), true);

    ChangeAnimation("Dash");
    rb.gravityScale = 0f;
    rb.velocity = new Vector2(facingDirection * dashSpeed, 0);

    Invoke(nameof(EndDash), dashDuration);
}

private void EndDash()
{
    isDashing = false;
    rb.gravityScale = originalGravity;

    // ✅ Bật lại va chạm giữa Player và Enemy
    Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Enemy"), false);

    // ✅ Nếu người chơi đang giữ phím di chuyển, tiếp tục hướng đó
    if (Mathf.Abs(Input.GetAxisRaw("Horizontal")) > 0.1f)
        ChangeAnimation("Run");
    else
        ChangeAnimation("Idle");
}


    public void TakeDamage(int dmg)
    {
        if (isInvincible) return;

        currentHealth -= dmg;
        if (healthUI != null) healthUI.UpdateHealth(currentHealth);

        // Nếu máu > 0 thì phát animation Hurt
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
    currentAnim = animName;if (anim == null || anim.layerCount == 0) return; // tránh lỗi layer -1
        if (currentAnim == animName) return;

        anim.Play(animName, 0, 0f); // luôn phát ở Base Layer
        currentAnim = animName;
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
    

}


