using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExposureRoutine : MonoBehaviour
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


    //Keyboard keyPress; 
    // Start is called before the first frame update
    void Start()
    {
        //keyPress = GetComponent<Keyboard>();
        //m_Material = GetComponent<Renderer>().material;
        //m_Material.DisableKeyword("_EMISSION");
        
    }


    // Update is called once per frame
    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.A))
        //{
        //    //Time.fixedDeltaTime
        //    GetComponent<MeshRenderer>().material.DisableKeyword("_EMISSION");
        //}

        
        //number of times this loop will run 
        if (m_startCoRoutine)
        {
            if (firstTime)
            {
                prevTime = Time.time;
                firstTime = !firstTime;
            }

            StartCoroutine(Example());
        }
        

        //print("All Done!");


    }

    IEnumerator Example()
    {
        m_startCoRoutine = false; //stop coroutine from starting again

        int temp = 0; 
        while (temp  < 10000)
        {
            if ((Time.time - prevTime) >= timeDelay)
            {
                ////run for 1 second
                //GetComponent<MeshRenderer>().material.EnableKeyword("_EMISSION");
                timeParsed = timeParsed + Time.deltaTime;

                //Debug.Log("timeParsed " + timeParsed + " -- " + (new System.Diagnostics.StackFrame(0, true)).GetFileLineNumber());

                if (timeParsed <= timePeriod)
                {

                    //On for timePeriod seconds 
                    GetComponent<MeshRenderer>().material.EnableKeyword("_EMISSION");
                    b_lightOn = !b_lightOn; // flag false; 
                    //Debug.Log("lights on ");


                    //if (b_lightOn == false)
                    //{
                    //    GetComponent<MeshRenderer>().material.EnableKeyword("_EMISSION");
                    //    b_lightOn = !b_lightOn;
                    //    //Debug.Log(" light on : Time parsed %f  " + timeParsed); 
                    //}
                    //else
                    //{
                    //    GetComponent<MeshRenderer>().material.DisableKeyword("_EMISSION");
                    //    b_lightOn = !b_lightOn;
                    //    //Debug.Log("light off - Time parsed %f " + timeParsed);
                    //}

                    //timeParsed = 0;
                    //Debug.Log("light on : " + timeParsed);
                    //Debug.Log("timeParsed " + timeParsed + " -- " + (new System.Diagnostics.StackFrame(0, true)).GetFileLineNumber());
                    temp++; 
                }
                else
                {
                    //Debug.Log("light off: " + timeParsed);
                    //Debug.Log("timeParsed " + timeParsed + " -- " + (new System.Diagnostics.StackFrame(0, true)).GetFileLineNumber());
                    GetComponent<MeshRenderer>().material.DisableKeyword("_EMISSION");
                    b_lightOn = !b_lightOn; //flag true
                    //Debug.Log("lights off ");


                    //if (timeParsed > (timePause + timePeriod))
                    //{
                    //    timeParsed = 0;

                    //}

                    //if (timeParsed > (timePause + timePeriod))
                    //{
                    //    timeParsed = 0;

                    //    //pause and get input 
                    //    if (!Input.GetKey("right") && !Input.GetKey("left"))
                    //    {
                    //        Debug.Log(" press right or left keys");
                    //        gameIsPaused = true;
                    //        PauseGame();
                    //    }
                    //    else
                    //    {
                    //        Debug.Log(" got the input : press right or left keys");
                    //        gameIsPaused = false;
                    //        PauseGame();
                    //    }

                    //}
                    //wait 
                    //Debug.Log("timeParsed " + timeParsed + " -- " + (new System.Diagnostics.StackFrame(0, true)).GetFileLineNumber());
                    Debug.Log("Started Coroutine at timestamp : " + Time.time);
                    yield return new WaitForSecondsRealtime(1);
                    //yield return new WaitForSeconds(timePause);
                    Debug.Log("Finished Coroutine at timestamp : " + Time.time);
                    timeParsed = 0; // mystery 
                    m_startCoRoutine = true; // restart coroutine
                    //Debug.Log("timeParsed " + timeParsed + " -- " + (new System.Diagnostics.StackFrame(0, true)).GetFileLineNumber());
                    //timeParsed = 0; // Watch this 
                    //Debug.Log("timeParsed " + timeParsed + " -- " + (new System.Diagnostics.StackFrame(0, true)).GetFileLineNumber());
                }
                


            }
        }
        

        //yield return null; 
    }
}
