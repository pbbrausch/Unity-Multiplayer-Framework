using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;

public class PlayerUsernameModule : MonoBehaviour
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

    private void Awake()
    {
        SceneManager.activeSceneChanged += OnSceneChanged;
    }

    private bool inLobby = true;
    private void OnSceneChanged(Scene current, Scene next)
    {
        if (next.name == "Lobby")
        {
            inLobby = true;

            foreach (PlayerManager player in Manager.PlayerManagers)
            {
                player.userInfoCanvas.rotation = Quaternion.Euler(0, 0, 0);
            }
        }
        else
        {
            inLobby = false;
        }
    }

    private void LateUpdate()
    {
        if (inLobby) { return; }

        foreach (PlayerManager player in Manager.PlayerManagers)
        {
            player.userInfoCanvas.LookAt(player.userInfoCanvas.position + camPos.rotation * Vector3.forward, camPos.rotation * Vector3.up);
        }
    }
}
