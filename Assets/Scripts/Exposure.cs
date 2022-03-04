using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Exposure : MonoBehaviour
{
    
    //public Material m_Material;
    bool firstTime            = true;
    float prevTime            = 0;
    public float timeDelay    = 0 ;
    [System.NonSerialized] // this won't save the timePeriod variable in between the running instances
    public float timePeriod   = 1.0f; // based on the MATLAB script
    public float timeParsed   = 0;
    bool b_lightOn            = true;
    public float timePause    = 1f;

    public bool startKeyInput = false; 
    //key board control
    bool gameIsPaused = false;
    //void PauseGame()
    //{
    //    if (gameIsPaused)
    //    {
    //        Time.timeScale = 0f;
    //    }
    //    else
    //    {
    //        Time.timeScale = 1;
    //    }
    //}

    //Keyboard keyPress; 
    // Start is called before the first frame update
    void Start()
    {
        //keyPress = GetComponent<Keyboard>();
        //m_Material = GetComponent<Renderer>().material;
        //m_Material.DisableKeyword("_EMISSION");
        //Debug.Log("Time.timeScale " + Time.timeScale); 
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

        if (firstTime) 
        {
            prevTime = Time.time;
            firstTime = !firstTime;
        }

        if ((Time.time - prevTime) > timeDelay)
        {
            ////run for 1 second
            //GetComponent<MeshRenderer>().material.EnableKeyword("_EMISSION");
            timeParsed = timeParsed + Time.deltaTime;
            Debug.Log("timeParsed " + timeParsed + " -- " + (new System.Diagnostics.StackFrame(0, true)).GetFileLineNumber());
            if (timeParsed <= timePeriod)
            {
                Debug.Log("timeParsed " + timeParsed + " -- " + (new System.Diagnostics.StackFrame(0, true)).GetFileLineNumber());
                Debug.Log("timeParsed " + timeParsed + " -- " + (new System.Diagnostics.StackFrame(0, true)).GetFileLineNumber());
                //On for timePeriod seconds 
                GetComponent<MeshRenderer>().material.EnableKeyword("_EMISSION");     
                b_lightOn = !b_lightOn; // flag false; 
                Debug.Log("timeParsed " + timeParsed + " -- " + (new System.Diagnostics.StackFrame(0, true)).GetFileLineNumber());

            }
            else 
            {
                Debug.Log("timeParsed " + timeParsed + " -- " + (new System.Diagnostics.StackFrame(0, true)).GetFileLineNumber());
                GetComponent<MeshRenderer>().material.DisableKeyword("_EMISSION");
                b_lightOn = !b_lightOn; //flag true

                if (timeParsed > (timePause + timePeriod))
                {

                    Debug.Log("timeParsed " + timeParsed + " -- " + (new System.Diagnostics.StackFrame(0, true)).GetFileLineNumber());
                    startKeyInput = true; 
                    //pause the game 
                    if (GetComponent<Keyboard>().gameIsPaused)
                    {
                        //Debug.Log("timeParsed " + timeParsed + " -- " + (new System.Diagnostics.StackFrame(0, true)).GetFileLineNumber());
                        //timeParsed = 0; // restart the time/loop only when either or left key is pressed
                        Debug.Log("timeParsed " + timeParsed + " -- " + (new System.Diagnostics.StackFrame(0, true)).GetFileLineNumber());
                        Debug.Log("Game paused"); 
                        //game is paused
                        // DIsplay a message probably 
                        // get user input 
                    }
                    
                    Debug.Log("timeParsed " + timeParsed + " -- " + (new System.Diagnostics.StackFrame(0, true)).GetFileLineNumber());
                }

            }
        }

    }
}
