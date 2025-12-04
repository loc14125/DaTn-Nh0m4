using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthUI : MonoBehaviour
{
    [Header("UI References")]
    public Slider playerSlider;
    public GameObject playerHealthUI;   // parent UI (nếu có)
    public GameObject playerHealthBG;   // background (nếu có)

    public void Init(int maxHP)
    {
        if (playerSlider == null)
        {
            Debug.LogError("❌ PlayerHealthUI: playerSlider chưa được gán!");
            return;
        }

        playerSlider.maxValue = maxHP;
        playerSlider.value = maxHP;

        if (playerHealthUI != null) playerHealthUI.SetActive(true);
        if (playerHealthBG != null) playerHealthBG.SetActive(true);
    }

    public void UpdateHealth(int currentHP)
    {
        if (playerSlider != null)
            playerSlider.value = currentHP;
        else
            Debug.LogError("❌ PlayerHealthUI: playerSlider = NULL, không cập nhật được HP!");
    }

    public void HideUI()
    {
        if (playerHealthUI != null) playerHealthUI.SetActive(false);
        if (playerHealthBG != null) playerHealthBG.SetActive(false);
    }
}
