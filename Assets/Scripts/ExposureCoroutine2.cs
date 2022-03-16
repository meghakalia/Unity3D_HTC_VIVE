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

    [SerializeField] private XRBaseController controller;
    [SerializeField]  float _mEmissionPower = 3.0f;

  
    //key board control
    bool gameIsPaused = false;
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
     

    //Keyboard keyPress; 
    // Start is called before the first frame update
    void Start()
    {
        //read file and generate list 
        List<List<int>> comb = new List<List<int>>(listFromFile("C:/Users/megha/Documents/Unity/visualTactile/Data/dataTest.csv"));
        //shuffle 
        Random rng = new Random();
        shuffledComb = comb.OrderBy(a => rng.Next()).ToList();

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

    public void ActivateHaptic()
    {
        if (controller != null)
        {
            controller.SendHapticImpulse(0.7f, 1.0f);
        }

    }

    IEnumerator Example()
    {
        m_startCoRoutine = false; //stop coroutine from starting again

        int m_flashCount = 0;

        // this loop will be run 108 times in a block of 36 
        while (m_flashCount < 8) //run 8 times 
        {
                         
            timeParsed = timeParsed + Time.deltaTime;

            yield return new WaitForSecondsRealtime(1);
            GetComponent<MeshRenderer>().material.EnableKeyword("_EMISSION");
            if (m_flashCount % 2 == 0)
            {
                GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", new Vector4(191.0f / 255f, 180f / 255f, 180f / 255f, 1f) * Mathf.Pow(2, 1.7f)); //To get HDR intensity is pow of 2

            }
            else 
            {
                GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", new Vector4(191.0f / 255f, 180f / 255f, 180f / 255f, 1f) * Mathf.Pow(2, 2.9f)); //To get HDR intensity is pow of 2

            }

            //ActivateHaptic();   
            yield return new WaitForSecondsRealtime(1);
            GetComponent<MeshRenderer>().material.DisableKeyword("_EMISSION");
                                                                 
            //wait is moved in the beginning of this loop
            m_flashCount++; 
           
        }
        yield return StartCoroutine(WaitForKeyDown());

    }

    IEnumerator WaitForKeyDown()
    {
        while (!(Input.GetKey("right")) && !(Input.GetKey("left")))
            yield return null;

        //check response and write the response to a file 
        m_startCoRoutine = true;

        //wait for 1 second and start again 
        Debug.Log(" got input ");
    }

}
