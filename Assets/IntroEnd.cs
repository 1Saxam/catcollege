using UnityEngine;
using Cinemachine;
using UnityEngine.Playables;

public class IntroEnd : MonoBehaviour
{
    public CinemachineVirtualCamera introCam;
    public PlayerMovement player;
    public GameObject cutsceneButterfly;
    public GameObject companionButterfly;

    public void EndIntro()
    {
        if (introCam != null) introCam.Priority = 5;
        if (player != null) player.canMove = true;

        PlayableDirector director = GetComponent<PlayableDirector>();
        if (director != null) director.Stop();

        if (cutsceneButterfly != null) cutsceneButterfly.SetActive(false);

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