using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Seek : MonoBehaviour
{
    public Transform playerTransform;
    public GameObject playerGameObject;
    public float maxVisionDistance;
    public float visionFieldAngle;

    private PatrolWithKeyPoints scriptPatrol;
    private Chase scriptChase;

    private bool _hasLineOfSight = false;
    private bool _isPatrolling = true;

    void Start()
    {
        scriptPatrol = GetComponent<PatrolWithKeyPoints>();
        scriptChase = GetComponent<Chase>();
    }

    private void Update()
    {
        if (playerTransform == null)
        {
            Debug.LogWarning("Player Transform is not assigned.");
            return;
        }


        if (_isPatrolling)
        {
            Vector3 seekerPosition = transform.position;
            seekerPosition.y += 5;
            Vector3 PlayerPosition = playerTransform.position;
            PlayerPosition.y -= 1;
            Vector3 directionToPlayer = PlayerPosition - seekerPosition;
            RaycastHit rayHit;

            if (Physics.Raycast(seekerPosition, directionToPlayer, out rayHit, maxVisionDistance))
            {
                // Player found ?
                var angleToTarget = Vector3.Angle(transform.forward, directionToPlayer);
                Debug.Log("Hit objct : "+ rayHit.transform.gameObject);
                
                _hasLineOfSight = (rayHit.transform.gameObject == playerGameObject) && (angleToTarget <= (visionFieldAngle / 2));

                if (_hasLineOfSight)
                {
                    _isPatrolling = false;
                    scriptPatrol.enabled = false;
                    scriptChase.enabled = true;
                }                
            }
            else
            {
                _hasLineOfSight = false;
            }

            // Raycast in green if Player in sight ; red otherwise
            Color rayColor = _hasLineOfSight ? Color.green : Color.red;
            Debug.DrawRay(seekerPosition, directionToPlayer, rayColor);  //DrawRay(StartPoint, EndPoint, Color)
        }

    }
}
