using UnityEngine;

public class NPCInteract : MonoBehaviour
{
    public GameObject pressFText;
    public GameObject interactPanel;
    public GameObject guidePanel;   // 👈 THÊM

    private bool playerNear = false;

    void Start()
    {
        pressFText.SetActive(false);
        interactPanel.SetActive(false);
        guidePanel.SetActive(false); // 👈 THÊM
    }

    void Update()
    {
        if (playerNear && Input.GetKeyDown(KeyCode.F))
        {
            OpenInteractPanel();
        }
    }

    void OpenInteractPanel()
    {
        interactPanel.SetActive(true);
        pressFText.SetActive(false);
        guidePanel.SetActive(false);
        Time.timeScale = 0f;
    }

    // ====== BUTTON FUNCTIONS ======

    // NÚT: XEM HƯỚNG DẪN
    public void OpenGuide()
    {
        interactPanel.SetActive(false);
        guidePanel.SetActive(true);
    }

    // NÚT: QUAY LẠI
    public void BackToInteract()
    {
        guidePanel.SetActive(false);
        interactPanel.SetActive(true);
    }

    // NÚT: THOÁT
    public void ExitAll()
    {
        interactPanel.SetActive(false);
        guidePanel.SetActive(false);
        pressFText.SetActive(true);
        Time.timeScale = 1f;
    }

    // ====== TRIGGER ======

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerNear = true;
            pressFText.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerNear = false;
            pressFText.SetActive(false);
            interactPanel.SetActive(false);
            guidePanel.SetActive(false);
            Time.timeScale = 1f;
        }
    }
}
