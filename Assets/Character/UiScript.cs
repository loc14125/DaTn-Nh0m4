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

    private PlayerMovement player;

    void Start()
    {
        player = FindFirstObjectByType<PlayerMovement>();

        dashCooldownFill.fillAmount = 0;
        boltCooldownFill.fillAmount = 0;
    }

    void Update()
    {
        UpdateDashUI();
        UpdateBoltUI();
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
}
