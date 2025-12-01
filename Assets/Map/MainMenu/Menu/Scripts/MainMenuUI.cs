using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    public GameObject leaderboardPanel;

    public void PlayGame()
    {
        SceneManager.LoadScene("MAP"); // đổi tên scene gameplay
    }

    public void OpenLeaderboard()
    {
        leaderboardPanel.SetActive(true);
    }

    public void CloseLeaderboard()
    {
        leaderboardPanel.SetActive(false);
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Quit!");
    }
}
