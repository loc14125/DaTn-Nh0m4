using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] private GameObject attackHitbox;
    [SerializeField] private int damage = 20;

    private HashSet<Collider2D> damagedEnemies = new HashSet<Collider2D>();
    private Collider2D hitboxCollider;

    void Start()
    {
        hitboxCollider = attackHitbox.GetComponent<Collider2D>();
        if (hitboxCollider == null)
        {
            Debug.LogError("❌ AttackHitBox is missing a Collider2D!");
        }

        attackHitbox.SetActive(false);
    }

    public void EnableHitbox()
    {
        attackHitbox.SetActive(true);
        damagedEnemies.Clear();
    }

    public void DisableHitbox()
    {
        attackHitbox.SetActive(false);
    }

    private void Update()
    {
        if (attackHitbox.activeSelf)
        {
            CheckHitEnemies();
        }
    }

    private void CheckHitEnemies()
    {
        // ✅ Quét collider trong vùng hitbox
        Collider2D[] hits = Physics2D.OverlapBoxAll(
            hitboxCollider.bounds.center,
            hitboxCollider.bounds.size,
            0f
        );

        foreach (var enemyCollider in hits)
        {
            if (!enemyCollider.CompareTag("Enemy")) continue;
            if (damagedEnemies.Contains(enemyCollider)) continue;

            Enemy enemy = enemyCollider.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
                damagedEnemies.Add(enemyCollider);
            }
        }
    }

    // 🔹 Vẽ hitbox ra Scene để dễ debug
    private void OnDrawGizmos()
    {
        if (attackHitbox == null) return;
        Collider2D box = attackHitbox.GetComponent<Collider2D>();
        if (box == null) return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(box.bounds.center, box.bounds.size);
    }
}