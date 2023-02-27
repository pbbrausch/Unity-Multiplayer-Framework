using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;
using TMPro;

public class LobbyListItem : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TMP_Text LobbyNameText;
    [SerializeField] private TMP_Text NumerOfPlayersText;

    [HideInInspector] public CSteamID lobbyId;
    [HideInInspector] public string lobbyName;
    [HideInInspector] public int numberOfPlayers;
    [HideInInspector] public int maxNumberOfPlayers;

    public void SetLobbyItemValues()
    {
        LobbyNameText.text = lobbyName;
        NumerOfPlayersText.text = numberOfPlayers.ToString() + "/" + maxNumberOfPlayers.ToString();
    }

    public void JoinLobby()
    {
        Debug.Log("Player selected to join lobby with steam id of: " + lobbyId.ToString());
        LobbyManager.instance.JoinLobby(lobbyId);
    }
}
