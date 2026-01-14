using UnityEngine;
using System;

public class PlayerLevelData : MonoBehaviour
{
    public static PlayerLevelData Instance;

    public int level = 1;
    public int currentExp = 0;
    public int expToNextLevel = 100;

    public event Action OnLevelUp; // ✅ báo lên cấp

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void AddExp(int amount)
    {
        currentExp += amount;

        while (currentExp >= expToNextLevel)
        {
            currentExp -= expToNextLevel;
            LevelUp();
        }
    }

    void LevelUp()
    {
        level++;
        expToNextLevel += 50;

        Debug.Log("LEVEL UP → Lv " + level);

        OnLevelUp?.Invoke(); // ✅ báo cho UI
    }
}