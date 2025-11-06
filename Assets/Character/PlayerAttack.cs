using UnityEngine;
using System.Collections.Generic;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] private GameObject attackHitbox;
    [SerializeField] private int damage = 20;

    private Collider2D hitboxCollider;
    private HashSet<GameObject> damagedEnemies = new HashSet<GameObject>();
    private bool hitboxActiveThisSwing = false; // tránh bật nhiều lần trong 1 swing

    void Start()
    {
        hitboxCollider = attackHitbox.GetComponent<Collider2D>();
        hitboxCollider.isTrigger = true;
        attackHitbox.SetActive(false);
    }

    // Animation Event: bắt đầu swing
    public void EnableHitbox()
    {
        if (hitboxActiveThisSwing) return; // chỉ bật 1 lần
        hitboxActiveThisSwing = true;

        damagedEnemies.Clear();  // reset cho swing mới
        attackHitbox.SetActive(true);

        // Gây damage tất cả enemy trong hitbox ngay 1 lần
        Collider2D[] hits = Physics2D.OverlapBoxAll(hitboxCollider.bounds.center, hitboxCollider.bounds.size, 0f);
        foreach (Collider2D hit in hits)
        {
            if (!hit.CompareTag("Enemy")) continue;

            GameObject enemyRoot = hit.transform.root.gameObject;
            if (damagedEnemies.Contains(enemyRoot)) continue;

            Boss boss = enemyRoot.GetComponent<Boss>();
            if (boss != null)
            {
                boss.TakeDamage(damage);
                damagedEnemies.Add(enemyRoot);
            }
        }
    }

    // Animation Event: kết thúc swing
    public void DisableHitbox()
    {
        attackHitbox.SetActive(false);
        hitboxActiveThisSwing = false;
    }
    public int GetDamage()
{
    return damage;
}
}
