using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Linq;

using Random = System.Random;

public class PracticeTOJ : MonoBehaviour
{
    List<int> shuffledComb;
    string filePath = "C:/Users/dailyuser/Documents/Megha/repos/VisualTactileUnity/UnityTactileVisual/Data/Subjects/TOJPracticeOutput.csv";

    //Touch haptic 
    public HapticPlugin HapticDevice = null;
    private int FXID = -1;
    [NonSerialized] public float frequency = 300.0f;
    [NonSerialized] public float gain = 1.5f;
    [NonSerialized] public float lowGain = 0.5f;
    [NonSerialized] public float magnitude = 1.2f;
    //public bool m_startExposure = true; 

    //[NonSerialized] public double[] dir = { 1.0, 1.0, 1.0 };
    double[] dir = { 1.0, 1.0, 1.0 };

    public RunMenu triggerMenuMsg;
    public ExposureHapticStylusDeltaTime exposureObject;

    int score = 0; 

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
    int blockCount = 2;// make it 36 x 3 

    int expectedScore = 40; //approx 70% of 46

    double timeLapsed = 0.0f;
    double startTime = 0.0f;

    float _mIntensityLED = 1.8f;
    float _mIntensityLEDLow = 0.5f;

    PracticeExposure taskPracticeExposure;

    public bool m_startPreExposureTOJ = false; 

    //audio 
    [SerializeField] public AudioClip beepsoundCorrect;
    [SerializeField] public AudioClip beepsoundWrong;
    [SerializeField] public AudioSource beep;

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
        var canvas = GameObject.Find("InstructionsMenu");
        triggerMenuMsg = canvas.GetComponent<RunMenu>();

        //read file and generate list 
        List<int> comb = new List<int>(listFromFile("C:/Users/dailyuser/Documents/Megha/repos/VisualTactileUnity/UnityTactileVisual/Data/TOJPracticeConditions.csv", 1));

        //shuffle 
        Random rng = new Random();
        shuffledComb = comb.OrderBy(a => rng.Next()).ToList();
        tempListCount = shuffledComb.Count;

        writeToFile("AsynchronyVal, LEDDelay, BuzzDelay, correctResponse, subjectResponse, stimulusDuration", filePath);
        

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

        //get exposure script 
        taskPracticeExposure = GetComponent<PracticeExposure>(); 
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (startTime > 1.0f)
        {
            timeLapsed = timeLapsed + Time.deltaTime * 1000.0f;

            if (m_startCoRoutine && triggerMenuMsg.startExperiment && taskPracticeExposure.m_startPracticeTOJ) // if exposure script gives nod to run TOJ
            //if (m_startCoRoutine && triggerMenuMsg.startExperiment) // if exposure script gives nod to run TOJ
            {
                if (timeLapsed > LEDDelay && !exitCoroutineLEDLoop && triggerMenuMsg.startExperiment) // problem in timing 
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
                        GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", new Vector4(191.0f / 255f, 180f / 255f, 180f / 255f, 1f) * Mathf.Pow(2, _mIntensityLED)); //To get HDR intensity is pow of 2

                        //GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", new Vector4(191.0f / 255f, 180f / 255f, 180f / 255f, 1f) * Mathf.Pow(2, 2.4f)); //To get HDR intensity is pow of 2
                    }
                    else
                    {
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
                            for (int k = 0; k < 3; k++)
                                dir[k] = 1.0f;

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

        //record response
        if (Input.GetKey("right"))
        {
            subjectResponse = 2; //light was first
        }

        if ((Input.GetKey("left")))
        {
            subjectResponse = 1; //tactile was first
        }

        //feedback 
        if (subjectResponse == correctResponse)
        {
            beep.PlayOneShot(beepsoundCorrect);
            score++; 
        }
        else 
        {
            beep.PlayOneShot(beepsoundWrong);
            yield return new WaitForSecondsRealtime(0.3f);
            beep.PlayOneShot(beepsoundWrong);
        }

        decimal[] arr = { (decimal)shuffledComb[tempList], (decimal)LEDDelay, (decimal)BuzzDelay, (decimal)correctResponse, (decimal)subjectResponse, (decimal)BuzzDuration };
        writeToFile(string.Join(", ", arr), filePath);

        subjectResponse = 0;

        yield return new WaitForSecondsRealtime(0.7f);

        if (tempList < tempListCount - 1)
        {
            tempList++;

            //reset values
            LEDDelay = 0; // commented for debugging
            BuzzDelay = 0;

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

            timeLapsed = 0; //reset clock 
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

                timeLapsed = 0; //reset clock 
                m_startCoRoutine = true;

                exitCoroutineLEDLoop = false;
                exitCoroutineBuzzLoop = false;
            }
            else
            {
                if (score < expectedScore)
                {
                    triggerMenuMsg.startExperiment = false;
                    triggerMenuMsg.index = 6;
                    triggerMenuMsg.runCoRoutine = true;
                    ResetBlockTOJ(); // to run after exposure 

                    score = 0; 
                }
                else 
                {
                    triggerMenuMsg.startExperiment = false;
                    triggerMenuMsg.index = 10; // Megha change index
                    triggerMenuMsg.runCoRoutine = true;

                    taskPracticeExposure.m_startPracticeTOJ = false;
                    m_startPreExposureTOJ = true; 
                }

            }
        }

    }

    private void ResetBlockTOJ()
    {
        //reshuffle and restart the list 
        blockrun = 0;

        Random rng = new Random();
        shuffledComb = shuffledComb.OrderBy(a => rng.Next()).ToList();

        tempList = 0;

        timeLapsed = 0; //reset clock 
        m_startCoRoutine = true;

        exitCoroutineLEDLoop = false;
        exitCoroutineBuzzLoop = false;
    }
}
