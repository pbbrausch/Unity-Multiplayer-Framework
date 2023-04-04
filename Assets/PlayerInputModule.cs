using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInputModule : MonoBehaviour
{
    private PlayerInput playerInput = null;

    private PlayerMovementModule playerMovementModule;

    private void Awake()
    {
        playerInput = new PlayerInput();
        playerMovementModule = GetComponent<PlayerMovementModule>();
    }

    private void OnEnable()
    {
        playerInput.Enable();
    }

    private void OnDisable()
    {
        playerInput.Disable();
    }
}
