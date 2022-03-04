using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    // Start is called before the first frame update
    CharacterController character;
    Vector3 moveVector;
    [SerializeField] float speed = 10f; 
    private void Start()
    {
        character = GetComponent<CharacterController>(); 
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        character.Move(moveVector * Time.fixedDeltaTime * speed); 
    }

    public void OnMovementChanged(InputAction.CallbackContext context) //should be public
    {
        Vector2 direction = context.ReadValue<Vector2>();
        moveVector = new Vector3(direction.x, 0, direction.y); 
    }
}
