using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerList : MonoBehaviour
{
    public TextMeshProUGUI PlayerIdText;
    public NetworkConnect networkConnect;

    // Start is called before the first frame update
    void Start()
    {
        // You might want to check if networkConnect is not null before using it
        if (networkConnect != null)
        {
            UpdatePlayerList(networkConnect.getPlayerList());
        }
    }

    public void UpdatePlayerList(List<string> players)
    {
        Debug.Log("Player List:");
        foreach (string playerId in players)
        {
            Debug.Log(playerId);
            if(playerId != null){
                PlayerIdText.text =  playerId;
            }
            
        }
    }

}
