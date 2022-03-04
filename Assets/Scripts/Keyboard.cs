using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Keyboard : MonoBehaviour
{
    public bool gameIsPaused = false;
    void Update()
    {
        if (GetComponent<Exposure>().startKeyInput)
        {
            if ((!Input.GetKey("right")) && (!Input.GetKey("left")))
            {
                Debug.Log(" press right or left keys");
                gameIsPaused = true;

                //GetComponent<Exposure>().startKeyInput = false; // pausing the loop 

                PauseGame();

                
            }
            else
            {
                Debug.Log(" got the input : press right or left keys");
                gameIsPaused = false;
                PauseGame();
                
                GetComponent<Exposure>().startKeyInput = false;
                GetComponent<Exposure>().timeParsed = 0;
            }
        }
    }
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
}
