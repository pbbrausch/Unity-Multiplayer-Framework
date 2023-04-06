using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovementModule : MonoBehaviour
{
    [Header("Variables")]
    [SerializeField] private float speed = 6f;

    [HideInInspector] public Vector2 moveVector;

    [SerializeField] private Rigidbody rb;

    private void FixedUpdate()
    {
        MovePlayer();
    }

    public void MoveVector(InputAction.CallbackContext callback)
    {
        moveVector = callback.ReadValue<Vector2>().normalized;
    }

    private void MovePlayer()
    {
        rb.AddForce(new Vector3(moveVector.x, 0, moveVector.y) * speed);
    }
}
