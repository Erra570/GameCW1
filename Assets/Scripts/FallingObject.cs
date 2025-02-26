using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class FallingObject : MonoBehaviour
{
    public Transform fallingObject;
    private bool triggered;
    private bool finished;

    public CinemachineVirtualCamera fallingObjectCam; // Caméra qui regarde l'objet qui tombe

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player" && !finished)
        {
            triggered = true;
            fallingObjectCam.Priority = 11;
        }
    }
    
    private void Update()
    { 
        if (fallingObject.position.y < 20f){
            triggered = false;
            finished = true;
            fallingObjectCam.Priority = 9;
            return;
        }
        if(triggered){
            Vector3 newPos = fallingObject.transform.position;
            newPos.y = 24f;
            newPos.z = 36f;
            fallingObject.position = Vector3.MoveTowards(fallingObject.position, newPos, 2 * Time.deltaTime);
        }
    }
}
