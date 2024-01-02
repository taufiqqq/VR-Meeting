using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AvatarTransform : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject characterSelectionPanel;
    [SerializeField] private Button selectButton1;
    [SerializeField] private Button selectButton2;
    [SerializeField] private Button selectButton3;
    [SerializeField] private Button selectButton4;
    [SerializeField] private Button selectButton5;

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
        player.SetActive(true);
    }
}
