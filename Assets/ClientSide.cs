using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class ClientSide : MonoBehaviour
{
    [SerializeField] private Transform camPos;

    private CustomNetworkManager manager;

    private CustomNetworkManager Manager
    {
        get
        {
            if (manager != null)
            {
                return manager;
            }
            return manager = NetworkManager.singleton as CustomNetworkManager;
        }
    }

    private void LateUpdate()
    {
        foreach (PlayerManager player in Manager.PlayerManagers)
        {
            Transform info = player.userInfoCanvas.transform;
            info.LookAt(info.position + camPos.rotation * Vector3.forward, camPos.rotation * Vector3.up);
        }
    }
}
