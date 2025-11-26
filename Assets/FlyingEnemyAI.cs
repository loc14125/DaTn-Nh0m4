using UnityEngine;

public class FlyingEnemyAI : MonoBehaviour
{
    [Header("Stats")]
    public int maxHealth = 20;
    public int damage = 5;
    public float speed = 3f;

    [HideInInspector] public Transform target;

    private int currentHealth;

    void Start()
    {
        currentHealth = maxHealth;
    }

    void Update()
    {
        if (target == null) return;

        Vector2 dir = (target.position - transform.position).normalized;
        transform.position += (Vector3)dir * speed * Time.deltaTime;
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            PlayerMovement p = col.GetComponent<PlayerMovement>();
            if (p != null)
                p.TakeDamage(damage, transform.position);
        }
    }

    public void TakeDamage(int dmg)
    {
        currentHealth -= dmg;
        if (currentHealth <= 0)
            Destroy(gameObject);
    }
}
