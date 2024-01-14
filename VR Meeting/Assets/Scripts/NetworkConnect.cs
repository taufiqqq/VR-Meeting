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
    private string playerName;
    private int lobbyBackground = 1;

    private List<string> joinedPlayers = new List<string>();

    public int maxConnection;
    public UnityTransport transport;

    public AuthManager authManager;
    public CloudSave cloudSave;
    public Spawner markerSpawn;
    
     private async void Awake()
    {
        await UnityServices.InitializeAsync();
        AuthManager.Instance.OnSignInComplete += OnSignInComplete;
    }

    private void OnSignInComplete()
    {
        AuthManager.Instance.Create();

    }

    // Start is called before the first frame update

    public async void Create()
    {
            SceneManager.LoadScene(lobbyBackground);

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
        
            DataObject lobbyBGDataObject = new DataObject(DataObject.VisibilityOptions.Public, lobbyBackground.ToString());
        
            lobbyOptions.Data.Add("JOIN_CODE", dataObject);
            lobbyOptions.Data.Add("HOST_NAME", hostNameDataObject);
            lobbyOptions.Data.Add("LOBBY_BG", lobbyBGDataObject);

            currentLobby = await Lobbies.Instance.CreateLobbyAsync("Meeting Name", maxConnection, lobbyOptions);

            Debug.LogError("Lobby Code : " + currentLobby.LobbyCode);
            
            playerId = await AuthenticationService.Instance.GetPlayerNameAsync();
            Debug.Log("Player ID: " + playerId);
        
            joinCode = currentLobby.LobbyCode;
            HandlePlayer();
            markerSpawn.SpawnMarker();
            UpdateLobbyCode(currentLobby.LobbyCode);
            NetworkManager.Singleton.StartHost();
            UpdateLobbyCode(joinCode);
    }
    

    public async void Join()
    {
        QueryResponse queryResponse = await Lobbies.Instance.QueryLobbiesAsync();

        if (joinCode != "") { 
        try
        {
            currentLobby = await Lobbies.Instance.JoinLobbyByCodeAsync(joinCode);
                Debug.LogWarning("dapat we");
            
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
        
        playerId = await AuthenticationService.Instance.GetPlayerNameAsync();
        Debug.Log("Player ID: " + playerId);

        HandlePlayer();
        markerSpawn.SpawnMarker();
        Debug.LogError(currentLobby.Data["LOBBY_BG"].Value);

        lobbyBackground = int.Parse(currentLobby.Data["LOBBY_BG"].Value);
        SceneManager.LoadScene(lobbyBackground);

         NetworkManager.Singleton.StartClient();
        UpdateLobbyCode(joinCode);
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
    }

    public string getJoinCode()
    {
        return joinCode;
    }

    private void UpdateLobbyCode(string lobbyCode)
    {
        if (lobbyCode == null)
        {
            Debug.LogError("Error: Lobby code is null.");
            return;
        }

        DisplayJoinCode displayJoinCode = FindObjectOfType<DisplayJoinCode>();

        if (displayJoinCode == null)
        {
            Debug.LogError("Error: DisplayJoinCode not found in the scene.");
            return;
        }

        try
        {
            displayJoinCode.UpdateLobbyCode(lobbyCode);
        }
        catch (Exception e)
        {
            Debug.LogError($"Error updating lobby code: {e}");
        }
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

    

    private void HandlePlayer()
    {
        joinedPlayers.Add(playerId);

        // Log the joined player ID to the console
        Debug.Log("Player joined: " + playerId);
        Debug.Log("Total players in the lobby: " + joinedPlayers.Count);


        // Update your UI or perform other actions as needed
        UpdatePlayerList(joinedPlayers);
    }

    private void UpdatePlayerList(List<string> players)
    {
        Debug.Log("Player List:");
        foreach (string playerId in players)
        {
            Debug.Log(playerId);
            PlayerList pl = FindObjectOfType<PlayerList>();
            pl.UpdatePlayerList(players);
        }
    }

    public List<string> getPlayerList(){
        return joinedPlayers;
    }

    public async void LeaveLobby(){
        try{
            await LobbyService.Instance.RemovePlayerAsync(currentLobby.LobbyCode, playerId);
            SceneManager.LoadScene(0);
            Debug.Log("dah keluar");
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
            Debug.Log("takleh keluar");
        }
    }

    // public async void KickPlayer()
    // {
    //     try
    //     {
    //         // Remove the player from the lobby
    //         await LobbyService.Instance.RemovePlayerAsync(joinCode, joinedPlayers[1]);

    //         // Update your UI or perform other actions as needed
    //         Debug.Log("Player kicked out: " + joinedPlayers[1]);
    //         joinedPlayers.Remove(joinedPlayers[1]);
    //         UpdatePlayerList(joinedPlayers);
    //     }
    //     catch (LobbyServiceException e)
    //     {
    //         Debug.LogError("Failed to kick out player: " + e.Message);
    //     }

    // }

    private void setIndexPlayer(){

    }

    
    public string getPlayerId(){
        return playerId;
    }

    public string getPlayerName()
    {
        return playerId;
    }

    public async void SaveReminder()
    {
        cloudSave.SaveData(await authManager.GetUserName(), "");
    }

    public void SignIn()
    {
        authManager.SignIn();
    }

    public void Register()
    {
        authManager.Create();
    }

    public void SetLobbyBackground(int i)
    {
        lobbyBackground = i;
    }

    public int GetLobbyBackground()
    {
        return lobbyBackground;
    }


}