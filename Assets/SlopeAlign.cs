using UnityEngine;

public class SlopeAlign : MonoBehaviour
{
    [Header("References")]
    public Transform groundCheck;          // drag your GroundCheck child here
    public LayerMask groundLayer;          // set this to whatever layer your platforms are on

    [Header("Raycast Settings")]
    public float rayDistance = 0.6f;       // how far down to check for ground
    public float rotationSpeed = 10f;      // higher = snaps faster, lower = smoother/slower

    [Header("Limits")]
    public float maxSlopeAngle = 45f;      // ignore surfaces steeper than this

    private bool isGrounded;
    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        AlignToSlope();
    }

    void AlignToSlope()
    {
        RaycastHit2D hit = Physics2D.Raycast(groundCheck.position, Vector2.down, rayDistance, groundLayer);

        float targetAngle;

        if (hit.collider != null)
        {
            isGrounded = true;

            float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);


            if (slopeAngle <= maxSlopeAngle)
            {
                // Signed angle so rotation direction matches slope direction (left-down vs right-down)
                targetAngle = Vector2.SignedAngle(Vector2.up, hit.normal);
                targetAngle = Mathf.Round(targetAngle); // snap to nearest degree, kills micro-jitter
            }
            else
            {
                targetAngle = 0f; // too steep, treat as flat
            }
        }
        else
        {
            isGrounded = false;
            targetAngle = 0f; // in the air, straighten up
        }

        float newAngle = Mathf.LerpAngle(rb.rotation, targetAngle, rotationSpeed * Time.fixedDeltaTime);
        rb.MoveRotation(newAngle);
    }

    void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawLine(groundCheck.position, groundCheck.position + Vector3.down * rayDistance);
    }
}