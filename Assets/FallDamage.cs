using UnityEngine;

// Put this on the CAT (same object as PlayerHealth and PlayerMovement).
// The cat falls freely. When it lands on the Ground layer, the fall height is checked.
// Too high = lose a heart. No respawn - the cat just carries on from where it landed.
public class FallDamage : MonoBehaviour
{
    public LayerMask groundLayer;            // pick your "Ground" layer here
    public float safeFallHeight = 4f;        // falls higher than this cost a heart
    public bool showDebugLogs = true;

    PlayerHealth health;
    PlayerMovement movement;
    Rigidbody2D rb;

    readonly ContactPoint2D[] contacts = new ContactPoint2D[8];
    bool wasGrounded = true;
    float highestY;

    void Reset()
    {
        groundLayer = LayerMask.GetMask("Ground");
    }

    void Start()
    {
        health = GetComponent<PlayerHealth>();
        movement = GetComponent<PlayerMovement>();
        rb = GetComponent<Rigidbody2D>();

        if (groundLayer.value == 0) groundLayer = LayerMask.GetMask("Ground");
        highestY = transform.position.y;
    }

    void FixedUpdate()
    {
        if (health != null && health.IsDead) return;

        float y = transform.position.y;

        // Climbing doesn't count as falling
        if (movement != null && movement.isClimbing)
        {
            highestY = y;
            wasGrounded = false;
            return;
        }

        bool grounded = OnGround();

        if (!grounded)
        {
            // track the highest point since leaving the ground
            if (wasGrounded) highestY = y;
            else highestY = Mathf.Max(highestY, y);
        }
        else if (!wasGrounded)
        {
            // just landed on the ground -> how far did we fall?
            float fallHeight = highestY - y;
            if (showDebugLogs) Debug.Log("Landed. Fell " + fallHeight.ToString("F1") + " units");

            if (fallHeight > safeFallHeight && health != null)
                health.TakeDamage((Vector2)transform.position + Vector2.down * 0.5f);
        }

        wasGrounded = grounded;
    }

    // Touching something on the Ground layer from above
    bool OnGround()
    {
        int count = rb.GetContacts(contacts);
        for (int i = 0; i < count; i++)
        {
            Collider2D other = contacts[i].collider;
            bool isGroundLayer = (groundLayer.value & (1 << other.gameObject.layer)) != 0;
            if (isGroundLayer && !other.isTrigger && contacts[i].normal.y > 0.5f)
                return true;
        }
        return false;
    }
}