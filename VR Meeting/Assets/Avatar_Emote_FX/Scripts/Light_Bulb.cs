using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Light_Bulb : MonoBehaviour {

    public GameObject bulbFX;

	// Use this for initialization
	void Start () {

        bulbFX.SetActive(false);

    }
	
	// Update is called once per frame
	void Update () {

        if (Input.GetKeyDown(KeyCode.X)) //check to see if the left mouse was pushed.
        {

            StartCoroutine("BulbOn");

        }

    }

    IEnumerator BulbOn()
    {


        bulbFX.SetActive(true);

        yield return new WaitForSeconds(3.0f);

        bulbFX.SetActive(false);

    }

}
