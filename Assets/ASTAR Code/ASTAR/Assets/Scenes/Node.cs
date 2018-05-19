using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Put on A* Gameobject
public class Node : IHeapItem<Node>
{

    public bool walkable;
    //What point in the world this node represent
    public Vector3 worldPosition;
    //keep track where the node is in the grid
    public int gridX;
    public int gridY;
    //gives a value how good the terrain is to move
    public int movementPenalty;

    public int gCost;
    public int hCost;
    public Node parent;
    int heapIndex;

    public Node(bool _walkable, Vector3 _worldPos, int _gridX, int _gridY, int _penalty)
    {
        walkable = _walkable;
        worldPosition = _worldPos;
        gridX = _gridX;
        gridY = _gridY;
        movementPenalty = _penalty;
    }

    //Impelemnted from the Interface
    public int FCost
    {
        get
        {
            return gCost + hCost;
        }
    }

    public int HeapIndex
    {
        get
        {
            return heapIndex;
        }

        set
        {
            heapIndex = value;
        }
    }

    public int CompareTo(Node nodeToComapre)
    {
        int compare = FCost.CompareTo(nodeToComapre.FCost);
        //if the two fCosts are equal, than hCost decides
        if(compare == 0)
        {
            compare = hCost.CompareTo(nodeToComapre.hCost);
        }
        //Compare returns 1 if its higher, but we wont lower so -compare
        return -compare;
    }
}
