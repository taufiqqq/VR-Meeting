using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Lobbies; //rectangular notation external actor (server side)
using Unity.Services.Lobbies.Models;
using UnityEngine.SceneManagement;

public class NetworkConnect : MonoBehaviour
{   
    private string joinCode;
    private Lobby currentLobby;
    private float heartBeatTimer;
    private string playerId;

    private List<string> joinedPlayers = new List<string>();

    public int maxConnection;
    public UnityTransport transport;

    public AuthManager authManager;
    
     private async void Awake()
    {
        await UnityServices.InitializeAsync();

        // Subscribe to the sign-in complete event
        AuthManager.Instance.OnSignInComplete += OnSignInComplete;

    }

    private void OnSignInComplete()
    {
        AuthManager.Instance.Create();
    }

    // Start is called before the first frame update

    public async void Create()
    {
            SceneManager.LoadScene(2);

            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(maxConnection);
            string newJoinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
            Debug.Log(newJoinCode);
            Debug.LogError(newJoinCode);

            transport.SetHostRelayData(allocation.RelayServer.IpV4, (ushort)allocation.RelayServer.Port, allocation.AllocationIdBytes, allocation.Key, allocation.ConnectionData);

            CreateLobbyOptions lobbyOptions = new CreateLobbyOptions();

            lobbyOptions.IsPrivate = false;

            lobbyOptions.Data = new Dictionary<string, DataObject>();
            DataObject dataObject = new DataObject(DataObject.VisibilityOptions.Public, newJoinCode);

            string hostName = "admin"; // Replace with the actual host name
            DataObject hostNameDataObject = new DataObject(DataObject.VisibilityOptions.Public, hostName);
            
            lobbyOptions.Data.Add("JOIN_CODE", dataObject);
            lobbyOptions.Data.Add("HOST_NAME", hostNameDataObject);

            currentLobby = await Lobbies.Instance.CreateLobbyAsync("Meeting Name", maxConnection, lobbyOptions);

            Debug.LogError("Lobby Code : " + currentLobby.LobbyCode);

            joinCode = currentLobby.LobbyCode;
            
            UpdateLobbyCode(joinCode);
            NetworkManager.Singleton.StartHost();
    }
    

    public async void Join()
    {
        QueryResponse queryResponse = await Lobbies.Instance.QueryLobbiesAsync();

        if (joinCode != "") { 
        try
        {
            currentLobby = await Lobbies.Instance.JoinLobbyByCodeAsync(joinCode);
            SceneManager.LoadScene(2);
        }
        catch (LobbyServiceException e)
        {
            Debug.LogError(e);
        }

        string relayJoinCode = currentLobby.Data["JOIN_CODE"].Value;
        Debug.LogError(relayJoinCode);
        JoinAllocation allocation = await RelayService.Instance.JoinAllocationAsync(relayJoinCode);

        transport.SetClientRelayData(allocation.RelayServer.IpV4, (ushort)allocation.RelayServer.Port, allocation.AllocationIdBytes, allocation.Key, allocation.ConnectionData, allocation.HostConnectionData);
        
        Debug.LogError("Lobby ID : " + currentLobby.Id);
        Debug.LogError(currentLobby.Data["JOIN_CODE"].Value);
        
        UpdateLobbyCode(joinCode);
        NetworkManager.Singleton.StartClient();
        }
    }

    private void Update()
    {
        try
        {
            if (heartBeatTimer > 15)
            {
                heartBeatTimer -= 15;

                if (currentLobby != null)
                {
                    if (currentLobby.HostId == AuthenticationService.Instance.PlayerId)
                    {
                        LobbyService.Instance.SendHeartbeatPingAsync(currentLobby.Id);
                        Debug.LogError(currentLobby.Id);
                        Debug.LogError(currentLobby.Data["JOIN_CODE"].Value);
                    }
                }
                else
                {
                    // Handle the case when currentLobby is null (e.g., the main lobby)
                    Debug.LogWarning("Current lobby is null (Main Lobby)");
                }
            }
            heartBeatTimer += Time.deltaTime;
        }
        catch (Exception e)
        {
            Debug.LogError($"An error occurred: {e}");
        }
    }


    public void setJoinCode(string data)
    {
        joinCode = data;
        // You can perform additional network-related operations if needed
    }

    public string getJoinCode()
    {
        return joinCode;
    }
    private void UpdateLobbyCode(string lobbyCode)
    {
        DisplayJoinCode displayJoinCode = FindObjectOfType<DisplayJoinCode>();

        displayJoinCode.UpdateLobbyCode(lobbyCode);
    }

    public async void ListLobbies()
    {
        try
        {
            QueryResponse queryResponse = await Lobbies.Instance.QueryLobbiesAsync();

            Debug.Log("Lobbies found: " + queryResponse.Results.Count);
            foreach (Lobby lobby in queryResponse.Results)
            {
                Debug.Log(lobby.LobbyCode + " " + lobby.MaxPlayers);

                if (lobby.Data.ContainsKey("HOST_NAME"))
                {
                    if (lobby.Data["HOST_NAME"].Value == "admin")
                    {
                        Debug.Log("lurve: " + lobby.LobbyCode);
                        joinCode = lobby.LobbyCode;
                        Debug.Log(lobby.Data["HOST_NAME"].Value);
                        Debug.Log(joinCode);
                    }
                }
            }
        } catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    public void setMaxPlayer(int data)
    {
        maxConnection = data;
        // You can perform additional network-related operations if needed
    }

    public int getMaxPlayer()
    {
        return maxConnection;
    }
 

    private bool IsHost()
    {

        if (currentLobby != null && currentLobby.HostId == playerId)
        {
            Debug.Log("this is Host ");
            return true;
        }
        return false;
    }

    private void HandlePlayerJoined(ulong clientId)
    {
        string playerId = clientId.ToString(); // Convert the client ID to a string
        joinedPlayers.Add(playerId);

        // You can do additional operations with the playerId if needed
        Debug.Log("Player joined: " + playerId);
        Debug.Log("Total players in the lobby: " + joinedPlayers.Count);

        // Update your UI or perform other actions as needed
        UpdatePlayerList(joinedPlayers);
    }

    private void UpdatePlayerList(List<string> players)
    {
        // Example: Display the player list in the console
        Debug.Log("Player List:");
        foreach (string playerId in players)
        {
            Debug.Log(playerId);
        }

        // You can update your UI or perform other actions with the player list
    }

    public async void LeaveRoom()
    {
        try
        {
            await LobbyService.Instance.RemovePlayerAsync(currentLobby.Id, playerId);
            currentLobby = null;

        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
            Debug.Log("taknak bye");
        }
    }

    public string getPlayerId(){
        return playerId;
    }
}