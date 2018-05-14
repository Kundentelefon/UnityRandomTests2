using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Put on A* Gameobject
public class Node
{

    public bool walkable;
    //What point in the world this node represent
    public Vector3 worldPosition;

    public Node(bool _walkable, Vector3 _worldPos)
    {
        walkable = _walkable;
        worldPosition = _worldPos;
    }

}
