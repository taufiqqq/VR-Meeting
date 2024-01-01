using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class NetworkPlayer : NetworkBehaviour
{
    public Transform root;
    public Transform head;
    public Transform leftHand;
    public Transform rightHand;

    public Renderer[] meshToDisable;

    // Start is called before the first frame update
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (IsOwner)
        {
            foreach(var item in meshToDisable)
            {
                item.enabled = false;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (IsOwner)
        {
            root.position = VRRigReferences.getInstance().root.position;
            root.rotation = VRRigReferences.getInstance().root.rotation;

            head.position = VRRigReferences.getInstance().head.position;
            head.rotation = VRRigReferences.getInstance().head.rotation;

            leftHand.position = VRRigReferences.getInstance().leftHand.position;
            leftHand.rotation = VRRigReferences.getInstance().leftHand.rotation;

            rightHand.position = VRRigReferences.getInstance().rightHand.position;
            rightHand.rotation = VRRigReferences.getInstance().rightHand.rotation;
        }


    }
}
