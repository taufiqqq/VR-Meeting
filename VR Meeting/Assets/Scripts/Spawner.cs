using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject Marker; 
    public void SpawnMarker(){
        Instantiate(Marker);
    }
}
