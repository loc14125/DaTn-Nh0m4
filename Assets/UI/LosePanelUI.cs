using UnityEngine;
using TMPro;

public class LosePanelUI : MonoBehaviour
{
    public TMP_Text timeText;
    public TMP_Text scoreText;

    public void SetupLosePanel(string time, int score)
    {
        timeText.text = "Time: " + time;
        scoreText.text = "Score: " + score;
    }

    public void ExitButton()
    {
        WinLoseManager.Instance.ExitGame();
    }
}
