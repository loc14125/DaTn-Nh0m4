using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthUI : MonoBehaviour
{
    public Slider playerSlider;

    public void Init(int maxHP)
    {
        playerSlider.maxValue = maxHP;
        playerSlider.value = maxHP;
    }

    public void UpdateHealth(int currentHP)
    {
        playerSlider.value = currentHP;
    }
}
