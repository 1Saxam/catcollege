using System.Collections.Generic;
using UnityEngine;

// Cat's attack hitbox (child of the cat). Now hits Bats and Crows, once per swing.
public class AttackHitbox : MonoBehaviour
{
    public int damage = 1;

    private CatAttack catAttack;
    private readonly HashSet<Component> hitThisSwing = new HashSet<Component>();

    void Awake()
    {
        // Awake (not Start) because this object gets switched on/off.
        catAttack = GetComponentInParent<CatAttack>();
    }

    void OnEnable()
    {
        hitThisSwing.Clear();
    }

    private void OnTriggerEnter2D(Collider2D other) => TryHit(other);
    private void OnTriggerStay2D(Collider2D other) => TryHit(other);

    void TryHit(Collider2D other)
    {
        if (catAttack == null || !catAttack.isAttacking)
            return;

        BatEnemy bat = other.GetComponent<BatEnemy>();
        if (bat != null && hitThisSwing.Add(bat))
        {
            Debug.Log("Bat Hit!");
            bat.TakeDamage(damage, catAttack.transform.position);
            return;
        }

        CrowHealth crow = other.GetComponent<CrowHealth>();
        if (crow != null && hitThisSwing.Add(crow))
        {
            Debug.Log("Crow Hit!");
            crow.TakeDamage(damage);
        }
    }
}