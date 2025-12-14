using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TutorialUI : MonoBehaviour
{
    public TextMeshProUGUI tutorialText;
    public Button nextButton;
    public Button backButton;

    [TextArea(3, 6)]
    public string[] tutorialContents;

    int index = 0;

    void Start()
    {
        UpdateUI();
    }

    public void Next()
    {
        if (index < tutorialContents.Length - 1)
        {
            index++;
            UpdateUI();
        }
        else
        {
            FinishTutorial();
        }
    }

    public void Back()
    {
        if (index > 0)
        {
            index--;
            UpdateUI();
        }
    }

    void UpdateUI()
    {
        tutorialText.text = tutorialContents[index];

        backButton.gameObject.SetActive(index > 0);

        if (index == tutorialContents.Length - 1)
            nextButton.GetComponentInChildren<TextMeshProUGUI>().text = "Hoàn thành";
        else
            nextButton.GetComponentInChildren<TextMeshProUGUI>().text = "Tiếp";
    }

    void FinishTutorial()
    {
        gameObject.SetActive(false);
        // ở đây bạn load map hoặc cho player điều khiển
    }
}
