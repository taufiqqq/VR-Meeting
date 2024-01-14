using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AvatarTransform : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject characterSelectionPanel;
    [SerializeField] private GameObject currentXR;
    [SerializeField] private GameObject referenceXR;
    [SerializeField] private GameObject currentoffset;
    [SerializeField] private GameObject referenceoffset;
    [SerializeField] private Button selectButton1;
    [SerializeField] private Button selectButton2;
    [SerializeField] private Button selectButton3;
    [SerializeField] private Button selectButton4;
    [SerializeField] private Button selectButton5;
    [SerializeField] private Renderer[] meshToDisable;


    // Start is called before the first frame update
    void Start()
    {
        selectButton1.onClick.AddListener(changePlayerView);
        selectButton2.onClick.AddListener(changePlayerView);
        selectButton3.onClick.AddListener(changePlayerView);
        selectButton4.onClick.AddListener(changePlayerView);
        selectButton5.onClick.AddListener(changePlayerView);
    }

    void changePlayerView()
    {
        characterSelectionPanel.SetActive(false);
        /*foreach (Transform child in characterSelectionPanel.transform)
        {
            if (child.gameObject != currentXR)
            {
                child.gameObject.SetActive(false);
            }
        }*/

        player.SetActive(true);

        //ChangeToXRView();
    }

    void ChangeToXRView()
    {
        if (currentXR != null)
        {
            // Set the player's position and rotation to match XR's position and rotation
            currentXR.transform.position = referenceXR.transform.position;
            currentXR.transform.rotation = referenceXR.transform.rotation;
        }
        else
        {
            Debug.LogError("XR object is not assigned.");
        }
        if (currentoffset != null)
        {
            // Set the player's position and rotation to match XR's position and rotation
            currentoffset.transform.position = referenceoffset.transform.position;
            currentoffset.transform.rotation = referenceoffset.transform.rotation;
        }
        else
        {
            Debug.LogError("camera offset is not assigned.");
        }

        foreach (var item in meshToDisable)
        {
            item.enabled = false;
        }
    }
}
