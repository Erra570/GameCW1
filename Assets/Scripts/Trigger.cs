using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trigger : MonoBehaviour
{

    private GameObject door;

    void Start(){
        door = GameObject.Find("TestDoor");
    }

    void OnTriggerEnter(Collider other){
        door.SetActive(false);
    }

    void OnTriggerExit(Collider other){
        // do nothing for now
    }
}
