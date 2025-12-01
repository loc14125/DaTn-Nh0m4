using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class WinPanelUI : MonoBehaviour
{
    public TMP_Text timeText;
    public TMP_Text scoreText;
    public Button saveButton;

    private int latestScore;
    private string latestTime;

    public void SetupWinPanel(string time, int score)
    {
        latestTime = time;
        latestScore = score;

        timeText.text = "Time: " + time;
        scoreText.text = "Score: " + score;

        // reset UI khi mở win panel
        saveButton.interactable = true;
    }

    public void SaveScore()
    {
        // =======================
        //  LƯU DỮ LIỆU
        // =======================
        PlayerPrefs.SetInt("LastScore", latestScore);
        PlayerPrefs.SetString("LastTime", latestTime);

        if (latestScore > PlayerPrefs.GetInt("BestScore", 0))
            PlayerPrefs.SetInt("BestScore", latestScore);

        PlayerPrefs.SetString("BestTime", latestTime);

        PlayerPrefs.Save();

        // =======================
        //  CẬP NHẬT UI
        // =======================
        saveButton.interactable = false;  // khóa nút save

        // (tùy chọn) đổi text nút Save nếu muốn
        saveButton.GetComponentInChildren<TMP_Text>().text = "Saved";
    }

    public void ExitToMainMenu()
    {
        WinLoseManager.Instance.ExitGame();
    }
}
