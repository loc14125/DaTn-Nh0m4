using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportTrap : MonoBehaviour
{
    public float stayTime = 1f;   // đứng trong vùng bao lâu thì kích hoạt
    public float lifeTime = 2f;   // trap tồn tại tối đa

    private float stayTimer = 0f;
    private float lifeTimer = 0f;

    private Boss boss;
    private Transform player;

    public void Init(Boss b, Transform p)
    {
        boss = b;
        player = p;
    }

    void Update()
    {
        if (!boss || !player) return;

        // ⏳ đếm thời gian tồn tại
        lifeTimer += Time.deltaTime;
        if (lifeTimer >= lifeTime)
        {
            Destroy(gameObject);
            return;
        }

        // 📍 check player đứng trong vùng
        Vector2 diff = (Vector2)(player.position - transform.position);

        if (diff.sqrMagnitude < 0.6f * 0.6f)
        {
            stayTimer += Time.deltaTime;

            if (stayTimer >= stayTime)
            {
                boss.TeleportAndStrike();
                Destroy(gameObject);
            }
        }
        else
        {
            stayTimer = 0f;
        }
    }
}


