using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;   // ⭐ Quan trọng: dùng TextMeshPro
using System.Text;

public class UiQuest : MonoBehaviour
{
    public TextMeshProUGUI questText;

    public void UpdateQuestUI(List<QuestData> quests)
    {
        questText.text = "";

        foreach (var q in quests)
        {
            questText.text += $"{q.questName}: {q.currentValue}/{q.goalValue}\n";
        }
    }
}