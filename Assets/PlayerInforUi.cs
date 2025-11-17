using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class PlayerInforUi : MonoBehaviour
{
    [Header("UI References")]
    public GameObject panel;
    public Image playerImage;
    public TMP_Text damageText;
    public TMP_Text healthText;

    [Header("Player Reference")]
    public PlayerMovement player; // Gán player trong Inspector

    private bool isVisible = false;

    void Start()
    {
        panel.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            TogglePanel();
        }

        if (isVisible && player != null)
        {
            // ✅ Cập nhật thông tin mỗi frame
            healthText.text = $"Máu: {player.GetCurrentHealth()}";
            damageText.text = $"Damage: {player.playerAttack.GetDamage()}";
        }
    }

    void TogglePanel()
    {
        isVisible = !isVisible;
        panel.SetActive(isVisible);
    }
}