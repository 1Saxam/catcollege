using UnityEngine;
using Cinemachine;

public class IntroEnd : MonoBehaviour
{
    public CinemachineVirtualCamera introCam;
    public PlayerMovement player;

    public void EndIntro()
    {
        Debug.Log("INTRO ENDED");
        introCam.Priority = 5;

        player.canMove = true;
    }
}