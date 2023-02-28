using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Steamworks;

public class LobbyListManager : MonoBehaviour
{
    public static LobbyListManager instance;

    [Header("References")]
    [SerializeField] private GameObject LobbyListItemPrefab;
    [SerializeField] private Transform content;

    [HideInInspector] public bool reset;

    //Callbacks
    protected Callback<LobbyDataUpdate_t> lobbyData;
    protected Callback<LobbyMatchList_t> lobbyList;

    private readonly List<GameObject> lobbyListItems = new();
    private readonly List<CSteamID> lobbyIDS = new();

    private void Start()
    {
        //Check if initialized
        if (!SteamManager.Initialized) { return; }

        //Instance Create
        if (instance == null) { instance = this; }

        //Create Callbacks For Lobby
        lobbyData = Callback<LobbyDataUpdate_t>.Create(OnGetLobbyData);
        lobbyList = Callback<LobbyMatchList_t>.Create(OnGetLobbiesList);
    }

    public void GetListOfLobbies()
    {
        reset = false;
        if (lobbyIDS.Count > 0)
            lobbyIDS.Clear();

        SteamMatchmaking.AddRequestLobbyListResultCountFilter(100);
        SteamMatchmaking.RequestLobbyList();
    }

    private void OnGetLobbiesList(LobbyMatchList_t result)
    {
        DestroyOldLobbies();

        Debug.Log("Found " + result.m_nLobbiesMatching + " lobbies!");

        for (int i = 0; i < result.m_nLobbiesMatching; i++)
        {
            CSteamID lobbyID = SteamMatchmaking.GetLobbyByIndex(i);
            lobbyIDS.Add(lobbyID);
            SteamMatchmaking.RequestLobbyData(lobbyID);
        }
    }

    private void OnGetLobbyData(LobbyDataUpdate_t result)
    {
        DisplayLobbies(lobbyIDS, result);
    }

    public void DestroyOldLobbies()
    {
        Debug.Log("Destroyed old lobbies");

        foreach (GameObject lobbyItem in lobbyListItems)
        {     
            Destroy(lobbyItem);
        }

        lobbyListItems.Clear();
    }

    private void DisplayLobbies(List<CSteamID> lobbyIDS, LobbyDataUpdate_t result)
    {
        for (int i = 0; i < lobbyIDS.Count; i++)
        {
            if (lobbyIDS[i].m_SteamID == result.m_ulSteamIDLobby)
            {
                Debug.Log("Player searched for lobbies");

                GameObject lobbyListItem = Instantiate(LobbyListItemPrefab);
                LobbyListItem lobbyListItemScript = lobbyListItem.GetComponent<LobbyListItem>();

                lobbyListItemScript.lobbyId = (CSteamID)lobbyIDS[i].m_SteamID;
                lobbyListItemScript.lobbyName = SteamMatchmaking.GetLobbyData((CSteamID)lobbyIDS[i].m_SteamID, "name");
                lobbyListItemScript.numberOfPlayers = SteamMatchmaking.GetNumLobbyMembers((CSteamID)lobbyIDS[i].m_SteamID);
                lobbyListItemScript.maxNumberOfPlayers = SteamMatchmaking.GetLobbyMemberLimit((CSteamID)lobbyIDS[i].m_SteamID);
                lobbyListItemScript.SetLobbyItemValues();

                lobbyListItem.transform.SetParent(content);
                lobbyListItem.transform.localScale = Vector3.one;

                lobbyListItems.Add(lobbyListItem);

                return;
            }
        }

    }
}
