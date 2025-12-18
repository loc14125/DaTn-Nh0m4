using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class NewBehaviourScript : MonoBehaviour
{
    public float cooldown = 30f;
    public float stopDuration = 5f;

    private float nextUseTime = 0f;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            TryTimeStop();
        }
    }

    void TryTimeStop()
    {
        if (Time.time < nextUseTime)
        {
            Debug.Log("⏳ Skill đang hồi chiêu");
            return;
        }

        StartCoroutine(TimeStopCoroutine());
        nextUseTime = Time.time + cooldown;
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
}}