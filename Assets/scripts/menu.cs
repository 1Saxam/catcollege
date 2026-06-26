using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class menu : MonoBehaviour
{
    public Button continueBtn;
    public Button start;
    public Button quit;
    public Button settings;
    public GameObject settingsPanel;

    // Start is called before the first frame update
    void Start()
    {

        // --- Start ---
        start.onClick.AddListener(() => {
            PlayerPrefs.SetInt("HasSavedGame", 0); // save قدیمی پاک مییشود
            SceneManager.LoadScene("SampleScene");
        });

        // --- Continue ---
        if (continueBtn != null)
        {
            bool hasSave = PlayerPrefs.GetInt("HasSavedGame", 0) == 1;
            continueBtn.gameObject.SetActive(hasSave);

            continueBtn.onClick.AddListener(() => {
                string savedScene = PlayerPrefs.GetString("SavedScene", "SampleScene");
                float savedX = PlayerPrefs.GetFloat("SavedPosX", 0f);
                float savedY = PlayerPrefs.GetFloat("SavedPosY", 0f);

                SceneManager.LoadScene(savedScene);
            });
        }

        quit.onClick.AddListener(() => Application.Quit());

        //to exit the game in editor
        quit.onClick.AddListener(() => {
          #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
          #else
             Application.Quit();
          #endif
        });

        
        if (settingsPanel != null)
            settingsPanel.SetActive(false);
        settings.onClick.AddListener(ShowSettings);

    }

    // Update is called once per frame
    void Update()
    {
        
        if (Input.GetKeyDown(KeyCode.Escape) && settingsPanel != null && settingsPanel.activeSelf)
        {
            settingsPanel.SetActive(false); 
        }
    }

    void ShowSettings()
    {
        if (settingsPanel != null)
            settingsPanel.SetActive(true);
    }

    
    public void CloseSettings()
    {
        if (settingsPanel != null)
            settingsPanel.SetActive(false);
    }
}
