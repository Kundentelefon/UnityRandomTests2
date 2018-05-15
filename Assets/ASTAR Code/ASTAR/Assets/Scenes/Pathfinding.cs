using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Collections.Generic;

public class Pathfinding : MonoBehaviour {

    public Transform seeker, target;

    Grid grid;

    void Awake()
    {        
            grid = GetComponent<Grid>();        
    }

    //Updates with the movement of the gameObject attached to (seeker = player), (Target = opposite player)
    void Update()
    {
        FindPath(seeker.position, target.position);
    }

    void FindPath(Vector3 startPos, Vector3 targetPos)
    {
        //Convert Worldpos in Nodes, already done in grid method
        Node startNode = grid.NodeFromWorldPoint(startPos);
        Node targetNode = grid.NodeFromWorldPoint(targetPos);

        //Openset of nodes to be evaluated
        List<Node> openSet = new List<Node>();
        //Closedset of nodes to be already evaluated
        HashSet<Node> closedSet = new HashSet<Node>();
        openSet.Add(startNode);

        //search for node with lowes f_cost is the most expensive part of algorithm
        while (openSet.Count > 0)
        {
            //find Node with the lowest f_cost
            Node currentNode = openSet[0]; //first element in openset
            //loop to all of the nodes in openset
            for (int i = 1; i < openSet.Count; i++)
            {    
                //if the fCost ist equal with the other node, than we see which one is closest to end node by comapring hCost
                if (openSet[i].FCost < currentNode.FCost || openSet[i].FCost == currentNode.FCost && openSet[i].hCost < currentNode.hCost)
                {
                    //find node in the openset with the lowest fCost
                    currentNode = openSet[i];
                }
            }

            //now we found the node with the lowest fCost in the openset
            //remove it from openSet and add to closeSet
            openSet.Remove(currentNode);
            closedSet.Add(currentNode);

            //we found ouer path
            if(currentNode == targetNode)
            {
                RetracePath(startNode, targetNode);
                return;
            }

            //if not found the past, loop through each neighbouring node of current node
            foreach(Node neighbour in grid.GetNeighbours(currentNode))
            {
                //check if the neighbour in the not walkable or in the close list, than skip
                if(!neighbour.walkable || closedSet.Contains(neighbour))
                {
                    continue;
                }

                //Check if the new path to the neighbar is shorter than the old one, or if the neighbour is not in the node list
                int newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour);
                if(newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                {
                    //set the fXost of the neighbour
                    //we calculate the gCost and hCost
                    neighbour.gCost = newMovementCostToNeighbour;
                    //distance from the node to the end node
                    neighbour.hCost = GetDistance(neighbour, targetNode);
                    //set the parent of the neighbour to the current node
                    neighbour.parent = currentNode;

                    //check if the neighbour is not in the open set, if not add to open set
                    if (!openSet.Contains(neighbour))
                        openSet.Add(neighbour);
                }
            }

        }
    }

    //Ones we found target node (current node = target node), we need to retrace the steps to get the path from start to end node
    void RetracePath(Node startNode, Node endNode)
    {
        List<Node> path = new List<Node>();
        Node currentNode = endNode;

        //Trace the path backwards with the parents
        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }

        //reverse to get it the right way around
        path.Reverse();

        //visual the path with gizmos
        grid.path = path;
    }

    //count on the x,y Axis how far the node is away
    //first get the smaller Axis, so we know how far we can walk diagonally
    //second substract this number fon the bigger Axis to know how far wie have to go this way
    int GetDistance(Node nodeA, Node nodeB)
    {
        int dstX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
        int dstY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

        //Costs depends on the bigger side of the Axis
        //Cost is 14y + 10(x-y) 
        if (dstX > dstY)        
            return 14 * dstY + 10 * (dstX - dstY);
        //ifCost is 14x + 10(x-y)
        return 14 * dstX + 10 * (dstY - dstX);
    }
}
