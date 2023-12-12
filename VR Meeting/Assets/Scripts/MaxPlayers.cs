using System.Collections;
using System.Collections.Generic;
<<<<<<< Updated upstream
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;


public class GetMaxPlayers : MonoBehaviour
{
    public InputField tmpInputField;
    public NetworkConnect networkConnect;

    public void TransferDataToNetworkManager()
    {
        string inputData = tmpInputField.text;
        networkConnect.setJoinCode(inputData);
    }
=======
using UnityEngine;
using TMPro;

public class MaxPlayer : MonoBehaviour
{
    public TMP_InputField maxPlayerInput;
    public TextMeshProUGUI maxPlayerNumber;
    public NetworkConnect networkConnect;

    // Start is called before the first frame update
    void Start()
    {
        // You might want to check if networkConnect is not null before using it
        if (networkConnect != null)
        {
            UpdateMaxPlayer(networkConnect.getMaxPlayer());
        }
    }

    public void UpdateMaxPlayer(int NumberofPlayer)
    {
        Debug.LogError("hello 2" + NumberofPlayer);

        maxPlayerNumber.text = "Number of Max Participants " + NumberofPlayer;
    
    }

    public void TransferDataToNetworkManager()
    {
        int.TryParse(maxPlayerInput.text, out int inputData);
    
        // If parsing is successful, set the value
        networkConnect.setMaxPlayer(inputData);

    }

>>>>>>> Stashed changes
}