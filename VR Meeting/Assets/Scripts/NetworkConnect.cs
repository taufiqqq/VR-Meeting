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
  
    public int maxConnection ;
    public UnityTransport transport;

    private GameObject playerInfoContent;

    private string playerId;
    private string hostId;
    private List<string> joinedPlayers = new List<string>(); // List to store joined player IDs
    public Spawner markerSpawn;

    private async void Awake()
    {
        await UnityServices.InitializeAsync();
        AuthenticationService.Instance.SignedIn += () =>
        {
            playerId = AuthenticationService.Instance.PlayerId;
            Debug.Log("Signed in " + playerId);
        };
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }
    // Start is called before the first frame update

    public async void Create()
    {
        SceneManager.LoadScene(1);

        Allocation allocation = await RelayService.Instance.CreateAllocationAsync(maxConnection);
        string newJoinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
        Debug.Log(newJoinCode);
        Debug.LogError(newJoinCode);

        transport.SetHostRelayData(allocation.RelayServer.IpV4, (ushort)allocation.RelayServer.Port, allocation.AllocationIdBytes, allocation.Key, allocation.ConnectionData);

        CreateLobbyOptions lobbyOptions = new CreateLobbyOptions();

        lobbyOptions.IsPrivate = false;

        lobbyOptions.Data = new Dictionary<string, DataObject>();
        DataObject dataObject = new DataObject(DataObject.VisibilityOptions.Public, newJoinCode);
        lobbyOptions.Data.Add("JOIN_CODE", dataObject);
       
        currentLobby = await Lobbies.Instance.CreateLobbyAsync("Meeting Name", maxConnection, lobbyOptions);
        
        hostId = playerId; 

            Debug.LogError("Lobby Code : " + currentLobby.LobbyCode);
            Debug.LogError("Max Participants : " + maxConnection);
            Debug.LogError("Host Player ID: " + hostId);

        joinCode = currentLobby.LobbyCode;
        Debug.Log("Number of Players in room : " + currentLobby.Players.Count);
        HandlePlayer();
        UpdateMaxPlayer(maxConnection);
        markerSpawn.SpawnMarker();

        UpdateLobbyCode(joinCode);
        NetworkManager.Singleton.StartHost();
    }


    public async void Join()
    {
        if (joinCode != "")
        {
            try
            {
                currentLobby = await Lobbies.Instance.JoinLobbyByCodeAsync(joinCode);
                SceneManager.LoadScene(1);
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
        Debug.LogError("My Player Id : " + playerId);
        Debug.Log("Number of Players in room : " + currentLobby.Players.Count);
        HandlePlayer();
        markerSpawn.SpawnMarker();
        UpdateLobbyCode(joinCode);
        NetworkManager.Singleton.StartClient();
        }
    }

    private void Update()
    {
        if (heartBeatTimer > 15)
        {
            heartBeatTimer -= 15;
            if (currentLobby != null && currentLobby.HostId == AuthenticationService.Instance.PlayerId)
                LobbyService.Instance.SendHeartbeatPingAsync(currentLobby.Id);
            Debug.LogError(currentLobby.Id);
            Debug.LogError(currentLobby.Data["JOIN_CODE"].Value);
        }
        heartBeatTimer += Time.deltaTime;

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
        Debug.LogError("hello");

        displayJoinCode.UpdateLobbyCode(lobbyCode);

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
    private void UpdateMaxPlayer(int maxPlayer)
    {
        DisplayMaxPlayer displayMaxPlayer = FindObjectOfType<DisplayMaxPlayer>();

        displayMaxPlayer.UpdateMaxPlayer(maxPlayer);
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
        // Example: Display the player list in the console
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

    public void LeaveLobby(){
        try{
            LobbyService.Instance.RemovePlayerAsync(joinCode, playerId);
            SceneManager.LoadScene(2);
            Debug.Log("dah keluar");
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
            Debug.Log("takleh keluar");
        }
    }
}
