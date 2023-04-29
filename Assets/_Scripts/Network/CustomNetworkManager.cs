using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Steamworks;

public class CustomNetworkManager : NetworkManager
{
    [SerializeField] private PlayerManager playerManager;

    public List<PlayerManager> PlayerManagers { get; } = new();

    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        PlayerManager PlayerManagerInstance = Instantiate(playerManager);

        PlayerManagerInstance.connectionId = conn.connectionId;
        PlayerManagerInstance.playerIdNumber = PlayerManagers.Count + 1;
        PlayerManagerInstance.steamId = (ulong)SteamMatchmaking.GetLobbyMemberByIndex((CSteamID)LobbyManager.instance.joinedLobbyID, PlayerManagers.Count);

        NetworkServer.AddPlayerForConnection(conn, PlayerManagerInstance.gameObject);

        Debug.Log("Player added. Player name: " + PlayerManagerInstance.username + ". Player connection id: " + PlayerManagerInstance.connectionId.ToString());
    }

    public override void OnServerDisconnect(NetworkConnectionToClient conn)
    {
        if (conn.identity != null) 
        { 
            PlayerManagers.Remove(conn.identity.GetComponent<PlayerManager>()); 
            NetworkServer.RemovePlayerForConnection(conn, true); 
        }
    }

    public override void OnStopServer()
    {
        PlayerManagers.Clear();
    }

    public void ChangeScene(string sceneName)
    {
        ServerChangeScene(sceneName);
    }
}
