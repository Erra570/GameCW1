using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Seek : MonoBehaviour
{
    public Transform playerTransform;
    public GameObject playerGameObject;
    public float maxVisionDistance;
    public float visionFieldAngle;

    private bool _hasLineOfSight = false;

    void Start()
    {

    }

    private void Update()
    {
        if (playerTransform == null)
        {
            Debug.LogWarning("Player Transform is not assigned.");
            return;
        }

        Vector3 seekerPosition = transform.position;        
        Vector3 directionToPlayer = playerTransform.position - transform.position;


        //Debug.Log("seekerPosition.y:" + seekerPosition.y + " | directionToPlayer.y:" + directionToPlayer.y);

        RaycastHit rayHit;
        if (Physics.Raycast(seekerPosition, directionToPlayer, out rayHit, maxVisionDistance))
        {
            // Player found ?
            var angleToTarget = Vector3.Angle(transform.forward, directionToPlayer);
            _hasLineOfSight = (rayHit.transform.gameObject==playerGameObject) && (angleToTarget<=(visionFieldAngle/2));
        }
        else
        {
            _hasLineOfSight = false;
        }


        // Affichage du raycast en vert si le joueur est visible, sinon en rouge
        Color rayColor = _hasLineOfSight ? Color.green : Color.red;
        Debug.DrawRay(seekerPosition, directionToPlayer, rayColor); //DrawRay(StartPoint, EndPoint, Color)
    }
}
