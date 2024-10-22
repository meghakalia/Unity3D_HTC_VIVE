using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using System;
using System.IO;
using System.Linq;

using Random = System.Random;

public class ExposureCoroutine2 : MonoBehaviour
{
    //public Material m_Material;
    bool firstTime              = true;
    float prevTime              = 0;
    public float timeDelay      = 0;
    [System.NonSerialized] // this won't save the timePeriod variable in between the running instances
    public float timePeriod     = 1.0f; // based on the MATLAB script
    float timeParsed            = 0;
    bool b_lightOn = true;
    public float timePause      = 1.0f;
    bool m_startCoRoutine       = true;
    float time_delay            = 0f;
    List<List<int>> shuffledComb;
    int correctResponse         = 0;
    [SerializeField] float stimulusDuration = 0.1f; // 100 ms
    int subjectResponse         = 0; 

    [SerializeField] private XRBaseController controller;
    [SerializeField]  float _mEmissionPower = 3.0f;
    [SerializeField] float _mIntensityHaptic = 1.0f;



    public RunMenu triggerMenuMsg;

    int tempListCount           = 0;
    int tempList                = 0;

    // file input output 
    public string filePath = ""; // to give in the editor 

    
    //key board control
    bool gameIsPaused           = false;
    void PauseGame()
    {
        if (gameIsPaused)
        {
            Time.timeScale = 0f;
        }
        else
        {
            Time.timeScale = 1;
        }
    }

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
        //read file and generate list 
        List<List<int>> comb = new List<List<int>>(listFromFile("C:/Users/megha/Documents/Unity/visualTactile/Data/dataTest.csv"));
        //shuffle 
        Random rng = new Random();
        shuffledComb = comb.OrderBy(a => rng.Next()).ToList();
        tempListCount = shuffledComb.Count;

        //file input output 
        filePath = "C:/Users/megha/Documents/Unity/visualTactile/Data/Subjects/MeghaPilotExposure.csv";
        writeToFile("AsynchronyVal, numVisLow, numTactLow, correctResponse, subjectResponse, stimulusDuration");
    }


    // Update is called once per frame
    void Update()
    {

        if (time_delay > 1.0f)
        {
            if (m_startCoRoutine)
            {
                StartCoroutine(Example());
            }
        }
        else 
        {
            time_delay = time_delay + Time.deltaTime;
        }

        //if (m_startCoRoutine)
        //{
        //    StartCoroutine(Example());
        //}

    }

    public void ActivateHaptic(float amplitude, float duration)
    {
        if (controller != null)
        {
            controller.SendHapticImpulse(amplitude, duration);
        }

    }

    IEnumerator Example()
    {
        m_startCoRoutine = false; //stop coroutine from starting again

        int m_flashCount = 0;

        // this loop will be run 108 times in a block of 36 
        if (tempList != tempListCount) // run till the list is empty
        {
            // read the number of random stimuli from file 
            List<int> numbersRand_V = new List<int>(generateRand(8 - shuffledComb[tempList][0])); // idx of low intensity trials
            List<int> numbersRand_T = new List<int>(generateRand(8 - shuffledComb[tempList][1])); // idx of low intensity trials

            

            while (m_flashCount < 8) //run 8 times 
            {

                timeParsed = timeParsed + Time.deltaTime;

                yield return new WaitForSecondsRealtime(0.50f); // time between stimulus

                

                //start stimulus
                GetComponent<MeshRenderer>().material.EnableKeyword("_EMISSION");

                //if any 

                if (numbersRand_T.Any(x => x == m_flashCount)) // check whether current trial shoudl be low intensity
                {
                    ActivateHaptic(_mIntensityHaptic*0.2f, stimulusDuration); // low intensity haptic
                }
                else
                {
                    ActivateHaptic(_mIntensityHaptic , stimulusDuration);// high intensity haptic
                    
                }

                if (numbersRand_V.Any(x => x == m_flashCount)) // check whether current trial shoudl be low intensity
                {
                    // low intensity visual
                    GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", new Vector4(191.0f / 255f, 180f / 255f, 180f / 255f, 1f) * Mathf.Pow(2, 1.2f)); //To get HDR intensity is pow of 2
                }
                else
                {
                    // high intensity visual
                    GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", new Vector4(191.0f / 255f, 180f / 255f, 180f / 255f, 1f) * Mathf.Pow(2, 2.9f)); //To get HDR intensity is pow of 2
                }

                //if (m_flashCount % 2 == 0)
                //{
                //    GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", new Vector4(191.0f / 255f, 180f / 255f, 180f / 255f, 1f) * Mathf.Pow(2, 1.7f)); //To get HDR intensity is pow of 2

                //}
                //else 
                //{
                //    GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", new Vector4(191.0f / 255f, 180f / 255f, 180f / 255f, 1f) * Mathf.Pow(2, 2.9f)); //To get HDR intensity is pow of 2

                //}

                //ActivateHaptic();   // time will be different from visual 
                yield return new WaitForSecondsRealtime(stimulusDuration); // time for which stimulus is presented
                GetComponent<MeshRenderer>().material.DisableKeyword("_EMISSION"); // switch off stimulus

                //wait is moved in the beginning of this loop
                m_flashCount++;

            }

            yield return StartCoroutine(WaitForKeyDown()); //get user input 
        }
       
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

        //write the response to a file 
        int numVisLow   = 8 - shuffledComb[tempList][0];
        int numTactLow  = 8 - shuffledComb[tempList][1];

        if (numVisLow < numTactLow)
        {
            correctResponse = 1; //vision has hihger number of high intensity responses
        }
        else 
        {
            correctResponse = 2; //tactile has hihger number of high intensity responses
        }

        //AsynchronyVal, numVisLow, numTactLow, correctResponse, subjectResponse, stimulusDuration
        decimal[] arr = { 0.0m, (decimal)numVisLow, (decimal)numTactLow, (decimal)correctResponse, (decimal)subjectResponse, (decimal)stimulusDuration };
        writeToFile(string.Join(", ", arr));
       

        // increment tempList 
        tempList++;

        //triggerMenuMsg.index += 1;

        //check response and write the response to a file 
        m_startCoRoutine = true;

        //wait for 1 second and start again 
        Debug.Log(" got input ");
    }

}
