using UnityEngine;
using Steamworks;
using Mirror;
using UnityEngine.SceneManagement;
using TMPro;


public class PlayerManager : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject clientSide;
    [SerializeField] private GameObject serverSide;
    public TMP_Text usernameText;

    //Player Info (static)
    [SyncVar] public ulong steamId;
    [SyncVar] public int playerIdNumber;
    [SyncVar] public int connectionId;
    [SyncVar] public bool leader;

    //Player Info (updated)
    [SyncVar(hook = nameof(PlayerNameUpdate))] public string username;

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

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    public override void OnStartAuthority()
    {
        CmdUpdatePlayerName(SteamFriends.GetPersonaName().ToString());

        serverSide.SetActive(false);
        clientSide.SetActive(true);

        gameObject.name = "LocalGamePlayer";

        GameManager.instance.UpdateLobbyName();
    }

    public override void OnStartClient()
    {
        Manager.PlayerManagers.Add(this);
        GameManager.instance.UpdateLobbyName();
        GameManager.instance.UpdatePlayerListItems();
    }

    public override void OnStopClient()
    {
        Manager.PlayerManagers.Remove(this);
        GameManager.instance.UpdatePlayerListItems();
        Debug.Log(username + " is quiting the game.");
    }

    //Leave Lobby
    public void LeaveLobby()
    {
        SceneManager.LoadScene("MainMenu");
        GameManager.instance.DestroyPlayerListItems();
        SteamMatchmaking.LeaveLobby((CSteamID)LobbyManager.instance.currentLobbyID);
        print("Leave lobby");

        if (isOwned)
        {
            if (leader)
            {
                Manager.StopHost();
            }
            else
            {
                Manager.StopClient();
            }

            print("Destroy List");
        }
    }

    //Name Update
    [Command]
    private void CmdUpdatePlayerName(string name)
    {
        Debug.Log("CmdSetPlayerName: Setting username name to: " + name);
        PlayerNameUpdate(username, name);
    }
    private void PlayerNameUpdate(string oldValue, string newValue)
    {
        if (isServer)
            username = newValue;
        if (isClient)
            GameManager.instance.UpdatePlayerListItems();
    }
}