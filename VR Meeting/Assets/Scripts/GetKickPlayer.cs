using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GetKickPlayer : MonoBehaviour
{
    public TMP_Dropdown dropdown;
    private int playerVal = 0;
    public NetworkConnect networkConnect;

    private void Start()
    {
        // Subscribe to the dropdown's OnValueChanged event
        dropdown.onValueChanged.AddListener(setPlayerVal);
        // Populate the dropdown with the player names
        List<string> playerList = networkConnect.getPlayerList();
        dropdown.ClearOptions();
        dropdown.AddOptions(playerList);
    }

    private void setPlayerVal(int index)
    {
        playerVal = index;
    }

    public int getPlayerVal()
    {
        return playerVal;
    }

    public async void TransferDataToNetworkManager()
    {
        try
        {
            await networkConnect.KickPlayer(playerVal);
            Debug.Log("after networkConnect.KickPlayer");
        }
        catch (Exception e)
        {
            Debug.Log($"Error: {e.Message}");
        }
    }

}
