using UnityEngine;
using Mirror;
using Steamworks;

public class LobbyManager : MonoBehaviour
{
    public static LobbyManager instance;

    //Callbacks
    protected Callback<GameLobbyJoinRequested_t> joinRequested;
    protected Callback<LobbyCreated_t> lobbyCreated;
    protected Callback<LobbyEnter_t> lobbyEntered;

    //Current Lobby Data
    [HideInInspector] public ulong joinedLobbyID;

    private const string HostAddressKey = "HostAddress";

    private CustomNetworkManager manager;


    private void Awake()
    {
        //Check if initialized
        if (!SteamManager.Initialized) { return; }

        //Instance Create
        if (instance == null) { instance = this; }

        //Get custom network manager
        manager = GetComponent<CustomNetworkManager>();

        //Create Callbacks for Lobby
        joinRequested = Callback<GameLobbyJoinRequested_t>.Create(OnJoinRequested);
        lobbyCreated = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
        lobbyEntered = Callback<LobbyEnter_t>.Create(OnLobbyEntered);
    }

    private void OnDisable()
    {
        joinRequested.Dispose();
        lobbyCreated.Dispose();
        lobbyEntered.Dispose();
    }

    private void OnJoinRequested(GameLobbyJoinRequested_t callback)
    {
        SteamMatchmaking.JoinLobby(callback.m_steamIDLobby);
    }

    private void OnLobbyCreated(LobbyCreated_t callback)
    {
        if (callback.m_eResult != EResult.k_EResultOK) { return; }

        Debug.Log("Lobby created successfully.");

        manager.StartHost();

        SteamMatchmaking.SetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), HostAddressKey, SteamUser.GetSteamID().ToString());
        SteamMatchmaking.SetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), "name", SteamFriends.GetPersonaName().ToString() + "'s Lobby");
        SteamMatchmaking.SetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), "active", "true");
    }

    private void OnLobbyEntered(LobbyEnter_t callback)
    {
        Debug.Log("Entered lobby with id: " + joinedLobbyID.ToString());

        joinedLobbyID = callback.m_ulSteamIDLobby;

        if (NetworkServer.active) { return; }

        manager.networkAddress = SteamMatchmaking.GetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), HostAddressKey);

        manager.StartClient();

        if (GameObject.Find("LobbyListMenu"))
        {
            LobbyListManager.instance.DestroyOldLobbies();
        }
    }

    public void JoinLobby(CSteamID lobbyId)
    {
        Debug.Log("Trying to join lobby with steam id: " + lobbyId.ToString());
        SteamMatchmaking.JoinLobby(lobbyId);
    }

    public void LeaveLobby(CSteamID lobbyID)
    {
        SteamMatchmaking.LeaveLobby(lobbyID);
    }

    public void HostLobby(int lobbyType, int maxPlayers)
    {
        Debug.Log("Hosting lobby");

        manager.maxConnections = maxPlayers;

        switch (lobbyType)
        {
            case 0:
                SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypePublic, manager.maxConnections);
                break;
            case 1:
                SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypePrivate, manager.maxConnections);
                break;
            case 2:
                SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypeFriendsOnly, manager.maxConnections);
                break;
        }
    }
}
