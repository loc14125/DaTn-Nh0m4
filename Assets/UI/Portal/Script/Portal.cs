using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class Portal : MonoBehaviour
{
    public GameObject uiPanel;              // Panel Yes/No
    public GameObject uiPanelEnemy;         // Panel báo còn quái
    public Text enemyCountText;             // Text hiển thị số quái

    public float cooldown = 5f;
    public MonoBehaviour movementScript;
    [SerializeField] Animator TransitionAnim;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            int enemyCount = GetEnemyCount();

            if (enemyCount > 0)
            {
                // Còn quái → hiện panel enemy
                uiPanel.SetActive(false);
                uiPanelEnemy.SetActive(true);
                enemyCountText.text = "Còn lại " + enemyCount + " kẻ địch";
            }
            else
            {
                // Hết quái → hiện panel Yes/No
                uiPanelEnemy.SetActive(false);
                uiPanel.SetActive(true);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            uiPanel.SetActive(false);
            uiPanelEnemy.SetActive(false);
        }
    }

    // Gọi khi chọn "Yes"
    public void OnYesClicked()
    {
        uiPanel.SetActive(false);
        movementScript.enabled = false;
        StartCoroutine(LoadLevel());
    }

    int GetEnemyCount()
    {
        return GameObject.FindGameObjectsWithTag("Enemy").Length;
    }

    IEnumerator LoadLevel()
    {
        TransitionAnim.SetTrigger("End");
        yield return new WaitForSeconds(2);
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex + 1);
        TransitionAnim.SetTrigger("Start");
    }
}
