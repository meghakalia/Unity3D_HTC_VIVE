using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

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

    [SerializeField] private XRBaseController controller;

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
        while (m_flashCount < 4)
        {
                         
            timeParsed = timeParsed + Time.deltaTime;

                         
            yield return new WaitForSecondsRealtime(1);
            GetComponent<MeshRenderer>().material.EnableKeyword("_EMISSION");                                                                                                
            ActivateHaptic();   
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
        m_startCoRoutine = true;

        //wait for 1 second and start again 
        Debug.Log(" got input ");
    }

}
