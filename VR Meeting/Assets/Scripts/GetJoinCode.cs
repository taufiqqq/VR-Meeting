using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.Services.Lobbies;

public class GetJoinCode : MonoBehaviour
{
    public TMP_InputField lobbyCodeInput;
    public TMP_Text errorText; // Assuming you have a reference to the error text
    public NetworkConnect networkConnect;

    public void TransferDataToNetworkManager()
    {
        try
        {
            string inputData = lobbyCodeInput.text;
            networkConnect.setJoinCode(inputData);
            networkConnect.Join();
        }
        catch (LobbyServiceException e)
        {
            // Handle LobbyServiceException by displaying the error message
            DisplayErrorMessage($"LobbyError: {e.Message}");
        }
        catch (Exception e)
        {
            // Handle other exceptions by displaying the error message
            DisplayErrorMessage($"Error: {e.Message}");
        }
    }

    private void DisplayErrorMessage(string errorMessage)
    {
        if (errorText != null)
        {
            errorText.text = errorMessage;
            Debug.LogWarning("Error message displayed: " + errorMessage);
        }
        else
        {
            Debug.LogError("Error: Error Text not assigned in the inspector.");
        }
    }
}