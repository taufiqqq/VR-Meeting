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

public class NetworkConnect : MonoBehaviour
{   
    public string lobbyId;
    public string joinCode;
    public int maxConnection = 20;
    public UnityTransport transport;

    private Lobby currentLobby;
    private float heartBeatTimer;

    private async void Awake()
    {
        await UnityServices.InitializeAsync();
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }
    // Start is called before the first frame update

    public async void Create()
    {
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(maxConnection);
            string newJoinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
            Debug.Log(newJoinCode);
            Debug.LogError(newJoinCode);
            joinCode = newJoinCode;

            transport.SetHostRelayData(allocation.RelayServer.IpV4, (ushort)allocation.RelayServer.Port, allocation.AllocationIdBytes, allocation.Key, allocation.ConnectionData);

            CreateLobbyOptions lobbyOptions = new CreateLobbyOptions();


            lobbyOptions.IsPrivate = false;

            lobbyOptions.Data = new Dictionary<string, DataObject>();
             DataObject dataObject = new DataObject(DataObject.VisibilityOptions.Public, newJoinCode);
             lobbyOptions.Data.Add("JOIN_CODE", dataObject);


            currentLobby = await Lobbies.Instance.CreateLobbyAsync("Meeting Name", maxConnection, lobbyOptions);

            NetworkManager.Singleton.StartHost();
    }
    //Control class (client side)
    //Combined control / boundary


    public async void Join()
    {
        try
        { 
            currentLobby = await Lobbies.Instance.JoinLobbyByIdAsync(lobbyId);
        }
        catch (LobbyServiceException e)
        {
            Debug.LogError(e);
        }

        string relayJoinCode = currentLobby.Data["JOIN_CODE"].Value;
        Debug.LogError(relayJoinCode);
        JoinAllocation allocation = await RelayService.Instance.JoinAllocationAsync(relayJoinCode);

        transport.SetClientRelayData(allocation.RelayServer.IpV4, (ushort)allocation.RelayServer.Port, allocation.AllocationIdBytes, allocation.Key, allocation.ConnectionData, allocation.HostConnectionData);

        NetworkManager.Singleton.StartClient();
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
}
