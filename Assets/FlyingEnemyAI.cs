using UnityEngine;

public class FlyingEnemyAI : MonoBehaviour
{
    [Header("Stats")]
    public int maxHealth = 20;
    public int damage = 2;
    public float speed = 3f;
    public float lifeTime = 6f;   // tự biến mất

    [Header("Animation")]
    public Animator anim;

    [HideInInspector] public Transform target;

    private int currentHealth;
    private bool isDead = false;

    void Start()
    {
        currentHealth = maxHealth;

        if (anim != null)
            anim.SetBool("isRun", true); // bay ngay từ đầu

        // tự biến mất sau lifeTime giây
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        if (isDead || target == null) return;

        // đuổi theo người chơi
        Vector2 dir = (target.position - transform.position).normalized;
        transform.position += (Vector3)dir * speed * Time.deltaTime;

        FlipToPlayer();
    }

    void FlipToPlayer()
    {
        if (target == null) return;

        if (target.position.x > transform.position.x)
            transform.localScale = new Vector3(-1, 1, 1); // nhìn phải
        else
            transform.localScale = new Vector3(1, 1, 1);  // nhìn trái

    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (isDead) return;

        if (col.CompareTag("Player"))
        {
            PlayerMovement p = col.GetComponent<PlayerMovement>();
            if (p != null)
                p.TakeDamage(damage, transform.position);

            // chơi animation táp
            anim.SetTrigger("Attack");

            // KHÔNG chết khi tấn công
        }
    }

    public void TakeDamage(int dmg)
    {
        if (isDead) return;

        currentHealth -= dmg;

        if (currentHealth <= 0)
            Die();
    }

    void Die()
    {
        if (isDead) return;
        isDead = true;

        anim.SetBool("isRun", false);
        anim.SetTrigger("Die");

        // delay để anim chết chạy xong
        Destroy(gameObject, 0.8f);
    }
}
