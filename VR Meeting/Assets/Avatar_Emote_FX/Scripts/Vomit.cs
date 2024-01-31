using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vomit : MonoBehaviour {

    public GameObject vomitFX;

	// Use this for initialization
	void Start ()
    {

        vomitFX.SetActive(false);

    }
	
	// Update is called once per frame

	void Update () {

        if (Input.GetButtonDown("Fire1")) //check to see if the left mouse was pushed.
        {

            StartCoroutine("VomitStart");

        }

    }

    IEnumerator VomitStart()
    {


        vomitFX.SetActive(true);

        yield return new WaitForSeconds(2.5f);

        vomitFX.SetActive(false);

    }

}
