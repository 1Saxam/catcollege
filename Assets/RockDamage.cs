using UnityEngine;

public class RockDamage : MonoBehaviour
{
    private bool canDamage = true;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Rock touched ground
        if (collision.gameObject.CompareTag("groundLayer"))
        {
            canDamage = false;
            return;
        }

        PlayerHealth health = collision.gameObject.GetComponent<PlayerHealth>();

        if (canDamage && health != null)
        {
            health.TakeDamage();

            Destroy(gameObject);
        }
    }
}