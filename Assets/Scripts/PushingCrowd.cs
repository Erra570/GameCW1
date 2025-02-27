using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushingCrowd : MonoBehaviour
{
    public float pushForce = 10f;
    public GameObject seeker;

    void OnCollisionEnter(Collision collision)
    {
        seeker = GameObject.Find("Seeker");
        if (collision.gameObject.CompareTag("Player"))
        {
            seeker.GetComponent<Chase>().enabled = false;
            seeker.GetComponent<PatrolWithKeyPoints>().enabled = true;
            Rigidbody playerRb = collision.gameObject.GetComponent<Rigidbody>();
            if (playerRb != null)
            {
                Vector3 pushDirection = collision.transform.position - transform.position;
                pushDirection.y = 0; // Prevents the player from being pushed upwards
                playerRb.AddForce(pushDirection.normalized * pushForce, ForceMode.Impulse);
            }
        }
    }
}
