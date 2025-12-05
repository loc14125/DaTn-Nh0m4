using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiScript : MonoBehaviour
{
    [Header("Dash UI")]
    public Image dashCooldownFill;

    [Header("Bolt UI")]
    public Image boltCooldownFill;

    [Header("Thunder UI")]
    public Image thunderCooldownFill;

    private PlayerMovement player;
    private ThunderScript thunder;

    void Start()
    {
        player = FindFirstObjectByType<PlayerMovement>();
        thunder = FindFirstObjectByType<ThunderScript>();

        dashCooldownFill.fillAmount = 0;
        boltCooldownFill.fillAmount = 0;

        if (thunderCooldownFill != null)
            thunderCooldownFill.fillAmount = 0;
    }

    void Update()
    {
        UpdateDashUI();
        UpdateBoltUI();
        UpdateThunderUI();
    }

    void UpdateDashUI()
    {
        float cd = player.dashCooldown;
        float used = Time.time - player.lastDashTime;

        dashCooldownFill.fillAmount = 1 - Mathf.Clamp01(used / cd);
    }

    void UpdateBoltUI()
    {
        float cd = player.boltCooldown;
        float used = Time.time - player.GetLastBoltTime();

        boltCooldownFill.fillAmount = 1 - Mathf.Clamp01(used / cd);
    }

    void UpdateThunderUI()
    {
        if (thunder == null) return;

        // phần trăm hồi chiêu (0 = sẵn sàng, 1 = đang hồi)
        float p = thunder.GetCooldownPercent();

        thunderCooldownFill.fillAmount = p;
    }
}
