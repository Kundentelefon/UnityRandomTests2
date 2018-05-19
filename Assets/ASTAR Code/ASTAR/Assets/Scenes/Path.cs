using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Path  {

    public readonly Vector3[] lookPoints;
    public readonly Line[] turnBoundaries;
    public readonly int finishLineIndex;
    public readonly int slowDownIndex;

    public Path(Vector3[] waypoints, Vector3 startPos, float turnDst, float stoppingDst)
    {
        //lookpoints rebrands waypoints
        lookPoints = waypoints;
        turnBoundaries = new Line[lookPoints.Length];
        //Last turnboundaries in the array
        finishLineIndex = turnBoundaries.Length - 1;

        Vector2 previousPoint = V3ToV2(startPos);
        //look through all the points from start position
        for (int i = 0; i < lookPoints.Length; i++)
        {
            Vector2 currentPoint = V3ToV2(lookPoints[i]);
            Vector2 directionToCurrentPoint = (currentPoint - previousPoint).normalized;
            //last point in the turnboundariesarray is the finish line, we dont want to substract turndst from that
            Vector2 turnBoundaryPoint = (i== finishLineIndex) ? currentPoint : currentPoint - directionToCurrentPoint * turnDst;
            //pas in bound on line and point perpendicular to line(previouspoint)
            //what if the turnDst is bigger than the previousPoint and the currentPoin? in that case previous point will be on the wrong side and cross line method wont work so - dirTocurrentpoin * turndst
            turnBoundaries[i] = new Line(turnBoundaryPoint, previousPoint -directionToCurrentPoint * turnDst);
            previousPoint = turnBoundaryPoint;
        }

        //adds the distance from the endpoint backwards so the unit wont slow down when its near the endpoint put has still to do alot of other points
        float dstFromEndPoint = 0;
        for (int i = lookPoints.Length - 1; i > 0; i--)
        {
            dstFromEndPoint += Vector3.Distance(lookPoints[i], lookPoints[i - 1]);
            if(dstFromEndPoint > stoppingDst)
            {
                slowDownIndex = i;
                break;
            }
        }
    }

    //line struct works with vector2 so we need to convert Vector3 to vector2, in this case we need x & z
    //convenient method to do that
    Vector2 V3ToV2(Vector3 v3)
    {
        return new Vector2(v3.x, v3.z);
    }

    //visualizing the path
    public void DrawWithGizmos()
    {
        Gizmos.color = Color.black;
        foreach (Vector3 p in lookPoints)
        {
            Gizmos.DrawCube(p + Vector3.up, Vector3.one);
        }

        Gizmos.color = Color.white;

        foreach(Line l in turnBoundaries)
        {
            l.DrawWithGizmos(10);
        }
    }
}
