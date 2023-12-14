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
  
    public int maxConnection = 20;
    public UnityTransport transport;

    
    private async void Awake()
    {
        await UnityServices.InitializeAsync();
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
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
            lobbyOptions.Data.Add("JOIN_CODE", dataObject);

            currentLobby = await Lobbies.Instance.CreateLobbyAsync("Meeting Name", maxConnection, lobbyOptions);

            Debug.LogError("Lobby Code : " + currentLobby.LobbyCode);

            joinCode = currentLobby.LobbyCode;
            
            UpdateLobbyCode(joinCode);
            NetworkManager.Singleton.StartHost();
    }
    

    public async void Join()
    {
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

        displayJoinCode.UpdateLobbyCode(lobbyCode);
    }

}
