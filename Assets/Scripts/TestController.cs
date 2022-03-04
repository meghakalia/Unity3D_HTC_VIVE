using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.OpenXR.Input;
public class TestController : MonoBehaviour
{
    public InputActionReference action;
    public InputActionReference hapticAction;
    public float _amplitude = 1.0f;
    public float _duration = 0.1f;
    public float _frequency = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        if (action == null || hapticAction == null)
            return;

        action.action.Enable();
        hapticAction.action.Enable();
        action.action.performed += (ctx) =>
        {
            var control = action.action.activeControl;
            if (null == control)
                return;

            OpenXRInput.SendHapticImpulse(hapticAction.action, _amplitude, _frequency, _duration, control.device);
        };
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
