using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Than.Input;

[RequireComponent(typeof(CharacterController))]
public class CharacterControllerMovement : MonoBehaviour
{
    public Brain brain;
    CharacterController characterController;
    Vector2 moveInput;

    float speed = 10;

    // Start is called before the first frame update
    void Start()
    {
        characterController = GetComponent<CharacterController>();
    }

    void Update()
    {
        moveInput = brain.Move;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        MoveUpdate();
    }

    void MoveUpdate()
    {
        Vector3 movement = new Vector3(moveInput.x, 0, moveInput.y) * speed * Time.fixedDeltaTime;
        characterController.Move(movement);
    }
}
