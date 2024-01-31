using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.Services.Lobbies;

public class getMaxPlayer : MonoBehaviour
{
    public TMP_InputField maxPlayerInput;
    public TMP_Text errorText; // Assuming you have a reference to the error text

    public NetworkConnect networkConnect;

    public async void TransferDataToNetworkManager()
    {
        try
        {
            int.TryParse(maxPlayerInput.text, out int inputData);

            // If parsing is successful, set the value
            networkConnect.setMaxPlayer(inputData);
            networkConnect.Create();
        }
        catch (LobbyServiceException e)
        {
            DisplayErrorMessage($"LobbyError: {e.Message}");
        }
        catch (Exception e)
        {
            DisplayErrorMessage($"Error: {e.Message}");
        }
    }

    private void DisplayErrorMessage(string errorMessage)
    {
        if (errorText != null)
        {
            errorText.text = errorMessage;
        }
        else
        {
            Debug.LogError("Error: Error Text not assigned in the inspector.");
        }
    }
}
