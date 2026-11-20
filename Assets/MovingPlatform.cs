using System.Collections.Generic;
using UnityEngine;

// Put on the platform. Needs a BoxCollider2D (Is Trigger OFF).
[RequireComponent(typeof(Rigidbody2D))]
public class MovingPlatform : MonoBehaviour
{
    public float speed = 2f;       // how fast it moves
    public float distance = 3f;    // how far it goes to the right, then comes back

    Rigidbody2D rb;
    Collider2D col;
    Vector2 startPos;
    readonly HashSet<Rigidbody2D> riders = new HashSet<Rigidbody2D>();

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        startPos = rb.position;
    }

    void FixedUpdate()
    {
        float x = Mathf.PingPong(Time.time * speed, distance);
        Vector2 newPos = startPos + new Vector2(x, 0f);
        Vector2 delta = newPos - rb.position;

        rb.MovePosition(newPos);

        // carry the cat when it stands on top
        foreach (Rigidbody2D r in riders)
            if (r != null) r.position += delta;
    }

    void OnCollisionStay2D(Collision2D c)
    {
        if (c.rigidbody == null) return;
        bool onTop = c.collider.bounds.min.y >= col.bounds.max.y - 0.1f;
        if (onTop) riders.Add(c.rigidbody); else riders.Remove(c.rigidbody);
    }

    void OnCollisionExit2D(Collision2D c)
    {
        if (c.rigidbody != null) riders.Remove(c.rigidbody);
    }

    void OnDrawGizmos()
    {
        Vector3 a = Application.isPlaying ? (Vector3)startPos : transform.position;
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(a, a + Vector3.right * distance);
    }
}