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
    public TMP_Text boltDamageText;  // ⭐ THÊM
    public TMP_Text thunderText;

    [Header("Player Reference")]
    public PlayerMovement player;

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
            UpdateStats();
        }
    }

    void TogglePanel()
    {
        isVisible = !isVisible;
        panel.SetActive(isVisible);

        if (isVisible) UpdateStats();
    }

    void UpdateStats()
    {
        if (player == null) return;

        // Máu
        healthText.text = $"Máu: {player.GetCurrentHealth()}";

        // ATK (đánh thường)
        damageText.text = $"Damage: {player.playerAttack.damage}";

        // ⭐ Damage Bolt — thêm vào UI
        boltDamageText.text =  $"Bolt Damage: {player.boltBonusDamage}";
        thunderText.text = $"Thunder: {BuffManager.Instance.GetThunderDamage()}";
    }
}
