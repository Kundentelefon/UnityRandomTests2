using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Put on seeker
//Test if Pathfinding works
public class Unit : MonoBehaviour {

    //drag in the target
    public Transform target;
    public float speed = 20;
    Vector3[] path;
    int targetIndex;

    private void Start()
    {
        PathRequestManager.RequestPath(transform.position, target.position, OnpathFound);
    }

    public void OnpathFound(Vector3[] newPath, bool pathSuccessfull)
    {
        //when we get the new path
        if (pathSuccessfull)
        {
            path = newPath;
            targetIndex = 0;
            //Stop the Coroutine if its already started
            StopCoroutine("FollowPath");
            StartCoroutine("FollowPath");
        }
    }

    IEnumerator FollowPath()
    {
        //default Vector is first one
        Vector3 currentWaypoint = path[0];

        while (true)
        {
            if(transform.position == currentWaypoint)
            {
                //advance to the next waypoint
                targetIndex++;
                if(targetIndex >= path.Length)
                {
                    //finished following the path
                    yield break;
                }
                currentWaypoint = path[targetIndex];
            }

            //each frame move the transform closer to the waypoint
            transform.position = Vector3.MoveTowards(transform.position, currentWaypoint, speed * Time.deltaTime);
            yield return null;
        }

    }

    //Show path of unit as Gizmo
    private void OnDrawGizmos()
    {
        if(path!= null)
        {
            //we dont want to draw the waypoints we already past so we start at targetIndex
            for (int i = targetIndex; i < path.Length; i++)
            {
                Gizmos.color = Color.black;
                Gizmos.DrawCube(path[i], Vector3.one);

                //Drawlines between waypoints, move line with us
                if (i == targetIndex)
                {
                    Gizmos.DrawLine(transform.position, path[i]);
                }
                else
                {
                    Gizmos.DrawLine(path[i - 1], path[i]);
                }
            }
        }
    }
}
