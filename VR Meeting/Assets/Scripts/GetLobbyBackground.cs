using UnityEngine;
using TMPro;

public class GetLobbyBackground : MonoBehaviour
{
    public TMP_Dropdown dropdown;
    private int lobbyValue = 1;
    public NetworkConnect networkConnect;

    private void Start()
    {
        // Subscribe to the dropdown's OnValueChanged event
        dropdown.onValueChanged.AddListener(setLobbyValue);
        // No need to set lobby background here
    }

    private void setLobbyValue(int index)
    {
        lobbyValue = index + 1;
        networkConnect.SetLobbyBackground(lobbyValue);
    }

    public int getLobbyValue()
    {
        return lobbyValue;
    }
}
