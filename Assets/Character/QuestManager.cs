using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance;

    [Header("Kill Quest")]
    public int killEnemyAI = 0;      // Giết quái bộ
    public int killBatEnemy = 0;     // Giết quái bay

    public int killAI_Target = 10;   // Mốc thưởng
    public int killBat_Target = 5;

    public bool rewardAI_Given = false;
    public bool rewardBat_Given = false;

    [Header("Damage Quest")]
    public int boltDamage = 0;
    public int thunderDamage = 0;

    public int boltTarget = 300;
    public int thunderTarget = 300;

  

    [Header("Fragments")]
    public int totalFragments = 0;
    public int fragmentsNeeded = 50;

    [Header("UI")]
    public GameObject questUI;
    public TextMeshProUGUI questText;

    private void Awake()
    {
        if (Instance != null && Instance != this)
    {
        Destroy(gameObject);
        return;
    }

    Instance = this;
    DontDestroyOnLoad(gameObject);
        
    }

    private void Start()
    {
        LoadProgress();
        if (questUI != null)
            questUI.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
            ToggleQuestUI();

        UpdateQuestText();
    }

    private void ToggleQuestUI()
    {
        if (questUI != null)
            questUI.SetActive(!questUI.activeSelf);
    }

    private void UpdateQuestText()
    {
        if (questText == null) return;

        questText.text =
            $" Golem: {killEnemyAI}/{killAI_Target}\n" +
            $" Bat: {killBatEnemy}/{killBat_Target}\n\n" +
            $" Bolt Damage: {boltDamage}/{boltTarget}\n" +
            $"Thunder Damage: {thunderDamage}/{thunderTarget}\n\n" +
            $" Mảnh đã có: {totalFragments}/{fragmentsNeeded}";
    }

    // ======================================================
    // KILL QUEST
    // ======================================================

    public void AddKill_EnemyAI()
    {
        killEnemyAI++;
        if (killEnemyAI >= killAI_Target && !rewardAI_Given)
        {
            totalFragments += 3;
            rewardAI_Given = true;
            Debug.Log("Nhận 3 mảnh từ quest giết EnemyAI!");
        }
        SaveProgress();
    }

    public void AddKill_BatEnemy()
    {
        killBatEnemy++;
        if (killBatEnemy >= killBat_Target && !rewardBat_Given)
        {
            totalFragments += 3;
            rewardBat_Given = true;
            Debug.Log("Nhận 3 mảnh từ quest giết BatEnemy!");
        }
        SaveProgress();
    }

    // ======================================================
    // DAMAGE QUEST
    // ======================================================

    public void AddBoltDamage(int dmg)
{
    boltDamage += dmg;

    // Lặp để xử lý trường hợp vượt nhiều lần trong 1 hit
    while (boltDamage >= boltTarget)
    {
        totalFragments += 2;
        Debug.Log($"Nhận 2 mảnh từ Bolt đạt {boltTarget} Damage!");

        boltTarget += 300; // tăng mốc lên 600, rồi 900, 1200...
    }

    SaveProgress();
}

public void AddThunderDamage(int dmg)
{
    thunderDamage += dmg;

    while (thunderDamage >= thunderTarget)
    {
        totalFragments += 2;
        Debug.Log($"Nhận 2 mảnh từ Thunder đạt {thunderTarget} Damage!");

        thunderTarget += 300; // tăng mốc lên 600, rồi 900...
    }

    SaveProgress();
}

    // ======================================================
    // STAFF
    // ======================================================
    public bool CanForgeStaff()
    {
        return totalFragments >= fragmentsNeeded;
    }

    public void UseFragmentsForStaff()
    {
        totalFragments -= fragmentsNeeded;
        Debug.Log("Ghép thành công Quyền Trượng!");
        SaveProgress();
    }

    // ======================================================
    // SAVE & LOAD
    // ======================================================

    public void SaveProgress()
    {
        PlayerPrefs.SetInt("killAI", killEnemyAI);
        PlayerPrefs.SetInt("killBat", killBatEnemy);

        PlayerPrefs.SetInt("boltDmg", boltDamage);
        PlayerPrefs.SetInt("thunderDmg", thunderDamage);

        PlayerPrefs.SetInt("fragments", totalFragments);

        PlayerPrefs.SetInt("rewAI", rewardAI_Given ? 1 : 0);
        PlayerPrefs.SetInt("rewBat", rewardBat_Given ? 1 : 0);
        PlayerPrefs.SetInt("boltTarget", boltTarget);
        PlayerPrefs.SetInt("thunderTarget", thunderTarget);

    }

    public void LoadProgress()
    {
        killEnemyAI = PlayerPrefs.GetInt("killAI", 0);
        killBatEnemy = PlayerPrefs.GetInt("killBat", 0);

        boltDamage = PlayerPrefs.GetInt("boltDmg", 0);
        thunderDamage = PlayerPrefs.GetInt("thunderDmg", 0);

        totalFragments = PlayerPrefs.GetInt("fragments", 0);

        rewardAI_Given = PlayerPrefs.GetInt("rewAI", 0) == 1;
        rewardBat_Given = PlayerPrefs.GetInt("rewBat", 0) == 1;
        boltTarget = PlayerPrefs.GetInt("boltTarget", 300);
        thunderTarget = PlayerPrefs.GetInt("thunderTarget", 300);

    }

    public void ResetQuest()
    {
        killEnemyAI = 0;
        killBatEnemy = 0;

        boltDamage = 0;
        thunderDamage = 0;

        rewardAI_Given = false;
        rewardBat_Given = false;
        SaveProgress();
    }
}