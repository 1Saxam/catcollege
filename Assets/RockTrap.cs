using UnityEngine;
using System.Collections;

public class RockTrap : MonoBehaviour
{
    public GameObject rockPrefab;
    public Transform spawnPoint;

    public float spawnHeight = 8f;

    public float delay = 1f;

    private bool triggered = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (triggered)
            return;

        if (other.GetComponent<PlayerMovement>() != null)
        {
            triggered = true;

            StartCoroutine(DropRock(other.transform));
        }
    }

    IEnumerator DropRock(Transform player)
    {
        yield return new WaitForSeconds(delay);

        Vector3 spawnPos = player.position +
                           Vector3.up * spawnHeight;

        Instantiate(
            rockPrefab,
            spawnPoint.position,
            Quaternion.identity
        );
    }
}