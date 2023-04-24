using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovementModule : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform orientation;
    [SerializeField] private Rigidbody rb;

    [Header("Speed")]
    [SerializeField] private float walkSpeed = 6f;
    [SerializeField] private float sprintSpeed = 8f;
    [SerializeField] private float groundDrag = 5f;
    private float moveSpeed;
    private bool sprinting;
    private Vector3 moveDirection;

    [Header("Jumping")]
    [SerializeField] private float jumpForce = 7f;
    [SerializeField] private float jumpCooldown = 0.25f;
    [SerializeField] private float airMultiplier = 0.3f;
    private bool readyToJump = true;

    [Header("Crouching")]
    [SerializeField] private float crouchSpeed;
    [SerializeField] private float crouchYScale;
    [SerializeField] private float startYScale;
    private bool crouching;

    [Header("Slopes")]
    [SerializeField] private float maxSlopeAngle;
    private bool exitingSlope;

    [Header("Ground Check")]
    [SerializeField] private float playerHeight = 2f;
    [SerializeField] private LayerMask ground;

    [HideInInspector] public Vector2 moveVector;

    private void Start()
    {
        rb.freezeRotation = true;

        startYScale = transform.root.localScale.y;
    }

    private void Update()
    {
        SpeedControl();
        HandleDrag();
        HandleSpeed();
    }
    private void FixedUpdate()
    {
        MovePlayer();
    }

    [HideInInspector] public MoveState state;
    public enum MoveState
    {
        walking,
        sprinting,
        crouching,
        air
    }
    private void HandleSpeed()
    {
        if (crouching)
        {
            state = MoveState.crouching;
            moveSpeed = crouchSpeed;
        }
        else if (Grounded())
        { 
            if (sprinting)
            {
                state = MoveState.sprinting;
                moveSpeed = sprintSpeed;
            }
            else
            {
                state = MoveState.walking;
                moveSpeed = walkSpeed;
            }
        }
        else
        {
            state = MoveState.air;
        }
    }

    public void Crouch(InputAction.CallbackContext callback)
    {
        crouching = callback.ReadValue<float>() > 0.1f;

        if (crouching)
        {
            transform.root.localScale = new(transform.root.localScale.x, crouchYScale, transform.root.localScale.z);
        }
        else
        {
            transform.root.localScale = new(transform.root.localScale.x, startYScale, transform.root.localScale.z);
        }
    }

    public void Jump(InputAction.CallbackContext _)
    {
        if (!readyToJump || !Grounded()) { return; }

        exitingSlope = true;

        readyToJump = false;
             
        rb.velocity = new(rb.velocity.x, 0, rb.velocity.z);
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);

        Invoke(nameof(ResetJump), jumpCooldown);
    }

    private void ResetJump()
    {
        readyToJump = true;

        exitingSlope = false;
    }

    private void HandleDrag()
    {
        if (Grounded())
        {
            rb.drag = groundDrag;
        }
        else
        {
            rb.drag = 0;
        }
    }

    public void MoveVector(InputAction.CallbackContext callback)
    {
        moveVector = callback.ReadValue<Vector2>();
    }

    private void MovePlayer()
    {
        moveDirection = orientation.forward * moveVector.y + orientation.right * moveVector.x;

        if (Grounded())
        {
            if (OnSlope() && !exitingSlope)
            {
                rb.AddForce(10 * moveSpeed * SlopeMoveDirection(), ForceMode.Force);

                if (rb.velocity.y > 0)
                {
                    rb.AddForce(Vector3.down * 80f, ForceMode.Force);
                }
            }
            else
            {
                rb.AddForce(10 * moveSpeed * moveDirection.normalized, ForceMode.Force);
            }
        }
        else
        {
            rb.AddForce(10 * moveSpeed * airMultiplier * moveDirection.normalized, ForceMode.Force);
        }
        rb.useGravity = !OnSlope();
    }

    private void SpeedControl()
    {
        if (OnSlope() && !exitingSlope)
        {
            if (rb.velocity.magnitude > moveSpeed)
            {
                rb.velocity = rb.velocity.normalized * moveSpeed;
            }
        }
        else
        {
            Vector3 flatVel = new(rb.velocity.x, 0f, rb.velocity.z);

            if (flatVel.magnitude > moveSpeed)
            {
                Vector3 limitedVel = flatVel.normalized * moveSpeed;
                rb.velocity = new(limitedVel.x, rb.velocity.y, limitedVel.z);
            }
        }
    }

    public void ChangeSprint(InputAction.CallbackContext callback)
    {
        sprinting = callback.ReadValue<float>() > 0.1f;
    }

    private bool Grounded()
    {
        return Physics.Raycast(transform.root.position, Vector3.down, playerHeight * 0.5f + 0.2f, ground);
    }

    private RaycastHit slopeHit;
    private bool OnSlope()
    {
        if (Physics.Raycast(transform.root.position, Vector3.down, out slopeHit, playerHeight * 0.5f + 0.3f))
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);

            return angle < maxSlopeAngle && angle != 0;
        }

        return false;
    }

    private Vector3 SlopeMoveDirection()
    {
        return Vector3.ProjectOnPlane(moveDirection, slopeHit.normal).normalized;
    }
}
