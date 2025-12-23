using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StaffCooldownUI : MonoBehaviour
{
    public Image cooldownFill;

    private float cooldownTime;
    private float timer;
    private bool isCooling = false;

    void Update()
    {
        if (!isCooling) return;

        timer -= Time.deltaTime;
        cooldownFill.fillAmount = timer / cooldownTime;

        if (timer <= 0)
        {
            cooldownFill.fillAmount = 0;
            isCooling = false;
        }
    }

    public void StartCooldown(float cd)
    {
        cooldownTime = cd;
        timer = cd;
        isCooling = true;
        cooldownFill.fillAmount = 1f;
    }

    public bool IsCooling()
    {
        return isCooling;
    }
}