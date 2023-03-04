using System.Collections.Generic;
using UnityEngine;
using Steamworks;
using TMPro;

public class LobbyListManager : MonoBehaviour
{
    public static LobbyListManager instance;

    [Header("References")]
    [SerializeField] private GameObject LobbyListItemPrefab;
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private Transform content;

    [HideInInspector] public bool reset;

    //Callbacks
    protected Callback<LobbyDataUpdate_t> lobbyData;
    protected Callback<LobbyMatchList_t> lobbyList;

    private readonly List<GameObject> lobbyListItems = new();
    private readonly List<CSteamID> lobbyIDs = new();

    private void Awake()
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
        if (lobbyIDs.Count > 0) { lobbyIDs.Clear(); }

        if (string.IsNullOrEmpty(inputField.text))
        {
            SteamMatchmaking.AddRequestLobbyListResultCountFilter(100);

        }
        else
        {
            print(inputField.text);
            SteamMatchmaking.AddRequestLobbyListStringFilter("name", "%" + inputField.text + "%", ELobbyComparison.k_ELobbyComparisonEqual);
        }
        SteamMatchmaking.RequestLobbyList();
    }

    private void OnGetLobbiesList(LobbyMatchList_t result)
    {
        if (lobbyListItems.Count > 0) { DestroyOldLobbies(); }

        Debug.Log("Found " + result.m_nLobbiesMatching + " lobbies!");

        for (int i=0; i < result.m_nLobbiesMatching; i++)
        {
            CSteamID lobbyID = SteamMatchmaking.GetLobbyByIndex(i);
            lobbyIDs.Add(lobbyID);
            SteamMatchmaking.RequestLobbyData(lobbyID);
        }
    }

    private void OnGetLobbyData(LobbyDataUpdate_t result)
    {
        DisplayLobbies(lobbyIDs, result);
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

    private void DisplayLobbies(List<CSteamID> lobbyIDs, LobbyDataUpdate_t result)
    {
        for (int i=0; i < lobbyIDs.Count; i++)
        {
            if (lobbyIDs[i].m_SteamID == result.m_ulSteamIDLobby)
            {
                Debug.Log("Player searched for lobbies");

                GameObject lobbyListItem = Instantiate(LobbyListItemPrefab);
                LobbyListItem lobbyListItemScript = lobbyListItem.GetComponent<LobbyListItem>();

                lobbyListItemScript.lobbyId = (CSteamID)lobbyIDs[i].m_SteamID;
                lobbyListItemScript.lobbyName = SteamMatchmaking.GetLobbyData((CSteamID)lobbyIDs[i].m_SteamID, "name");
                lobbyListItemScript.numberOfPlayers = SteamMatchmaking.GetNumLobbyMembers((CSteamID)lobbyIDs[i].m_SteamID);
                lobbyListItemScript.maxNumberOfPlayers = SteamMatchmaking.GetLobbyMemberLimit((CSteamID)lobbyIDs[i].m_SteamID);
                lobbyListItemScript.SetLobbyItemValues();

                lobbyListItem.transform.SetParent(content);
                lobbyListItem.transform.localScale = Vector3.one;

                lobbyListItems.Add(lobbyListItem);

                return;
            }
        }

    }
}
