using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerLevelSystem : MonoBehaviour
{
    [Header("Level")]
    public int level = 1;
    public int currentExp = 0;
    public int expToNextLevel = 100;

    [Header("UI")]
    public Slider expSlider;
    public TextMeshProUGUI levelText;

    [Header("Buff UI")]
    public UiBuff uiBuff; // UI chọn buff khi lên cấp

    void Start()
    {
        UpdateUI();
    }

    public void AddExp(int amount)
    {
        currentExp += amount;

        while (currentExp >= expToNextLevel)
        {
            currentExp -= expToNextLevel;
            LevelUp();
        }

        UpdateUI();
    }

    void LevelUp()
    {
        level++;
        expToNextLevel += 50; // mỗi level cần nhiều exp hơn

        Debug.Log("LEVEL UP! → Lv " + level);

        // Hiện UI buff
        if (uiBuff != null)
            uiBuff.ShowBuffs();
    }

    void UpdateUI()
    {
        if (levelText != null)
            levelText.text = "Lv " + level;

        if (expSlider != null)
        {
            expSlider.maxValue = expToNextLevel;
            expSlider.value = currentExp;
        }
    }
    void Awake()
{
    DontDestroyOnLoad(gameObject);
}

}
