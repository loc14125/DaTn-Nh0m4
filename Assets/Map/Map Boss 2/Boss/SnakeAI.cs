using UnityEngine;
using System.Collections;

public class SnakeAI : MonoBehaviour
{
    [Header("Detect + Attack")]
    public float detectRange = 6f;
    public float attackRange = 1.6f;
    public LayerMask playerLayer;

    [Header("Move")]
    public float moveSpeed = 2.5f;

    [Header("Cooldown")]
    public float attackCooldown = 1.2f;
    private float lastAttack = 0;

    [Header("Hitbox từng đòn")]
    public GameObject hitboxAtk1;
    public GameObject hitboxAtk2;
    public GameObject hitboxAtk3;
    public float hitDuration = 0.25f;

    [Header("HP + Damage")]
    public int maxHP = 120;
    public float damageAtk1 = 10;
    public float damageAtk2 = 16;
    public float damageAtk3 = 25;
    int curHP;

    Rigidbody2D rb;
    Animator anim;
    Transform player;
    bool attacking = false, dead = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        curHP = maxHP;

        hitboxAtk1.SetActive(false);
        hitboxAtk2.SetActive(false);
        hitboxAtk3.SetActive(false);
    }

    void Update()
    {
        if (dead) return;

        DetectPlayer();

        if(player != null && !attacking)
        {
            float dist = Vector2.Distance(transform.position, player.position);

            if (dist > attackRange) Move();
            else Attack();
        }
        else anim.SetBool("Run", false);

        FacePlayer();
    }

    void DetectPlayer()
    {
        Collider2D hit = Physics2D.OverlapCircle(transform.position, detectRange, playerLayer);
        if(hit) player = hit.transform;
    }

    void Move()
    {
        anim.SetBool("Run", true);
        Vector2 dir = (player.position - transform.position).normalized;
        rb.velocity = new Vector2(dir.x * moveSpeed, rb.velocity.y);
    }

    void FacePlayer()
    {
        if(!player) return;
        transform.localScale = new Vector3(player.position.x > transform.position.x ? 1 : -1, 1, 1);
    }

    void Attack()
    {
        if(Time.time < lastAttack + attackCooldown) return;

        lastAttack = Time.time;
        StartCoroutine(AttackFlow());
    }

    IEnumerator AttackFlow()
    {
        attacking = true;
        rb.velocity = Vector2.zero;

        int atk = Random.Range(1,4); // 1 - 3
        anim.SetTrigger("Atk" + atk);

        yield return new WaitForSeconds(0.35f);
        ActivateHitbox(atk);

        yield return new WaitForSeconds(0.6f);
        attacking = false;
    }

    void ActivateHitbox(int id)
    {
        if(id == 1) StartCoroutine(EnableHitbox(hitboxAtk1));
        if(id == 2) StartCoroutine(EnableHitbox(hitboxAtk2));
        if(id == 3) StartCoroutine(EnableHitbox(hitboxAtk3));
    }

    IEnumerator EnableHitbox(GameObject hb)
    {
        hb.SetActive(true);
        yield return new WaitForSeconds(hitDuration);
        hb.SetActive(false);
    }

    //================== DAMAGE RECEIVE ==================

    public void TakeDamage(int dmg)
    {
        if (dead) return;

        curHP -= dmg;
        anim.SetTrigger("Hurt");

        if(curHP <= 0) Die();
    }

    void Die()
    {
        dead = true;
        rb.velocity = Vector2.zero;

        anim.SetTrigger("Die");
        Destroy(gameObject, 1.3f);
    }

    // GIZMO
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectRange);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
