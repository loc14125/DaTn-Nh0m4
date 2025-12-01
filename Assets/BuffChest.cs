using UnityEngine;

public class BuffChest : MonoBehaviour
{
    public GameObject interactUI;   // Text "Nhấn F để mở"
    public UiBuff buffUI;           // Canvas buff
    private bool playerNear = false;
    private bool opened = false;

    void Start()
    {
        interactUI.SetActive(false);
    }

    void Update()
    {
        if (playerNear && !opened && Input.GetKeyDown(KeyCode.F))
        {
            opened = true;
            interactUI.SetActive(false);
            buffUI.ShowBuffs();

            // Khi nhân vật nhận buff, chest này biến mất
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (opened) return;

        if (collision.CompareTag("Player"))
        {
            playerNear = true;
            interactUI.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerNear = false;
            interactUI.SetActive(false);
        }
    }
}
