using UnityEngine;

public class AttackHitbox : MonoBehaviour
{
    public int damage = 1;

    private void OnTriggerEnter2D(Collider2D other)
    {
        CrowHealth crow = other.GetComponent<CrowHealth>();

        if (crow != null)
        {
            crow.TakeDamage(damage);
        }
    }
}