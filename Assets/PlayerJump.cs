using UnityEngine;

public class PlayerJump : MonoBehaviour
{
    public float jumpForce = 8f;

    public Transform groundCheck;
    public LayerMask groundLayer;

    private bool isGrounded;
    private bool wasGrounded;

    private Rigidbody2D rb;
    private Animator animator;

    private PlayerMovement movement;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        movement = GetComponent<PlayerMovement>();
    }

    void Update()
    {
        isGrounded = Physics2D.OverlapCircle(
            groundCheck.position,
            0.2f,
            groundLayer
        );


        if (isGrounded != wasGrounded)
        {
            Debug.Log("Grounded = " + isGrounded);
            wasGrounded = isGrounded;
        }

        if (!movement.canMove)
            return;

        if (Input.GetKeyDown(KeyCode.Space) &&
            isGrounded)
        {
            rb.velocity = new Vector2(
                rb.velocity.x,
                jumpForce
            );

            animator.SetBool("isJumping", true);
        }

        animator.SetBool(
            "isFalling",
            rb.velocity.y < -0.1f
        );

        if (isGrounded)
        {
            animator.SetBool(
                "isJumping",
                false
            );
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck == null)
            return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(groundCheck.position, 0.2f);
    }



}