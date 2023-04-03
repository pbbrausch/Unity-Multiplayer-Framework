using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private Rigidbody rb;
    [SerializeField] private float speed;

    private PlayerInput input;

    private Vector2 moveVector;

    private void Awake()
    {
        input = new PlayerInput();
    }

    private void OnEnable()
    {
        input.Enable();
        input.OnFoot.Movement.performed += OnMovement;
        input.OnFoot.Movement.canceled += OnMovementStopped;
    }

    private void OnDisable()
    {
        input.Disable();
        input.OnFoot.Movement.performed -= OnMovement;
        input.OnFoot.Movement.canceled -= OnMovementStopped;
    }

    private void FixedUpdate()
    {
        rb.AddForce(new Vector3(moveVector.x, 0, moveVector.y).normalized * speed);
    }

    private void OnMovement(InputAction.CallbackContext value)
    {
        Debug.Log("Moving");
        moveVector = value.ReadValue<Vector2>();
    }

    private void OnMovementStopped(InputAction.CallbackContext value)
    {
        moveVector = Vector2.zero;
    }
}
