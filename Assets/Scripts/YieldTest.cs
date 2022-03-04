using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YieldTest : MonoBehaviour
{
    int frameNum = 0; 
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log("ival " + " update frameNum before" + frameNum + " -- " + (new System.Diagnostics.StackFrame(0, true)).GetFileLineNumber());
        StartCoroutine(MyCoroutine());
        frameNum++; 
        Debug.Log("ival " + " update frameNum " + frameNum + " -- " + (new System.Diagnostics.StackFrame(0, true)).GetFileLineNumber());
    }

    IEnumerator MyCoroutine()
    {
        Debug.Log("ival " + " before i =0  -- " + (new System.Diagnostics.StackFrame(0, true)).GetFileLineNumber());
        int i = 0;
        Debug.Log("ival " + " after i =0 " + i + " -- " + (new System.Diagnostics.StackFrame(0, true)).GetFileLineNumber());
        while (i < 10)
        {
            // Count to Ten
            Debug.Log("ival " + " before i++ " + i + " -- " + (new System.Diagnostics.StackFrame(0, true)).GetFileLineNumber());
            i++;
            Debug.Log("ival " + " i<10 " + i + " -- " + (new System.Diagnostics.StackFrame(0, true)).GetFileLineNumber());
            yield return null;
        }
        while (i > 10)
        {
            // Count back to Zero
            i--;
            Debug.Log("ival " + " i>10 " + i + " -- " + (new System.Diagnostics.StackFrame(0, true)).GetFileLineNumber());
            yield return null;
        }
        // All done!
    }
}
