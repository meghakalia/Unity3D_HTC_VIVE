using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Exposure : MonoBehaviour
{
    
    //public Material m_Material;
    bool firstTime            = true;
    float prevTime            = 0;
    public float timeDelay    = 0 ;
    //[System.NonSerialized]
    public float timePeriod   = 1.0f;
    float timeParsed          = 0;
    bool b_lightOn            = true; 
    // Start is called before the first frame update
    void Start()
    {
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

            if (timeParsed >= timePeriod)
            {
                if (b_lightOn == false)
                {
                    GetComponent<MeshRenderer>().material.EnableKeyword("_EMISSION");
                    b_lightOn = !b_lightOn;
                    Debug.Log(" light on : Time parsed %f  " + timeParsed); 
                }
                else
                {
                    GetComponent<MeshRenderer>().material.DisableKeyword("_EMISSION");
                    b_lightOn = !b_lightOn;
                    Debug.Log("light off - Time parsed %f " + timeParsed);
                }

                timeParsed = 0;
            }
        }

    }
}
