using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerMovementModule))]
[RequireComponent(typeof(PlayerCameraModule))]
public class PlayerInputModule : MonoBehaviour
{
    private PlayerInput playerInput = null;

    //Actions
    private PlayerInput.OnFootActions onFoot;
    private PlayerInput.MenusActions menus;

    //Modules
    private PlayerMovementModule movementModule;
    private PlayerCameraModule cameraModule;

    private void Awake()
    {
        playerInput = new PlayerInput();

        //Set Actions
        onFoot = playerInput.OnFoot;
        menus = playerInput.Menus;

        //Get Modules
        movementModule = GetComponent<PlayerMovementModule>();
        cameraModule = GetComponent<PlayerCameraModule>();
    }

    private void OnEnable()
    {
        playerInput.Enable();

        //Make Actions
        onFoot.Move.performed += movementModule.MoveVector;
        onFoot.Move.canceled += movementModule.MoveVector;

        onFoot.Sprint.performed += movementModule.ChangeSprint;
        onFoot.Sprint.canceled += movementModule.ChangeSprint;

        onFoot.Crouch.performed += movementModule.Crouch;
        onFoot.Crouch.canceled += movementModule.Crouch;

        onFoot.Jump.performed += movementModule.Jump;

        onFoot.Look.performed += cameraModule.Look;
        onFoot.Look.canceled += cameraModule.Look;

        menus.Scoreboard.performed += cameraModule.ChangeScoreboard;
        menus.Scoreboard.canceled += cameraModule.ChangeScoreboard;

        menus.Options.performed += cameraModule.ChangeOptionsMenu;
        menus.Options.canceled += cameraModule.ChangeOptionsMenu;
    }

    private void OnDisable()
    {
        playerInput.Disable();

        //Dispose Actions
        onFoot.Move.performed -= movementModule.MoveVector;
        onFoot.Move.canceled -= movementModule.MoveVector;

        onFoot.Sprint.performed -= movementModule.ChangeSprint;
        onFoot.Sprint.canceled -= movementModule.ChangeSprint;

        onFoot.Crouch.performed -= movementModule.Crouch;
        onFoot.Crouch.canceled -= movementModule.Crouch;

        onFoot.Jump.performed -= movementModule.Jump;

        onFoot.Look.performed -= cameraModule.Look;
        onFoot.Look.canceled -= cameraModule.Look;

        menus.Scoreboard.performed -= cameraModule.ChangeScoreboard;
        menus.Scoreboard.canceled -= cameraModule.ChangeScoreboard;

        menus.Options.performed -= cameraModule.ChangeOptionsMenu;
        menus.Options.canceled -= cameraModule.ChangeOptionsMenu;
    }
}
