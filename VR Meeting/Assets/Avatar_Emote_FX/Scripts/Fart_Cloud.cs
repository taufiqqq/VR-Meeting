using UnityEngine;
using System.Collections;

public class Fart_Cloud : MonoBehaviour {

public GameObject fartCloudFX;


    void Start ()
    {

        fartCloudFX.SetActive(false);

    }  
  
  
void Update (){
 
    if (Input.GetButtonDown("Fire1")) //check to see if the left mouse was pressed - trigger firework
    {
   
        StartCoroutine("Fart");
      
    }
            
}


IEnumerator Fart()

    {

        fartCloudFX.SetActive(true);

        yield return new WaitForSeconds(2.0f);

        fartCloudFX.SetActive(false);

    }


}