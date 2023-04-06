using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerCameraModule : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform orientation;

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

        float mouseX = lookVector.x * Time.deltaTime * sensX;
        float mouseY = lookVector.y * Time.deltaTime * sensY;

        yRot += mouseX;

        xRot -= mouseY;
    }

    public void ChangeCursorMode(bool parameter)
    {
        if (parameter)
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
        string sceneName = SceneManager.GetActiveScene().name;
        float press = callback.ReadValue<float>();

        if (sceneName == "Lobby")
        {
            return;
        }
        else
        {
            ChangeCursorMode(press < 0.1f);
        }

        GameManager.instance.scoreboard.SetActive(press > 0.1f);
    }

    public void ChangeOptionsMenu(InputAction.CallbackContext callback)
    {
        float press = callback.ReadValue<float>();

        GameManager.instance.options.SetActive(press > 0.1f);

        if (SceneManager.GetActiveScene().name == "Lobby") { return; }

        ChangeCursorMode(press < 0.1f);
    }
}
