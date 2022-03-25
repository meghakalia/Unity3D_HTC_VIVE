using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class RunMenu : MonoBehaviour
{
    [SerializeField] ActionBasedController controller; //if you need controller input
    [SerializeField] private Text instruction;

    [SerializeField] public AudioClip beepsound;
    public AudioSource beep;

    public bool startExperiment = false; 

    public int index = 0;
    public string[] instructions = new string[]
    {
        "Look at your finger & Press any Arrow Key to continue."
    };

    void Start()
    {
        index = 0;
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
    }

    IEnumerator WaitForKeyDown()
    {
        
        while (!(Input.GetKey("right")) && !(Input.GetKey("left")))
            //display the message here 
            yield return null;

        //yield return new WaitForSecondsRealtime(3.0f);
        startExperiment = true;

        
        //index++; 
    }
}
