using UnityEngine;

public class GateController : MonoBehaviour
{
    public GameObject closedGate;
    public GameObject openGate;
    public GameObject butt;

    public void OpenGate()
    {
        closedGate.SetActive(false);
        openGate.SetActive(true);
        butt.SetActive(true);
    }
}