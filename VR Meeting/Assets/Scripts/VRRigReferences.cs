using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRRigReferences : MonoBehaviour
{
    private static VRRigReferences Singleton;

    public Transform root;
    public Transform head;
    public Transform leftHand;
    public Transform rightHand;

    private VRRigReferences()
    {
        // Ensure that the constructor is private to prevent external instantiation
    }

    public static VRRigReferences getInstance()
    {
            return Singleton;
    }

    private void Awake()
    {
        if (Singleton == null)
        {
            Singleton = this;
        }
        else
        {
            // If an instance already exists, destroy the new one
            Destroy(gameObject);
        }
    }
}
