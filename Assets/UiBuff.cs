using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiBuff : MonoBehaviour
{
    public GameObject buffPanel;

    public void ShowBuffs()
    {
        buffPanel.SetActive(true);
        Time.timeScale = 0f; // pause game khi chọn buff
    }

    public void ChooseHP()
    {
        BuffManager.Instance.bonusMaxHP += 20;
        Close();
    }

    public void ChooseATK()
    {
        BuffManager.Instance.bonusATK += 5;
        Close();
    }

    public void ChooseBolt()
    {
        BuffManager.Instance.bonusBoltDamage += 10;
        Close();
    }

    void Close()
    {
        buffPanel.SetActive(false);
        Time.timeScale = 1f;
    }
}
