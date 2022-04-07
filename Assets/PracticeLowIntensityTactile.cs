using System.Collections;
using UnityEngine;
using System;

using Random = System.Random;

public class PracticeLowIntensityTactile : MonoBehaviour
{
    public int subjectNum = 1;
    public string PathFolder = "C:/Users/megha/Documents/Unity/visualTactile/Data/Subjects/";
    public string FullFilePath; 
    public RunMenu triggerMenuMsg;
    public GameObject MenuCanvas;
    public practiveLowIntensityVision TaskLowIntensityVision;

    public bool m_startPracticeExposure = false; 
    bool m_startCoRoutine = true;
    float time_delay = 0f;

    int correctReponse;

    float stimulusDuration = 0.1f;

    int score = 0;

    int blockCount = 10;
    int blockrun = 0;

    int requiredScore = 8;
    int subjectResponse;

    //Touch haptic 
    public HapticPlugin HapticDevice = null;
    private int FXID = -1;
    [NonSerialized] public float frequency = 300.0f;
    [NonSerialized] public float gain = 1.5f;
    [NonSerialized] public float lowGain = 0.5f;
    [NonSerialized] public float magnitude = 1.2f;
    [NonSerialized] public double[] dir = { 1.0, 1.0, 1.0 };

    //audio 
    [SerializeField] public AudioClip beepsoundCorrect;
    [SerializeField] public AudioClip beepsoundWrong;
    [SerializeField] public AudioSource beep;

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

        TaskLowIntensityVision = GetComponent<practiveLowIntensityVision>();

        FullFilePath = PathFolder + subjectNum + "/"; 

        //haptic Touch 
        if (HapticDevice == null)
            HapticDevice = (HapticPlugin)FindObjectOfType(typeof(HapticPlugin));
    }

    // Update is called once per frame
    void Update()
    {
        if (time_delay > 1.0f)
        {
            if (m_startCoRoutine && triggerMenuMsg.startExperiment && TaskLowIntensityVision.m_startPracticeLowIntensityTactile)
            {
                StartCoroutine(Example());
            }

        }
        else
        {
            time_delay = time_delay + Time.deltaTime;
        }
    }

    IEnumerator Example()
    {
        m_startCoRoutine = false;
        var rand = new Random();
        int number = rand.Next(0, 2);

        if (number == 0)
        {
            correctReponse = 0; //first trial is low intensity 
        }
        else
        {
            correctReponse = 1; //second trial is low intensity 
        }

        int count = 0;

        if (blockrun < blockCount)
        {
            while (count < 2)
            {
                yield return new WaitForSecondsRealtime(1.0f); //wait for one second
                

                if (number == 0)
                {
                    for (int k = 0; k < 3; k++)
                        dir[k] = 1.0f;
                    //low intensity
                    ActivateTouchHaptic(lowGain, magnitude, frequency, dir);
                }
                else
                {
                    for (int k = 0; k < 3; k++)
                        dir[k] = 1.0f;
                    //high intensity
                    ActivateTouchHaptic(gain, magnitude, frequency, dir);
                }

                yield return new WaitForSecondsRealtime(stimulusDuration);
                DeactivateTouchHaptic();

                number = Mathf.Abs(number - 1); // next will be different 
                count++;
            }

            yield return StartCoroutine(WaitForKeyDown()); //get user input 
        }
        else
        {
            //check the scores and rerun the blocks 
            if (score < requiredScore)
            {
                //display message and rerun 
                triggerMenuMsg.startExperiment = false;
                triggerMenuMsg.index = 6;
                triggerMenuMsg.runCoRoutine = true;

                m_startCoRoutine = true;
                blockrun = 0;
                score = 0;

            }
            else
            {
                //display msg 
                TaskLowIntensityVision.m_startPracticeLowIntensityTactile = false; 
                triggerMenuMsg.startExperiment = false;
                triggerMenuMsg.index = 9; //will lead to practice exposure
                triggerMenuMsg.runCoRoutine = true;

                m_startPracticeExposure = true; 
                m_startCoRoutine = false;

            }
        }
    }

    IEnumerator WaitForKeyDown()
    {
        while (!(Input.GetKey("right")) && !(Input.GetKey("left")))
            //display the message here 
            yield return null;

        if ((Input.GetKey("right")))
        {
            subjectResponse = 1; //second 
        }

        if ((Input.GetKey("left")))
        {

            subjectResponse = 0; //first
        }

        if (subjectResponse == correctReponse)
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
            //double high pitch beeps
        }

        blockrun++;
        m_startCoRoutine = true;
    }
}
