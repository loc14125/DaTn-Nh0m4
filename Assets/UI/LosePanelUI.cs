using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Linq;

public class LosePanelUI : MonoBehaviour
{
    public TMP_Text timeText;
    public TMP_Text scoreText;

    private int latestScore;
    private string latestTime;

    public void SetupLosePanel(string time, int score)
    {
        latestTime = time;
        latestScore = score;

        timeText.text = "Thời gian: " + time;
        scoreText.text = "Điểm: " + score;
    }

    public void ExitButton()
    {
        // ===============================
        //   1. LƯU LAST SCORE / TIME
        // ===============================
        PlayerPrefs.SetInt("LastScore", latestScore);
        PlayerPrefs.SetString("LastTime", latestTime);

        // ===============================
        //   2. LOAD TOP 5 HIỆN TẠI
        // ===============================
        List<ScoreEntry> list = new List<ScoreEntry>();

        for (int i = 0; i < 5; i++)
        {
            int score = PlayerPrefs.GetInt($"Highscore_{i}_Score", -1);
            string time = PlayerPrefs.GetString($"Highscore_{i}_Time", "99:99");

            if (score >= 0)
                list.Add(new ScoreEntry(score, time));
        }

        // ===============================
        //   3. THÊM ENTRY MỚI
        // ===============================
        list.Add(new ScoreEntry(latestScore, latestTime));

        // ===============================
        //   4. SORT (score ↓, time ↑)
        // ===============================
        list = list
            .OrderByDescending(s => s.score)
            .ThenBy(s => s.GetSeconds())
            .Take(5)
            .ToList();

        // ===============================
        //   5. GHI LẠI TOP 5
        // ===============================
        for (int i = 0; i < list.Count; i++)
        {
            PlayerPrefs.SetInt($"Highscore_{i}_Score", list[i].score);
            PlayerPrefs.SetString($"Highscore_{i}_Time", list[i].timeString);
        }

        PlayerPrefs.Save();

        WinLoseManager.Instance.ExitGame();
    }
}
