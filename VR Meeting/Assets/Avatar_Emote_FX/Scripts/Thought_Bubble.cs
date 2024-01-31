using UnityEngine;
using System.Collections;

public class Thought_Bubble : MonoBehaviour
{

    public GameObject thoughtBubbleFX;

    void Start()
    {

        thoughtBubbleFX.SetActive(false);

    }


    void Update()
    {

        if (Input.GetButtonDown("Fire1")) //check to see if the left mouse was pressed - trigger firework
        {

                ThoughtBubbleOn();

        }

        if (Input.GetButtonUp("Fire1")) //check to see if the left mouse was pressed - trigger firework
        {

            thoughtBubbleFX.SetActive(false);

        }

    }


    void ThoughtBubbleOn()
    {

        thoughtBubbleFX.SetActive(true);

    }

}