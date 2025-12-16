using UnityEngine;
using TMPro;

public class LeaderboardUI : MonoBehaviour
{
    [Header("Last Score")]
    public TMP_Text lastScoreText;
    public TMP_Text lastTimeText;

    [Header("Top 5 Highscore Rows")]
    public TMP_Text[] rowTexts;  // Row1 → Row5

    void OnEnable()
    {
        LoadLastScore();
        LoadHighscore();
    }

    void LoadLastScore()
    {
        int lastScore = PlayerPrefs.GetInt("LastScore", 0);
        string lastTime = PlayerPrefs.GetString("LastTime", "00:00");

        lastScoreText.text = "Điểm gần nhất: " + lastScore;
        lastTimeText.text = "Thời gian: " + lastTime;
    }

    void LoadHighscore()
    {
        for (int i = 0; i < 5; i++)
        {
            int score = PlayerPrefs.GetInt($"Highscore_{i}_Score", -1);
            string time = PlayerPrefs.GetString($"Highscore_{i}_Time", "00:00");

            if (score < 0)
            {
                rowTexts[i].text = $"{i + 1}. ---";
            }
            else
            {
                rowTexts[i].text = $"{i + 1}. Điểm: {score}   Thời gian: {time}";
            }
        }
    }
}
