using UnityEngine;
using System.Collections;

public class CatJump : MonoBehaviour
{
    [Header("Jump")]
    public float jumpHeight = 3f;
    public float gravity = -20f;

    [Header("Refs")]
    public Animator animator;
    public Rigidbody2D rb;

    private bool isGrounded = true;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
            StartCoroutine(JumpSequence());

        // Switch rise  fall when velocity turns negative
        if (!isGrounded && rb.velocity.y < -0.1f)
        {
            animator.SetBool("isFalling", true);
        }

        SyncAnimSpeed();
    }

    IEnumerator JumpSequence()
    {
        isGrounded = false;
        animator.SetBool("isJumping", true);

        // Wait for crouch clip to finish before launching
        yield return new WaitForSeconds(GetClipLength("cat_crouch"));

        float launchV = Mathf.Sqrt(2f * Mathf.Abs(gravity) * jumpHeight);
        rb.velocity = new Vector2(rb.velocity.x, launchV);
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Ground"))
        {
            animator.SetBool("isJumping", false);
            animator.SetBool("isFalling", false);
            isGrounded = true;
        }
    }

    void SyncAnimSpeed()
    {
        // Keeps mid-air frames spread naturally across arc at any height
        float baseHeight = 3f;
        animator.speed = Mathf.Sqrt(jumpHeight / baseHeight);
    }

    float GetClipLength(string clipName)
    {
        foreach (var clip in animator.runtimeAnimatorController.animationClips)
            if (clip.name == clipName) return clip.length;
        return 0.25f;
    }
}