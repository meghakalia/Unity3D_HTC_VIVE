using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Linq;

using Random = System.Random;

public class PostExposure2 : MonoBehaviour
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

    double LEDDelay = 0.0f;
    double BuzzDelay = 0.0f;

    double LEDStartMillis = 0.0f;
    double BuzzStartMillis = 0.0f;

    double LEDDuration = 80f; //even numbers 
    double BuzzDuration = 80f;

    bool m_first_LED_loop = true;
    bool m_first_buzz_loop = true;

    bool exitCoroutineLEDLoop = false;
    bool exitCoroutineBuzzLoop = false;

    bool m_activeHaptic = false;
    bool m_activeLED = false;

    int subjectResponse = 0;
    int correctResponse = 0;

    int blockrun = 0;
    int blockCount = 2;

    double timeLapsed = 0.0f;

    double startTime = 0.0f; 

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
        //Debug.Log("testTOJ " + " -- " + (new System.Diagnostics.StackFrame(0, true)).GetFileLineNumber());
        //read file and generate list 
        List<int> comb = new List<int>(listFromFile("C:/Users/megha/Documents/Unity/visualTactile/Data/TOJConditions.csv", 1));
        //Debug.Log("testTOJ " + " -- " + (new System.Diagnostics.StackFrame(0, true)).GetFileLineNumber());

        //shuffle 
        Random rng = new Random();
        shuffledComb = comb.OrderBy(a => rng.Next()).ToList();
        tempListCount = shuffledComb.Count;
        //Debug.Log("testTOJ " + " -- " + (new System.Diagnostics.StackFrame(0, true)).GetFileLineNumber());

        writeToFile("AsynchronyVal, LEDDelay, BuzzDelay, correctResponse, subjectResponse, stimulusDuration", filePath);
        //Debug.Log("testTOJ " + " -- " + (new System.Diagnostics.StackFrame(0, true)).GetFileLineNumber());

        //exposure object
        //var exposure = GameObject.Find("LEDCylinder");
        //ExposureScript = exposure.GetComponent<ExposureHapticStylus>();
        //Debug.Log("testTOJ " + " -- " + (new System.Diagnostics.StackFrame(0, true)).GetFileLineNumber());

        //haptic Touch 
        if (HapticDevice == null)
            HapticDevice = (HapticPlugin)FindObjectOfType(typeof(HapticPlugin));

        Application.targetFrameRate = -1;

        prevTime = Time.realtimeSinceStartupAsDouble * 1000.0f;

        //read first value from the list shuffled list
        if (shuffledComb[tempList] < 0.0f) // negative means vision is delayed and tactile first 
        {
            LEDDelay = Mathf.Abs(shuffledComb[tempList]); // check this value
            correctResponse = 1; //tactile first 
        }
        else
        {
            BuzzDelay = shuffledComb[tempList];
            correctResponse = 2; //vision first
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (startTime > 1.0f)
        {
            timeLapsed = timeLapsed + Time.deltaTime * 1000.0f;

            LEDDelay = 0;
            BuzzDelay = 0;

            if (m_startCoRoutine)
            {
                if (timeLapsed > LEDDelay && !exitCoroutineLEDLoop) // problem in timing 
                {
                    if (m_first_LED_loop) //runs only once 
                    {
                        LEDStartMillis = timeLapsed;
                        m_first_LED_loop = false;
                    }

                    double checkTime = timeLapsed - LEDStartMillis;
                    //Debug.Log("checkTime " + " -- " + checkTime);
                    if (checkTime <= (LEDDuration - Time.deltaTime * 1000))
                    {
                        Debug.Log("LEDflag enter " + " -- " + checkTime);
                        GetComponent<MeshRenderer>().material.EnableKeyword("_EMISSION");
                    }
                    else
                    {
                        Debug.Log("LEDflag finish " + " -- " + checkTime);
                        GetComponent<MeshRenderer>().material.DisableKeyword("_EMISSION");

                        m_first_LED_loop = true;
                        exitCoroutineLEDLoop = true;
                    }

                }

                if (timeLapsed > BuzzDelay && !exitCoroutineBuzzLoop) // problem in timing 
                {
                    if (m_first_buzz_loop) //runs only once 
                    {
                        BuzzStartMillis = timeLapsed;
                        m_first_buzz_loop = false;
                    }

                    double buzz_currentTime = timeLapsed - BuzzStartMillis;
                    if (buzz_currentTime <= (BuzzDuration - Time.deltaTime * 1000))
                    {
                        //activate haptic once 
                        if (!m_activeHaptic)
                        {
                            ActivateTouchHaptic(gain, magnitude, frequency, dir);
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

                if (exitCoroutineLEDLoop && exitCoroutineBuzzLoop)
                {
                    m_startCoRoutine = false;
                    StartCoroutine(WaitForKeyDown());
                }
            }

        }
        else 
        { 
           startTime = startTime + Time.deltaTime; 
        }
    }
    IEnumerator WaitForKeyDown()
    {
        while (!(Input.GetKey("right")) && !(Input.GetKey("left")))
            //display the message here 
            yield return null;

        yield return new WaitForSecondsRealtime(0.7f);

        //record response
        if ((Input.GetKey("right")))
        {
            subjectResponse = 1; //tactile was first
        }

        if ((Input.GetKey("left")))
        {
            subjectResponse = 2; //vision was first
        }

        decimal[] arr = { (decimal)shuffledComb[tempList], (decimal)LEDDelay, (decimal)BuzzDelay, (decimal)correctResponse, (decimal)subjectResponse, (decimal)BuzzDuration };
        writeToFile(string.Join(", ", arr), filePath);
        //update the delay values 
        //update tempList

        if (tempList < tempListCount-1)
        {
            tempList++;

            //reset values
            LEDDelay    = 0;
            BuzzDelay   = 0; 

            //check if tempList is the end of hte list 
            if (shuffledComb[tempList] < 0.0f) // negative means vision is delayed and tactile first 
            {
                LEDDelay = Mathf.Abs(shuffledComb[tempList]);
                correctResponse = 1; //tactile first 
            }
            else
            {
                BuzzDelay = shuffledComb[tempList];
                correctResponse = 2; //vision first
            }

            m_startCoRoutine = true;
            exitCoroutineLEDLoop = false;
            exitCoroutineBuzzLoop = false;
        }
        else 
        {
            
            blockrun++;
            if (blockrun != blockCount)
            {
                //reshuffle and restart the list 
                Random rng = new Random();
                shuffledComb = shuffledComb.OrderBy(a => rng.Next()).ToList();

                tempList = 0;
                m_startCoRoutine = true;

                exitCoroutineLEDLoop = false;
                exitCoroutineBuzzLoop = false;
            }
        }

       
    }
}

        //IEnumerator TOJCoroutine()
        //{
        //    //Debug.Log("timeDelta " + " -- " + Time.deltaTime * 1000.0f);
        //    m_startCoRoutine = false; //stop coroutine from starting again

        //    if (blockrun < blockCount)
        //    {
        //        //Debug.Log("Delta time " + " -- " + Time.deltaTime * 1000.0f);

        //        //    int m_flashCount = 0;
        //        // this loop will be run 108 times in a block of 36 
        //        if (tempList != tempListCount) // run till the list is empty
        //        {
        //            //timeParsed = timeParsed + Time.deltaTime;

        //            // read the delay value 
        //            if (shuffledComb[tempList] < 0.0f) // negative means vision is delayed and tactile first 
        //            {
        //                LEDDelay = shuffledComb[tempList]; // check this value
        //                correctResponse = 1; //tactile first 
        //            }
        //            else
        //            {
        //                BuzzDelay = shuffledComb[tempList];
        //                correctResponse = 2; //vision first
        //            }

        //            //for debugging
        //            LEDDelay = 240;
        //            BuzzDelay = 0;

        //            while (!exitCoroutineLEDLoop || !exitCoroutineBuzzLoop)
        //            {
        //                if (((Time.realtimeSinceStartupAsDouble * 1000.0f) - prevTime) > LEDDelay) // problem in timing 
        //                {

        //                    //Debug.Log("start LED Delay " + " -- " + ((Time.realtimeSinceStartupAsDouble * 1000.0f) - prevTime)); 

        //                    if (m_first_LED_loop) //runs only once 
        //                    {
        //                        LEDStartMillis = Time.realtimeSinceStartupAsDouble * 1000.0f;
        //                        m_first_LED_loop = false;
        //                    }

        //                    if (((Time.realtimeSinceStartupAsDouble * 1000.0f) - LEDStartMillis) <= LEDDuration) // Problem is here in this line 
        //                    {
        //                        //Debug.Log("start LED 1" + " -- " + ((Time.realtimeSinceStartupAsDouble * 1000.0f) - LEDStartMillis));
        //                        GetComponent<MeshRenderer>().material.EnableKeyword("_EMISSION");
        //                        //Debug.Log("timeDelta start" + " -- " + ((Time.realtimeSinceStartupAsDouble * 1000.0f) - LEDStartMillis));
        //                        //Debug.Log("start LED 1 finish" + " -- " + ((Time.realtimeSinceStartupAsDouble * 1000.0f) - LEDStartMillis));
        //                        //yield return null; // this line is adding a lot of delay
        //                        yield return new WaitForFixedUpdate();
        //                        //Debug.Log("timeDelta end" + " -- " + ((Time.realtimeSinceStartupAsDouble * 1000.0f) - LEDStartMillis));
        //                        //Debug.Log("start LED 1 finish after yield" + " -- " + ((Time.realtimeSinceStartupAsDouble * 1000.0f) - LEDStartMillis));

        //                    }
        //                    else
        //                    {
        //                        //Debug.Log("timeDelta finish " + " -- " + ((Time.realtimeSinceStartupAsDouble * 1000.0f) - LEDStartMillis));
        //                        //Debug.Log("finish start LED 1" + " -- " + ((Time.realtimeSinceStartupAsDouble * 1000.0f) - LEDStartMillis));
        //                        GetComponent<MeshRenderer>().material.DisableKeyword("_EMISSION");
        //                        exitCoroutineLEDLoop = true;
        //                        //exitCoroutineBuzzLoop = true; // only for debug
        //                    }

        //                }
        //                else
        //                {
        //                    // for debugging
        //                    //Debug.Log("finish start LED Delay " + " -- " + ((Time.realtimeSinceStartupAsDouble * 1000.0f) - prevTime));

        //                }

        //                if (((Time.realtimeSinceStartupAsDouble * 1000.0f) - prevTime) > BuzzDelay)
        //                {
        //                    //Debug.Log("testTOJ " + " -- " + (new System.Diagnostics.StackFrame(0, true)).GetFileLineNumber());
        //                    //Debug.Log("start Buzz Delay " + " -- " + ((Time.realtimeSinceStartupAsDouble * 1000.0f) - prevTime)); // 2ms with 0 delay
        //                    if (m_first_buzz_loop) //runs only once 
        //                    {
        //                        //Debug.Log("testTOJ " + " -- " + (new System.Diagnostics.StackFrame(0, true)).GetFileLineNumber());

        //                        BuzzStartMillis = Time.realtimeSinceStartupAsDouble * 1000.0f;
        //                        //Debug.Log("testTOJ " + " -- " + (new System.Diagnostics.StackFrame(0, true)).GetFileLineNumber());

        //                        m_first_buzz_loop = false;
        //                    }

        //                    if (((Time.realtimeSinceStartupAsDouble * 1000.0f) - BuzzStartMillis) <= BuzzDuration)
        //                    {
        //                        //Debug.Log("testTOJ " + " -- " + (new System.Diagnostics.StackFrame(0, true)).GetFileLineNumber());
        //                        //Debug.Log("start Buzz 1" + " -- " + ((Time.realtimeSinceStartupAsDouble * 1000.0f) - BuzzStartMillis));
        //                        //Debug.Log("start Buzz 1 haptic" + " -- " + ((Time.realtimeSinceStartupAsDouble * 1000.0f) - BuzzStartMillis));
        //                        if (!m_activeHaptic)
        //                        {
        //                            ActivateTouchHaptic(gain, magnitude, frequency, dir);
        //                            m_activeHaptic = true;
        //                        }
        //                        //Debug.Log("start Buzz 1 haptic finish" + " -- " + ((Time.realtimeSinceStartupAsDouble * 1000.0f) - BuzzStartMillis));
        //                        yield return null;

        //                    }
        //                    else
        //                    {
        //                        // this loop is taking 10-20 ms to run : crazy 
        //                        //Debug.Log("finish start Buzz 1" + " -- " + ((Time.realtimeSinceStartupAsDouble * 1000.0f) - BuzzStartMillis));
        //                        //Debug.Log("finish start Buzz 1 haptic" + " -- " + ((Time.realtimeSinceStartupAsDouble * 1000.0f) - BuzzStartMillis));
        //                        if (m_activeHaptic)
        //                        {
        //                            DeactivateTouchHaptic();
        //                            m_activeHaptic = false;
        //                        }

        //                        //Debug.Log("finish start Buzz 1 haptic finish" + " -- " + ((Time.realtimeSinceStartupAsDouble * 1000.0f) - BuzzStartMillis));
        //                        exitCoroutineBuzzLoop = true;
        //                    }
        //                }
        //                else
        //                {
        //                    // for debugging
        //                    //Debug.Log("finish start buzz Delay " + " -- " + ((Time.realtimeSinceStartupAsDouble * 1000.0f) - prevTime));

        //                }
        //            }

        //            yield return StartCoroutine(WaitForKeyDown());
        //        }
        //        else if (tempList == tempListCount)
        //        {
        //            // reset tempList variable and also reshuffle the list
        //            tempList = 0;

        //            Random rng = new Random();
        //            shuffledComb = shuffledComb.OrderBy(a => rng.Next()).ToList();

        //            //run the stimulus
        //            // read the delay value 
        //            if (shuffledComb[tempList] < 0.0f) // negative means vision is delayed and tactile first 
        //            {
        //                LEDDelay = shuffledComb[tempList];
        //                correctResponse = 1; //tactile first 
        //            }
        //            else
        //            {
        //                BuzzDelay = shuffledComb[tempList];
        //                correctResponse = 2; //vision first
        //            }


        //            while (!exitCoroutineLEDLoop || !exitCoroutineBuzzLoop)
        //            {
        //                if (((Time.realtimeSinceStartupAsDouble * 1000.0f) - prevTime) > LEDDelay)
        //                {
        //                    if (m_first_LED_loop) //runs only once 
        //                    {
        //                        LEDStartMillis = Time.realtimeSinceStartupAsDouble * 1000.0f;
        //                        m_first_LED_loop = false;
        //                    }

        //                    if (((Time.realtimeSinceStartupAsDouble * 1000.0f) - LEDStartMillis) <= LEDDuration) // Problem is here in this line 
        //                    {
        //                        GetComponent<MeshRenderer>().material.EnableKeyword("_EMISSION");
        //                        yield return null;
        //                    }
        //                    else
        //                    {
        //                        GetComponent<MeshRenderer>().material.DisableKeyword("_EMISSION");
        //                        exitCoroutineLEDLoop = true;
        //                        //exitCoroutineBuzzLoop = true; // only for debug
        //                    }
        //                }

        //                if (((Time.realtimeSinceStartupAsDouble * 1000.0f) - prevTime) > BuzzDelay)
        //                {
        //                    //Debug.Log("testTOJ " + " -- " + (new System.Diagnostics.StackFrame(0, true)).GetFileLineNumber());

        //                    if (m_first_buzz_loop) //runs only once 
        //                    {
        //                        //Debug.Log("testTOJ " + " -- " + (new System.Diagnostics.StackFrame(0, true)).GetFileLineNumber());

        //                        BuzzStartMillis = Time.realtimeSinceStartupAsDouble * 1000.0f;
        //                        //Debug.Log("testTOJ " + " -- " + (new System.Diagnostics.StackFrame(0, true)).GetFileLineNumber());

        //                        m_first_buzz_loop = false;
        //                    }

        //                    if (((Time.realtimeSinceStartupAsDouble * 1000.0f) - BuzzStartMillis) <= BuzzDuration)
        //                    {
        //                        //Debug.Log("testTOJ " + " -- " + (new System.Diagnostics.StackFrame(0, true)).GetFileLineNumber());

        //                        if (!m_activeHaptic)
        //                        {
        //                            ActivateTouchHaptic(gain, magnitude, frequency, dir);
        //                            m_activeHaptic = true;
        //                        }

        //                        yield return null;

        //                    }
        //                    else
        //                    {
        //                        if (m_activeHaptic)
        //                        {
        //                            DeactivateTouchHaptic();
        //                            m_activeHaptic = false;
        //                        }

        //                        exitCoroutineBuzzLoop = true;
        //                    }
        //                }
        //            }

        //            blockrun++;

        //            // At the end of the script stimlus will run but will not be recorded 
        //            yield return StartCoroutine(WaitForKeyDown()); //get user input 
        //        }
        //    }

        //}


        //IEnumerator WaitForKeyDown()
        //{
        //    while (!(Input.GetKey("right")) && !(Input.GetKey("left")))
        //        //display the message here 
        //        yield return null;

        //    if ((Input.GetKey("right")))
        //    {
        //        subjectResponse = 1; //tactile was first
        //    }

        //    if ((Input.GetKey("left")))
        //    {
        //        subjectResponse = 2; //vision was first
        //    }

        //    //write to file 
        //    if (blockrun < blockCount)
        //    {
        //        decimal[] arr = { (decimal)shuffledComb[tempList], (decimal)correctResponse, (decimal)subjectResponse, (decimal)BuzzDuration };
        //        writeToFile(string.Join(", ", arr), filePath);
        //    }
        //    //writeToFile("AsynchronyVal,correctResponse, subjectResponse, stimulusDuration", filePath);

        //    // increment tempList 
        //    tempList++;

        //    yield return new WaitForSecondsRealtime(0.5f); // wait before starting the next stimulus

        //    m_startCoRoutine = true;

        //    m_first_LED_loop = true;
        //    m_first_buzz_loop = true;

        //    exitCoroutineLEDLoop = false;
        //    exitCoroutineBuzzLoop = false; // only for debug
        //                                   // we can wait before giving the next stimulus 



    



