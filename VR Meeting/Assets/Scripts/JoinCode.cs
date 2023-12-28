using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class JoinCode : MonoBehaviour
{
    public TMP_InputField lobbyCodeInput;
    public TextMeshProUGUI lobbyCodeText;
    public NetworkConnect networkConnect;

    // Start is called before the first frame update
    void Start()
    {
        // You might want to check if networkConnect is not null before using it
        if (networkConnect != null)
        {
            UpdateLobbyCode(networkConnect.getJoinCode());
        }
    }

    public void UpdateLobbyCode(string lobbyCode)
    {
        Debug.LogError("hello 2" + lobbyCode);

        if (lobbyCode != null)
        {
            lobbyCodeText.text = "Join Code: " + lobbyCode;
        }
    }

    public void TransferDataToNetworkManager()
    {
        string inputData = lobbyCodeInput.text;
        networkConnect.setJoinCode(inputData);
        networkConnect.Join();
    }

}
