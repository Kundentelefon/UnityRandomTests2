using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//smothing the turns by drawing a line befor each vector and starts to slowly change the direction the the next point
//whole point of this line struct is to determine wether or not the unit has past a turn boundry
public struct Line  {

    const float verticalLineGradient = 1e5f;

    float gradient;
    float y_intercept;
    Vector2 pointOnLine_1;
    Vector2 pointOnLine_2;

    float gradientPerpendicular;

    bool approachSide;

    public Line(Vector2 pointOnLine, Vector2 pointPerpendicularToLine)
    {
        float dx = pointOnLine.x - pointPerpendicularToLine.x;
        float dy = pointOnLine.y - pointPerpendicularToLine.y;

        //if the line is vertical, than dx = 0, so we set the gradient as a high value (problem it would be straight and less accurate but thats not such a problem)
        if (dx == 0)
        {
            gradientPerpendicular = verticalLineGradient;
        }
        else { 
        gradientPerpendicular = dy / dx;
        }

        //calculate the gradient of the actual line, line * perpendicularLine = -1, counter that with this
        if(gradientPerpendicular == 0)
        {
            gradient = verticalLineGradient;
        }
        else { 
        gradient = -1 / gradientPerpendicular;
        }

        //calculate y intercept, line = y=mx+c, intercept = c=y-mx
        y_intercept = pointOnLine.y - gradient * pointOnLine.x;
        pointOnLine_1 = pointOnLine;
        pointOnLine_2 = pointOnLine + new Vector2(2, gradient);

        //for struct you need to asign every variable befor using it
        approachSide = false;
        //When we are creating the turnboundary we gonna use the previous point in the past as the point perpendicular to the line, thats the side in which the unit will approach the line
        approachSide = GetSide(pointPerpendicularToLine);
    }

    //Todo: watch what this does
    //whole point of this line struct is to determine wether or not the unit has past a turn boundry
    bool GetSide(Vector2 p)
    {
        return (p.x - pointOnLine_1.x) * (pointOnLine_2.y - pointOnLine_1.y) > (p.y - pointOnLine_1.y) * (pointOnLine_2.x - pointOnLine_1.x);
    }

    //if its on the other side of the line of the perpendicular point of the constructor
    public bool HasCrossedLine(Vector2 p)
    {
        return GetSide(p) != approachSide;
    }

    //get the distance towards the end, point. Calculate the ray as a straight line perpendicular to the end 
    public float DistanceFromPoint(Vector2 p)
    {
        //we know already the gradient and line intercept of the line itself and its gradientPerpendicular, so we only need the y intercept
        float yInterceptPerpendicular = p.y - gradientPerpendicular * p.x;
        //x coordinate of the point of intersection is x= (c2-x1) / (m1-m2) which is y intercept of 1st line - y intercept of 2nd line
        float intersectX = (yInterceptPerpendicular - y_intercept) / (gradient - gradientPerpendicular);
        // point of intersection of t he y axis
        float intersectY = gradient * intersectX + y_intercept;
        return Vector2.Distance(p, new Vector2(intersectX, intersectY));
    }


    public void DrawWithGizmos(float length)
    {
        Vector3 lineDirection = new Vector3(1, 0, gradient).normalized;
        Vector3 lineCentre = new Vector3(pointOnLine_1.x, 0, pointOnLine_1.y) + Vector3.up;
        Gizmos.DrawLine(lineCentre - lineDirection * length / 2f, lineCentre + lineDirection * length  /2f);
    }
}
