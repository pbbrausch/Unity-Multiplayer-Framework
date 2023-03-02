using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Client : MonoBehaviour
{
    [SerializeField] private Transform camPos;
    [HideInInspector] public List<Transform> userInfoCanvases = new();

    private void LateUpdate()
    {
        foreach (Transform userInfoCanvas in userInfoCanvases)
        {
            userInfoCanvas.LookAt(userInfoCanvas.position + camPos.rotation * Vector3.forward, camPos.rotation * Vector3.up);
        }
    }
}
