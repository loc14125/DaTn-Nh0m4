using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Enemy Spawn Settings")]
    public GameObject enemyPrefab;     // Prefab quái
    public Transform[] spawnPoints;    // Các vị trí spawn
    public int maxEnemies = 20;        // Giới hạn tối đa (nếu cần)

    private List<GameObject> spawnedEnemies = new List<GameObject>();

    void Start()
    {
        SpawnEnemiesOnce();
    }

    void SpawnEnemiesOnce()
    {
        if (enemyPrefab == null || spawnPoints.Length == 0)
        {
            Debug.LogWarning("⚠️ Chưa gán Enemy Prefab hoặc Spawn Points!");
            return;
        }

        // Spawn ở tất cả các vị trí
        foreach (Transform point in spawnPoints)
        {
            if (spawnedEnemies.Count >= maxEnemies)
                break;

            GameObject newEnemy = Instantiate(enemyPrefab, point.position, point.rotation);
            spawnedEnemies.Add(newEnemy);
        }

        Debug.Log($"✅ Spawned {spawnedEnemies.Count} enemies tại {spawnPoints.Length} vị trí.");
    }
}
