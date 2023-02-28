using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Steamworks;

public class CustomNetworkManager : NetworkManager
{
    [SerializeField] private PlayerManager playerManager;

    public List<PlayerManager> PlayerManagers { get; } = new List<PlayerManager>();

    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        PlayerManager PlayerManagerInstance = Instantiate(playerManager);

        PlayerManagerInstance.leader = PlayerManagers.Count == 0;
        PlayerManagerInstance.connectionId = conn.connectionId;
        PlayerManagerInstance.playerIdNumber = PlayerManagers.Count + 1;
        PlayerManagerInstance.steamId = (ulong)SteamMatchmaking.GetLobbyMemberByIndex((CSteamID)LobbyManager.instance.currentLobbyID, PlayerManagers.Count);

        NetworkServer.AddPlayerForConnection(conn, PlayerManagerInstance.gameObject);

        Debug.Log("Player added. Player name: " + PlayerManagerInstance.username + ". Player connection id: " + PlayerManagerInstance.connectionId.ToString());
    }

    public override void OnServerDisconnect(NetworkConnectionToClient conn)
    {
        if (conn.identity != null) { PlayerManagers.Remove(conn.identity.GetComponent<PlayerManager>()); }
    }

    public override void OnStopServer()
    {
        PlayerManagers.Clear();
    }
}