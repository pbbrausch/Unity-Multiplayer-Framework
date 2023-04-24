using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerCameraModule : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform orientation;
    [SerializeField] private Transform playerCamera;

    private float sensX, sensY, xRot, yRot;

    private Vector2 lookVector;

    private void Start()
    {
        sensX = FBPP.GetFloat("sensX", 1);
        sensY = FBPP.GetFloat("sensY", 1);
    }

    public void Look(InputAction.CallbackContext callback)
    {
        lookVector = callback.ReadValue<Vector2>();

        float mouseX = 10 * lookVector.x * Time.deltaTime * sensX;
        float mouseY = 10 * lookVector.y * Time.deltaTime * sensY;

        yRot += mouseX;

        xRot -= mouseY;
        xRot = Mathf.Clamp(xRot, -75f, 75f);

        playerCamera.rotation = Quaternion.Euler(xRot, yRot, 0);
        orientation.rotation = Quaternion.Euler(0, yRot, 0);
    }

    private void OnDestroy()
    {
        ChangeCursorMode(false);
    }

    public void ChangeCursorMode(bool lockCursor)
    {
        if (lockCursor)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    public void ChangeScoreboard(InputAction.CallbackContext callback)
    {
        if (SceneManager.GetActiveScene().name == "Lobby") { return; }

        bool open = callback.ReadValue<float>() > 0.1f;

        GameManager.instance.scoreboard.SetActive(open);
    }

    public void ChangeOptionsMenu(InputAction.CallbackContext callback)
    {
        if (SceneManager.GetActiveScene().name == "Lobby") { return; }

        bool open = callback.ReadValue<float>() > 0.1f;

        GameManager.instance.options.SetActive(open);

        ChangeCursorMode(!open);
    }
}
