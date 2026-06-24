using UnityEngine;

public class Rock : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("groundLayer"))
        {
            GetComponent<Collider2D>().enabled = false;
        }
    }
}