using UnityEngine;

public class PauseManager : MonoBehaviour
{
    [Header("Panels")]
    public GameObject pausePanel;        // پنل اصلی توقف (شامل resume و settings)
    public GameObject settingsPanel;     // پنل تنظیمات صدا (Page_Audio یا همان Panel)

    private bool isPaused = false;
    private bool isSettingsOpen = false;

    void Update()
    {
        // توقف/ادامه با Escape
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // اگر تنظیمات باز است، اول آن را ببند
            if (isSettingsOpen)
            {
                CloseSettings();
                return;
            }
            TogglePause();
        }
    }

    // ===== توقف/ادامه بازی =====
    public void TogglePause()
    {
        isPaused = !isPaused;

        if (isPaused)
        {
            Time.timeScale = 0f;
            if (pausePanel != null)
                pausePanel.SetActive(true);

            // بستن تنظیمات در صورت باز بودن
            if (settingsPanel != null && settingsPanel.activeSelf)
                settingsPanel.SetActive(false);
            isSettingsOpen = false;

            // قطع صدا (اختیاری)
            if (AudioManager.Instance != null)
                AudioManager.Instance.SetMute(true);
        }
        else
        {
            Time.timeScale = 1f;
            if (pausePanel != null)
                pausePanel.SetActive(false);

            // برگرداندن صدا
            if (AudioManager.Instance != null)
                AudioManager.Instance.SetMute(false);
        }
    }

    // ===== ادامه بازی (دکمه Resume) =====
    public void ResumeGame()
    {
        if (isPaused)
            TogglePause();
    }

    // ===== باز کردن تنظیمات صدا (دکمه Settings) =====
    public void OpenSettings()
    {
        Debug.Log("OpenSettings called. isPaused = " + isPaused);
        if (!isPaused)
        {
            Debug.Log("Not paused! Settings wont open.");
            return;
        }

        isSettingsOpen = true;

        // مخفی کردن پنل اصلی توقف (تا فقط تنظیمات دیده شود)
        if (pausePanel != null)
            pausePanel.SetActive(false);

        // نمایش پنل تنظیمات
        if (settingsPanel != null)
            settingsPanel.SetActive(true);
    }

    // ===== بستن تنظیمات و برگشت به پنل اصلی =====
    public void CloseSettings()
    {
        isSettingsOpen = false;

        // مخفی کردن پنل تنظیمات
        if (settingsPanel != null)
            settingsPanel.SetActive(false);

        // نمایش دوباره پنل اصلی توقف
        if (pausePanel != null)
            pausePanel.SetActive(true);
    }

    // ===== رفتن به منو و ذخیره وضعیت =====
    public void GoToMainMenu()
    {
        // ذخیره کن که بازی در جریان بود
        PlayerPrefs.SetInt("HasSavedGame", 1);
        PlayerPrefs.SetString("SavedScene",
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
        PlayerPrefs.Save();

        Time.timeScale = 1f;
        UnityEngine.SceneManagement.SceneManager.LoadScene("menu");
    }
}