using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stunned_Stars : MonoBehaviour {

    public GameObject stunnedStarsFX;

	// Use this for initialization

	void Start ()
    {

        stunnedStarsFX.SetActive(false);

    }
	

	void Update () {

        if (Input.GetKeyDown(KeyCode.C)) //check to see if the left mouse was pushed.
        {

            StartCoroutine("Stunned");

        }

    }

    IEnumerator Stunned()
    {


        stunnedStarsFX.SetActive(true);

        yield return new WaitForSeconds(6.0f);

        stunnedStarsFX.SetActive(false);

    }

}
