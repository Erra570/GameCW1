using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class FallingObject : MonoBehaviour
{
    public Transform fallingObject;
    private bool triggered;
    private bool finished;

    public CinemachineVirtualCamera fallingObjectCam; 
    public GameObject player; 
    private ThirdViewMvt playerScript;

    private void Start()
    {
        playerScript = player.GetComponent<ThirdViewMvt>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player" && !finished)
        {
            triggered = true;
            fallingObjectCam.Priority = 11;
            if (playerScript != null)
            {
                playerScript.enabled = false;
            }
        }
    }
    
    private void Update()
    { 
        if (fallingObject.position.y < 20f){
            triggered = false;
            finished = true;
            fallingObjectCam.Priority = 9;
            if (playerScript != null)
            {
                playerScript.enabled = true;
            }
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
