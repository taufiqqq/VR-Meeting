using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DisplayMaxPlayer : MonoBehaviour
{
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
        Debug.LogError("hello hihi :"+ NumberofPlayer);

        
        maxPlayerNumber.text = "Max Number Of Participants: " + NumberofPlayer;
        
    }

}
