using UnityEngine;
using System.Collections;

public class Speech_Bubble : MonoBehaviour
{

    public GameObject speechBubbleFX;

    void Start()
    {

        speechBubbleFX.SetActive(false);

    }


    void Update()
    {

        if (Input.GetButtonDown("Fire1")) //check to see if the left mouse was pressed - trigger firework
        {

                SpeechBubbleOn();

        }

        if (Input.GetButtonUp("Fire1")) //check to see if the left mouse was pressed - trigger firework
        {

            speechBubbleFX.SetActive(false);

        }

    }


    void SpeechBubbleOn()
    {

        speechBubbleFX.SetActive(true);

    }

}