using UnityEngine;
using System.Collections;

public class PlayerJump : MonoBehaviour
{
    public float jumpForce = 8f;
    public Transform groundCheck;
    public LayerMask groundLayer;

    private Rigidbody2D rb;
    private Animator animator;
    private PlayerMovement movement;
    private bool isGrounded;
    private bool hasJumped = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        movement = GetComponent<PlayerMovement>();
    }

    void Update()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);

        if (!movement.canMove) return;

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded && !hasJumped)
        {
            hasJumped = true;
            animator.SetTrigger("doCrouch");
            Debug.Log("Trigger fired");
            StartCoroutine(JumpAfterCrouch());
        }

        if (!isGrounded && rb.velocity.y < -0.1f)
        {
            animator.SetInteger("jumpState", 2); // cat_fall
        }

        // Only reset when we have actually jumped and come back to ground
        if (isGrounded && hasJumped && rb.velocity.y <= 0)
        {
            animator.SetTrigger("doLand");
            hasJumped = false;
            StartCoroutine(LandAfterDelay());
        }



    }

    IEnumerator JumpAfterCrouch()
    {
        yield return new WaitForSeconds(0.2f);
        rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        animator.SetInteger("jumpState", 1); // cat_rise
    }

    IEnumerator LandAfterDelay()
    {
        yield return new WaitForSeconds(6f);
        animator.SetTrigger("doLand");
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(groundCheck.position, 0.2f);
    }
}