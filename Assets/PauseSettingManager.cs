using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class SettingUIManager : MonoBehaviour
{
    [Header("Setting Panel")]
    public GameObject settingPanel;

    [Header("VFX Volume")]
    public Slider vfxSlider;
    public TextMeshProUGUI vfxText;
    public GameObject vfxIconOn;
    public GameObject vfxIconOff;
    public AudioSource vfxAudio;

    [Header("Scene Names")]
    public string mainMenuScene = "MainMenu";   // đặt tên theo scene của bạn
    public string levelScene = "LevelSelect";   // scene chọn màn chơi

    bool isOpen = false;

    private void Start()
    {
        settingPanel.SetActive(false);
        vfxSlider.onValueChanged.AddListener(OnVFXVolumeChange);
        OnVFXVolumeChange(vfxSlider.value);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            ToggleSetting();
    }

    // =============== Toggle ESC ===============
    public void ToggleSetting()
    {
        isOpen = !isOpen;
        settingPanel.SetActive(isOpen);
        Time.timeScale = isOpen ? 0 : 1;
    }

    // =============== Back Button ===============
    public void OnBackButton()
    {
        ToggleSetting(); // chỉ đóng panel
    }

    // =============== Home Button ===============
    public void OnHomeButton()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(mainMenuScene);
    }

    // =============== Level Button ===============
    public void OnLevelButton()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(levelScene);
    }

    // =============== Slider Volume VFX ===============
    void OnVFXVolumeChange(float v)
    {
        vfxAudio.volume = v;
        vfxText.text = Mathf.RoundToInt(v * 100f) + "%";

        vfxIconOn.SetActive(v > 0.01f);
        vfxIconOff.SetActive(v <= 0.01f);
    }
}
