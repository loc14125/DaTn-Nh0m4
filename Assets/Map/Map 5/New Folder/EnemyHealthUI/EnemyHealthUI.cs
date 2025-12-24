using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthUI : MonoBehaviour
{
    public Slider EnemySlider;
    public void Init(int maxHP)
    {
        EnemySlider.maxValue = maxHP;
        EnemySlider.value = maxHP;
    }

    public void UpdateHealth(int currentHP)
    {
        EnemySlider.value = currentHP;
    }
}
