using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using System; 

public class RunMenu : MonoBehaviour
{
    [SerializeField] ActionBasedController controller; //if you need controller input
    [SerializeField] private Text instruction;

    [SerializeField] public AudioClip beepsound;
    public AudioSource beep;

    public bool startExperiment = false; 

    public int index = 0;

    public bool runCoRoutine = false; 
   
    //public string[] instructions = new string[]
    //{

    //    "Which has greater number of low intensity trials ?\n Buzzer (Left Arrow Key)      LED (Right Arrow Key)\n\nPress any arrow button to continue"

    //};

    [NonSerialized]
     public string[] instructions = new string[]
    {
        "Which has greater number of low intensity trials ?\n Buzzer (Left Key)     LED (Right Key)\n\nPress any arrow key to continue",
        "Look at your Finger \n \n throughout the experiment\n \n\n Press any arrow key to continue",
        "Which comes first ?\n\n\n\n Buzz (Left Key)    Light (Right Key)\n\n\n Press any arrow key to continue",
        "Look at your Finger \n \n throughout the experiment\n \n\n Press any arrow key to continue",
        "End of Block. \n You can take break \n \n Press any arrow key to continue" ,
        " \n\nLook at your Finger ",
        "Score is less than 80%. Try Again \n\n\n  Press any arrow key to continue",
        "Good Job!  \n\n\n Press any arrow key to continue ",
        "Which of the two trials is low intensity ?\n\n First (Left Key)    Second (Right Key)\n\n Press any arrow key to continue",
        "Good Job!  \n\n\n Press any arrow key to continue ",
        "Good Job!  \n\n\n Press any arrow key to continue "
    };

    void Start()
    {
        index = 8;
        instruction.text = instructions[index];
        controller.selectAction.action.performed += gripButtonPressed;

        // coroutine to check the key press 
        StartCoroutine(WaitForKeyDown()); 
        //beep.PlayOneShot(beepsound);
    }

    private void gripButtonPressed(InputAction.CallbackContext obj)
    {
        //do something when grip button is pressed
    }

    void Update()
    {
        instruction.text = instructions[index];

        if (runCoRoutine)
        {
            StartCoroutine(WaitForKeyDown());
            runCoRoutine = false; 
        }

    }

    IEnumerator WaitForKeyDown()
    {
        while (!(Input.GetKey("right")) && !(Input.GetKey("left")))
            //display the message here 
            yield return null;

        //yield return new WaitForSecondsRealtime(3.0f);
        switch (index)
        {
            case 0:
                index = 1; //1
                yield return new WaitForSecondsRealtime(2.0f);
                runCoRoutine = true;                
                break;
            case 1:
                yield return new WaitForSecondsRealtime(2.0f);
                startExperiment = true;
                break;
            case 2:
                index = 1;
                yield return new WaitForSecondsRealtime(0.2f);
                runCoRoutine = true;
                break;
            case 4:
                index = 0;
                yield return new WaitForSecondsRealtime(2.0f);
                runCoRoutine = true;
                break;
            case 6:
                yield return new WaitForSecondsRealtime(2.0f);
                startExperiment = true;
                break;
            case 7:
                index = 8;
                yield return new WaitForSecondsRealtime(2.0f);
                runCoRoutine = true;
                break;
            case 8:
                yield return new WaitForSecondsRealtime(2.0f);
                startExperiment = true;
                break;
            case 9:
                index = 0; //1
                yield return new WaitForSecondsRealtime(2.0f);
                runCoRoutine = true;
                break;
            case 10:
                index = 2; //1
                yield return new WaitForSecondsRealtime(2.0f);
                runCoRoutine = true;
                break;

            default:
                break;
        }

    }
}
