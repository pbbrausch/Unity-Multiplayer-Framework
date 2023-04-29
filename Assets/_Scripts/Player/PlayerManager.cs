using UnityEngine.SceneManagement;
using UnityEngine;
using Steamworks;
using Mirror;
using TMPro;
using System.Collections.Generic;

public class PlayerManager : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject clientSide;
    [SerializeField] private GameObject cameraHolder;
    public Transform userInfoCanvas;
    public TMP_Text usernameText;
    public Rigidbody rb;

    private Transform[] spawns;

    //Player Info (static)
    [SyncVar] public ulong steamId;
    [SyncVar] public int playerIdNumber;
    [SyncVar] public int connectionId;
    [SyncVar] public bool leader;

    //Player Info (updated)
    [SyncVar(hook = nameof(PlayerNameUpdate))] public string username;
    [SyncVar(hook = nameof(PlayerReadyUpdate))] public bool ready;
    [SyncVar(hook = nameof(PlayerColorUpdate))] public Color color;
    [SyncVar(hook = nameof(PlayerKickUpdate))] public ulong playerToKickSteamId;

    public MeshRenderer mesh;

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
        DontDestroyOnLoad(gameObject);
    }

    public override void OnStartAuthority()
    {
        CmdSetPlayerName(SteamFriends.GetPersonaName().ToString());
        CmdSetPlayerColor(new Color(FBPP.GetFloat("colorR") / 255, FBPP.GetFloat("colorG") / 255, FBPP.GetFloat("colorB") / 255));

        gameObject.name = "LocalGamePlayer";

        GameManager.instance.FindLocalPlayerManager();

        clientSide.SetActive(true);
    }

    public override void OnStartClient()
    {
        Manager.PlayerManagers.Add(this);

        GameManager.instance.UpdateLobbyName();

        GameManager.instance.UpdatePlayersAndListItems();
    }

    private void OnDestroy()
    {
        Destroy(cameraHolder);
    }

    public void LeaveLobby()
    {
        if (isOwned)
        {
            if (leader)
            {
                SteamMatchmaking.SetLobbyData((CSteamID)LobbyManager.instance.joinedLobbyID, "active", "false");
            }

            GameManager.instance.DestroyPlayerListItems();

            SteamMatchmaking.LeaveLobby((CSteamID)LobbyManager.instance.joinedLobbyID);

            if (isServer)
            {
                Manager.StopHost();
            }
            if (isClient)
            {
                Manager.StopClient();
            }
        }
    }

    public override void OnStopClient()
    {
        Debug.Log(username + " is quiting the game.");

        Manager.PlayerManagers.Remove(this);

        GameManager.instance.UpdatePlayersAndListItems();
    }

    //Kick Player
    [Command]
    private void CmdAddKickPlayer(ulong steamId)
    {
        PlayerKickUpdate(playerToKickSteamId, steamId);
    }
    public void PlayerKickUpdate(ulong oldValue, ulong newValue)
    {
        if (isServer)
        {
            playerToKickSteamId = newValue;
        }
        if (isClient) //Client
        {
            GameManager.instance.UpdatePlayersAndListItems();
        }
    }
    public void KickPlayer(ulong steamId)
    {
        if (isOwned)
        {
            CmdAddKickPlayer(steamId);
        }
    }

    //Name
    [Command]
    private void CmdSetPlayerName(string username)
    {
        PlayerNameUpdate(this.username, username);
    }
    public void PlayerNameUpdate(string oldValue, string newValue)
    {
        if (isServer) //Host
        {
            username = newValue;
        }
        if (isClient) //Client
        {
            GameManager.instance.UpdatePlayersAndListItems();
        }
    }

    //Ready
    [Command]
    private void CmdSetPlayerReady()
    {
        PlayerReadyUpdate(ready, !ready);
    }
    public void PlayerReadyUpdate(bool oldValue, bool newValue)
    {
        if (isServer)
        {
            ready = newValue;
        }
        if (isClient)
        {
            GameManager.instance.UpdatePlayersAndListItems();
        }
    }
    public void ChangeReady()
    {
        if (isOwned)
        {
            CmdSetPlayerReady();
        }
    }

    //Color
    [Command]
    private void CmdSetPlayerColor(Color color)
    {
        PlayerColorUpdate(this.color, color);
    }
    public void PlayerColorUpdate(Color oldValue, Color newValue)
    {
        if (isServer)
        {
            color = newValue;
        }
        if (isClient)
        {
            GameManager.instance.UpdatePlayersAndListItems();
        }
    }

    //Start
    [Command]
    private void CmdChangeScene(string sceneName)
    {
        manager.ChangeScene(sceneName);
    }
    public void ChangeScene(string sceneName)
    {
        if (isOwned)
        {
            CmdChangeScene(sceneName);
        }
    }

}