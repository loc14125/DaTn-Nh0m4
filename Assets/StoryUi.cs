using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class StoryUi : MonoBehaviour
{
    public GameObject storyPanel;
    public TextMeshProUGUI storyText;

    [TextArea(3, 5)]
    public string[] storyLines;

    private int index = 0;

    void Start()
    {
        storyPanel.SetActive(true);
        storyText.text = storyLines[index];

        // Nếu muốn khóa player lúc đọc story
        Time.timeScale = 0f;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
        {
            NextLine();
        }
    }

    void NextLine()
    {
        index++;

        if (index < storyLines.Length)
        {
            storyText.text = storyLines[index];
        }
        else
        {
            EndStory();
        }
    }

    void EndStory()
    {
        storyPanel.SetActive(false);
        Time.timeScale = 1f;
        Destroy(gameObject); // không cần nữa
    }
}