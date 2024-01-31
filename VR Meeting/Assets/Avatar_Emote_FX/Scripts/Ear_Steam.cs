using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ear_Steam : MonoBehaviour {

    public GameObject ear_SteamFX;

	// Use this for initialization
	void Start () {

        ear_SteamFX.SetActive(false);

    }
	
	// Update is called once per frame
	void Update () {

        if (Input.GetKeyDown(KeyCode.V)) //check to see if the left mouse was pushed.
        {

            StartCoroutine("Ear_Steam_On");

        }

    }

    IEnumerator Ear_Steam_On()
    {


        ear_SteamFX.SetActive(true);

        yield return new WaitForSeconds(3.0f);

        ear_SteamFX.SetActive(false);

    }

}
