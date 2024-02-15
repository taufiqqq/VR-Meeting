using System;
using System.Collections;
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Lobbies; // rectangular notation external actor (server side)
using Unity.Services.Lobbies.Models;
using UnityEngine.SceneManagement;
using UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets;
using TMPro;

public class NetworkConnect : MonoBehaviour
{
    private string joinCode;
    private static Lobby currentLobby;
    private float heartBeatTimer;
    private string playerId;
    private string playerName;
    private int lobbyBackground = 1;

    private List<string> joinedPlayers = new List<string>();

    public int maxConnection;
    public UnityTransport transport;

    public AuthManager authManager;
    public CloudSave cloudSave;

    public markerSpawn marker;

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

    public async Task Create()
    {

        try
        {
            if (maxConnection > 0 && maxConnection <= 20)
            {
                CreateLobbyOptions lobbyOptions = new CreateLobbyOptions();

                lobbyOptions.IsPrivate = false;

                lobbyOptions.Data = new Dictionary<string, DataObject>();

                string hostName = "admin"; // Replace with the actual host name
                DataObject hostNameDataObject =
                  new DataObject(DataObject.VisibilityOptions.Public, hostName);

                DataObject lobbyBGDataObject = new DataObject(
                  DataObject.VisibilityOptions.Public, lobbyBackground.ToString());

                lobbyOptions.Data.Add("HOST_NAME", hostNameDataObject);
                lobbyOptions.Data.Add("LOBBY_BG", lobbyBGDataObject);

                currentLobby = await Lobbies.Instance.CreateLobbyAsync(
                  "Meeting Name", maxConnection, lobbyOptions);

                Allocation allocation =
                  await RelayService.Instance.CreateAllocationAsync(maxConnection);

                transport.SetHostRelayData(allocation.RelayServer.IpV4,
                  (ushort)allocation.RelayServer.Port,
                  allocation.AllocationIdBytes, allocation.Key,
                  allocation.ConnectionData);

                Debug.LogError("Lobby Code : " + currentLobby.LobbyCode);

                playerId = await AuthenticationService.Instance.GetPlayerNameAsync();
                Debug.Log("Player ID: " + playerId);

                joinCode = currentLobby.LobbyCode;

                StartCoroutine(LoadSceneAsync(lobbyBackground, 0));
            }
            else
            {
                throw new Exception("Max participant must be 1-20 only");
            }
        }
        catch (Exception e)
        {
            // Optionally log or handle the exception here before rethrowing
            throw; // Rethrow the exception to the caller
        }
    }

    public async Task Join()
    {
        try
        {
            QueryResponse queryResponse = await Lobbies.Instance.QueryLobbiesAsync();

            if (joinCode != "")
            {
                currentLobby = await Lobbies.Instance.JoinLobbyByCodeAsync(joinCode);

                string relayJoinCode = currentLobby.Data["JOIN_CODE"].Value;
                Debug.LogError(relayJoinCode);
                JoinAllocation allocation =
                  await RelayService.Instance.JoinAllocationAsync(relayJoinCode);

                transport.SetClientRelayData(
                  allocation.RelayServer.IpV4, (ushort)allocation.RelayServer.Port,
                  allocation.AllocationIdBytes, allocation.Key,
                  allocation.ConnectionData, allocation.HostConnectionData);

                Debug.LogError("Lobby ID : " + currentLobby.Id);
                Debug.LogError(currentLobby.Data["JOIN_CODE"].Value);

                playerId = await AuthenticationService.Instance.GetPlayerNameAsync();
                Debug.Log("Player ID: " + playerId);

                joinCode = currentLobby.LobbyCode;
                Debug.LogError(currentLobby.Data["LOBBY_BG"].Value);

                lobbyBackground = int.Parse(currentLobby.Data["LOBBY_BG"].Value);

                StartCoroutine(LoadSceneAsync(lobbyBackground, 1));
            }
            else
            {
                throw new Exception("Join code is empty.");
            }
        }
        catch (LobbyServiceException e)
        {
            // Optionally log or handle the LobbyServiceException here before rethrowing
            throw;
        }
        catch (Exception e)
        {
            // Optionally log or handle the exception here before rethrowing
            throw;
        }
    }

    private async void Update()
    {
        try
        {
            if (heartBeatTimer > 15)
            {
                heartBeatTimer -= 15;
                Debug.Log("HB timer 15");

                if (currentLobby != null)
                {
                    Lobby lobby = await Lobbies.Instance.GetLobbyAsync(currentLobby.Id);
                    currentLobby = lobby;
                    Debug.Log("Lobby Debug Log");
                    try
                    {
                        Debug.Log(currentLobby.Players[0].Id);
                        if (currentLobby.Players[0].Id == null || currentLobby.Players[0].Id == "")
                        {
                            Debug.Log("Mane Load Scene. Ini sebelum");
                            StartCoroutine(LoadSceneAsync(0, 2));
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.Log(e);
                    }
                    if (currentLobby.HostId == AuthenticationService.Instance.PlayerId)
                    {
                        LobbyService.Instance.SendHeartbeatPingAsync(currentLobby.Id);
                        Debug.LogError(currentLobby.Id);
                        Debug.LogError(currentLobby.Data["JOIN_CODE"].Value);
                        Debug.Log("HeartBeat Ping shit");
                    }
                }
                else
                {
                    // Handle the case when currentLobby is null (e.g., the main lobby)
                    Debug.LogWarning("Current lobby is null (Main Lobby)");
                    Debug.Log("lobby is null");
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
        try
        {
            displayJoinCode.UpdateLobbyCode(lobbyCode);
        }
        catch (Exception e)
        {
            Debug.LogError($"Error updating lobby code: {e}");
        }

        if (displayJoinCode == null)
        {
            Debug.LogError("Error: DisplayJoinCode not found in the scene.");
            return;
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
        }
        catch (LobbyServiceException e)
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

    public List<string> getPlayerList()
    {
        joinedPlayers = new List<string>();

        foreach (var player in currentLobby.Players)
        {
            joinedPlayers.Add(player.Id);
            //problemdia sebab dia guna player id, bukan name. nanti kau try guna name

        }

        return joinedPlayers;
    }

    public void LeaveLobby()
    {
        try
        {
            if (currentLobby != null)
            {
                LobbyService.Instance.RemovePlayerAsync(currentLobby.LobbyCode,
                  playerId);
                AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(0);
                authManager.SignOut();
                NetworkManager.Singleton.Shutdown();
                Debug.Log("dah keluar");
            }
            else
            {
                Debug.LogError("currentLobby is null. Cannot leave lobby.");
            }
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
            Debug.Log("takleh keluar");
        }
    }

    public void KickPlayer(int id)
    {
        try
        {
            LobbyService.Instance.RemovePlayerAsync(currentLobby.LobbyCode, currentLobby.Players[id].Id);
            Debug.Log("Mane Lobby anjing");
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    public string getPlayerId()
    {
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

    private IEnumerator LoadSceneAsync(int sceneNumber, int condition)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneNumber);

        while (!asyncLoad.isDone)
        {
            yield
            return null;
        }

        // The scene is now loaded
        Debug.Log("Scene loaded: " + sceneNumber);

        HandlePlayer(); // TAK CHECK UNTUK SEMUA JUST UNTUK DIRI SENDIRI
        UpdateLobbyCode(joinCode);

        if (condition == 0)
        {
            NetworkManager.Singleton.StartHost();
            markerSpawn();
        }
        else if (condition == 1)
        {
            NetworkManager.Singleton.StartClient();
            markerSpawn();
        }
        else
        {
            NetworkManager.Singleton.Shutdown();
        }
    }

    private void markerSpawn()
    {
        marker.OnNetworkSpawn();
    }
}