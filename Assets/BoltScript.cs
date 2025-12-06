using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoltScript : MonoBehaviour
{
    [Header("Projectile")]
    public float speed = 10f;
    public int damage = 20;
    public float lifeTime = 3f;

    [Header("Hit Effect")]
    [SerializeField] private GameObject hitEffectPrefab; // hiệu ứng khi trúng
    [SerializeField] private float hitAnimDuration = 0.25f; // thời gian chờ trước khi destroy

    private int direction = 1;
    private bool hasHit = false;        // đã va chạm rồi (tránh xử lý nhiều lần)
    private Animator anim = null;
    
    private Rigidbody2D rb = null;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        // Tự hủy sau lifeTime nếu không trúng gì
        Destroy(gameObject, lifeTime);
    }

    public void SetDirection(int dir)
    {
        direction = dir;
        transform.localScale = new Vector3(dir, 1, 1); // lật sprite theo hướng
        
    }

    private void Update()
    {
        // Nếu đã va chạm thì không di chuyển nữa
        if (hasHit) return;

        // Di chuyển projectile theo trục local-right
        transform.Translate(Vector2.right * direction * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Chỉ xử lý khi chạm "Enemy"
        if (!collision.CompareTag("Enemy")) return;

        // Tránh xử lý nhiều lần
        if (hasHit) return;
        hasHit = true;

        // Tạo hiệu ứng hit (prefab) tại vị trí hiện tại
        if (hitEffectPrefab != null)
        {
            Instantiate(hitEffectPrefab, transform.position, Quaternion.identity);
        }

        // Dừng di chuyển ngay lập tức
        speed = 0f;
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            // nếu dùng kinematic, rb sẽ không ảnh hưởng; nhưng set zero cho chắc chắn
        }

        // Play animation HitBolt nếu có animator (Animator phải có state "HitBolt")
        if (anim != null)
        {
            anim.Play("HitBolt", 0, 0f);
        }

        // Gây damage lên các loại enemy theo component (giữ tương thích với project của bạn)
        Boss boss = collision.transform.root.GetComponent<Boss>();
        if (boss != null)
        {
            boss.TakeDamage(damage, true); // báo là projectile
            
        }

        EnemyAI2 e2 = collision.transform.root.GetComponent<EnemyAI2>();
        if (e2 != null)
        {
            e2.TakeDamage(damage);
        }

        EnemyAI golem = collision.transform.root.GetComponent<EnemyAI>();
        if (golem != null)
        {
            golem.TakeDamage(damage);
        }

        BatEnemy bat = collision.transform.root.GetComponent<BatEnemy>();
        if (bat != null)
        {
            bat.TakeDamage(damage);
        }
        EnemyCode enemyCode = collision.transform.root.GetComponent<EnemyCode>();
        if (enemyCode != null)
        {
            enemyCode.TakeDamage(damage);
        }
        

        // Huỷ bolt sau khi animation/hiệu ứng hit kết thúc
        StartCoroutine(DestroyAfterHitAnimation());
    }

    private IEnumerator DestroyAfterHitAnimation()
    {
        // nếu animator có dạng animation ngắn, bạn có thể điều chỉnh thời gian
        yield return new WaitForSeconds(hitAnimDuration);
        Destroy(gameObject);
    }
}