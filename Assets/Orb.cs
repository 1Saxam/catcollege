using UnityEngine;

public class Orb : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        OrbCounter counter = other.GetComponent<OrbCounter>();

        if (counter != null)
        {
            counter.CollectOrb();
            Destroy(gameObject);
        }
    }
}