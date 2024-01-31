using UnityEngine;
using System.Collections;

public class Love_Hearts : MonoBehaviour
{

    public GameObject loveHeartsFX;
    public ParticleSystem loveHearts;
    public AudioSource loveAudio;

    void Start()
    {

        loveHeartsFX.SetActive(false);

    }


    void Update()
    {

        if (Input.GetButtonDown("Fire1")) //check to see if the left mouse was pressed - trigger firework
        {

            loveHeartsFX.SetActive(true);
            loveHearts.Play();
            loveAudio.Play();

        }

        if (Input.GetButtonUp("Fire1")) //check to see if the left mouse was pressed - trigger firework
        {

            loveHearts.Stop();
            loveAudio.Stop();

        }

    }

}