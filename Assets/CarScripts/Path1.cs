using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Collections.Generic;

//Script on PathParent
public class Path : MonoBehaviour {

    public Color lineColor;
    private List<Transform> nodes = new List<Transform>();

    //Für allway visible OnDrawGizmos()
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = lineColor;

        Transform[] pathTransform = GetComponentsInChildren<Transform>();
        nodes = new List<Transform>();

        //holt sich alle Transforms die nicht in der Liste sind
        for(int i=0; i < pathTransform.Length; i++)
        {
            if(pathTransform[i] != transform)
            {
                nodes.Add(pathTransform[i]);
            }
        }

        //Zeichnet alle Child Nodes vom Parent
        for(int i=0; i < nodes.Count; i++)
        {
            //Current position of the node, -1 erhält den vorherigen Node und verbindet diese mit Linie
            Vector3 currentNode = nodes[i].position;
            Vector3 previousNode = Vector3.zero;
            if (i > 0)
            {
                previousNode = nodes[i - 1].position;

            }
            //Verbindet ersten Node mit dem Letzten
            else if (i == 0 && nodes.Count > 1)
            {
                previousNode = nodes[nodes.Count - 1].position;
            }
            
            //Zeichnet Gizmo lines und Node spheres
            Gizmos.DrawLine(previousNode, currentNode);
            Gizmos.DrawWireSphere(currentNode, 0.3f);
            
            
        }
    }

}
