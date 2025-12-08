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
    public Button vfxOnButton;     // dùng khi VFX đang bật
    public Button vfxOffButton;    // dùng khi VFX đang tắt
    public AudioSource vfxAudio;

    private float lastVolume = 1f; // lưu volume trước khi tắt
    bool isOpen = false;

    private void Start()
    {
        settingPanel.SetActive(false);

        // Gán sự kiện Slider + nút On/Off
        vfxSlider.onValueChanged.AddListener(OnVFXVolumeChange);
        vfxOnButton.onClick.AddListener(OnVFXOff);
        vfxOffButton.onClick.AddListener(OnVFXOn);

        OnVFXVolumeChange(vfxSlider.value);
    }

    private void Update()
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

    // Nhấn BACK chỉ tắt UI
    public void OnBackButton() => ToggleSetting();

    // Home
    public void OnHomeButton()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("MainMenu");
    }


    // ——— SLIDER VFX ———
    void OnVFXVolumeChange(float v)
    {
        if (vfxAudio) vfxAudio.volume = v;

        vfxText.text = Mathf.RoundToInt(v * 100f) + "%";

        // Hiện nút tương ứng
        vfxOnButton.gameObject.SetActive(v > 0.01f);  // volume còn → hiện nút OFF
        vfxOffButton.gameObject.SetActive(v <= 0.01f);
    }

    // ——— NÚT OFF ———
    void OnVFXOff()
    {
        lastVolume = vfxSlider.value; // lưu âm lượng cũ
        vfxSlider.value = 0;
    }

    // ——— NÚT ON ———
    void OnVFXOn()
    {
        if (lastVolume < 0.1f) lastVolume = 1f; // nếu lần đầu => set mặc định 100%
        vfxSlider.value = lastVolume;
    }
}
