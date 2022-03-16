using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class RunMenu : MonoBehaviour
{
    [SerializeField] ActionBasedController controller;
    [SerializeField] private Text instruction;

    private int index = 0;
    public string[] instructions = new string[]
    {
        "Place your hand above the yellow area."
    };
    
    void Start()
    {
        index = 0;
        instruction.text = instructions[index];
        controller.selectAction.action.performed += gripButtonPressed;
    }

    private void gripButtonPressed(InputAction.CallbackContext obj)
    {
        //do something when grip button is pressed
    }

    void Update()
    {
        instruction.text = instructions[index];
    }
}
