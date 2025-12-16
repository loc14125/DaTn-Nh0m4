using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

public class WinPanelUI : MonoBehaviour
{
    public TMP_Text timeText;
    public TMP_Text scoreText;
    //public Button saveButton;

    private int latestScore;
    private string latestTime;

    public void SetupWinPanel(string time, int score)
    {
        latestTime = time;
        latestScore = score;

        timeText.text = "Thời gian: " + time;
        scoreText.text = "Điểm: " + score;

        // Reset UI mỗi lần hiện Win Panel
        //saveButton.interactable = true;
    }

    //public void SaveScore()
    //{
    //    // ===============================
    //    //   1. LƯU LAST SCORE / TIME
    //    // ===============================
    //    PlayerPrefs.SetInt("LastScore", latestScore);
    //    PlayerPrefs.SetString("LastTime", latestTime);

    //    // ===============================
    //    //   2. LOAD TOP 5 HIỆN TẠI
    //    // ===============================
    //    List<ScoreEntry> list = new List<ScoreEntry>();

    //    for (int i = 0; i < 5; i++)
    //    {
    //        int score = PlayerPrefs.GetInt($"Highscore_{i}_Score", -1);
    //        string time = PlayerPrefs.GetString($"Highscore_{i}_Time", "99:99");

    //        if (score >= 0)
    //            list.Add(new ScoreEntry(score, time));
    //    }

    //    // ===============================
    //    //   3. THÊM ENTRY MỚI
    //    // ===============================
    //    list.Add(new ScoreEntry(latestScore, latestTime));

    //    // ===============================
    //    //   4. SORT (score ↓, time ↑)
    //    // ===============================
    //    list = list
    //        .OrderByDescending(s => s.score)
    //        .ThenBy(s => s.GetSeconds())
    //        .Take(5)
    //        .ToList();

    //    // ===============================
    //    //   5. GHI LẠI TOP 5
    //    // ===============================
    //    for (int i = 0; i < list.Count; i++)
    //    {
    //        PlayerPrefs.SetInt($"Highscore_{i}_Score", list[i].score);
    //        PlayerPrefs.SetString($"Highscore_{i}_Time", list[i].timeString);
    //    }

    //    PlayerPrefs.Save();

    //    // ===============================
    //    //   6. UPDATE UI
    //    // ===============================
    //    saveButton.interactable = false;
    //    saveButton.GetComponentInChildren<TMP_Text>().text = "Saved";
    //}

    public void ExitToMainMenu()
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

public class ScoreEntry
{
    public int score;
    public string timeString;

    public ScoreEntry(int s, string t)
    {
        score = s;
        timeString = t;
    }

    // chuyển "MM:SS" → tổng giây
    public int GetSeconds()
    {
        string[] p = timeString.Split(':');
        int m = int.Parse(p[0]);
        int s = int.Parse(p[1]);
        return m * 60 + s;
    }
}
