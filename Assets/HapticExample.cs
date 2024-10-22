using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class HapticExample : MonoBehaviour
{
    // Adding the SerializeField attribute to a field will make it appear in the Inspector window
    // where a developer can drag a reference to the controller that you want to send haptics to.
    [SerializeField]
    XRBaseController controller;

    protected void Start()
    {
        if (controller == null)
            Debug.LogWarning("Reference to the Controller is not set in the Inspector window, this behavior will not be able to send haptics. Drag a reference to the controller that you want to send haptics to.", this);

        StartCoroutine(StartPeriodicHaptics());
    }

    IEnumerator StartPeriodicHaptics()
    {
        // Trigger haptics every second
        var delay = new WaitForSeconds(1f);

        while (true)
        {
            yield return delay;
            SendHaptics();
        }
    }

    void SendHaptics()
    {
        if (controller != null)
            controller.SendHapticImpulse(0.7f, 0.1f);
    }
}
