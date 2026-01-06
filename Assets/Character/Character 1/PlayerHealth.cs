using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthUI : MonoBehaviour
{
    [Header("UI References")]
    public Slider playerSlider;

    public Image baseFill;      // Fill gốc (đỏ)
    public Image overFill;      // Fill máu dư (xanh)

    public GameObject playerHealthUI;
    public GameObject playerHealthBG;

    [Header("Health")]
    public int maxHP = 100;          // máu thật
    public int overMaxHP = 700;      // máu xanh tối đa

    private bool initialized = false;

    public void Init(int maxHP)
    {
        this.maxHP = maxHP;

        if (playerSlider == null)
        {
            Debug.LogError("❌ PlayerHealthUI: playerSlider chưa được gán!");
            return;
        }

        playerSlider.maxValue = maxHP;
        playerSlider.value = maxHP;

        // 🔥 RESET CHẮC CHẮN
        if (baseFill != null) baseFill.fillAmount = 1f;
        if (overFill != null) overFill.fillAmount = 0f;

        if (playerHealthUI != null) playerHealthUI.SetActive(true);
        if (playerHealthBG != null) playerHealthBG.SetActive(true);

        initialized = true;
    }

    public void UpdateHealth(int currentHP)
    {
        // ❌ CHƯA INIT → KHÔNG UPDATE
        if (!initialized || playerSlider == null) return;

        // ❌ KHÔNG CHO HP ÂM
        currentHP = Mathf.Max(0, currentHP);

        // Máu thường
        float normalHP = Mathf.Clamp(currentHP, 0, maxHP);
        playerSlider.value = normalHP;

        // Máu xanh (CHỐT LỖI Ở ĐÂY)
        float overHP = Mathf.Clamp(currentHP - maxHP, 0, overMaxHP);

        if (overFill != null)
            overFill.fillAmount = overHP / overMaxHP;
    }

    public void HideUI()
    {
        if (playerHealthUI != null) playerHealthUI.SetActive(false);
        if (playerHealthBG != null) playerHealthBG.SetActive(false);
    }
}
