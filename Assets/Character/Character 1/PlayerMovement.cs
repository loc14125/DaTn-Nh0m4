using UnityEngine;

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

    private Rigidbody2D _rb;
    private Animator _anim;
    private bool isGrounded;
    private bool isAttacking;
    private bool isDashing;
    private int jumpCount;
    private float inputX;
    private string currentAnim;
    private float lastDashTime;
    private float originalGravity;

    private int facingDirection = 1; // ✅ 1 = phải, -1 = trái

    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _anim = GetComponent<Animator>();
        originalGravity = _rb.gravityScale;
    }

    void Update()
    {
        inputX = Input.GetAxisRaw("Horizontal");
        bool wasGrounded = isGrounded;
        isGrounded = CheckGround();

        if (isGrounded && !wasGrounded)
        {
            jumpCount = 0;
        }

        if (!isAttacking && !isDashing)
        {
            Move();
            if (Input.GetKeyDown(KeyCode.Space))
                Jump();
        }

        if (Input.GetKeyDown(KeyCode.J) && !isDashing)
            Attack();

        if (Input.GetKeyDown(KeyCode.L))
            TryDash();

        HandleAirAnimations();
    }

    private void Move()
    {
        _rb.velocity = new Vector2(inputX * moveSpeed, _rb.velocity.y);

        // 🔹 Xác định hướng mặt nhân vật
        if (Mathf.Abs(inputX) > 0.1f)
        {
            facingDirection = inputX > 0 ? 1 : -1; // ✅ cập nhật hướng
            transform.rotation = Quaternion.Euler(0, facingDirection == 1 ? 0 : 180, 0);
        }

        if (isGrounded && !isAttacking)
        {
            if (Mathf.Abs(inputX) > 0.1f)
                ChangeAnimation("Run");
            else
                ChangeAnimation("Idle");
        }
    }

    private void Jump()
    {
        if (jumpCount < maxJumps)
        {
            _rb.velocity = new Vector2(_rb.velocity.x, jumpForce);
            ChangeAnimation("Jump");
            jumpCount++;
        }
    }

    private void Attack()
    {
        if (isAttacking) return;

        isAttacking = true;
        _rb.velocity = Vector2.zero;
        ChangeAnimation("Attack1");
        Invoke(nameof(EndAttack), 0.4f);
    }

    private void EndAttack()
    {
        isAttacking = false;
    }

    private bool CheckGround()
    {
        Collider2D hit = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        return hit != null;
    }

    private void HandleAirAnimations()
    {
        if (!isGrounded && !isAttacking && !isDashing)
        {
            if (_rb.velocity.y > 0.1f)
                ChangeAnimation("Jump");
            else if (_rb.velocity.y < -0.1f)
                ChangeAnimation("Fall");
        }
    }

    // 🚀 Dash Logic (sửa hướng theo facingDirection)
    private void TryDash()
    {
        if (Time.time < lastDashTime + dashCooldown) return;

        isDashing = true;
        lastDashTime = Time.time;

        ChangeAnimation("Dash");

        // ✅ Dash theo hướng đang nhìn
        _rb.velocity = new Vector2(facingDirection * dashSpeed, 0f);

        originalGravity = _rb.gravityScale;
        _rb.gravityScale = 0f;

        Invoke(nameof(EndDash), dashDuration);
    }

    private void EndDash()
    {
        isDashing = false;
        _rb.gravityScale = originalGravity;
    }

    private void ChangeAnimation(string animName)
    {
        if (currentAnim == animName) return;
        _anim.Play(animName);
        currentAnim = animName;
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}
