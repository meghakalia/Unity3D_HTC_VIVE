using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Linq;

using Random = System.Random;

public class PostExposure : MonoBehaviour
{
    //List<int> shuffledComb;
    //string filePath = "C:/Users/megha/Documents/Unity/visualTactile/Data/Subjects/TestingTOJ.csv";

    ////Touch haptic 
    //public HapticPlugin HapticDevice = null;
    //private int FXID = -1;
    //[NonSerialized] public float frequency = 300.0f;
    //[NonSerialized] public float gain = 1.5f;
    //[NonSerialized] public float lowGain = 0.5f;
    //[NonSerialized] public float magnitude = 1.2f;
    //[NonSerialized] public double[] dir = { 1.0, 1.0, 1.0 };

    //public RunMenu triggerMenuMsg;

    //public bool startTOJ = false;

    //bool m_startCoRoutine = false; 

    //float time_delay = 0f;
    //public float timePeriod = 1.0f; // based on the MATLAB script
    //float timeParsed = 0;

    //int tempListCount = 0;
    //int tempList = 0;

    //int stimulusDuration = 70; 
    ////functions
    //List<int> generateRand(int numCount)
    //{
    //    var rand = new Random();
    //    List<int> listNumbers = new List<int>();
    //    int number;
    //    for (int i = 0; i < numCount; i++)
    //    {
    //        do
    //        {
    //            number = rand.Next(1, 8);
    //        } while (listNumbers.Contains(number));
    //        listNumbers.Add(number);
    //    }

    //    return listNumbers;
    //}

    //List<int> listFromFile(string FilePath, int repetitions)
    //{
    //    List<int> comb = new List<int>();
    //    using (var reader = new StreamReader(FilePath))
    //    {
    //        while (!reader.EndOfStream)
    //        {
    //            //List<int> pair = new List<int>();

    //            var line = reader.ReadLine();
    //            var values = line.Split(',');

    //            comb.Add(Convert.ToInt32(values[0]));

    //        }
    //    }

    //    var l = Enumerable.Repeat('x', 5).ToList();

    //    var serviceEndPoints = comb.SelectMany(t => Enumerable.Repeat(t, repetitions)).ToList();

    //    //for (int i = 0; i < serviceEndPoints.Count; i++)
    //    //{
    //    //    Console.WriteLine(serviceEndPoints[i]);
    //    //}

    //    return serviceEndPoints;
    //}

    //void writeToFile(string str_output, string filePath)
    //{
    //    //do this only once 
    //    //FileStream writer = File.OpenWrite(filePath);

    //    using FileStream fileStream = File.Open(filePath, FileMode.Append);
    //    using StreamWriter writer = new StreamWriter(fileStream);

    //    writer.WriteLine(str_output);

    //    writer.Flush();
    //    writer.Close();

    //}


    //void DeactivateTouchHaptic()
    //{
    //    if (HapticDevice == null) return;       //If there is no device, bail out early.
    //    if (FXID == -1) return;                 //If there is no effect, bail out early.

    //    HapticPlugin.effects_stopEffect(HapticDevice.configName, FXID);
    //}

    //public void ActivateTouchHaptic(float gain, float magnitude, float frequency, double[] dir)
    //{
    //    if (HapticDevice == null) return;       //If there is no device, bail out early.

    //    // If a haptic effect has not been assigned through Open Haptics, assign one now.
    //    if (FXID == -1)
    //    {
    //        FXID = HapticPlugin.effects_assignEffect(HapticDevice.configName);

    //        if (FXID == -1) // Still broken?
    //        {
    //            Debug.LogError("Unable to assign Haptic effect.");
    //            return;
    //        }
    //    }

    //    // Send the effect settings to OpenHaptics.
    //    double[] pos = { 0.0, 0.0, 0.0 }; // Position (not used for vibration)
    //                                      //double[] dir = {0.0, 1.0, 0.0}; // Direction of vibration

    //    HapticPlugin.effects_settings(
    //        HapticDevice.configName,
    //        FXID,
    //        gain, // Gain
    //        magnitude, // Magnitude
    //        frequency,  // Frequency
    //        pos,  // Position (not used for vibration)
    //        dir); //Direction.

    //    HapticPlugin.effects_type(HapticDevice.configName, FXID, 4); // Vibration effect == 4

    //    HapticPlugin.effects_startEffect(HapticDevice.configName, FXID);

    //}

    // Start is called before the first frame update
    void Start()
    {
        ////read file and generate list 
        //List<int> comb = new List<int>(listFromFile("C:/Users/megha/Documents/Unity/visualTactile/Data/TOJConditions.csv", 2));
        ////shuffle 
        //Random rng = new Random();
        //shuffledComb = comb.OrderBy(a => rng.Next()).ToList();
        //tempListCount = shuffledComb.Count;

        //writeToFile("AsynchronyVal,correctResponse, subjectResponse, stimulusDuration", filePath);

        ////haptic Touch 
        //if (HapticDevice == null)
        //    HapticDevice = (HapticPlugin)FindObjectOfType(typeof(HapticPlugin));
    }

    // Update is called once per frame
    void Update()
    {
        //if (time_delay > 1.0f)
        //{
        //    if (startTOJ)
        //    {
        //        //StartCoroutine(Example());
        //    }
        //}
        //else
        //{
        //    time_delay = time_delay + Time.deltaTime;
        //}
    }


    //IEnumerator TOJCoroutine()
    //{
    //    m_startCoRoutine = false; //stop coroutine from starting again

    //    //if (blockrun < blockCount)
    //    //{
    //    //    int m_flashCount = 0;
    //        // this loop will be run 108 times in a block of 36 
    //    if (tempList != tempListCount) // run till the list is empty
    //    {
            

    //        int AsynchVal = shuffledComb[tempList];

    //        if (AsynchVal < 0) // tactile first 
    //        {
    //            ActivateTouchHaptic(gain, magnitude, frequency, dir);
    //        }
    //        else 
    //        { 
    //        }

    //        // read the number of random stimuli from file 
    //        yield return new WaitForSecondsRealtime(0.50f); // time between stimulus

    //        //start stimulus
    //        GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", new Vector4(191.0f / 255f, 180f / 255f, 180f / 255f, 1f) * Mathf.Pow(2, 2.9f)); //To get HDR intensity is pow of 2

    //        // Add delay between visual and tactile stimulus 

    //        ActivateTouchHaptic(lowGain, magnitude, frequency, dir);

    //        yield return new WaitForSecondsRealtime(stimulusDuration); // time for which stimulus is presented



    //        List<int> numbersRand_V = new List<int>(generateRand(8 - shuffledComb[tempList][0])); // idx of low intensity trials
    //            List<int> numbersRand_T = new List<int>(generateRand(8 - shuffledComb[tempList][1])); // idx of low intensity trials

    //            while (m_flashCount < 8) //run 8 times 
    //            {

    //                timeParsed = timeParsed + Time.deltaTime;

    //                yield return new WaitForSecondsRealtime(0.50f); // time between stimulus


    //            //start stimulus
    //            // high intensity visual
               
    //            GetComponent<MeshRenderer>().material.EnableKeyword("_EMISSION");

    //                //if any 

    //                if (numbersRand_T.Any(x => x == m_flashCount)) // check whether current trial shoudl be low intensity
    //                {
    //                    //ActivateHaptic(_mIntensityHaptic * 0.2f, stimulusDuration); // low intensity haptic
    //                    ActivateTouchHaptic(lowGain, magnitude, frequency, dir);
    //                }
    //                else
    //                {
    //                    ActivateTouchHaptic(gain, magnitude, frequency, dir);
    //                    //ActivateHaptic(_mIntensityHaptic, stimulusDuration);// high intensity haptic

    //                }

    //                if (numbersRand_V.Any(x => x == m_flashCount)) // check whether current trial shoudl be low intensity
    //                {
    //                    // low intensity visual
    //                    GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", new Vector4(191.0f / 255f, 180f / 255f, 180f / 255f, 1f) * Mathf.Pow(2, 1.2f)); //To get HDR intensity is pow of 2
    //                }
    //                else
    //                {
    //                    // high intensity visual
    //                    GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", new Vector4(191.0f / 255f, 180f / 255f, 180f / 255f, 1f) * Mathf.Pow(2, 2.9f)); //To get HDR intensity is pow of 2
    //                }

    //                //if (m_flashCount % 2 == 0)
    //                //{
    //                //    GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", new Vector4(191.0f / 255f, 180f / 255f, 180f / 255f, 1f) * Mathf.Pow(2, 1.7f)); //To get HDR intensity is pow of 2

    //                //}
    //                //else 
    //                //{
    //                //    GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", new Vector4(191.0f / 255f, 180f / 255f, 180f / 255f, 1f) * Mathf.Pow(2, 2.9f)); //To get HDR intensity is pow of 2

    //                //}

    //                //ActivateHaptic();   // time will be different from visual 
    //                yield return new WaitForSecondsRealtime(stimulusDuration); // time for which stimulus is presented
    //                DeactivateTouchHaptic();
    //                GetComponent<MeshRenderer>().material.DisableKeyword("_EMISSION"); // switch off stimulus

    //                // Disable this keyword 
    //                //wait is moved in the beginning of this loop
    //                m_flashCount++;

    //            }

    //            yield return StartCoroutine(WaitForKeyDown()); //get user input 
    //        //}
    //        //else if (tempList == tempListCount)
    //        //{
    //        //    tempList = 0;
    //        //    blockrun++;

    //        //    yield return StartCoroutine(WaitForKeyDown()); //get user input 
    //        //}
    //    }






    //}

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
