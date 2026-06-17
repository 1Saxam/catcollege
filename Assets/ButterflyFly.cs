using UnityEngine;

public class ButterflyFly : MonoBehaviour
{
    public float speed = 2f;
    public float horizontalSpeed = 1f;

    private Vector3 direction;

    void Start()
    {
        direction = new Vector3(
            Random.Range(-horizontalSpeed, horizontalSpeed),
            0,
            0
        ).normalized;

        Destroy(gameObject, 5f);
    }

    void Update()
    {
        transform.position += direction * speed * Time.deltaTime;

        transform.position += new Vector3(
            Mathf.Sin(Time.time * 10f) * 0.5f * Time.deltaTime,
            0,
            0);
    }
}