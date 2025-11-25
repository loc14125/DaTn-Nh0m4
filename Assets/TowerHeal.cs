using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerHeal : MonoBehaviour
{
    public int healAmount = 50;
    public GameObject interactUI;

    private bool playerInRange = false;
    private PlayerMovement player;
    private bool hasUsed = false; // ⭐ Chỉ cho dùng 1 lần

    void Start()
    {
        if (interactUI != null)
            interactUI.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !hasUsed) // ⭐ chỉ hiện UI nếu chưa dùng
        {
            playerInRange = true;
            player = other.GetComponent<PlayerMovement>();

            if (interactUI != null)
                interactUI.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            player = null;

            if (interactUI != null)
                interactUI.SetActive(false);
        }
    }

    void Update()
    {
        if (playerInRange && !hasUsed && Input.GetKeyDown(KeyCode.F))
        {
            if (player != null)
            {
                player.Heal(healAmount);
            }

            hasUsed = true; // ⭐ đánh dấu đã sử dụng

            if (interactUI != null)
                interactUI.SetActive(false); // ẩn UI sau khi dùng
        }
    }
}
