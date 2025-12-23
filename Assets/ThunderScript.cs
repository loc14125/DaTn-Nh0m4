using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThunderScript : MonoBehaviour
{
    [Header("Settings")]
    public float radius = 6f;
    public float cooldownHit = 30f;
    public float cooldownMiss = 3f;
    public LayerMask enemyLayer;

    [Header("Effect")]
    public GameObject thunderPrefab;
    public float effectSpawnHeight = 3f;

    private float nextUseTime = 0f;
    

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
            TryCastThunder();
    }

    public void TryCastThunder()
    {
        if (Time.time < nextUseTime)
        {
            Debug.Log("Thunder on cooldown");
            return;
        }

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, radius, enemyLayer);
        foreach (Collider2D col in hits)
{
    Debug.Log("FOUND ENEMY: " + col.name + "  | Layer: " + LayerMask.LayerToName(col.gameObject.layer));
}

        if (hits.Length == 0)
        {
            nextUseTime = Time.time + cooldownMiss;
            Debug.Log("Thunder: no enemies found -> cooldown 3s");
            return;
        }

        nextUseTime = Time.time + cooldownHit;

        int totalDmg = BuffManager.Instance != null ? BuffManager.Instance.GetThunderDamage() : 100;

        foreach (Collider2D col in hits)
        {
            if (col == null) continue;
            GameObject enemyRoot = col.transform.root.gameObject;

            // Spawn effect
            if (thunderPrefab != null)
            {
                Vector3 spawnPos = enemyRoot.transform.position + Vector3.up * effectSpawnHeight;
                Instantiate(thunderPrefab, spawnPos, Quaternion.identity);
            }

            // Mark hit
            enemyRoot.SendMessage("MarkHitByThunder", SendMessageOptions.DontRequireReceiver);

            // ======= GÂY DAMAGE CHUẨN TỪNG LOẠI ENEMY =======
           Boss boss = enemyRoot.GetComponent<Boss>();
            if (boss != null)
            {
                boss.TakeDamage(totalDmg, false, true); // isProjectile = false, isThunder = true
                QuestManager.Instance.AddThunderDamage(totalDmg);              
                continue;
            }

            EnemyAI2 e2 = enemyRoot.GetComponent<EnemyAI2>();
            if (e2 != null)
            {
                e2.TakeDamage(totalDmg);
                QuestManager.Instance.AddThunderDamage(totalDmg);
                continue;
            }

            EnemyAI e = enemyRoot.GetComponent<EnemyAI>();
            if (e != null)
            {
                e.TakeDamage(totalDmg);
                QuestManager.Instance.AddThunderDamage(totalDmg);
                continue;
            }

            BatEnemy2D bat2D = enemyRoot.GetComponent<BatEnemy2D>();
            if (bat2D != null)
            {
                bat2D.TakeDamage(totalDmg);
                QuestManager.Instance.AddThunderDamage(totalDmg);
                continue;
            }

            BatEnemy bat = enemyRoot.GetComponent<BatEnemy>();
            if (bat != null)
            {
                bat.TakeDamage(totalDmg);
                QuestManager.Instance.AddThunderDamage(totalDmg);
                continue;
            }
            EnemyCode enemyCode = enemyRoot.GetComponent<EnemyCode>();
            if (enemyCode != null)
            {
                enemyCode.TakeDamage(totalDmg);
                QuestManager.Instance.AddThunderDamage(totalDmg);
                continue;
            }
            Snake snake = enemyRoot.GetComponent<Snake>();
            if (snake != null)
            {
                snake.TakeDamage(totalDmg);
                QuestManager.Instance.AddThunderDamage(totalDmg);
                continue;
            }
        }

        Debug.Log($"Thunder struck {hits.Length} enemies, dmg {totalDmg}, cooldown {cooldownHit}s");
    }

    public float GetCooldownPercent()
    {
        float remaining = nextUseTime - Time.time;
        float total = (remaining > cooldownMiss && remaining <= cooldownHit) ? cooldownHit : cooldownMiss;
        if (remaining <= 0) return 0;
        return Mathf.Clamp01(remaining / total);
    }

    public float GetCooldownRemaining()
    {
        return Mathf.Max(0f, nextUseTime - Time.time);
    }
    public float GetCooldownMax()
    {
    return cooldownHit;
    }
}