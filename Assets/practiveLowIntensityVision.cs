using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Random = System.Random;
public class practiveLowIntensityVision : MonoBehaviour
{
    public int subjectNum = 1;
    string seq = "a"; 
    public string PathFolder = "C:/Users/megha/Documents/Unity/visualTactile/Data/Subjects/";
    public string FullFilePath;
    public RunMenu triggerMenuMsg;
    public GameObject MenuCanvas;
    bool m_startCoRoutine = true;
    float time_delay = 0f;

    int correctReponse;

    float stimulusDuration = 0.1f;

    int score = 0;

    int blockCount = 10;
    int blockrun = 0;

    int requiredScore = 8;
    int subjectResponse;

    bool m_startPracticeLowIntensityVision = true; 
    public bool m_startPracticeLowIntensityTactile = false; 

    //audio 
    [SerializeField] public AudioClip beepsoundCorrect;
    [SerializeField] public AudioClip beepsoundWrong;
    [SerializeField] public AudioSource beep;

    // Start is called before the first frame update
    void Start()
    {
        var canvas = GameObject.Find("InstructionsMenu");
        triggerMenuMsg = canvas.GetComponent<RunMenu>();

        FullFilePath = PathFolder + subjectNum + "/" + seq + "/"; 
    }

    // Update is called once per frame
    void Update()
    {
        if (time_delay > 1.0f)
        {
            //if (m_startCoRoutine && triggerMenuMsg.startExperiment)
            //{
            //    //StartCoroutine(Example());
            //}
            if (m_startCoRoutine && triggerMenuMsg.startExperiment && m_startPracticeLowIntensityVision)
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

        if (number ==0)
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
                GetComponent<MeshRenderer>().material.EnableKeyword("_EMISSION");
                if (number == 0)
                {
                    //low intensity
                    GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", new Vector4(191.0f / 255f, 180f / 255f, 180f / 255f, 1f) * Mathf.Pow(2, 1.2f)); //To get HDR intensity is pow of 2
                }
                else
                {
                    //high intensity LED
                    GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", new Vector4(191.0f / 255f, 180f / 255f, 180f / 255f, 1f) * Mathf.Pow(2, 2.9f)); //To get HDR intensity is pow of 2
                }

                yield return new WaitForSecondsRealtime(stimulusDuration);

                GetComponent<MeshRenderer>().material.DisableKeyword("_EMISSION");

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
                m_startPracticeLowIntensityVision = false;
                triggerMenuMsg.startExperiment = false;
                triggerMenuMsg.index = 7;
                triggerMenuMsg.runCoRoutine = true;

                m_startCoRoutine = false;
                m_startPracticeLowIntensityTactile = true;
                
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
            //double high pitch beeps
            beep.PlayOneShot(beepsoundWrong);
            yield return new WaitForSecondsRealtime(0.3f);
            beep.PlayOneShot(beepsoundWrong);
        }

        blockrun++; 
        m_startCoRoutine = true; 
    }
}
