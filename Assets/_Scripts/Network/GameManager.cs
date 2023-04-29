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
    [SerializeField] private GameObject lobbyButtons;
    [SerializeField] private GameObject playerListMenu;
    [SerializeField] private GameObject mapSelectMenu;
    [SerializeField] private Button mapMenuButton;
    [SerializeField] private Button startGameButton;
    [SerializeField] private Button endGameButton;
    [SerializeField] private TMP_Text lobbyNameText;
    [SerializeField] private TMP_Text readyText;
    [SerializeField] private Transform content;
    [SerializeField] private Material temp;

    public GameObject scoreboard;
    public GameObject options;

    [HideInInspector] public PlayerManager localPlayerManager;

    private Transform[] spawns;

    private bool PlayerItemsCreated;
    private List<PlayerListItem> playerListItems = new();

    private CustomNetworkManager manager;

    public CustomNetworkManager Manager
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

                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;

                scoreboard.SetActive(true);
                options.SetActive(false);
                lobbyButtons.SetActive(true);

                spawns = GameObject.FindGameObjectWithTag("Spawns").GetComponent<Spawns>().spawns;
                int memberLimit = SteamMatchmaking.GetLobbyMemberLimit((CSteamID)LobbyManager.instance.joinedLobbyID);
                for (int i = memberLimit; i < 10; i++)
                {
                    spawns[i].parent.gameObject.SetActive(false);
                }

                foreach (PlayerManager playerManager in Manager.PlayerManagers)
                {
                    playerManager.ready = false;
                    playerManager.rb.isKinematic = true;
                    playerManager.gameObject.transform.SetPositionAndRotation(spawns[playerManager.playerIdNumber - 1].position, spawns[playerManager.playerIdNumber - 1].rotation);
                }

                foreach (PlayerListItem playerListItemScript in playerListItems)
                {
                    playerListItemScript.readyText.gameObject.SetActive(true);
                }

                break;

            //any game scenes
            default:
                Debug.Log("Going to Game Scene");

                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;

                scoreboard.SetActive(false);
                lobbyButtons.SetActive(false);
                playerListMenu.SetActive(true);
                mapSelectMenu.SetActive(false);

                foreach (PlayerManager playerManager in Manager.PlayerManagers)
                {
                    playerManager.rb.isKinematic = false;
                }

                foreach (PlayerListItem playerListItemScript in playerListItems)
                {
                    playerListItemScript.readyText.gameObject.SetActive(false);
                }

                break;
        }

        UpdatePlayersAndListItems();
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

    public void AddKickPlayer(ulong steamId)
    {
        localPlayerManager.KickPlayer(steamId);
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
    public void ChangeScene(bool lobby)
    {
        Debug.Log("Changing Scene");

        if (lobby)
        {
            SteamMatchmaking.SetLobbyData((CSteamID)LobbyManager.instance.joinedLobbyID, "status", "In-Lobby");
            localPlayerManager.ChangeScene("Lobby");
        }
        else
        {
            SteamMatchmaking.SetLobbyData((CSteamID)LobbyManager.instance.joinedLobbyID, "status", "In-Game");
            localPlayerManager.ChangeScene(gameScene);
        }
    }

    //Update Lobby Data
    public void UpdateLobbyName()
    {
        lobbyNameText.text = SteamMatchmaking.GetLobbyData((CSteamID)LobbyManager.instance.joinedLobbyID, "name");
    }

    //Update PlayerListITems
    public void UpdatePlayersAndListItems()
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

            playerListItemScript.ready = playerManager.ready;
            playerListItemScript.username = playerManager.username;
            playerListItemScript.connectionId = playerManager.connectionId;
            playerListItemScript.playerIdNumber = playerManager.playerIdNumber;
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
            if (!playerListItems.Any(b => b.connectionId == playerManager.connectionId))
            {
                GameObject playerListItem = Instantiate(playerListItemPrefab);
                PlayerListItem playerListItemScript = playerListItem.GetComponent<PlayerListItem>();
               
                //PlayerListItemScript           s             
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

                playerListItemScript.ready = playerManager.ready;
                playerListItemScript.username = playerManager.username;
                playerListItemScript.connectionId = playerManager.connectionId;
                playerListItemScript.playerIdNumber = playerManager.playerIdNumber;
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
                if (playerListItemScript.connectionId == playerManager.connectionId)
                {
                    //PlayerManager
                    playerManager.usernameText.text = playerManager.username;

                    Material mat = new(temp);
                    mat.color = playerManager.color;
                    playerManager.mesh.material = mat;

                    //PlayerListItemScript                        
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

                    if (playerManager.playerToKickSteamId > 0 && playerManager.leader)
                    {
                        if (playerManager.playerToKickSteamId == localPlayerManager.steamId)
                        {
                            LeaveLobby();
                        }
                    }

                    playerListItemScript.ready = playerManager.ready;
                    playerListItemScript.username = playerManager.username;
                    playerListItemScript.SetPlayerListItemValues();

                    if (SceneManager.GetActiveScene().name == "Lobby")
                    {
                        playerManager.rb.isKinematic = true;
                        playerManager.gameObject.transform.SetPositionAndRotation(spawns[playerManager.playerIdNumber - 1].position, spawns[playerManager.playerIdNumber - 1].rotation);
                    }

                    if (playerManager == localPlayerManager)
                    {
                        UpdateButton();

                        if (playerManager.leader)
                        {
                            startGameButton.interactable = AllReady();
                            endGameButton.interactable = true;
                            mapMenuButton.interactable = true;
                        }
                        else
                        {
                            startGameButton.interactable = false;
                            endGameButton.interactable = false;
                            mapMenuButton.interactable = false;
                        }
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
            if (!Manager.PlayerManagers.Any(b => b.connectionId == playerListItem.connectionId))
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

    [System.Serializable]
    public struct Map
    {
        public string name;
        public Sprite image;
        [Scene]
        public string scene;
    }

    [Header("Maps")]
    public TMP_Text currentMapName;
    public TMP_Text mapName;
    public Image mapImage;

    public Map[] maps;

    [Scene]
    public string gameScene;

    private int currentMapIndex;

    public void ChangeIndex(bool right)
    {
        if (right)
        {
            if (currentMapIndex + 1 > maps.Length - 1)
            {
                currentMapIndex = 0;
            }
            else
            {
                currentMapIndex++;
            }
        }
        else
        {
            if (currentMapIndex - 1 < 0)
            {
                currentMapIndex = maps.Length - 1;
            }
            else
            {
                currentMapIndex--;
            }
        }

        mapImage.sprite = maps[currentMapIndex].image;
        mapName.text = maps[currentMapIndex].name;
    }

    public void SelectMap()
    {
        currentMapName.text = "Current Map: " + maps[currentMapIndex].name;
        gameScene = maps[currentMapIndex].scene;
    }
}