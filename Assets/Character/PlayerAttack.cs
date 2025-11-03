using UnityEngine;
using System.Collections.Generic;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] private GameObject attackHitbox;
    [SerializeField] private int damage = 20;
    public int Damage => damage;


    private HashSet<GameObject> damagedEnemies = new HashSet<GameObject>();
    private Collider2D hitboxCollider;
    private PlayerMovement player;

    void Start()
    {
        player = GetComponentInParent<PlayerMovement>();
        hitboxCollider = attackHitbox.GetComponent<Collider2D>();
        hitboxCollider.isTrigger = true;
        attackHitbox.SetActive(false);
    }

    public void EnableHitbox()
    {
        attackHitbox.SetActive(true);
        damagedEnemies.Clear();
        player.hasDealtDamage = false; // ✅ reset khi bật hitbox
    }

    public int GetDamage()
    {
        return damage;
    }


    public void DisableHitbox()
    {
        attackHitbox.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!attackHitbox.activeSelf) return;
        if (!other.CompareTag("Enemy")) return;
        if (player.hasDealtDamage) return; // ✅ chặn spam frame hit

        GameObject enemyRoot = other.transform.root.gameObject;
        if (damagedEnemies.Contains(enemyRoot)) return;

        Boss boss = enemyRoot.GetComponent<Boss>();
        if (boss != null)
        {
            boss.TakeDamage(damage);
            damagedEnemies.Add(enemyRoot);
            player.hasDealtDamage = true; // ✅ chỉ 1 hit / swing
        }
    }
}
