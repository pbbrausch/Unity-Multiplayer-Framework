using System.Collections.Generic;
using UnityEngine;
using Steamworks;
using Mirror;
using UnityEngine.UI;
using TMPro;
using System.Linq;

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

    private int regionIndex;

    private CustomNetworkManager manager;

    //Regions
    private List<string> regions = new List<string>()
    {
        "World",
        "US-East",
        "US-West",
        "SouthAmerica",
        "Europe",
        "Asia",
        "Australia",
        "MiddleEast",
        "Africa"
    };

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

        manager.StartHost();

        SteamMatchmaking.SetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), HostAddressKey, SteamUser.GetSteamID().ToString());
        SteamMatchmaking.SetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), "name", SteamFriends.GetPersonaName().ToString() + "'s Lobby");
        SteamMatchmaking.SetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), "region", regions[regionIndex]);
        SteamMatchmaking.SetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), "active", "true");
    }

    private void OnLobbyEntered(LobbyEnter_t callback)
    {
        joinedLobbyID = callback.m_ulSteamIDLobby;

        if (NetworkServer.active) { return; }

        manager.networkAddress = SteamMatchmaking.GetLobbyData((CSteamID)callback.m_ulSteamIDLobby, HostAddressKey);

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

    public void HostLobby(int lobbyType, int maxPlayers, int regionType)
    {
        Debug.Log("Hosting lobby");

        manager.maxConnections = maxPlayers;

        regionIndex = regionType;

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
