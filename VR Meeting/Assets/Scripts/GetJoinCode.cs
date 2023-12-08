using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;


public class GetJoinCode : MonoBehaviour
{
    public InputField tmpInputField;
    public NetworkConnect networkConnect;

    public void TransferDataToNetworkManager()
    {
        string inputData = tmpInputField.text;
        networkConnect.StoreJoinId(inputData);
    }
}


