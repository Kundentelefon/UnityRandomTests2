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

    public int gCost;
    public int hCost;
    public Node parent;
    int heapIndex;

    public Node(bool _walkable, Vector3 _worldPos, int _gridX, int _gridY)
    {
        walkable = _walkable;
        worldPosition = _worldPos;
        gridX = _gridX;
        gridY = _gridY;
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


    // -------------   DIJKSTRA NODES -----------------
    /// <summary>
    /// Reset all the values in the nodes.
    /// </summary>
    /// 

    [SerializeField] private float weight = int.MaxValue;
    [SerializeField] private Node parentNode;
    [SerializeField] private List<Transform> neighbourNode;

    // Use this for initialization
    void Start()
    {
        this.resetNode();
    }

    /// <summary>
    /// Reset all the values in the nodes.
    /// </summary>
    public void resetNode()
    {
        weight = int.MaxValue;
        parentNode = null;
    }

    /// <summary>
    /// Set the parent node.
    /// </summary>
    /// <param name="node">Set the node for parent node.</param>
    public void setParentNode(Node node)
    {
        this.parentNode = node;
    }

    /// <summary>.
    /// Set the weight value
    /// </summary>
    /// <param name="value">weight value</param>
    public void setWeight(float value)
    {
        this.weight = value;
    }

    public List<Transform> getNeighbourNode()
    {
        List<Transform> result = this.neighbourNode;
        return result;
    }

    /// <summary>
    /// Adding neighbour node object.
    /// </summary>
    /// <param name="node">Node transform</param>
    public void addNeighbourNode(Transform node)
    {
        this.neighbourNode.Add(node);
    }

    /// <summary>
    /// Get weight
    /// </summary>
    /// <returns>get weight in float.</returns>
    public float getWeight()
    {
        float result = this.weight;
        return result;

    }

    /// <summary>
    /// Get the parent Node.
    /// </summary>
    /// <returns>Return the parent node.</returns>
    public Node getParentNode()
    {
        Node result = this.parentNode;
        return result;
    }
}
