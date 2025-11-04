using UnityEngine;
using UnityEngine.UI;

public class BossHealthUI : MonoBehaviour
{
    [Header("UI References")]
    public Slider bossSlider;
    public GameObject bossHealthUI; // parent UI
    public GameObject bossHealthBackground; // nền đằng sau
    public GameObject bossNameUI; // tên boss

    public void InitHealth(int maxHP)
    {
        bossSlider.maxValue = maxHP;
        bossSlider.value = maxHP;

        bossHealthUI.SetActive(false);
        bossHealthBackground.SetActive(false);
        bossNameUI.SetActive(false);
    }

    public void ActivateBossHealth()
    {
        bossHealthUI.SetActive(true);
        bossHealthBackground.SetActive(true);
        bossNameUI.SetActive(true);
    }

    public void UpdateHealth(int currentHP)
    {
        bossSlider.value = currentHP;
    }

    public void HideUI()
    {
        bossHealthUI.SetActive(false);
        bossHealthBackground.SetActive(false);
        bossNameUI.SetActive(false);
    }
}
