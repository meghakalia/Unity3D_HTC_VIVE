

using UnityEngine.InputSystem;
using UnityEngine.XR.OpenXR.Input;
//using UnityEngine.XR.Interaction.Toolkit;

namespace UnityEngine.XR.OpenXR.Samples.ControllerSample
{
    public class TestHapticsController : MonoBehaviour
    {
        public InputActionReference action;
        public InputActionReference hapticAction;
        public float _amplitude = 1.0f;
        public float _duration = 0.1f;
        public float _frequency = 0.0f;

        private void Start()
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
    }
}















//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.InputSystem;
//using UnityEngine.XR.OpenXR.Input;
//using UnityEngine.XR.Interaction.Toolkit;

//using UnityEngine.XR;
//public class TestHapticsController : MonoBehaviour
//{
//    public InputActionReference hapticAction;
//    public InputActionReference action;
//    public float _amplitude = 1.0f;
//    public float _duration = 0.1f;
//    public float _frequency = 0.0f;
//    //public InputControl controller; // type check ?
//    [SerializeField]

//    //XRController checkLeft;
//    //List<UnityEngine.XR.InputDevice> devices; 
//    // Start is called before the first frame update
//    void Start()
//    {
//        if (hapticAction == null || action == null)
//            return;
//        hapticAction.action.Enable();
//        action.action.Enable(); 
//        //controller = hapticAction.action.activeControl;



//        //devices = new List<UnityEngine.XR.InputDevice>();

//        //UnityEngine.XR.InputDevices.GetDevicesWithCharacteristics(UnityEngine.XR.InputDeviceCharacteristics.Right, devices);

//        //StartCoroutine(SendHaptics()); 

//    }

//    // Update is called once per frame
//    void Update()
//    {
//        action.action.performed += (ctx) =>
//        {
//            var control = action.action.activeControl;
//            if (null == control)
//                return;

//            OpenXRInput.SendHapticImpulse(hapticAction.action, _amplitude, _frequency, _duration, control.device);
//        };
//    }

//    //coroutine 
//    IEnumerator SendHaptics()
//    {
//        var temp = 0; 
//        while (temp < 5)
//        {
//            action.action.performed += (ctx) =>
//            {
//                var control = action.action.activeControl;
//                if (null == control)
//                    return;

//                OpenXRInput.SendHapticImpulse(hapticAction.action, _amplitude, _frequency, _duration, control.device);
//            };
//            //var controller = hapticAction.action.activeControl; //not working 
//            //    if (rightHand != null)
//            //OpenXRInput.SendHapticImpulse(hapticAction.action, _amplitude, _frequency, _duration, rightHand.device);
//            temp++;
//            yield return new WaitForSecondsRealtime(1f); 
//        }


//    }
//}
