using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Put on seeker
//Test if Pathfinding works
public class Unit : MonoBehaviour {

    const float minPathUpdateTime = .2f;
    const float pathUpdateMoveThreshold = .5f;
    
    //drag in the target
    public Transform target;
    public float speed = 20;
    public float turnSpeed = 3;
    public float turnDst = 5;
    //how far from the finish line the unit start to stopping down
    public float stoppingDst = 10;
    //Vector3[] path;
    //int targetIndex;

    Path path;

    private void Start()
    {
        StartCoroutine(UpdatePath());
    }

    public void OnpathFound(Vector3[] waypoints, bool pathSuccessfull)
    {
        //when we get the new path
        if (pathSuccessfull)
        {
            path = new Path(waypoints, transform.position, turnDst, stoppingDst);
            //Stop the Coroutine if its already started
            StopCoroutine("FollowPath");
            StartCoroutine("FollowPath");
        }
    }

    public void Pathfinished()
    {
        Debug.Log("FINISHED");
    }

    //if the target position moves, the path automatical updates
    IEnumerator UpdatePath()
    {
        //every frame we request a new path from the pathREquestManager

            //at the start of the game it has an high delta time, so wait 0.3 seconds
            if(Time.timeSinceLevelLoad < 0.3f)
            {
                yield return new WaitForSeconds(.3f);
            }
            PathRequestManager.RequestPath(new PathRequest( transform.position, target.position, OnpathFound));

        //only update the path when it has moved for a certain threshold -> better performance
        //comparing square distance is better than the actual distance because square root wont be needed
        float sqrMoveThreshhold = pathUpdateMoveThreshold * pathUpdateMoveThreshold;
        Vector3 targetPosOld = target.position;
        while (true)
        {
            //minimum time befor update pathRequest
            yield return new WaitForSeconds(minPathUpdateTime);
            if ((target.position - targetPosOld).sqrMagnitude > sqrMoveThreshhold) { 
        PathRequestManager.RequestPath(new PathRequest( transform.position, target.position, OnpathFound));
                targetPosOld = target.position;
            }
        }
    }

    IEnumerator FollowPath()
    {
        bool followingPath = true;
        int pathIndex = 0;
        //start the unit facing the first lookpoint in the path
        transform.LookAt(path.lookPoints[0]);

        float speedPercent = 1;

        //run while we are following the path
        while (followingPath)
        {
            //constantly check if the unit has past the turn boundary
            Vector2 pos2D = new Vector2(transform.position.x, transform.position.z);
            //while so speed cant exceed the update frame and jumps over points, 
            while (path.turnBoundaries[pathIndex].HasCrossedLine(pos2D))
            {
                //if we actually finished the path
                if (pathIndex == path.finishLineIndex)
                {
                    followingPath = false;
                    //it keeps looping so it find the last turnboundary
                    break;
                }
                //otherwise increment the path index
                else
                {
                    pathIndex++;
                }
            }
            //everyframe, provided if we still follow the path, rotate the units a bit to the next point and move a bit forward
            if (followingPath)
            {
                //we only want to slow down if
                if (pathIndex >= path.slowDownIndex && stoppingDst > 0)
                {
                //distance from the finish line, speedpercent is 1 when the distance exceeds the stoppingDst and goes gradually to zero when its near it
                speedPercent = Mathf.Clamp01(path.turnBoundaries[path.finishLineIndex].DistanceFromPoint(pos2D) / stoppingDst);
                    //as speedpercent gets very low so the last bit to the finish line will take a long time so we add a threshhold
                    if(speedPercent < 0.01f)
                    {
                        followingPath = false;
                    }
                }
                Quaternion targetRotation = Quaternion.LookRotation(path.lookPoints[pathIndex] - transform.position);
                transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * turnSpeed);
                //move the unit forward
                transform.Translate(Vector3.forward * Time.deltaTime * speed * speedPercent, Space.Self);
            }

            yield return null;
        }

    }

    //changed because of new Path class
    //IEnumerator FollowPath()
    //{
    //    //default Vector is first one
    //    Vector3 currentWaypoint = path[0];

    //    while (true)
    //    {
    //        if(transform.position == currentWaypoint)
    //        {
    //            //advance to the next waypoint
    //            targetIndex++;
    //            if(targetIndex >= path.Length)
    //            {
    //                //finished following the path
    //                yield break;
    //            }
    //            currentWaypoint = path[targetIndex];
    //        }

    //        //each frame move the transform closer to the waypoint
    //        transform.position = Vector3.MoveTowards(transform.position, currentWaypoint, speed * Time.deltaTime);
    //        yield return null;
    //    }

    //}

    //Show path of unit as Gizmo
    private void OnDrawGizmos()
    {
        if(path!= null)
        {
            path.DrawWithGizmos();
            
        }
    }
}
