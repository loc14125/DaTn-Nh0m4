using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueUI : MonoBehaviour
{
    public GameObject panel;
    public string[] lines;
    private int index = 0;

    public void StartDialogue()
    {
        panel.SetActive(true);
        Time.timeScale = 0f;
        index = 0;
        ShowLine();
    }

    void Update()
    {
        if (!panel.activeSelf) return;

        if (Input.GetMouseButtonDown(0))
        {
            index++;
            if (index >= lines.Length)
            {
                EndDialogue();
            }
            else
            {
                ShowLine();
            }
        }
    }

    void ShowLine()
    {
        Debug.Log(lines[index]); // bạn thay bằng TextMeshPro
    }

    void EndDialogue()
    {
        panel.SetActive(false);
        Time.timeScale = 1f;
    }
}
