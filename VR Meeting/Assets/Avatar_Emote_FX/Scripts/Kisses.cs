using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kisses : MonoBehaviour {

    public GameObject kissesFX;

	// Use this for initialization
	void Start ()
    {

        kissesFX.SetActive(false);

    }
	
	// Update is called once per frame
	void Update () {

        if (Input.GetButtonDown("Fire1")) //check to see if the left mouse was pushed.
        {

            StartCoroutine("Kiss");

        }

    }

    IEnumerator Kiss()
    {


        kissesFX.SetActive(true);

        yield return new WaitForSeconds(3.0f);

        kissesFX.SetActive(false);

    }

}
