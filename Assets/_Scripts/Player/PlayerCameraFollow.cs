using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCameraFollow : MonoBehaviour
{
    public Transform cameraPos;

    private void Start()
    {
        transform.parent = null;
    }

    private void Update()
    {
        transform.position = cameraPos.position;
    }
}
