using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class Portal : MonoBehaviour
{
    public GameObject uiPanel;          // Panel UI Yes/No
    public float cooldown = 5f;         // Thời gian cooldown
    private bool canUse = true;
    [SerializeField] Animator TransitionAnim;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!canUse) return;

        if (other.CompareTag("Player"))
        {
            // Pause game
            Time.timeScale = 0f;
            uiPanel.SetActive(true);
        }
    }

    // Gọi khi chọn "Yes"
    public void OnYesClicked()
    {
        // Đóng UI
        uiPanel.SetActive(false);

        //Resume game trước khi load scene
        Time.timeScale = 1f;

        //Load scene tiếp theo
        StartCoroutine(LoadLevel());
    }

    // Gọi khi chọn "No"
    public void OnNoClicked()
    {
        // Đóng UI
        uiPanel.SetActive(false);

        // Resume game
        Time.timeScale = 1f;

        // Bắt đầu cooldown
        StartCoroutine(StartCooldown());
    }

    private IEnumerator StartCooldown()
    {
        canUse = false;
        yield return new WaitForSeconds(cooldown);
        canUse = true;
    }

    IEnumerator LoadLevel()
    {
        TransitionAnim.SetTrigger("End");
        yield return new WaitForSeconds(2);
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex + 1);
        TransitionAnim.SetTrigger("Start");
    }
}
