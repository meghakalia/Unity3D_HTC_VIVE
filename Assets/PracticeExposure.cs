using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using System;
using System.IO;
using System.Linq;

using Random = System.Random;

public class PracticeExposure : MonoBehaviour
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
    bool m_startCoRoutine = false;
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

    // file input output 
    public string filePath = "MeghaData.csv"; // to give in the editor 

    int blockCount = 1; // could be 3 in original experiment (list has 6x6)
    int blockrun = 0;

    int score = 0; 
    //key board control
    bool gameIsPaused = false;

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

    double timeLapsed = 0.0f;

    double startTime = 0.0f;

    int loopCounter = 0;
    int loopNum = 8;

    List<int> numbersRand_V;
    List<int> numbersRand_T;

    PracticeLowIntensityTactile taskLowIntensityTactile;
    public bool m_startPracticeTOJ = false; 

    int m_flashCount = 0;

    //audio 
    [SerializeField] public AudioClip beepsoundCorrect;
    [SerializeField] public AudioClip beepsoundWrong;
    [SerializeField] public AudioSource beep;

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

    List<List<int>> listFromFile(string FilePath)
    {
        List<List<int>> comb = new List<List<int>>();
        using (var reader = new StreamReader(FilePath))
        {
            while (!reader.EndOfStream)
            {
                List<int> pair = new List<int>();

                var line = reader.ReadLine();
                var values = line.Split(',');

                pair.Add(Convert.ToInt32(values[0]));
                pair.Add(Convert.ToInt32(values[1]));
                comb.Add(pair);

            }
        }

        return comb;
    }

    void writeToFile(string str_output)
    {
        //do this only once 
        //FileStream writer = File.OpenWrite(filePath);

        using FileStream fileStream = File.Open(filePath, FileMode.Append);
        using StreamWriter writer = new StreamWriter(fileStream);

        writer.WriteLine(str_output);

        writer.Flush();
        writer.Close();

    }

    //Keyboard keyPress; 
    // Start is called before the first frame update
    void Start()
    {
        var canvas = GameObject.Find("InstructionsMenu");
        triggerMenuMsg = canvas.GetComponent<RunMenu>();

        //read file and generate list 
        List<List<int>> comb = new List<List<int>>(listFromFile("C:/Users/megha/Documents/Unity/visualTactile/Data/dataExposurePractice.csv")); //predetermined pattern 
        //shuffle 
        Random rng = new Random();
        shuffledComb = comb.OrderBy(a => rng.Next()).ToList();
        tempListCount = shuffledComb.Count;

        //file input output 
        filePath = "C:/Users/megha/Documents/Unity/visualTactile/Data/Subjects/PracticeExposure.csv";
        writeToFile("AsynchronyVal, numVisLow, numTactLow, LEDDelay, BuzzDelay, correctResponse, subjectResponse, stimulusDuration");

        //haptic Touch 
        if (HapticDevice == null)
            HapticDevice = (HapticPlugin)FindObjectOfType(typeof(HapticPlugin));

        //// read the number of random stimuli from file 
        ///
        // for the first trial
        numbersRand_V = new List<int>(generateRand(8 - shuffledComb[tempList][0])); // idx of low intensity trials
        numbersRand_T = new List<int>(generateRand(8 - shuffledComb[tempList][1])); // idx of low intensity trials

        //for (int i = 0; i < shuffledComb.Count; i++)
        //{
        //    Debug.Log("ValueList start--" + shuffledComb[i][0] + " " + shuffledComb[i][1]);
        //}

        //for (int i = 0; i < numbersRand_V.Count; i++)
        //{
        //    Debug.Log("ValueList randInt start V--" + numbersRand_V[i] );
        //}

        //for (int i = 0; i < numbersRand_T.Count; i++)
        //{
        //    Debug.Log("ValueList randInt start T--" + numbersRand_T[i]);
        //}


        if (m_ExperimentLEDDelay)
        {
            LEDDelay = 160.0f;
            BuzzDelay = 0f;
        }
        else
        {
            LEDDelay = 0.0f;
            BuzzDelay = 160f;
        }

        //for practice 0  
        LEDDelay = 0.0f;
        BuzzDelay = 0f;

        //get postExposure2 object 
        //TOJObject_post = GetComponent<PostExposure2>();
        //TOJObject_pre = GetComponent<PreExposureTOJ>();

        taskLowIntensityTactile = GetComponent<PracticeLowIntensityTactile>(); 

        //audio 
        //beep = GetComponent<AudioSource>();

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

            //if (m_startCoRoutine && triggerMenuMsg.startExperiment && taskLowIntensityTactile.m_startPracticeExposure)
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
                        if (numbersRand_V.Any(x => x == loopCounter)) // check whether current trial shoudl be low intensity
                        {
                            // low intensity visual
                            GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", new Vector4(191.0f / 255f, 180f / 255f, 180f / 255f, 1f) * Mathf.Pow(2, 1.2f)); //To get HDR intensity is pow of 2
                        }
                        else
                        {
                            // high intensity visual
                            GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", new Vector4(191.0f / 255f, 180f / 255f, 180f / 255f, 1f) * Mathf.Pow(2, 2.4f)); //To get HDR intensity is pow of 2
                        }

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

                            if (numbersRand_T.Any(x => x == loopCounter)) // check whether current trial shoudl be low intensity
                            {
                                ActivateTouchHaptic(lowGain, magnitude, frequency, dir);
                            }
                            else
                            {
                                ActivateTouchHaptic(gain, magnitude, frequency, dir);
                            }

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

                        StartCoroutine(waitForSecondsAndReset(0.6f));

                    }
                    else
                    {
                        m_startCoRoutine = false;
                        StartCoroutine(WaitForKeyDown());

                    }
                }
            }
        }
        else
        {
            startTime = startTime + Time.deltaTime;
        }
    }

    public void ActivateHaptic(float amplitude, float duration)
    {
        if (controller != null)
        {
            controller.SendHapticImpulse(amplitude, duration);
        }

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



    IEnumerator WaitForKeyDown()
    {
        while (!(Input.GetKey("right")) && !(Input.GetKey("left")))
            //display the message here 
            yield return null;

        if ((Input.GetKey("right")))
        {
            subjectResponse = 1;
        }

        if ((Input.GetKey("left")))
        {
            subjectResponse = 2;
        }

        //Debug.Log("ValueList wait4Keydown templist--" + tempList);

        //for (int i = 0; i < shuffledComb.Count; i++)
        //{
        //    Debug.Log("ValueList wait4Keydown--" + shuffledComb[i][0] + " " + shuffledComb[i][1]);
        //}

        //for (int i = 0; i < numbersRand_V.Count; i++)
        //{
        //    Debug.Log("ValueList randInt wait4Keydown V--" + numbersRand_V[i]);
        //}

        //for (int i = 0; i < numbersRand_T.Count; i++)
        //{
        //    Debug.Log("ValueList randInt wait4Keydown T--" + numbersRand_T[i]);
        //}

        //write the response to a file 
        int numVisLow = 8 - shuffledComb[tempList][0];
        int numTactLow = 8 - shuffledComb[tempList][1];

        //Debug.Log("ValueList numVisLow wait4Keydown V--" + numVisLow);
        //Debug.Log("ValueList numTactLow wait4Keydown T--" + numTactLow);

       

        if (numVisLow > numTactLow)
        {
            correctResponse = 1; //vision has hihger number of high intensity responses
        }
        else
        {
            correctResponse = 2; //tactile has hihger number of high intensity responses
        }

        //Debug.Log("ValueList wait4Keydown correctResponse--" + correctResponse);
        //Debug.Log("ValueList wait4Keydown subResposne--" + subjectResponse);

        if (correctResponse == subjectResponse)
        {
            //single beep 
            beep.PlayOneShot(beepsoundCorrect);
            score++;
        }
        else
        {
            beep.PlayOneShot(beepsoundWrong);
            yield return new WaitForSecondsRealtime(0.3f);
            beep.PlayOneShot(beepsoundWrong);


        }

        //AsynchronyVal, numVisLow, numTactLow, correctResponse, subjectResponse, stimulusDuration
        decimal[] arr = { 0.0m, (decimal)numVisLow, (decimal)numTactLow, (decimal)LEDDelay, (decimal)BuzzDelay, (decimal)correctResponse, (decimal)subjectResponse, (decimal)stimulusDuration };
        writeToFile(string.Join(", ", arr));

        yield return new WaitForSecondsRealtime(0.5f);

        if (tempList < tempListCount - 1) // for some reason this is run multiple times
        {

            tempList++;
            int checkV = shuffledComb[tempList][0]; 
            int checkT = shuffledComb[tempList][1]; 
            numbersRand_V = new List<int>(generateRand(8 - shuffledComb[tempList][0])); // idx of low intensity trials
            numbersRand_T = new List<int>(generateRand(8 - shuffledComb[tempList][1])); // idx of low intensity trials

            //for (int i = 0; i < shuffledComb.Count; i++)
            //{
            //    Debug.Log("ValueList tempList--" + shuffledComb[i][0] + " " + shuffledComb[i][1]);
            //}

            //for (int i = 0; i < numbersRand_V.Count; i++)
            //{
            //    Debug.Log("ValueList randInt tempList V--" + numbersRand_V[i]);
            //}

            //for (int i = 0; i < numbersRand_T.Count; i++)
            //{
            //    Debug.Log("ValueList randInt tempList T--" + numbersRand_T[i]);
            //}

            loopCounter = 0; //reset loop counter
           
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
                //test this 
                Random rng = new Random();
                shuffledComb = shuffledComb.OrderBy(a => rng.Next()).ToList();

                //reset values 
                numbersRand_V = new List<int>(generateRand(8 - shuffledComb[tempList][0])); // idx of low intensity trials
                numbersRand_T = new List<int>(generateRand(8 - shuffledComb[tempList][1])); // idx of low intensity trials

                loopCounter = 0; //reset loop counter
                tempList = 0;
                timeLapsed = 0; //reset clock 
                m_startCoRoutine = true;
                exitCoroutineLEDLoop = false;
                exitCoroutineBuzzLoop = false;

            }
            else
            {
                //display total score 

                if (score < 9)
                {
                    ResetBlockExposure();
                    score = 0;
                    triggerMenuMsg.startExperiment = false;
                    triggerMenuMsg.index = 6;
                    triggerMenuMsg.runCoRoutine = true;
                }
                else 
                {
                    triggerMenuMsg.startExperiment = false;
                    triggerMenuMsg.index = 10; // will lead to TOJ practice
                    triggerMenuMsg.runCoRoutine = true;

                    taskLowIntensityTactile.m_startPracticeExposure = false;
                    m_startPracticeTOJ = true; 
                }
                // if score is less than 9 
                //then repeat 

                

                    ////start TOJ
                    //m_start_TOJ = true;
                

                //yield return new WaitForSecondsRealtime(0.7f);
                //wait for button press 

            }
            //block logic 
            //m_startCoRoutine = true;
        }

        //triggerMenuMsg.startExperiment = true; 
        //triggerMenuMsg.index += 1;

        //wait for 1 second and start again 
        Debug.Log(" got input ");
    }

    public void ResetBlockExposure()
    {
        blockrun = 0;

        //test this 
        Random rng = new Random();
        shuffledComb = shuffledComb.OrderBy(a => rng.Next()).ToList();

        //print debug 
        //untested
        //for (int k = 0; k < shuffledComb.Count; k++)
        //{
        //    Debug.Log("shuffledComb 1" + " -- " + shuffledComb[k][0]);
        //    Debug.Log("shuffledComb 2" + " -- " + shuffledComb[k][1]);
        //}

        //reset values 
        int checkV = shuffledComb[tempList][0];
        int checkT = shuffledComb[tempList][1]; 

        numbersRand_V = new List<int>(generateRand(8 - shuffledComb[tempList][0])); // idx of low intensity trials
        numbersRand_T = new List<int>(generateRand(8 - shuffledComb[tempList][1])); // idx of low intensity trials

        loopCounter = 0; //reset loop counter
        tempList = 0;
        timeLapsed = 0; //reset clock 
        m_startCoRoutine = true;
        exitCoroutineLEDLoop = false;
        exitCoroutineBuzzLoop = false;


    }


}
