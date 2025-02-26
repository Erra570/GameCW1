using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Chase : MonoBehaviour
{
    public Transform PlayerToChase;
    public NavMeshAgent MyAgent;

    void Start()
    {
        MyAgent.enabled = true;
        Debug.Log("IS NOW CHASING");
    }
        
    void Update()
    {
        MyAgent.SetDestination(PlayerToChase.position);
    }
}
