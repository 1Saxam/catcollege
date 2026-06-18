using TMPro;
using UnityEngine;

public class OrbCounter : MonoBehaviour
{
    public TMP_Text orbCountText;

    private int orbCount = 0;

    void Start()
    {
        UpdateUI();
    }

    public void CollectOrb()
    {
        orbCount++;
        UpdateUI();
    }

    void UpdateUI()
    {
        orbCountText.text = orbCount.ToString();
    }
}