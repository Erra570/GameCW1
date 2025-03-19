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
        Destroy(door);
    }

    void OnTriggerExit(Collider other){
        Destroy(gameObject); //avoid load unecessary things
    }
}
