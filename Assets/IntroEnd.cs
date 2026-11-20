using UnityEngine;
using Cinemachine;
using UnityEngine.Playables;

public class IntroEnd : MonoBehaviour
{
    public CinemachineVirtualCamera introCam;
    public PlayerMovement player;

    [Header("Butterfly Setup")]
    public GameObject cutsceneButterfly;
    public GameObject companionButterfly;

    private bool isFinished = false; // ✨ برای اینکه دوبار اجرا نشه

    void Update()
    {
        // ✨ چیت کد: اگر دکمه Tab زده شد و هنوز کات سین تموم نشده بود ✨
        if (Input.GetKeyDown(KeyCode.Tab) && !isFinished)
        {
            Debug.Log("Cheat: Skipped Cutscene with Tab!");
            EndIntro(); // کات سین رو تموم کن
        }
    }

    public void EndIntro()
    {
        if (isFinished) return; // اگر قبلاً تموم شده، کاری نکن
        isFinished = true;

        if (introCam != null) introCam.Priority = 5;
        if (player != null) player.canMove = true;

        // توقف تایم لاین
        PlayableDirector director = GetComponent<PlayableDirector>();
        if (director != null) director.Stop();

        // خاموش کردن پروانه کات سین
        if (cutsceneButterfly != null) cutsceneButterfly.SetActive(false);

        // روشن کردن پروانه همراه
        if (companionButterfly != null)
        {
            companionButterfly.SetActive(true);
            ButterflyCompanion comp = companionButterfly.GetComponent<ButterflyCompanion>();
            if (comp != null)
            {
                comp.target = player.transform;
                comp.enabled = true;
            }
        }
    }
}