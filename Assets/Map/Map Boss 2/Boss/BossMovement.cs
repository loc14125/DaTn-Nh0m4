using UnityEngine;

public class BossMovement : MonoBehaviour
{
    public Transform player;

    // ------------------- ACTIVATE RANGE -------------------
    public float activateRange = 12f; // Player phải vào gần mới kích hoạt boss
    private bool isActivated = false;

    // ------------------- FOLLOW -------------------
    public float followRange = 10f;   
    public float speed = 3f;

    // ------------------- TELEPORT -------------------
    public float teleportDistance = 20f;
    public float verticalLimit = 8f;
    public float teleportCooldown = 3f;
    private float teleportTimer = 0f;

    private Rigidbody2D rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        float distance = Vector2.Distance(transform.position, player.position);
        float verticalDiff = Mathf.Abs(transform.position.y - player.position.y);

        // =========================
        //     ACTIVATE BOSS
        // =========================
        if (!isActivated)
        {
            if (distance < activateRange)
                isActivated = true;

            return; // ❗ Boss chưa kích hoạt -> không follow, không teleport
        }

        // =========================
        //       FOLLOW PLAYER
        // =========================
        if (distance < followRange)
        {
            Vector2 dir = (player.position - transform.position).normalized;
            rb.velocity = new Vector2(dir.x * speed, rb.velocity.y);
        }
        else
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
        }

        // =========================
        //     TELEPORT CONDITION
        // =========================
        bool tooFar = distance > teleportDistance;
        bool tooHigh = verticalDiff > verticalLimit;

        if (tooFar || tooHigh)
        {
            teleportTimer += Time.deltaTime;

            if (teleportTimer >= teleportCooldown)
            {
                TeleportToPlayer();
                teleportTimer = 0;
            }
        }
        else
        {
            teleportTimer = 0;
        }
    }

    private void TeleportToPlayer()
    {
        float offsetX = (player.position.x > transform.position.x) ? -1.5f : 1.5f;

        transform.position = new Vector2(
            player.position.x + offsetX,
            player.position.y
        );
    }
}
