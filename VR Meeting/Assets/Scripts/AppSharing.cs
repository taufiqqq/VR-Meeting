using UnityEngine;
using Photon.Pun;

public class VRMultiplayerView : MonoBehaviourPun, IPunObservable
{
    // Set this GameObject in the inspector to the VR camera rig
    public GameObject playerCamera;

    void Update()
    {
        if (photonView.IsMine)
        {
            // Allow local player to control their own camera
            HandleLocalPlayerInput();
        }
        else
        {
            // Sync remote player's camera position and rotation
            SyncRemotePlayerView();
        }
    }

    void HandleLocalPlayerInput()
    {
        // Your code to control the local player's camera
        // (e.g., movement, interaction, etc.)
    }

    void SyncRemotePlayerView()
    {
        // Smoothly interpolate between current position/rotation and the networked values
        playerCamera.transform.position = Vector3.Lerp(playerCamera.transform.position, networkPosition, Time.deltaTime * 5f);
        playerCamera.transform.rotation = Quaternion.Slerp(playerCamera.transform.rotation, networkRotation, Time.deltaTime * 5f);
    }

    #region IPunObservable Implementation

    private Vector3 networkPosition;
    private Quaternion networkRotation;

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // Send local player's camera position and rotation to the network
            stream.SendNext(playerCamera.transform.position);
            stream.SendNext(playerCamera.transform.rotation);
        }
        else
        {
            // Receive remote player's camera position and rotation from the network
            networkPosition = (Vector3)stream.ReceiveNext();
            networkRotation = (Quaternion)stream.ReceiveNext();
        }
    }

    #endregion
}
