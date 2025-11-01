using UnityEngine;
using System.Collections.Generic;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] private GameObject attackHitbox;
    [SerializeField] private int damage = 20;

    private HashSet<GameObject> damagedEnemies = new HashSet<GameObject>(); // ⚡ đổi từ Collider sang GameObject
    private Collider2D hitboxCollider;

    void Start()
    {
        hitboxCollider = attackHitbox.GetComponent<Collider2D>();
        if (hitboxCollider == null)
            Debug.LogError("❌ AttackHitbox missing Collider2D!");
        else
            hitboxCollider.isTrigger = true;

        attackHitbox.SetActive(false);
    }

    public void EnableHitbox()
    {
        attackHitbox.SetActive(true);
        damagedEnemies.Clear(); // reset danh sách kẻ địch đã trúng đòn
    }

    public void DisableHitbox()
    {
        attackHitbox.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!attackHitbox.activeSelf) return;
        if (!other.CompareTag("Enemy")) return;

        // ⚡ chỉ xử lý 1 lần cho mỗi con boss (theo GameObject cha)
        GameObject enemyRoot = other.transform.root.gameObject;
        if (damagedEnemies.Contains(enemyRoot)) return;

        Boss boss = enemyRoot.GetComponent<Boss>();
        if (boss != null)
        {
            boss.TakeDamage(damage);
            damagedEnemies.Add(enemyRoot);
        }
    }

    private void OnDrawGizmos()
    {
        if (attackHitbox == null) return;
        Collider2D box = attackHitbox.GetComponent<Collider2D>();
        if (box == null) return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(box.bounds.center, box.bounds.size);
    }
}
