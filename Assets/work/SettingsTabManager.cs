using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class SettingsTabManager : MonoBehaviour
{
    [Header("Tab Buttons")]
    public Button[] tabButtons;  

    [Header("Content Pages")]
    public GameObject[] tabPages; 

    [Header("Default Tab Index")]
    public int defaultTabIndex = 0; 

    private int currentTabIndex = -1;

    void Start()
    {
        
        for (int i = 0; i < tabButtons.Length; i++)
        {
            int index = i; 
            tabButtons[i].onClick.AddListener(() => SwitchTab(index));
        }

        
        if (tabPages.Length > 0 && defaultTabIndex < tabPages.Length)
        {
            SwitchTab(defaultTabIndex);
        }
        else
        {
            HideAllPages();
        }
    }

    public void SwitchTab(int tabIndex)
    {
        if (tabIndex < 0 || tabIndex >= tabPages.Length)
            return;

        if (currentTabIndex == tabIndex)
            return; 

      
        HideAllPages();

       
        tabPages[tabIndex].SetActive(true);

      
        currentTabIndex = tabIndex;
    }

  
    private void HideAllPages()
    {
        foreach (GameObject page in tabPages)
        {
            if (page != null)
                page.SetActive(false);
        }
    }

   
    public int GetCurrentTabIndex()
    {
        return currentTabIndex;
    }
}