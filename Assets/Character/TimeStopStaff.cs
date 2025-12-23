using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class NewBehaviourScript : MonoBehaviour
{
    public float cooldown = 30f;
    public float stopDuration = 5f;

    public StaffCooldownUI cooldownUI; // 👈 GẮN UI VÀO ĐÂY

    private bool canUse = true;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            TryTimeStop();
        }
    }

    void TryTimeStop()
    {
        if (!canUse) return;

        canUse = false;

        if (cooldownUI != null)
            cooldownUI.StartCooldown(cooldown);

        StartCoroutine(TimeStopCoroutine());
        Invoke(nameof(ResetCooldown), cooldown);
    }

    void ResetCooldown()
    {
        canUse = true;
    }

    IEnumerator TimeStopCoroutine()
    {
        ITimeStopable[] targets =
            FindObjectsOfType<MonoBehaviour>().OfType<ITimeStopable>().ToArray();

        foreach (var t in targets)
            t.OnTimeStop(true);

        yield return new WaitForSeconds(stopDuration);

        foreach (var t in targets)
            t.OnTimeStop(false);
    }
}