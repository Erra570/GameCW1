using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrowdNPC : MonoBehaviour
{
    public Transform pointA;
    public Transform pointB;
    
    private UnityEngine.AI.NavMeshAgent agent;
    private Transform currentTarget;


    void Start()
    {
        pointA = GameObject.Find("StartStreet")?.transform;
        pointB = GameObject.Find("EndStreet")?.transform;
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        currentTarget = pointB;
        agent.SetDestination(currentTarget.position);
        agent.speed = Random.Range(2.5f, 5.5f);
        agent.angularSpeed = Random.Range(120, 180);
        agent.acceleration = Random.Range(8, 12);
    }

    void Update()
    {
        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            currentTarget = (currentTarget == pointA) ? pointB : pointA;
            agent.SetDestination(currentTarget.position);
        }
    }
}
