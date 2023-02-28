using System.Collections.Generic;
using UnityEngine;
using Steamworks;
using Mirror;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class GameManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject playerListItemPrefab;
    [SerializeField] private TMP_Text lobbyNameText;
    [SerializeField] private Transform content;

    public static GameManager instance;

    private bool PlayerItemsCreated;
    private List<PlayerListItem> playerListItems = new();

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
        if (instance == null) { instance = this; }
    }

    //Update Lobby Data
    public void UpdateLobbyName()
    {
        lobbyNameText.text = SteamMatchmaking.GetLobbyData((CSteamID)LobbyManager.instance.currentLobbyID, "name");
    }

    //Update PlayerListITems
    public void UpdatePlayerListItems()
    {
        if (!PlayerItemsCreated)
            CreateListItems();
        if (playerListItems.Count < Manager.PlayerManagers.Count)
            CreateNewListItems();
        if (playerListItems.Count > Manager.PlayerManagers.Count)
            RemoveListItems();
        if (playerListItems.Count == Manager.PlayerManagers.Count)
            UpdateListItems();
    }

    //Player List Items
    private void CreateListItems()
    {
        foreach (PlayerManager player in Manager.PlayerManagers)
        {
            GameObject playerListItem = Instantiate(playerListItemPrefab);
            PlayerListItem playerListItemScript = playerListItem.GetComponent<PlayerListItem>();

            if (player.leader)
                playerListItemScript.leaderIcon.SetActive(true);
            playerListItemScript.username = player.username;
            playerListItemScript.connectionID = player.connectionId;
            playerListItemScript.steamId = player.steamId;
            playerListItemScript.SetPlayerListItemValues();

            playerListItem.transform.SetParent(content);
            playerListItem.transform.localScale = Vector3.one;

            playerListItems.Add(playerListItemScript);
        }

        PlayerItemsCreated = true;
    }
    private void CreateNewListItems()
    {
        foreach (PlayerManager player in Manager.PlayerManagers)
        {
            if (!playerListItems.Any(b => b.connectionID == player.connectionId))
            {
                GameObject playerListItem = Instantiate(playerListItemPrefab);
                PlayerListItem playerListItemScript = playerListItem.GetComponent<PlayerListItem>();

                if (player.leader)
                    playerListItemScript.leaderIcon.SetActive(true);
                playerListItemScript.username = player.username;
                player.usernameText.text = playerListItemScript.username;
                playerListItemScript.connectionID = player.connectionId;
                playerListItemScript.steamId = player.steamId;
                playerListItemScript.SetPlayerListItemValues();

                playerListItem.transform.SetParent(content);
                playerListItem.transform.localScale = Vector3.one;

                playerListItems.Add(playerListItemScript);
            }
        }
    }
    private void RemoveListItems()
    {
        List<PlayerListItem> playerListItemsToRemove = new();
        foreach (PlayerListItem playerListItem in playerListItems)
        {
            if (!Manager.PlayerManagers.Any(b => b.connectionId == playerListItem.connectionID))
            {
                playerListItemsToRemove.Add(playerListItem);
            }
        }
        if (playerListItemsToRemove.Count > 0)
        {
            foreach (PlayerListItem playerListItemToRemove in playerListItemsToRemove)
            {
                playerListItems.Remove(playerListItemToRemove);
                Destroy(playerListItemToRemove.gameObject);
            }
        }
    }
    private void UpdateListItems()
    {
        foreach (PlayerManager player in Manager.PlayerManagers)
        {
            foreach (PlayerListItem playerListItemScript in playerListItems)
            {
                if (playerListItemScript.connectionID == player.connectionId)
                {
                    playerListItemScript.username = player.username;
                    playerListItemScript.SetPlayerListItemValues();
                }
            }
        }
    }

    public void DestroyPlayerListItems()
    {
        foreach (PlayerListItem playerListItem in playerListItems)
        {
            Destroy(playerListItem.gameObject);
        }
        playerListItems.Clear();
    }
}
