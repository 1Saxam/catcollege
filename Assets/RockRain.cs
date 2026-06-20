using UnityEngine;
using System.Collections;

public class RockRain : MonoBehaviour
{
    public GameObject rockPrefab;

    public Transform leftSpawn;
    public Transform rightSpawn;

    public int minRocks = 5;
    public int maxRocks = 15;

    public float minDelay = 0.2f;
    public float maxDelay = 1f;

    bool triggered = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (triggered)
            return;

        if (other.GetComponent<PlayerMovement>() != null)
        {
            triggered = true;
            StartCoroutine(SpawnRocks());
        }
    }

    IEnumerator SpawnRocks()
    {
        int rockCount = Random.Range(minRocks, maxRocks + 1);

        for (int i = 0; i < rockCount; i++)
        {
            float randomX = Random.Range(
                leftSpawn.position.x,
                rightSpawn.position.x
            );

            Vector3 spawnPos = new Vector3(
                randomX,
                leftSpawn.position.y,
                0
            );

            Instantiate(
                rockPrefab,
                spawnPos,
                Quaternion.identity
            );

            yield return new WaitForSeconds(
                Random.Range(minDelay, maxDelay)
            );
        }
    }
}