using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class SendHapticImpulse : MonoBehaviour
{
    [SerializeField] private XRBaseController controller;
    public float timer = 2f;
    private float counter;

    public void ActivateHaptic()
    {
        if (controller != null)
            controller.SendHapticImpulse(0.7f, 1f);
    }

    private void Update()
    {
        counter += Time.deltaTime;
        if (counter >= timer)
        {
            counter = 0f;
            ActivateHaptic();
        }
    }
}
