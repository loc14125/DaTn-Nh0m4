using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    public float cooldown = 30f;
    private float nextUse = 0f;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            TryTimeStop();
        }
    }

    void TryTimeStop()
    {
        if (Time.time < nextUse)
        {
            Debug.Log("Time stop cooldown!");
            return;
        }

        nextUse = Time.time + cooldown;
        StartCoroutine(FreezeAllEnemies(5f)); // thời gian dừng 5 giây
    }

    IEnumerator FreezeAllEnemies(float duration)
    {
        EnemyAI[] enemies = FindObjectsOfType<EnemyAI>();

        foreach (var e in enemies)
            e.enabled = false;

        yield return new WaitForSeconds(duration);

        foreach (var e in enemies)
            e.enabled = true;
    }
}