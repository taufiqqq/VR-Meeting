using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode;

public class NetworkAnimateHandOnInput : NetworkBehaviour
{
    [SerializeField] private InputActionProperty pinchAnimationAction;
    [SerializeField] private InputActionProperty gripAnimationAction;
    [SerializeField] private Animator handAnimator;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (IsOwner)
        {
            float triggerValue = pinchAnimationAction.action.ReadValue<float>();
            handAnimator.SetFloat("Trigger", triggerValue);

            float gripValue = gripAnimationAction.action.ReadValue<float>();
            handAnimator.SetFloat("Grip", gripValue);
            Debug.Log(triggerValue + " " + gripValue);
        }

    }
}
