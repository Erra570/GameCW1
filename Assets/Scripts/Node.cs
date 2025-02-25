using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    public Node previousNode { get; set; }
    public List<Node> neighbors;
    
    public float g {  get; set; }
    public float h { get; set; }

    public float getF()
    {
        return g+h;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        if (neighbors.Count > 0)
        {
            for (int i = 0; i < neighbors.Count; i++)
            {
                Gizmos.DrawLine(transform.position, neighbors[i].transform.position);
            }
        }
    }
}
