using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class GetJoinCode : MonoBehaviour
{
    public TMP_InputField lobbyCodeInput;
    public NetworkConnect networkConnect;

    public void TransferDataToNetworkManager()
    {
        string inputData = lobbyCodeInput.text;
        networkConnect.setJoinCode(inputData);
        networkConnect.Join();
    }
}


