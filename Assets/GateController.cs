using UnityEngine;

public class GateController : MonoBehaviour
{
    public GameObject closedGate;
    public GameObject openGate;
    public GameObject butt;
    public GameObject butstill;

    public void OpenGate()
    {
        closedGate.SetActive(false);
        openGate.SetActive(true);
        butt.SetActive(true);
        butstill.SetActive(false);
    }
}