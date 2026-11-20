using UnityEngine;

// Put this on a CHILD of the bat (e.g. "BatAttack") with a Collider2D set to Is Trigger.
// Place the collider in front of the bat's mouth/claws. BatEnemy turns it on only during the bite.
[RequireComponent(typeof(Collider2D))]
public class BatAttackHitbox : MonoBehaviour
{
    Collider2D col;
    bool hasHit;

    void Awake()
    {
        col = GetComponent<Collider2D>();
        col.isTrigger = true;
        col.enabled = false;
    }

    public void Activate()
    {
        hasHit = false;
        col.enabled = true;
    }

    public void Deactivate()
    {
        col.enabled = false;
    }

    void OnTriggerEnter2D(Collider2D other) => TryHit(other);
    void OnTriggerStay2D(Collider2D other) => TryHit(other);

    void TryHit(Collider2D other)
    {
        if (hasHit) return;

        // GetComponent (not InParent) so it only counts the cat's BODY collider,
        // not the cat's own attack hitbox.
        PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
        if (playerHealth == null) return;

        hasHit = true;

        BatEnemy bat = GetComponentInParent<BatEnemy>();
        Vector2 push = bat != null ? bat.GetPushFor(other.transform.position) : Vector2.zero;
        playerHealth.TakeDamage(transform.position, push);
    }
}