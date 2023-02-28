using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    private Transform camPos;

    private void Start()
    {
        camPos = Camera.main.transform;
    }

    private void LateUpdate()
    {
        transform.LookAt(transform.position + camPos.rotation * Vector3.forward, camPos.rotation * Vector3.up);
    }
}
