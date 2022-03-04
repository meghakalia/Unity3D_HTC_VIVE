using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class coRoutineTests : MonoBehaviour
{
    int frame = 0; 
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(MyCoroutine(frame));
    }

    // Update is called once per frame
    void Update()
    {
        
       
    }

    IEnumerator MyCoroutine(int fra)
    {

        fra++;
        //number++;
        Debug.Log("testRoutine " + " frame number " + fra + " -- " + (new System.Diagnostics.StackFrame(0, true)).GetFileLineNumber());
        int i = 0;
        while (i < 10)
        {
            Debug.Log("testRoutine " + " frame number inside while loop" + fra + " -- " + (new System.Diagnostics.StackFrame(0, true)).GetFileLineNumber());
            // Count to Ten
            Debug.Log("testRoutine " + i + " -- " + (new System.Diagnostics.StackFrame(0, true)).GetFileLineNumber());
            i++;
            Debug.Log("testRoutine " + i + " -- " + (new System.Diagnostics.StackFrame(0, true)).GetFileLineNumber());
            yield return null;
        }
        //while (i > 0)
        //{
        //    // Count back to Zero 
        //    i--;
        //    Debug.Log("testRoutine " + i + " -- " + (new System.Diagnostics.StackFrame(0, true)).GetFileLineNumber());
        //    yield return null;
        //}
        // All done!
        //yield return null;
    }

}
