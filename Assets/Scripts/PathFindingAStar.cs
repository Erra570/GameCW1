using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFindingAStar : MonoBehaviour
{
    public static List<Node> FindPath(Node start, Node goal)
    {
        /*Initialisation*/
        List<Node> nodeToCheck = new List<Node>();
        foreach (Node n in FindObjectsOfType<Node>())
        {
            // G can chance during the search, but not the heuristic
            n.g = float.MaxValue;
            n.h = Vector3.Distance(n.transform.position, goal.transform.position);
        }
        start.g = 0;
        start.h = Vector3.Distance(start.transform.position, goal.transform.position); // The heuristic is the direct distance
        nodeToCheck.Add(start);

        while (nodeToCheck.Count > 0)
        {
            // Search nearest node : ( == smaller f )
            Node currentNode = nodeToCheck[0];
            float minF = currentNode.getF();
            foreach (Node n in nodeToCheck)
            {
                if (n.getF() < minF)
                {
                    minF = n.getF();
                    currentNode = n;
                }
            }
            nodeToCheck.Remove(currentNode);

            // Is the current node the goal Node ? If so, the path is found
            if (currentNode == goal)
            {
                List<Node> res = new List<Node>();
                res.Insert(0, goal);
                while (currentNode != start) 
                {
                    currentNode = currentNode.previousNode;
                    res.Add(currentNode);
                } 
                res.Remove(start);// (no need to have the Node "start", as we are already there)
                res.Reverse();                 
                return res;
            }

            foreach (Node neigbor in currentNode.neighbors)
            {
                float tempG = currentNode.g + Vector3.Distance(currentNode.transform.position, neigbor.transform.position);
                if (tempG < neigbor.g)
                {
                    neigbor.previousNode = currentNode;
                    neigbor.g = tempG;
                    if (!nodeToCheck.Contains(neigbor))
                    {
                        nodeToCheck.Add(neigbor);
                    }
                }
            }
        }
        return null; // the Node goal coudn't be reached
    }
}
