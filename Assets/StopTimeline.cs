using UnityEngine;
using UnityEngine.Playables;

public class StopTimeline : MonoBehaviour
{
    public PlayableDirector director;

    public void StopCutscene()
    {
        director.Stop();
    }
}