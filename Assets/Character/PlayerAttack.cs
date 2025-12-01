using UnityEngine;
using System.Collections.Generic;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] private GameObject attackHitbox;
    [SerializeField] public int damage = 20;

    private Collider2D hitboxCollider;
    private HashSet<GameObject> damagedEnemies = new HashSet<GameObject>();
    private bool hitboxActiveThisSwing = false;

    void Start()
    {
        hitboxCollider = attackHitbox.GetComponent<Collider2D>();
        hitboxCollider.isTrigger = true;
        attackHitbox.SetActive(false);
    }

    public void EnableHitbox()
    {
         ApplyBuff();
        if (hitboxActiveThisSwing) return;
        hitboxActiveThisSwing = true;

        damagedEnemies.Clear();
        attackHitbox.SetActive(true);

        Collider2D[] hits = Physics2D.OverlapBoxAll(hitboxCollider.bounds.center, hitboxCollider.bounds.size, 0f);

        foreach (Collider2D hit in hits)
        {
            if (!hit.CompareTag("Enemy")) continue;

            GameObject enemyRoot = hit.transform.root.gameObject;
            if (damagedEnemies.Contains(enemyRoot)) continue;

            // === BOSS ===
            Boss boss = enemyRoot.GetComponent<Boss>();
            if (boss != null)
            {
                boss.TakeDamage(damage);
                damagedEnemies.Add(enemyRoot);
                continue;
            }

            // === EnemyAI2 ===
            EnemyAI2 enemy2 = enemyRoot.GetComponent<EnemyAI2>();
            if (enemy2 != null)
            {
                enemy2.TakeDamage(damage);
                damagedEnemies.Add(enemyRoot);
                continue;
            }

            // === EnemyAI (Golem / Slime / Zombie) ===
            EnemyAI enemyAI = enemyRoot.GetComponent<EnemyAI>();
            if (enemyAI != null)
            {
                enemyAI.TakeDamage(damage);
                damagedEnemies.Add(enemyRoot);
                continue;
            }

            // === Bat ===
            BatEnemy bat = enemyRoot.GetComponent<BatEnemy>();
            if (bat != null)
            {
                bat.TakeDamage(damage);
                damagedEnemies.Add(enemyRoot);
                continue;
            }

            
        }
    }

    public void DisableHitbox()
    {
        attackHitbox.SetActive(false);
        hitboxActiveThisSwing = false;
    }

    public int GetDamage()
    {
        return damage;
    }
    public void ApplyBuff()
{
    damage = 20 + BuffManager.Instance.bonusATK; 
}
}
