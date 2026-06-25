using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class pauseScrpt : MonoBehaviour
{
    public Button settings;
    public GameObject settingsPanel;

    // Start is called before the first frame update
    void Start()
    {
        // اطمینان از بسته بودن پنل در ابتدا
        if (settingsPanel != null)
            settingsPanel.SetActive(false);

        // اتصال دکمه Settings
        settings.onClick.AddListener(ShowSettings);
    }

    void ShowSettings()
    {
        if (settingsPanel != null)
            settingsPanel.SetActive(true);
    }
}
