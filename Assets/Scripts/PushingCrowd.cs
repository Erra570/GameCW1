using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushingCrowd : MonoBehaviour
{
    public float pushForce = 10f;

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Rigidbody playerRb = collision.gameObject.GetComponent<Rigidbody>();
            if (playerRb != null)
            {
                Vector3 pushDirection = collision.transform.position - transform.position;
                pushDirection.y = 0; // Pas de force verticale
                playerRb.AddForce(pushDirection.normalized * pushForce, ForceMode.Impulse);
            }
        }
    }
}
