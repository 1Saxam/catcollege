using UnityEngine;

public class ButterflyCompanion : MonoBehaviour
{
    [Header("Follow Settings")]
    public Transform target;
    public float heightOffset = 1.5f;
    public float smoothTime = 0.3f; // هرچی بزرگتر باشه، نرم‌تر و با تاخیرتر حرکت میکنه
    public float bobSpeed = 3f;
    public float bobAmount = 0.15f;

    private Vector3 velocity = Vector3.zero;

    void Update()
    {
        if (target == null) return;

        // موقعیت بالای سر گربه
        Vector3 targetPosition = target.position + new Vector3(0, heightOffset, 0);

        // افکت بال زدن (شناور بودن)
        float bob = Mathf.Sin(Time.time * bobSpeed) * bobAmount;
        targetPosition.y += bob;

        // حرکت نرم و کشیده (جایگزین Lerp)
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
    }
}