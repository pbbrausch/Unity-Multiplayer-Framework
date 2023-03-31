using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Steamworks;
using TMPro;

public class LobbyListItem : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text statusText;
    [SerializeField] private Button joinButton;
    [SerializeField] private TMP_Text numberOfPlayersText;

    [HideInInspector] public CSteamID lobbyId;
    [HideInInspector] public string lobbyName;
    [HideInInspector] public string lobbyStatus;
    [HideInInspector] public int numberOfPlayers;
    [HideInInspector] public int maxNumberOfPlayers;

    public void SetLobbyItemValues()
    {
        nameText.text = lobbyName;
        statusText.text = lobbyStatus;
        if (lobbyStatus == "In-Lobby")
        {
            joinButton.interactable = true;
        }
        numberOfPlayersText.text = numberOfPlayers.ToString() + "/" + maxNumberOfPlayers.ToString();
    }

    public void JoinLobby()
    {
        if (SteamMatchmaking.GetLobbyData(lobbyId, "status") == "In-Lobby")
        {
            Debug.Log("Player selected to join lobby with steam id of: " + lobbyId.ToString());
            LobbyManager.instance.JoinLobby(lobbyId);
        }
    }
}
