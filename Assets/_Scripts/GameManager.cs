using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using Steamworks;
using Mirror;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("References")]
    [SerializeField] private GameObject playerListItemPrefab;
    [SerializeField] private Button startGameButton;
    [SerializeField] private Button readyButton;
    [SerializeField] private Button endGameButton;
    [SerializeField] private TMP_Text lobbyNameText;
    [SerializeField] private TMP_Text readyText;
    [SerializeField] private Transform content;

    private PlayerManager localPlayerManager;

    public Transform[] spawns;

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

    private void Awake()
    {
        if (instance == null)
        {
            SceneManager.activeSceneChanged += SceneChanged;
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void SceneChanged(Scene current, Scene next)
    {
        StartCoroutine(WaitTillLoaded(next));
    }

    private IEnumerator WaitTillLoaded(Scene next)
    {
        while (!next.isLoaded)
        {
            yield return null;
        }

        switch (next.name)
        {
            case ("Main"):

                Debug.Log("Returning to Menu");

                SceneManager.activeSceneChanged -= SceneChanged;

                instance = null;

                Destroy(gameObject);

                break;

            case ("Lobby"):
                Debug.Log("Going to Lobby");

                startGameButton.gameObject.SetActive(true);
                readyButton.gameObject.SetActive(true);
                endGameButton.gameObject.SetActive(false);

                foreach (PlayerListItem playerListItemScript in playerListItems)
                {
                    playerListItemScript.readyText.gameObject.SetActive(true);
                }

                spawns = GameObject.FindGameObjectWithTag("Spawns").GetComponent<Spawns>().spawns;

                UpdateListItems();

                break;

            //any game scenes
            default:
                Debug.Log("Going to Game Scene");

                startGameButton.gameObject.SetActive(false);
                readyButton.gameObject.SetActive(false);
                endGameButton.gameObject.SetActive(true);

                foreach (PlayerListItem playerListItemScript in playerListItems)
                {
                    playerListItemScript.readyText.gameObject.SetActive(false);
                }

                foreach (PlayerManager playerManager in Manager.PlayerManagers)
                {
                    playerManager.rb.isKinematic = false;
                }

                UpdateListItems();

                break;
        }
    }

    public void FindLocalPlayerManager()
    {
        localPlayerManager = GameObject.Find("LocalGamePlayer").GetComponent<PlayerManager>();
    }

    //QuitLobby
    public void LeaveLobby()
    {
        localPlayerManager.LeaveLobby();
    }

    //Change Ready
    public void ChangeReady()
    {
        Debug.Log("Changing Ready");

        localPlayerManager.ChangeReady();
    }

    private void UpdateButton()
    {
        if (localPlayerManager.ready)
        {

            readyText.text = "Unready";
        }
        else
        {
            readyText.text = "Ready";
        }
    }

    //Start Game
    public void ChangeScene(string sceneName)
    {
        Debug.Log("Changing Scene");

        localPlayerManager.ChangeScene(sceneName);
    }

    //Update Lobby Data
    public void UpdateLobbyName()
    {
        lobbyNameText.text = SteamMatchmaking.GetLobbyData((CSteamID)LobbyManager.instance.joinedLobbyID, "name");
    }

    //Update PlayerListITems
    public void UpdatePlayerListItems()
    {
        if (!PlayerItemsCreated) { CreateListItems(); }
        if (playerListItems.Count < Manager.PlayerManagers.Count) { CreateNewListItems(); }
        if (playerListItems.Count > Manager.PlayerManagers.Count) { RemoveListItems(); }
        if (playerListItems.Count == Manager.PlayerManagers.Count) { UpdateListItems(); }
    }

    //Player List Items
    private void CreateListItems()
    {
        foreach (PlayerManager playerManager in Manager.PlayerManagers)
        {
            GameObject playerListItem = Instantiate(playerListItemPrefab);
            PlayerListItem playerListItemScript = playerListItem.GetComponent<PlayerListItem>();

            //PlayerListItemScript                        
            if (playerManager.isOwned || SteamFriends.GetFriendRelationship((CSteamID)playerManager.steamId) == EFriendRelationship.k_EFriendRelationshipFriend)
            {
                playerListItemScript.addFriendButton.SetActive(false);
                playerListItemScript.leaderIcon.transform.position = playerListItemScript.leaderIconsPos[0].position;
                if (playerManager.leader)
                {
                    playerListItemScript.leaderIcon.SetActive(true);
                    playerListItemScript.readyText.transform.position = playerListItemScript.readyTextsPos[1].position;
                }
                else
                {
                    playerListItemScript.leaderIcon.SetActive(false);
                    playerListItemScript.readyText.transform.position = playerListItemScript.readyTextsPos[0].position;
                }
            }
            else
            {
                playerListItemScript.addFriendButton.SetActive(true);
                playerListItemScript.leaderIcon.transform.position = playerListItemScript.leaderIconsPos[1].position;
                if (playerManager.leader)
                {
                    playerListItemScript.leaderIcon.SetActive(true);
                    playerListItemScript.readyText.transform.position = playerListItemScript.readyTextsPos[2].position;
                }
                else
                {
                    playerListItemScript.leaderIcon.SetActive(false);
                    playerListItemScript.readyText.transform.position = playerListItemScript.readyTextsPos[1].position;
                }
            }

            playerListItemScript.ready = playerManager.ready;
            playerListItemScript.username = playerManager.username;
            playerListItemScript.connectionID = playerManager.connectionId;
            playerListItemScript.steamId = playerManager.steamId;
            playerListItemScript.SetPlayerListItemValues();

            //PlayerListItem
            playerListItem.transform.SetParent(content);
            playerListItem.transform.localScale = Vector3.one;

            playerListItems.Add(playerListItemScript);
        }

        PlayerItemsCreated = true;
    }

    private void CreateNewListItems()
    {
        foreach (PlayerManager playerManager in Manager.PlayerManagers)
        {
            if (!playerListItems.Any(b => b.connectionID == playerManager.connectionId))
            {
                GameObject playerListItem = Instantiate(playerListItemPrefab);
                PlayerListItem playerListItemScript = playerListItem.GetComponent<PlayerListItem>();

                //PlayerListItemScript                        
                if (playerManager.isOwned || SteamFriends.GetFriendRelationship((CSteamID)playerManager.steamId) == EFriendRelationship.k_EFriendRelationshipFriend)
                {
                    playerListItemScript.addFriendButton.SetActive(false);
                    playerListItemScript.leaderIcon.transform.position = playerListItemScript.leaderIconsPos[0].position;
                    if (playerManager.leader)
                    {
                        playerListItemScript.leaderIcon.SetActive(true);
                        playerListItemScript.readyText.transform.position = playerListItemScript.readyTextsPos[1].position;
                    }
                    else
                    {
                        playerListItemScript.leaderIcon.SetActive(false);
                        playerListItemScript.readyText.transform.position = playerListItemScript.readyTextsPos[0].position;
                    }
                }
                else
                {
                    playerListItemScript.addFriendButton.SetActive(true);
                    playerListItemScript.leaderIcon.transform.position = playerListItemScript.leaderIconsPos[1].position;
                    if (playerManager.leader)
                    {
                        playerListItemScript.leaderIcon.SetActive(true);
                        playerListItemScript.readyText.transform.position = playerListItemScript.readyTextsPos[2].position;
                    }
                    else
                    {
                        playerListItemScript.leaderIcon.SetActive(false);
                        playerListItemScript.readyText.transform.position = playerListItemScript.readyTextsPos[1].position;
                    }
                }

                playerListItemScript.ready = playerManager.ready;
                playerListItemScript.username = playerManager.username;
                playerListItemScript.connectionID = playerManager.connectionId;
                playerListItemScript.steamId = playerManager.steamId;
                playerListItemScript.SetPlayerListItemValues();

                //PlayerListItem
                playerListItem.transform.SetParent(content);
                playerListItem.transform.localScale = Vector3.one;

                playerListItems.Add(playerListItemScript);
            }
        }
    }

    private void UpdateListItems()
    {
        foreach (PlayerManager playerManager in Manager.PlayerManagers)
        {
            foreach (PlayerListItem playerListItemScript in playerListItems)
            {
                if (playerListItemScript.connectionID == playerManager.connectionId)
                {
                    //PlayerManager
                    playerManager.usernameText.text = playerManager.username;

                    if (SceneManager.GetActiveScene().name == "Lobby")
                    {
                        playerManager.gameObject.transform.SetPositionAndRotation(spawns[playerManager.playerIdNumber - 1].position, spawns[playerManager.playerIdNumber - 1].rotation);
                        playerManager.rb.isKinematic = true;
                    }

                    //PlayerListItemScript                        
                    if (playerManager.isOwned || SteamFriends.GetFriendRelationship((CSteamID)playerManager.steamId) == EFriendRelationship.k_EFriendRelationshipFriend)
                    {
                        playerListItemScript.addFriendButton.SetActive(false);
                        playerListItemScript.leaderIcon.transform.position = playerListItemScript.leaderIconsPos[0].position;
                        if (playerManager.leader)
                        {
                            playerListItemScript.leaderIcon.SetActive(true);
                            playerListItemScript.readyText.transform.position = playerListItemScript.readyTextsPos[1].position;
                        }
                        else
                        {
                            playerListItemScript.leaderIcon.SetActive(false);
                            playerListItemScript.readyText.transform.position = playerListItemScript.readyTextsPos[0].position;
                        }
                    }
                    else
                    {
                        playerListItemScript.addFriendButton.SetActive(true);
                        playerListItemScript.leaderIcon.transform.position = playerListItemScript.leaderIconsPos[1].position;
                        if (playerManager.leader)
                        {
                            playerListItemScript.leaderIcon.SetActive(true);
                            playerListItemScript.readyText.transform.position = playerListItemScript.readyTextsPos[2].position;
                        }
                        else
                        {
                            playerListItemScript.leaderIcon.SetActive(false);
                            playerListItemScript.readyText.transform.position = playerListItemScript.readyTextsPos[1].position;
                        }
                    }

                    playerListItemScript.ready = playerManager.ready;
                    playerListItemScript.username = playerManager.username;
                    playerListItemScript.SetPlayerListItemValues();

                    if (playerManager == localPlayerManager)
                    {
                        if (playerManager.leader)
                        {
                            startGameButton.interactable = AllReady();
                            endGameButton.interactable = true;
                        }
                        else
                        {
                            startGameButton.interactable = false;
                            endGameButton.interactable = false;
                        }

                        UpdateButton();
                    }
                }
            }
        }
    }

    private bool AllReady()
    {
        foreach (PlayerManager playerManager in Manager.PlayerManagers)
        {
            if (!playerManager.ready)
            {
                return false;
            }
        }

        return true;
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

        foreach (PlayerListItem playerListItemToRemove in playerListItemsToRemove)
        {
            playerListItems.Remove(playerListItemToRemove);
            Destroy(playerListItemToRemove.gameObject);
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