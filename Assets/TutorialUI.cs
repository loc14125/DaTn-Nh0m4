using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TutorialUI : MonoBehaviour
{
    public PlayerMovement player;

    public TextMeshProUGUI tutorialText;
    public Button nextButton;
    public Button backButton;

    [TextArea(3, 6)]
    public string[] tutorialContents;

    int index = 0;

    void Start()
    {
        if (player != null)
            player.canControl = false; // khóa player lúc đầu

        UpdateUI();
    }

    public void Next()
    {
        // ✅ LUÔN mở player khi bấm Next
        if (player != null)
            player.canControl = true;

        // nếu còn trang → sang trang tiếp
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

            // (tuỳ chọn) Back thì lại khóa player
            if (player != null)
                player.canControl = false;
        }
    }

    void UpdateUI()
    {
        if (tutorialContents == null || tutorialContents.Length == 0)
        {
            Debug.LogError("❌ tutorialContents đang rỗng!");
            return;
        }

        index = Mathf.Clamp(index, 0, tutorialContents.Length - 1);
        tutorialText.text = tutorialContents[index];

        backButton.gameObject.SetActive(index > 0);

        nextButton.GetComponentInChildren<TextMeshProUGUI>().text =
            (index == tutorialContents.Length - 1) ? "Hoàn thành" : "Tiếp";
    }

    void FinishTutorial()
    {
        if (player != null)
            player.canControl = true;

        gameObject.SetActive(false); // tắt UI
    }
}
