using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Linq;

using Random = System.Random;

public class PostExposure : MonoBehaviour
{
    List<int> shuffledComb;
    string filePath = "C:/Users/megha/Documents/Unity/visualTactile/Data/Subjects/TestingTOJ.csv";

    //Touch haptic 
    public HapticPlugin HapticDevice = null;
    private int FXID = -1;
    [NonSerialized] public float frequency = 300.0f;
    [NonSerialized] public float gain = 1.5f;
    [NonSerialized] public float lowGain = 0.5f;
    [NonSerialized] public float magnitude = 1.2f;
    [NonSerialized] public double[] dir = { 1.0, 1.0, 1.0 };

    public RunMenu triggerMenuMsg;

    //public ExposureHapticStylus ExposureScript; 

    bool m_startCoRoutine = true;

    float time_delay = 0f;
    public float timePeriod = 1.0f; // based on the MATLAB script
    float timeParsed = 0;

    int tempListCount = 0;
    int tempList = 0;

    int stimulusDuration = 70;

    double prevTime = 0.0f;
    double currentTime = 0.0f;

    bool runLoop = false;

    double LEDDelay  = 0.0f;
    double BuzzDelay = 0.0f;

    double LEDStartMillis = 0.0f;
    double BuzzStartMillis = 0.0f;

    double LEDDuration = 70f;
    double BuzzDuration = 70f;

    bool m_first_LED_loop = true;
    bool m_first_buzz_loop = true;

    bool exitCoroutineLEDLoop  = false;
    bool exitCoroutineBuzzLoop = false;

    bool m_activeHaptic = false; 

    //functions
    List<int> generateRand(int numCount)
    {
        var rand = new Random();
        List<int> listNumbers = new List<int>();
        int number;
        for (int i = 0; i < numCount; i++)
        {
            do
            {
                number = rand.Next(1, 8);
            } while (listNumbers.Contains(number));
            listNumbers.Add(number);
        }

        return listNumbers;
    }

    List<int> listFromFile(string FilePath, int repetitions)
    {
        List<int> comb = new List<int>();
        using (var reader = new StreamReader(FilePath))
        {
            while (!reader.EndOfStream)
            {
                //List<int> pair = new List<int>();

                var line = reader.ReadLine();
                var values = line.Split(',');

                comb.Add(Convert.ToInt32(values[0]));

            }
        }

        var l = Enumerable.Repeat('x', 5).ToList();

        var serviceEndPoints = comb.SelectMany(t => Enumerable.Repeat(t, repetitions)).ToList();

        //for (int i = 0; i < serviceEndPoints.Count; i++)
        //{
        //    Console.WriteLine(serviceEndPoints[i]);
        //}

        return serviceEndPoints;
    }

    void writeToFile(string str_output, string filePath)
    {
        //do this only once 
        //FileStream writer = File.OpenWrite(filePath);

        using FileStream fileStream = File.Open(filePath, FileMode.Append);
        using StreamWriter writer = new StreamWriter(fileStream);

        writer.WriteLine(str_output);

        writer.Flush();
        writer.Close();

    }

    void DeactivateTouchHaptic()
    {
        if (HapticDevice == null) return;       //If there is no device, bail out early.
        if (FXID == -1) return;                 //If there is no effect, bail out early.

        HapticPlugin.effects_stopEffect(HapticDevice.configName, FXID);
    }
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

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("testTOJ " + " -- " + (new System.Diagnostics.StackFrame(0, true)).GetFileLineNumber());
        //read file and generate list 
        List<int> comb = new List<int>(listFromFile("C:/Users/megha/Documents/Unity/visualTactile/Data/TOJConditions.csv", 2));
        Debug.Log("testTOJ " + " -- " + (new System.Diagnostics.StackFrame(0, true)).GetFileLineNumber());

        //shuffle 
        Random rng = new Random();
        shuffledComb = comb.OrderBy(a => rng.Next()).ToList();
        tempListCount = shuffledComb.Count;
        Debug.Log("testTOJ " + " -- " + (new System.Diagnostics.StackFrame(0, true)).GetFileLineNumber());

        writeToFile("AsynchronyVal,correctResponse, subjectResponse, stimulusDuration", filePath);
        Debug.Log("testTOJ " + " -- " + (new System.Diagnostics.StackFrame(0, true)).GetFileLineNumber());

        //exposure object
        //var exposure = GameObject.Find("LEDCylinder");
        //ExposureScript = exposure.GetComponent<ExposureHapticStylus>();
        Debug.Log("testTOJ " + " -- " + (new System.Diagnostics.StackFrame(0, true)).GetFileLineNumber());

        //haptic Touch 
        if (HapticDevice == null)
            HapticDevice = (HapticPlugin)FindObjectOfType(typeof(HapticPlugin));
        Debug.Log("testTOJ " + " -- " + (new System.Diagnostics.StackFrame(0, true)).GetFileLineNumber());

        //for debugging 
        //ExposureScript.m_Start_TOJ = true;
        //Debug.Log("testTOJ " + " end of start-- " + (new System.Diagnostics.StackFrame(0, true)).GetFileLineNumber());


    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log("testTOJ " + " -- " + (new System.Diagnostics.StackFrame(0, true)).GetFileLineNumber());

        if (time_delay > 1.0f)
        {
            //Debug.Log("testTOJ " + " -- " + (new System.Diagnostics.StackFrame(0, true)).GetFileLineNumber());

            //if (m_startCoRoutine && ExposureScript.m_Start_TOJ)
            //{
            if (m_startCoRoutine)
            {
                    //Debug.Log("testTOJ " + " -- " + (new System.Diagnostics.StackFrame(0, true)).GetFileLineNumber());

                    prevTime = Time.realtimeSinceStartupAsDouble / 1000; //milliseconds

                //Debug.Log("testTOJ " + " -- " + (new System.Diagnostics.StackFrame(0, true)).GetFileLineNumber());

                StartCoroutine(TOJCoroutine());
            }
        }
        else
        {
            time_delay = time_delay + Time.deltaTime;
        }
    }


    IEnumerator TOJCoroutine()
    {
        m_startCoRoutine = false; //stop coroutine from starting again
        Debug.Log("testTOJ " + " -- " + (new System.Diagnostics.StackFrame(0, true)).GetFileLineNumber());

        //if (blockrun < blockCount)
        //{
        //    int m_flashCount = 0;
        // this loop will be run 108 times in a block of 36 
        if (tempList != tempListCount) // run till the list is empty
        {
            Debug.Log("testTOJ " + " -- " + (new System.Diagnostics.StackFrame(0, true)).GetFileLineNumber());

            timeParsed = timeParsed + Time.deltaTime;
            Debug.Log("testTOJ " + " -- " + (new System.Diagnostics.StackFrame(0, true)).GetFileLineNumber());

            while (!exitCoroutineLEDLoop || !exitCoroutineBuzzLoop)
            {
                if (((Time.realtimeSinceStartupAsDouble / 1000.0f) - prevTime) > LEDDelay)
                {
                    Debug.Log("testTOJ " + " -- " + (new System.Diagnostics.StackFrame(0, true)).GetFileLineNumber());

                    if (m_first_LED_loop) //runs only once 
                    {
                        Debug.Log("testTOJ " + " -- " + (new System.Diagnostics.StackFrame(0, true)).GetFileLineNumber());

                        LEDStartMillis = Time.realtimeSinceStartupAsDouble / 1000.0f;
                        m_first_LED_loop = false;
                    }

                    //Debug.Log("testTOJ " + " -- " + (new System.Diagnostics.StackFrame(0, true)).GetFileLineNumber());

                    if (((Time.realtimeSinceStartupAsDouble / 1000.0f) - LEDStartMillis) <= LEDDuration) // Problem is here in this line 
                    {
                        //Debug.Log("testTOJ " + " -- " + (new System.Diagnostics.StackFrame(0, true)).GetFileLineNumber());

                        //GetComponent<MeshRenderer>().material.EnableKeyword("_EMISSION");
                        //Debug.Log("testTOJ " + " -- " + (new System.Diagnostics.StackFrame(0, true)).GetFileLineNumber());

                        //GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", new Vector4(191.0f / 255f, 180f / 255f, 180f / 255f, 1f) * Mathf.Pow(2, 2.9f)); //To get HDR intensity is pow of 2
                        //Debug.Log("testTOJ " + " -- " + (new System.Diagnostics.StackFrame(0, true)).GetFileLineNumber());

                    }
                    else
                    {
                        //GetComponent<MeshRenderer>().material.DisableKeyword("_EMISSION");
                        Debug.Log("testTOJ " + " -- " + (new System.Diagnostics.StackFrame(0, true)).GetFileLineNumber());

                        exitCoroutineLEDLoop = true;
                    }
                }

                //if (((Time.realtimeSinceStartupAsDouble / 1000.0f) - prevTime) > BuzzDelay)
                //{
                //    Debug.Log("testTOJ " + " -- " + (new System.Diagnostics.StackFrame(0, true)).GetFileLineNumber());

                //    if (m_first_buzz_loop) //runs only once 
                //    {
                //        Debug.Log("testTOJ " + " -- " + (new System.Diagnostics.StackFrame(0, true)).GetFileLineNumber());

                //        BuzzStartMillis = Time.realtimeSinceStartupAsDouble / 1000.0f;
                //        Debug.Log("testTOJ " + " -- " + (new System.Diagnostics.StackFrame(0, true)).GetFileLineNumber());

                //        m_first_buzz_loop = false;
                //    }

                //    if (((Time.realtimeSinceStartupAsDouble / 1000.0f) - BuzzStartMillis) <= BuzzDuration)
                //    {
                //        Debug.Log("testTOJ " + " -- " + (new System.Diagnostics.StackFrame(0, true)).GetFileLineNumber());

                //        if (!m_activeHaptic)
                //        {
                //            ActivateTouchHaptic(gain, magnitude, frequency, dir);
                //            m_activeHaptic = true; 
                //        }
                        
                //        Debug.Log("testTOJ " + " -- " + (new System.Diagnostics.StackFrame(0, true)).GetFileLineNumber());

                //    }
                //    else
                //    {
                //        if (m_activeHaptic)
                //        {
                //            DeactivateTouchHaptic();
                //            m_activeHaptic = false;
                //        }

                //        Debug.Log("testTOJ " + " -- " + (new System.Diagnostics.StackFrame(0, true)).GetFileLineNumber());

                //        exitCoroutineBuzzLoop = true;
                //        Debug.Log("testTOJ " + " -- " + (new System.Diagnostics.StackFrame(0, true)).GetFileLineNumber());

                //    }
                //}

            }
            yield return StartCoroutine(WaitForKeyDown());
        }
    }

    IEnumerator WaitForKeyDown()
    {
        Debug.Log("testTOJ " + " -- " + (new System.Diagnostics.StackFrame(0, true)).GetFileLineNumber());

        while (!(Input.GetKey("right")) && !(Input.GetKey("left")))
            //display the message here 
            yield return null;

        Debug.Log("testTOJ " + " -- " + (new System.Diagnostics.StackFrame(0, true)).GetFileLineNumber());

        // increment tempList 
        Debug.Log(" got input ");
        tempList++;

        m_startCoRoutine = true;

        // we can wait before giving the next stimulus 
    }

    //IEnumerator WaitForKeyDown()
    //{
    //    while (!(Input.GetKey("right")) && !(Input.GetKey("left")))
    //        //display the message here 
    //        yield return null;

    //    if ((Input.GetKey("right")))
    //    {
    //        subjectResponse = 1;
    //    }

    //    if ((Input.GetKey("left")))
    //    {
    //        subjectResponse = 2;
    //    }

    //    //write the response to a file 
    //    int numVisLow = 8 - shuffledComb[tempList][0];
    //    int numTactLow = 8 - shuffledComb[tempList][1];

    //    if (numVisLow < numTactLow)
    //    {
    //        correctResponse = 1; //vision has hihger number of high intensity responses
    //    }
    //    else
    //    {
    //        correctResponse = 2; //tactile has hihger number of high intensity responses
    //    }

    //    //AsynchronyVal, numVisLow, numTactLow, correctResponse, subjectResponse, stimulusDuration
    //    decimal[] arr = { 0.0m, (decimal)numVisLow, (decimal)numTactLow, (decimal)correctResponse, (decimal)subjectResponse, (decimal)stimulusDuration };
    //    writeToFile(string.Join(", ", arr));

    //    // increment tempList 
    //    tempList++;

    //    triggerMenuMsg.startExperiment = true;
    //    //triggerMenuMsg.index += 1;

    //    //check response and write the response to a file 
    //    m_startCoRoutine = true;

    //    //wait for 1 second and start again 
    //    Debug.Log(" got input ");
    //}

}
