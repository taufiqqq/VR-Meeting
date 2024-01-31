using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Question : MonoBehaviour {

    public GameObject questionFX;

	// Use this for initialization
	void Start () {

        questionFX.SetActive(false);

    }
	
	// Update is called once per frame
	void Update () {

        if (Input.GetKeyDown(KeyCode.B)) //check to see if the left mouse was pushed.
        {

            StartCoroutine("QuestionOn");

        }

    }

    IEnumerator QuestionOn()
    {


        questionFX.SetActive(true);

        yield return new WaitForSeconds(2.0f);

        questionFX.SetActive(false);

    }

}
