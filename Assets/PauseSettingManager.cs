using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class SettingUIManager : MonoBehaviour
{
    [Header("Setting Panel")]
    public GameObject settingPanel;

    [Header("Music Volume")]
    public Slider musicSlider;
    public TextMeshProUGUI musicText;
    public Button musicOnButton;
    public Button musicOffButton;

    private float lastVolume = 1f;
    private bool isOpen = false;

    void Start()
    {
        settingPanel.SetActive(false);
    }

    void OnEnable()
    {
        // sync từ AudioManager
        float v = AudioManager.instance.GetVolume();
        musicSlider.value = v;
        UpdateUI(v);

        // reset listener
        musicSlider.onValueChanged.RemoveAllListeners();
        musicOnButton.onClick.RemoveAllListeners();
        musicOffButton.onClick.RemoveAllListeners();

        // add listener
        musicSlider.onValueChanged.AddListener(OnMusicVolumeChange);
        musicOnButton.onClick.AddListener(OnMusicOff);
        musicOffButton.onClick.AddListener(OnMusicOn);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            ToggleSetting();
    }

    public void ToggleSetting()
    {
        isOpen = !isOpen;
        settingPanel.SetActive(isOpen);
        Time.timeScale = isOpen ? 0 : 1;
    }

    public void OnBackButton()
    {
        ToggleSetting();
    }

    public void OnHomeButton()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("MainMenu");
    }

    // ===== MUSIC =====
    void OnMusicVolumeChange(float v)
    {
        AudioManager.instance.SetVolume(v);
        UpdateUI(v);
    }

    void UpdateUI(float v)
    {
        musicText.text = Mathf.RoundToInt(v * 100f) + "%";
        musicOnButton.gameObject.SetActive(v > 0.01f);
        musicOffButton.gameObject.SetActive(v <= 0.01f);
    }

    void OnMusicOff()
    {
        lastVolume = musicSlider.value;
        musicSlider.value = 0;
    }

    void OnMusicOn()
    {
        if (lastVolume < 0.1f) lastVolume = 1f;
        musicSlider.value = lastVolume;
    }
}
