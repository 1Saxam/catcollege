using UnityEngine;

public class ParallaxLayer : MonoBehaviour
{
    public Transform cameraTransform;
    public float parallaxAmount = 0.5f;

    private Vector3 lastCameraPosition;

    void Start()
    {
        lastCameraPosition = cameraTransform.position;
    }

    void LateUpdate()
    {
        Vector3 delta = cameraTransform.position - lastCameraPosition;

        transform.position += new Vector3(
            delta.x * parallaxAmount,
            0,
            0
        );

        lastCameraPosition = cameraTransform.position;
    }
}