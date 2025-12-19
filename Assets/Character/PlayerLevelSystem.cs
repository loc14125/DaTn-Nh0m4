using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
        UpdateUI();
    }

    public void AddExp(int amount)
    {
        data.AddExp(amount);
        UpdateUI();

        // Nếu vừa lên cấp thì show buff
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
