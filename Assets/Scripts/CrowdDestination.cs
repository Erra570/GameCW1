using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class CrowdDestination : MonoBehaviour
{
    public bool isStartingOnRightSide;

    public Transform[] NodesRightSide;
    public Transform[] NodesLeftSide;
    public NavMeshAgent MyAgent;
    public Animator animator;

    private Transform _currentDestination;

    void Start()
    {
        if (isStartingOnRightSide)
            _currentDestination = NodesLeftSide[UnityEngine.Random.Range(0, NodesLeftSide.Length)];            
        else
            _currentDestination = NodesRightSide[UnityEngine.Random.Range(0, NodesRightSide.Length)];

        MyAgent.SetDestination(_currentDestination.position);
    }

    void Update()
    {
        
        if (Vector3.Distance(this.transform.position, _currentDestination.position) < 0.5f)
        {
            //Change direction :
            if (NodesRightSide.Contains(_currentDestination))
            {
                // Go left side
                _currentDestination = NodesLeftSide[UnityEngine.Random.Range(0, NodesLeftSide.Length)];
            }
            else
            {
                // Go right side
                _currentDestination = NodesRightSide[UnityEngine.Random.Range(0, NodesRightSide.Length)];
            }
            MyAgent.SetDestination(_currentDestination.position);
        }
        



    }
}
