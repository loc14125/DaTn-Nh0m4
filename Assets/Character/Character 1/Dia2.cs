using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class Dia2 : MonoBehaviour
{
    [Header("UI")]
    public GameObject panel;
    public TextMeshProUGUI dialogueText;

    [Header("Choice Buttons")]
    public Button choiceAButton;
    public Button choiceBButton;
    public TextMeshProUGUI choiceAText;
    public TextMeshProUGUI choiceBText;

    private bool dialogueFinished = false;
    [Header("NPC Dialogue")]
    [TextArea(2, 4)]
    public string[] npcLines;

    [Header("Typing Effect")]
    public float typingSpeed = 0.04f;

    private int npcIndex = 0;
    private bool isTyping = false;
    private bool waitingForChoice = false;
    private Coroutine typingCoroutine;

    // ================= START =================
    public void StartDialogue()
    {
        
        panel.SetActive(true);
        Time.timeScale = 0f;

        npcIndex = 0;
        waitingForChoice = false;
        dialogueFinished = false;
        HideChoices();

        StartTyping(npcLines[npcIndex]);
    }

    // ================= UPDATE =================
    void Update()
{
    if (!panel.activeSelf) return;

    if (Input.GetMouseButtonDown(0))
    {
        // Nếu đang gõ → tua chữ
        if (isTyping)
        {
            StopTyping();
            return;
        }

        // Nếu đã kết thúc → thoát hội thoại
        if (dialogueFinished)
        {
            EndDialogue();
            return;
        }

        // Nếu đang chờ chọn → không xử lý click
        if (waitingForChoice) return;

        npcIndex++;

        if (npcIndex < npcLines.Length)
        {
            StartTyping(npcLines[npcIndex]);
        }
        else
        {
            ShowChoices();
        }
    }
}

    // ================= TYPING =================
    void StartTyping(string text)
    {
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        typingCoroutine = StartCoroutine(TypeText(text));
    }

    IEnumerator TypeText(string text)
    {
        isTyping = true;
        dialogueText.text = "";

        foreach (char c in text)
        {
            dialogueText.text += c;
            yield return new WaitForSecondsRealtime(typingSpeed);
        }

        isTyping = false;
    }

    void StopTyping()
    {
        StopCoroutine(typingCoroutine);
        dialogueText.text = npcLines[npcIndex];
        isTyping = false;
    }

    // ================= CHOICE =================
    void ShowChoices()
    {
        waitingForChoice = true;

        choiceAButton.gameObject.SetActive(true);
        choiceBButton.gameObject.SetActive(true);

        choiceAText.text = "Có thể sao ?.";
        choiceBText.text = "Nah I'd win.";

        choiceAButton.onClick.RemoveAllListeners();
        choiceBButton.onClick.RemoveAllListeners();

        choiceAButton.onClick.AddListener(ChooseLearnSkill);
        choiceBButton.onClick.AddListener(ChooseLeave);
    }
void ChooseLearnSkill()
{
    HideChoices();
    waitingForChoice = false;
    dialogueFinished = true;

    dialogueText.text = "Đúng rồi sức mạnh của hắn là 1 tầm cao mới , hắn đã chinh phục cả vùng đất này mà , mong ngài sẽ chiến thắng";
}

    void ChooseLeave()
{
    HideChoices();
    waitingForChoice = false;
    dialogueFinished = true;

    dialogueText.text = "Haha, ngài tự tin thế chắc tôi chỉ có thể chúc ngài sẽ chiến thắng trận đấu này";
}

    void HideChoices()
    {
        choiceAButton.gameObject.SetActive(false);
        choiceBButton.gameObject.SetActive(false);
    }

    // ================= END =================
    public void EndDialogue()
    {
        panel.SetActive(false);
        Time.timeScale = 1f;
    }
}
