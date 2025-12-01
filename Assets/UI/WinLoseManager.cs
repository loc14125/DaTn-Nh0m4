using UnityEngine;
using UnityEngine.SceneManagement;

public class WinLoseManager : MonoBehaviour
{
    public static WinLoseManager Instance;

    [Header("UI Panels")]
    public GameObject losePanel;
    public GameObject winPanel;

    [Header("External Managers")]
    public ScoreManager scoreManager;
    public PlayTimeTimer timeManager;

    private bool hasBossLevel = false;  // level hiện tại có boss hay không
    private bool handled = false;       // tránh lặp win/lose

    private void Awake()
    {
        // Singleton
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // Lắng nghe khi scene load để tự tìm lại reference
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        DetectManagers();
        DetectBoss();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        handled = false;

        // Reset lại UI
        if (losePanel != null) losePanel.SetActive(false);
        if (winPanel != null) winPanel.SetActive(false);

        DetectManagers();
        DetectBoss();
    }

    private void DetectManagers()
    {
        // Tự tìm lại ScoreManager và TimeManager trong scene mới
        if (scoreManager == null)
            scoreManager = FindObjectOfType<ScoreManager>();

        if (timeManager == null)
            timeManager = FindObjectOfType<PlayTimeTimer>();
    }

    private void DetectBoss()
    {
        // Level có Boss nếu GameObject tên "Boss" tồn tại ngay khi vào scene
        GameObject boss = GameObject.Find("Boss");
        hasBossLevel = (boss != null);
    }

    void Update()
    {
        if (handled) return;

        // ============================
        // 1. PLAYER CHẾT → LOSE
        // ============================
        GameObject player = GameObject.FindWithTag("Player");
        if (player == null)
        {
            TriggerLose();
            handled = true;
            return;
        }

        // ============================
        // 2. LEVEL KHÔNG CÓ BOSS → KHÔNG CHECK WIN
        // ============================
        if (!hasBossLevel) return;

        // ============================
        // 3. LEVEL CÓ BOSS → CHECK WIN
        // ============================
        GameObject boss = GameObject.Find("Boss");
        if (boss == null)
        {
            TriggerWin();
            handled = true;
            return;
        }
    }

    // ============================================================
    //                        LOSE
    // ============================================================
    public void TriggerLose()
    {
        if (losePanel != null)
        {
            losePanel.SetActive(true);

            string finalTime = timeManager != null ? timeManager.timeText.text : "00:00";
            int finalScore = scoreManager != null ? scoreManager.score : 0;

            LosePanelUI ui = losePanel.GetComponent<LosePanelUI>();
            ui.SetupLosePanel(finalTime, finalScore);
        }
    }

    // ============================================================
    //                        WIN
    // ============================================================
    public void TriggerWin()
    {
        if (winPanel != null)
        {
            winPanel.SetActive(true);

            string finalTime = timeManager != null ? timeManager.timeText.text : "00:00";
            int finalScore = scoreManager != null ? scoreManager.score : 0;

            WinPanelUI ui = winPanel.GetComponent<WinPanelUI>();
            ui.SetupWinPanel(finalTime, finalScore);
        }
    }

    // ============================================================
    //                 EXIT → DESTROY SINGLETON
    // ============================================================
    public void ExitGame()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;

        Instance = null;
        Destroy(gameObject);
        Destroy(GameObject.Find("GameManagerTimerScore"));

        SceneManager.LoadScene("MainMenu");
    }
}
