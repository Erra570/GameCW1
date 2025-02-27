using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class GetToStreet : MonoBehaviour
{
    public GameObject seeker;
    public CinemachineVirtualCamera streetCam; 
    private void Start()
    {
        seeker = GameObject.Find("Seeker");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            seeker.GetComponent<Chase>().enabled = false;
            seeker.GetComponent<PatrolWithKeyPoints>().enabled = true;
            streetCam.Priority = 11;
        }
    }
}
