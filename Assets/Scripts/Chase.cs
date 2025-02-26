using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Chase : MonoBehaviour
{
    public Transform PlayerToChase;
    public NavMeshAgent MyAgent;
    public Animator animator;

    void Start()
    {
        MyAgent.enabled = true;
        animator.SetBool("isRunning", true);
        Debug.Log("IS NOW CHASING");
    }

    void Update()
    {
        MyAgent.SetDestination(PlayerToChase.position);
    }
}
