using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class getMaxPlayer : MonoBehaviour
{
    public TMP_InputField maxPlayerInput;
    public NetworkConnect networkConnect;

    public void TransferDataToNetworkManager()
    {

        int.TryParse(maxPlayerInput.text, out int inputData);

        // If parsing is successful, set the value
        networkConnect.setMaxPlayer(inputData);
        networkConnect.Create();

    }
}

