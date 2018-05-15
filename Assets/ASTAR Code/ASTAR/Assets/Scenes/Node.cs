using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Put on A* Gameobject
public class Node
{

    public bool walkable;
    //What point in the world this node represent
    public Vector3 worldPosition;
    //keep track where the node is in the grid
    public int gridX;
    public int gridY;

    public int gCost;
    public int hCost;
    public Node parent;

    public Node(bool _walkable, Vector3 _worldPos, int _gridX, int _gridY)
    {
        walkable = _walkable;
        worldPosition = _worldPos;
        gridX = _gridX;
        gridY = _gridY;
    }

    public int FCost
    {
        get
        {
            return gCost + hCost;
        }
    }

}
