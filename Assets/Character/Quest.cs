using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class QuestData
{
    public string questName;        // tên nhiệm vụ
    public QuestType questType;     // loại quest
    public int goalValue;           // yêu cầu (vd: kill 10)
    public int currentValue;        // tiến độ hiện tại
    public int rewardPieces;        // số mảnh thưởng nhận được

    public bool completed => currentValue >= goalValue;
    
}