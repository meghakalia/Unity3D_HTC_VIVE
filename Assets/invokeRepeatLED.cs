using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class invokeRepeatLED : MonoBehaviour
{
    //public Material m_Material;
    bool firstTime = true;
    float prevTime = 0;
    public float timeDelay = 0;
    [System.NonSerialized] // this won't save the timePeriod variable in between the running instances
    public float timePeriod = 1.0f; // based on the MATLAB script
    float timeParsed = 0;
    bool b_lightOn = false;
    public float timePause = 1.0f;
    bool m_startCoRoutine = true;
    float time_delay = 0f;
    List<List<int>> shuffledComb;
    int correctResponse = 0;
    [SerializeField] float stimulusDuration = 0.1f; // 100 ms
    int subjectResponse = 0;
    public bool m_Start_TOJ = false;
    double prev; 
    // Start is called before the first frame update
    void Start()
    {
        //alteernate between two 
        prev = Time.realtimeSinceStartupAsDouble * 1000.0f; 
        InvokeRepeating("startLED", 0f, 0.01f);
        //InvokeRepeating("SwitchOffLED", 1f, 2f);

        //CancelInvoke("startLED");
        //CancelInvoke("SwitchOffLED");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //start stimulus
    void startLED()
    {
        
        if (!b_lightOn)
        {
            //Debug.Log("invoke LED Delay start" + " -- " + ((Time.realtimeSinceStartupAsDouble * 1000.0f) - prev));
            GetComponent<MeshRenderer>().material.EnableKeyword("_EMISSION");
            b_lightOn = true; 
        }
        else
        {
            //Debug.Log("invoke LED Delay end " + " -- " + ((Time.realtimeSinceStartupAsDouble * 1000.0f) - prev));
            GetComponent<MeshRenderer>().material.DisableKeyword("_EMISSION");
            b_lightOn = false; 
        }
        
    }

    void SwitchOffLED()
    {
        GetComponent<MeshRenderer>().material.DisableKeyword("_EMISSION");
    }

}
