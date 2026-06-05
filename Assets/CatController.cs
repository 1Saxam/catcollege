using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class CatController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;

    private Rigidbody2D rb;
    private float moveInput;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        moveInput = Input.GetAxisRaw("Horizontal");

        if (moveInput > 0)
            transform.localScale = Vector3.one;
        else if (moveInput < 0)
            transform.localScale = new Vector3(-1, 1, 1);
    }

    private void FixedUpdate()
    {
        rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);
    }
}