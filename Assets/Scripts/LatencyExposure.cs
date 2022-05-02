using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using System;
using System.IO;
using System.Linq;

public class LatencyExposure : MonoBehaviour
{
    bool m_ExperimentLEDDelay = true; // true, buzz first, false: LEd first

    //public Material m_Material;
    bool firstTime = true;
    float prevTime = 0;
    public float timeDelay = 0;
    [System.NonSerialized] // this won't save the timePeriod variable in between the running instances
    public float timePeriod = 1.0f; // based on the MATLAB script
    float timeParsed = 0;
    bool b_lightOn = true;
    public float timePause = 1.0f;
    bool m_startCoRoutine = true;
    float time_delay = 0f;
    List<List<int>> shuffledComb;
    int correctResponse = 0;
    [SerializeField] float stimulusDuration = 0.1f; // 100 ms
    int subjectResponse = 0;
    public bool m_Start_TOJ = false;

    [SerializeField] private XRBaseController controller;
    [SerializeField] float _mEmissionPower = 3.0f;
    [SerializeField] float _mIntensityHaptic = 1.0f;

    //Touch haptic 
    public HapticPlugin HapticDevice = null;
    private int FXID = -1;
    [NonSerialized] public float frequency = 300.0f;
    [NonSerialized] public float gain = 1.5f;
    [NonSerialized] public float lowGain = 0.5f;
    [NonSerialized] public float magnitude = 1.2f;
    [NonSerialized] public double[] dir = { 1.0, 1.0, 1.0 };

    public bool m_start_TOJ = false;

    public RunMenu triggerMenuMsg;
    public GameObject MenuCanvas;

    public PostExposure postExposureObj;

    int tempListCount = 0;
    int tempList = 0;

    double LEDDuration = 80f; //even numbers 
    double BuzzDuration = 80f;


    bool m_first_LED_loop = true;
    bool m_first_buzz_loop = true;

    bool exitCoroutineLEDLoop = false;
    bool exitCoroutineBuzzLoop = false;

    bool m_activeHaptic = false;
    bool m_activeLED = false;

    double timeLapsed = 0.0f;

    double startTime = 0.0f;

    double LEDStartMillis = 0.0f;
    double BuzzStartMillis = 0.0f;

    double LEDDelay = 0.0f;
    double BuzzDelay = 0.0f;

    float _mIntensityLED = 1.8f;

    int loopCounter = 0;
    int loopNum = 5000;


    public void ActivateTouchHaptic(float gain, float magnitude, float frequency, double[] dir)
    {
        if (HapticDevice == null) return;       //If there is no device, bail out early.

        // If a haptic effect has not been assigned through Open Haptics, assign one now.
        if (FXID == -1)
        {
            FXID = HapticPlugin.effects_assignEffect(HapticDevice.configName);

            if (FXID == -1) // Still broken?
            {
                Debug.LogError("Unable to assign Haptic effect.");
                return;
            }
        }

        // Send the effect settings to OpenHaptics.
        double[] pos = { 0.0, 0.0, 0.0 }; // Position (not used for vibration)
                                          //double[] dir = {0.0, 1.0, 0.0}; // Direction of vibration

        HapticPlugin.effects_settings(
            HapticDevice.configName,
            FXID,
            gain, // Gain
            magnitude, // Magnitude
            frequency,  // Frequency
            pos,  // Position (not used for vibration)
            dir); //Direction.

        HapticPlugin.effects_type(HapticDevice.configName, FXID, 4); // Vibration effect == 4

        HapticPlugin.effects_startEffect(HapticDevice.configName, FXID);

    }

    void DeactivateTouchHaptic()
    {
        if (HapticDevice == null) return;       //If there is no device, bail out early.
        if (FXID == -1) return;                 //If there is no effect, bail out early.

        HapticPlugin.effects_stopEffect(HapticDevice.configName, FXID);
    }

    // Start is called before the first frame update
    void Start()
    {
        var canvas = GameObject.Find("InstructionsMenu");
        triggerMenuMsg = canvas.GetComponent<RunMenu>();

        //haptic Touch 
        if (HapticDevice == null)
            HapticDevice = (HapticPlugin)FindObjectOfType(typeof(HapticPlugin));

    }

    // Update is called once per frame
    void FixedUpdate()
    {

        if (startTime > 1.0f)
        {
            //if (m_startCoRoutine && triggerMenuMsg.startExperiment)
            //{
            //    StartCoroutine(Example());
            //}

            
            if (m_startCoRoutine && triggerMenuMsg.startExperiment)
            {
                timeLapsed = timeLapsed + Time.deltaTime * 1000;

                if (timeLapsed > LEDDelay && !exitCoroutineLEDLoop)
                {
                    //Debug.Log("Timelapsed LED " + timeLapsed);
                    if (m_first_LED_loop) //runs only once 
                    {
                        LEDStartMillis = timeLapsed;
                        m_first_LED_loop = false;
                    }

                    double checkTime = timeLapsed - LEDStartMillis;
                    //Debug.Log("checkTime " + " -- " + checkTime);
                    if (checkTime <= (LEDDuration - Time.deltaTime * 1000))
                    {
                        GetComponent<MeshRenderer>().material.EnableKeyword("_EMISSION");
                        GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", new Vector4(191.0f / 255f, 180f / 255f, 180f / 255f, 1f) * Mathf.Pow(2, _mIntensityLED));

                    }
                    else
                    {
                        //Debug.Log("LEDflag finish " + " -- " + checkTime);
                        GetComponent<MeshRenderer>().material.DisableKeyword("_EMISSION");

                        m_first_LED_loop = true;
                        exitCoroutineLEDLoop = true;
                    }

                }

                if (timeLapsed > BuzzDelay && !exitCoroutineBuzzLoop) // problem in timing 
                {
                    //Debug.Log("Timelapsed buzz " + timeLapsed);
                    if (m_first_buzz_loop) //runs only once 
                    {
                        BuzzStartMillis = timeLapsed;
                        m_first_buzz_loop = false;
                    }

                    double buzz_currentTime = timeLapsed - BuzzStartMillis;
                    if (buzz_currentTime <= (BuzzDuration - Time.deltaTime * 1000))
                    {
                        if (!m_activeHaptic)
                        {
                            for (int k = 0; k < 3; k++)
                                dir[k] = 1.0f;

                            ActivateTouchHaptic(lowGain, magnitude, frequency, dir);

                           
                            m_activeHaptic = true;
                        }

                    }
                    else
                    {
                        //deactivate haptic 
                        if (m_activeHaptic)
                        {
                            DeactivateTouchHaptic();
                            m_activeHaptic = false;
                        }

                        //m_startCoRoutine = false;
                        m_first_buzz_loop = true;
                        exitCoroutineBuzzLoop = true;
                    }
                }

                // run it 8 times and add 
                if (exitCoroutineLEDLoop && exitCoroutineBuzzLoop)
                {
                    if (loopCounter < loopNum - 1)
                    {
                        m_startCoRoutine = false;

                        StartCoroutine(waitForSecondsAndReset(1.86f));

                    }
                    else
                    {
                        m_startCoRoutine = false;
                        //StartCoroutine(WaitForKeyDown());

                    }
                }
            }
        }
        else
        {
            startTime = startTime + Time.deltaTime;
        }
    }

    IEnumerator waitForSecondsAndReset(float sec)
    {
        yield return new WaitForSecondsRealtime(sec);

        //check the order of random trials

        // reset variables
        timeLapsed = 0; //reset clock 
        m_startCoRoutine = true;
        exitCoroutineLEDLoop = false;
        exitCoroutineBuzzLoop = false;

        loopCounter++;
    }

}
