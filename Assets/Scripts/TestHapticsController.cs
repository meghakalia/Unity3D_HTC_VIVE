using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.OpenXR.Input;
using UnityEngine.XR.Interaction.Toolkit;
public class TestHapticsController : MonoBehaviour
{
    public InputActionReference hapticAction;
    public float _amplitude = 1.0f;
    public float _duration = 0.1f;
    public float _frequency = 0.0f;
    [SerializeField]
    InputControl controller; // type check ?
    // Start is called before the first frame update
    void Start()
    {
        if (hapticAction == null)
            return;
        hapticAction.action.Enable();
        controller = hapticAction.action.activeControl;

        StartCoroutine(SendHaptics()); 
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //coroutine 
    IEnumerator SendHaptics()
    {
        var temp = 0; 
        while (temp < 5)
        {
            if (controller != null)
                OpenXRInput.SendHapticImpulse(hapticAction.action, _amplitude, _frequency, _duration, controller.device);
            temp++;
            yield return new WaitForSecondsRealtime(1f); 
        }
        
        
    }
}
