using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class menu : MonoBehaviour
{
    public Button start;
    public Button quit;
    public Button settings;
    public GameObject settingsPanel;

    // Start is called before the first frame update
    void Start()
    {

        start.onClick.AddListener(() => SceneManager.LoadScene("SampleScene"));
        quit.onClick.AddListener(() => Application.Quit());

        quit.onClick.AddListener(() => {
          #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
          #else
             Application.Quit();
          #endif
        });


        // اطمینان از بسته بودن پنل در ابتدا
        if (settingsPanel != null)
            settingsPanel.SetActive(false);

        // اتصال دکمه Settings
        settings.onClick.AddListener(ShowSettings);


        //start.onClick.AddListener(() =>
        //{
        //    SceneManager.LoadScene("main");
        //});

        //quit.onClick.AddListener(() => { 
        //    Application.Quit();
        //});

    }

    // Update is called once per frame
    void Update()
    {
        // اگر کلید Escape زده شد و پنل تنظیمات فعال است
        if (Input.GetKeyDown(KeyCode.Escape) && settingsPanel != null && settingsPanel.activeSelf)
        {
            settingsPanel.SetActive(false); // بستن پنل
        }
    }

    void ShowSettings()
    {
        if (settingsPanel != null)
            settingsPanel.SetActive(true);
    }

    // متد برای بستن پنل (مثلاً به دکمه Close داخل پنل بدهید)
    public void CloseSettings()
    {
        if (settingsPanel != null)
            settingsPanel.SetActive(false);
    }
}
