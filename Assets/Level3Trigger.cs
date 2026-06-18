using UnityEngine;
using UnityEngine.Playables;

public class Level3Trigger : MonoBehaviour
{
    public PlayableDirector director;

    bool triggered = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (triggered)
            return;

        if (other.GetComponent<PlayerMovement>() != null)
        {
            triggered = true;

            director.Play();
        }
    }
}