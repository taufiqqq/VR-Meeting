using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crying : MonoBehaviour {

    public GameObject cryingFX;
    public ParticleSystem tears_RightEye;
    public ParticleSystem tears_LeftEye;

    // Use this for initialization
    void Start ()
    {

        cryingFX.SetActive(false);

    }
	
	void Update () {

        if (Input.GetKeyDown(KeyCode.C)) //check to see if the left mouse was pushed.
        {

            StartCoroutine("Crying_On");

        }

    }

    IEnumerator Crying_On()
    {

        cryingFX.SetActive(true);
        tears_RightEye.Play();
        tears_LeftEye.Play();

        yield return new WaitForSeconds(3.0f);

        tears_RightEye.Stop();
        tears_LeftEye.Stop();
        cryingFX.SetActive(false);

    }

}
