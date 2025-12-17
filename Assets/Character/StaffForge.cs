using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaffForge : MonoBehaviour
{
    public GameObject staffItem;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (QuestManager.Instance.CanForgeStaff())
            {
                QuestManager.Instance.UseFragmentsForStaff();
                staffItem.SetActive(true);
                Debug.Log("Bạn đã nhận Quyền Trượng!");
            }
        }
    }
}