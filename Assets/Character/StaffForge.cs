using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaffForge : MonoBehaviour
{
    public GameObject staffItem;
    private bool hasStaff = false;

    void Start()
    {
        if (PlayerPrefs.GetInt("HasStaff", 0) == 1)
        {
            hasStaff = true;
            staffItem.SetActive(true);
        }
        else
        {
            staffItem.SetActive(false);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            TryForge();
        }
    }

    void TryForge()
    {
        if (hasStaff)
        {
            Debug.Log("⚠ Bạn đã có Quyền Trượng rồi!");
            return;
        }

        if (QuestManager.Instance != null && QuestManager.Instance.CanForgeStaff())
        {
            QuestManager.Instance.UseFragmentsForStaff();
            staffItem.SetActive(true);

            hasStaff = true;
            PlayerPrefs.SetInt("HasStaff", 1);

            Debug.Log("✅ Nhận Quyền Trượng!");
        }
        else
        {
            Debug.Log("❌ Chưa đủ mảnh");
        }
    }
}