using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
using System;

public class Pathfinding : MonoBehaviour {

    //replaced by path request manager class
    //public Transform seeker, target;

    Grid grid;

    void Awake()
    {        
            grid = GetComponent<Grid>();
    }

    //replaced durch Courutine
    ////Updates with the movement of the gameObject attached to (seeker = player), (Target = opposite player)
    //void Update()
    //{
    //    //Only change if we ask it to
    //    if (Input.GetButtonDown("Jump"))
    //    FindPath(seeker.position, target.position);
    //}


    //ones it found a path it needs to call the finishprocessingpath of the pathrequest manager script
    public void FindPath(PathRequest request, Action<PathResult> callback)
    {
        //To see performance gain
        Stopwatch sw = new Stopwatch();
        sw.Start();

        Vector3[] waypoints = new Vector3[0];
        //check if we find a path
        bool pathSuccess = false;

        //Convert Worldpos in Nodes, already done in grid method
        Node startNode = grid.NodeFromWorldPoint(request.pathStart);
        Node targetNode = grid.NodeFromWorldPoint(request.pathEnd);

        //Openset of nodes to be evaluated
        //List<Node> openSet = new List<Node>();

        //if they are not walkable its impossible to find a path
        if(startNode.walkable && targetNode.walkable)
        { 
        //Optimising with Heap
        Heap<Node> openSet = new Heap<Node>(grid.MaxSize);
        //Closedset of nodes to be already evaluated
        HashSet<Node> closedSet = new HashSet<Node>();
        openSet.Add(startNode);


        //search for node with lowes f_cost is the most expensive part of algorithm
        while (openSet.Count > 0)
        {
            //find Node with the lowest f_cost
            Node currentNode = openSet.RemoveFirst();

            ////now we found the node with the lowest fCost in the openset
            ////remove it from openSet and add to closeSet
            //openSet.Remove(currentNode);
            closedSet.Add(currentNode);

            //we found ouer path
            if(currentNode == targetNode)
            {
                sw.Stop();
                print("Path found: " + sw.ElapsedMilliseconds + " ms");
                //found path
                pathSuccess = true;
                break;
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
                int newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour) + neighbour.movementPenalty;
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
                    else
                        openSet.UpdateItem(neighbour);
                }
            }

        }
        }
        if (pathSuccess) { 
        waypoints = RetracePath(startNode, targetNode);
            //fix problem when target moves only a little bit
            pathSuccess = waypoints.Length > 0;
        }
        callback(new PathResult(waypoints, pathSuccess, request.callback));
    }

    //Ones we found target node (current node = target node), we need to retrace the steps to get the path from start to end node
    Vector3[] RetracePath(Node startNode, Node endNode)
    {
        List<Node> path = new List<Node>();
        Node currentNode = endNode;

        //Trace the path backwards with the parents
        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }

        Vector3[] waypoints = SimplifyPath(path);
        //reverse to get it the right way around
        Array.Reverse(waypoints);
        return waypoints;

        ////visual the path with gizmos
        //grid.path = path;

    }

    //simplify the path, only make waypoints when it changes direction
    Vector3[] SimplifyPath(List<Node> path)
    {
        List<Vector3> waypoints = new List<Vector3>();
        Vector2 directionOld = Vector2.zero;

        for(int i = 1; i < path.Count; i++)
        {
            //Get the direction from the last two nodes
            Vector2 directionNew = new Vector2(path[i - 1].gridX - path[i].gridX, path[i - 1].gridY - path[i].gridY);
            if(directionNew != directionOld)
            {
                //path changed direction so add the waypoint to the waypoints list
                waypoints.Add(path[i].worldPosition);
            }

            directionOld = directionNew;
        }
        //after finished the loop convert the list to an array an return
        return waypoints.ToArray();
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
