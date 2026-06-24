using UnityEngine;

public class CrowTrigger : MonoBehaviour
{
    public CrowEnemy crow;
    public Transform player;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.transform == player)
        {
            crow.StartFollowing();
        }
    }
}