using UnityEngine;

public class AttackHitbox : MonoBehaviour
{
    public int damage = 1;

    private CatAttack catAttack;

    void Start()
    {
        catAttack = GetComponentInParent<CatAttack>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Touched: " + other.name);

        if (!catAttack.isAttacking)
        {
            Debug.Log("Not Attacking");
            return;
        }

        CrowHealth crow = other.GetComponent<CrowHealth>();

        if (crow != null)
        {
            Debug.Log("Crow Hit!");

            crow.TakeDamage(damage);
        }
    }
}