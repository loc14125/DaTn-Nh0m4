using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerLevelSystem : MonoBehaviour
{
    [Header("UI")]
    public Slider expSlider;
    public TextMeshProUGUI levelText;

    [Header("Buff UI")]
    public UiBuff uiBuff;

    PlayerLevelData data;

    void Start()
    {
        data = PlayerLevelData.Instance;

        // ✅ lắng nghe sự kiện lên cấp
        data.OnLevelUp += HandleLevelUp;

        UpdateUI();
    }

    void OnDestroy()
    {
        if (data != null)
            data.OnLevelUp -= HandleLevelUp;
    }

    public void AddExp(int amount)
    {
        data.AddExp(amount);
        UpdateUI(); // ✅ luôn cập nhật thanh EXP
    }

    void HandleLevelUp()
    {
        UpdateUI();
        StartCoroutine(ShowBuffDelay());
    }

    IEnumerator ShowBuffDelay()
    {
        yield return new WaitForSeconds(1f); // ⏱ delay 1 giây

        if (uiBuff != null)
            uiBuff.ShowBuffs();
    }

    void UpdateUI()
    {
        if (levelText != null)
            levelText.text = "Lv " + data.level;

        if (expSlider != null)
        {
            expSlider.maxValue = data.expToNextLevel;
            expSlider.value = data.currentExp;
        }
    }
}