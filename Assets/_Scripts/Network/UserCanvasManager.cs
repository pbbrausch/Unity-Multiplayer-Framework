using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class UserCanvasManager : MonoBehaviour
{
    [SerializeField] private Transform camPos;

    [HideInInspector] public List<Transform> userInfoCanvases = new();

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
            player.userInfoCanvas.LookAt(player.userInfoCanvas.position + camPos.rotation * Vector3.forward, camPos.rotation * Vector3.up);
        }
    }
}
