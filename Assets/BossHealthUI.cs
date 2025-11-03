using UnityEngine;
using UnityEngine.UI;

public class BossHealthUI : MonoBehaviour
{
    public Slider bossSlider;
    public GameObject bossHealthUI;

    public void InitHealth(int maxHP)
    {
        bossSlider.maxValue = maxHP;
        bossSlider.value = maxHP;
        bossHealthUI.SetActive(false);
    }

    public void ActivateBossHealth()
    {
        bossHealthUI.SetActive(true);
    }

    public void UpdateHealth(int currentHP)
    {
        bossSlider.value = currentHP;
    }

    public void HideUI()
    {
        bossHealthUI.SetActive(false);
    }
}
