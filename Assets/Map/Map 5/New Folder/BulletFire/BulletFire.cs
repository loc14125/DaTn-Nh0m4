using UnityEngine;

public class BulletFire : MonoBehaviour
{
    public float speed = 5f;
    public float zigZagRadius = 2f;   // BÁN KÍNH ZIGZAG (2f)
    public float zigZagSpeed = 8f;
    public float lifeTime = 3f;  // tốc độ lắc
    public int damage = 30;
    private Vector2 direction;
    private Vector2 perpendicular;    // hướng vuông góc
    private Vector3 startPos;
    private float time;

    private bool exploded;

    
    void Update()
    {
        
        time += Time.deltaTime;

        // Tiến thẳng
        Vector3 forwardMove = (Vector3)(direction * speed * time);

        // Zigzag hình sin
        Vector3 zigzag = (Vector3)(perpendicular *
                        Mathf.Sin(time * zigZagSpeed) * zigZagRadius);

        transform.position = startPos + forwardMove + zigzag;
    }

    public void SetDirection(Vector2 dir)
    {
        direction = dir.normalized;
        // ❌ KHÔNG flip sprite ở đây
        perpendicular = new Vector2(-direction.y, direction.x);

        startPos = transform.position;
        time = 0f;
    }

     void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log("Player Hit!");

            // Gây damage
            PlayerMovement playerHealth = collision.GetComponent<PlayerMovement>();

            if (playerHealth != null)
            {
                // Truyền vị trí enemy (nguồn gây damage)
                playerHealth.TakeDamage(damage, transform.position);
            }

            Destroy(gameObject);
        }
        else if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            Destroy(gameObject);
        }
    }

    void OnBecameInvisible()
    {
        Destroy(gameObject);
    }

}
