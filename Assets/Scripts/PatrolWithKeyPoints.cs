using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolWithKeyPoints : MonoBehaviour
{
    public CharacterController myController;
    public Animator animator;
    public float agentWalkingSpeed;
    public float waitingTime;

    public Node[] myKeyNodes;
    public Node myStartingNode;
    private List<Node> myPath;

    private Node _currentGoal;
    private int _currentKeyNodeIndex = 0;
    private int _currentDestinationNodeIndex = 0;
    private Node _currentDestination; // next Node to go to (or current Node if our path is completed)
    private bool _isWaiting = false;
    

    void Start()
    {
        myPath = new List<Node>();
        myPath.Add(myStartingNode); // for the first currentNode in GoToNextKeyNode()
        GoToNextKeyNode();
    }

    void Update()
    {
        if (!_isWaiting && Vector3.Distance(this.transform.position, _currentGoal.transform.position) < 2f)
        {
            Debug.Log("Now waiting for " + waitingTime + "s. . .");
            StartCoroutine(waitAndGo(waitingTime));                      
        }
        else if (!_isWaiting && _currentDestination != null)
        {
            MoveToNextNode();
        }
        //else if (!_isWaiting && _currentDestination !=null && Vector3.Distance(this.transform.position, _currentDestination.transform.position) < 0.1f)
        //{            
        //    GoToNextNode();
        //}
    }

    void GoToNextKeyNode()
    {
        Debug.Log("---- Next key node : n°" + _currentKeyNodeIndex);

        _currentGoal = myKeyNodes[_currentKeyNodeIndex];
        Node currentNode = myPath[myPath.Count - 1];
        myPath = PathFindingAStar.FindPath(currentNode, _currentGoal);
        _currentDestinationNodeIndex = 0;

        Debug.Log("Beginning new path ! :");
        foreach (Node n in myPath)
        {
            Debug.Log("Node " + n.name + " : " + n.transform.position);
        }
       

        GoToNextNode();
        _currentKeyNodeIndex++;
    }

    void GoToNextNode()
    {
        Debug.Log("(go)");
        if (_currentDestinationNodeIndex < myPath.Count)
        {
            _currentDestination = myPath[_currentDestinationNodeIndex];
            _currentDestinationNodeIndex++;
        }
    }

    void MoveToNextNode()
    {
        if (_currentDestination != null)
        {            
            // Calculate direction and move/rotate the CharacterController
            Vector3 direction = (_currentDestination.transform.position - transform.position).normalized;
            Vector3 rotateDirection = Vector3.RotateTowards(this.transform.forward, direction, agentWalkingSpeed * Time.deltaTime, 0.0f);
            rotateDirection.y = 0; // in case the destination isn't on the same y plane as the GameObject
            transform.rotation = Quaternion.LookRotation(rotateDirection);            
            myController.Move(direction * agentWalkingSpeed * Time.deltaTime);

            // Check if the destination is reached
            if (Vector3.Distance(this.transform.position, _currentDestination.transform.position) < 2f)
            {
                Debug.Log("(reached!)");
                GoToNextNode();
            }
        }
    }

    IEnumerator waitAndGo(float seconds)
    {
        _isWaiting = true;
        animator.SetBool("isWaiting", true);
        // TODO (maybe) : make the agent look around (animation + change its rotation) 
        yield return new WaitForSeconds(seconds);

        if(_currentKeyNodeIndex == myKeyNodes.Length)
            _currentKeyNodeIndex = 0; // restart the patrolling        
        GoToNextKeyNode();
        animator.SetBool("isWaiting", false);
        _isWaiting = false;
    }
}
