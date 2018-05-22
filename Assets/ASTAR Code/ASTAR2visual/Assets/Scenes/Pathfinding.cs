using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
using System;

public class Pathfinding : MonoBehaviour {

    //replaced by path request manager class
    //public Transform seeker, target;

    public bool useDijkstra = false;

    PathRequestManager requestManager;
    List<Node> drawNodes = new List<Node>();
    bool drawThis = false;
    int drawSpeed = 30;
    int count = 0;

    Grid grid;

    void Awake()
    {        
            grid = GetComponent<Grid>();
        requestManager = GetComponent<PathRequestManager>();
        drawThis = true;
    }

    //replaced durch Courutine
    ////Updates with the movement of the gameObject attached to (seeker = player), (Target = opposite player)
    //void Update()
    //{
    //    //Only change if we ask it to
    //    if (Input.GetButtonDown("Jump"))
    //    FindPath(seeker.position, target.position);
    //}

    public void StartFindPath(Vector3 startPos, Vector3 targetPos)
    {
        if (!useDijkstra)
            StartCoroutine(FindPath(startPos, targetPos));
        else
            StartCoroutine(DijkstrasAlgo(startPos, targetPos));

    }

    //ASTAR algorithmus
    //ones it found a path it needs to call the finishprocessingpath of the pathrequest manager script
    IEnumerator FindPath(Vector3 startPos, Vector3 targetPos)
    {
        //To see performance gain
        Stopwatch sw = new Stopwatch();
        sw.Start();

        Vector3[] waypoints = new Vector3[0];
        //check if we find a path
        bool pathSuccess = false;

        //Convert Worldpos in Nodes, already done in grid method
        Node startNode = grid.NodeFromWorldPoint(startPos);
        Node targetNode = grid.NodeFromWorldPoint(targetPos);

        //Openset of nodes to be evaluated
        //List<Node> openSet = new List<Node>();

        //if they are not walkable its impossible to find a path
        if (startNode.walkable && targetNode.walkable)
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
                //This Code is optimizid through heap
                //Node currentNode = openSet[0]; //first element in openset
                ////loop to all of the nodes in openset
                //for (int i = 1; i < openSet.Count; i++)
                //{    
                //    //if the fCost ist equal with the other node, than we see which one is closest to end node by comapring hCost
                //    if (openSet[i].FCost < currentNode.FCost || openSet[i].FCost == currentNode.FCost && openSet[i].hCost < currentNode.hCost)
                //    {
                //        //find node in the openset with the lowest fCost
                //        currentNode = openSet[i];
                //    }
                //}

                ////now we found the node with the lowest fCost in the openset
                ////remove it from openSet and add to closeSet
                //openSet.Remove(currentNode);
                closedSet.Add(currentNode);
                //we found ouer path
                if (currentNode == targetNode)
                {
                    sw.Stop();
                    print("Path found: " + sw.ElapsedMilliseconds + " ms" + " with a frameWait of " + drawSpeed);
                    //found path
                    pathSuccess = true;
                    break;
                }

                //if not found the past, loop through each neighbouring node of current node
                foreach (Node neighbour in grid.GetNeighbours(currentNode))
                {
                    if (count >= drawSpeed)
                    {
                        yield return new WaitForEndOfFrame();
                        count = 0;
                    }
                    count++;
                    drawNodes.Add(neighbour);
                    //check if the neighbour in the not walkable or in the close list, than skip
                    if (!neighbour.walkable || closedSet.Contains(neighbour))
                    {
                        continue;
                    }

                    //Check if the new path to the neighbar is shorter than the old one, or if the neighbour is not in the node list
                    int newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour);
                    if (newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
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

        //wait for 1 frame befor returning
        yield return null;
        if (pathSuccess)
        {
            waypoints = RetracePath(startNode, targetNode);
        }
        requestManager.FinishedProcessingPath(waypoints, pathSuccess);
    }

    //private GameObject[] nodes;

    ///// <summary>
    ///// Finding the shortest path and return in a List
    ///// </summary>
    ///// <param name="start">The start point</param>
    ///// <param name="end">The end point</param>
    ///// <returns>A List of transform for the shortest path</returns>
    //IEnumerator findShortestPath(Vector3 start, Vector3 end)
    //{

    //    //nodes = GameObject.FindGameObjectsWithTag("Node");

    //    List<Vector3> result = new List<Vector3>();
    //    Vector3 node = DijkstrasAlgo(start, end);
    //    Vector3[] waypoints = new Vector3[0];


    //    // While there's still previous node, we will continue.
    //    while (node != null)
    //    {
    //        result.Add(node);
    //        Node currentNode = node.GetComponent<Node>();
    //        node = currentNode.getParentNode();
    //    }

    //    //wait for 1 frame befor returning
    //    yield return null;
    //        waypoints = RetracePath(startNode, targetNode);
        
    //    requestManager.FinishedProcessingPath(waypoints, pathSuccess);

    //    // Reverse the list so that it will be from start to end.
    //    result.Reverse();
    //    return result;
    //}

    /// <summary>
    /// Dijkstra Algorithm to find the shortest path
    /// </summary>
    /// <param name="start">The start point</param>
    /// <param name="end">The end point</param>
    /// <returns>The end node</returns>
    IEnumerator DijkstrasAlgo(Vector3 start, Vector3 end)
    {
        double startTime = Time.realtimeSinceStartup;

        Vector3[] waypoints = new Vector3[0];

        // Nodes that are unexplored
        List<Node> unexplored = new List<Node>();
        List<Node> nodes = new List<Node>();
         nodes = grid.getAllNodes();


        // We add all the nodes we found into unexplored.
        foreach (Node n in nodes)
        {
            if (n.walkable)
            {
                n.resetNode();
                unexplored.Add(n);
            }
        }

        // Set the starting node weight to 0;
        Node startNode = grid.NodeFromWorldPoint(start);
        Node targetNode = grid.NodeFromWorldPoint(end);
        startNode.setWeight(0);

        while (unexplored.Count > 0)
        {
           
            // Sort the explored by their weight in ascending order.
            unexplored.Sort((x, y) => x.getWeight().CompareTo(y.getWeight()));

            // Get the lowest weight in unexplored.
            Node current = unexplored[0];

            // Note: This is used for games, as we just want to reduce compuation, better way will be implementing A*

            //If we reach the end node, we will stop.
            //if (current.worldPosition == end)
            //{
            //    RetracePath(startNode, targetNode);
            //}

            //Remove the node, since we are exploring it now.
            unexplored.Remove(current);

            Node currentNode = current;
            List<Node> neighbours = grid.GetNeighbours(currentNode);
            foreach (Node neighNode in neighbours)
            {
                Node node = neighNode;

                if (count >= drawSpeed)
                {
                    yield return new WaitForEndOfFrame();
                    count = 0;
                }
                count++;
                drawNodes.Add(node);

                // We want to avoid those that had been explored and is not walkable.
                if (unexplored.Contains(neighNode) && node.walkable)
                {
                    // Get the distance of the object.
                    float distance = Vector3.Distance(neighNode.worldPosition, current.worldPosition);
                    distance = currentNode.getWeight() + distance;

                    // If the added distance is less than the current weight.
                    if (distance < node.getWeight())
                    {
                        // We update the new distance as weight and update the new path now.
                        node.setWeight(distance);
                        node.setParentNode(current);
                    }
                }
            }

        }

        //RetracePath(startNode, targetNode);

        //wait for 1 frame befor returning
        yield return null;
            waypoints = RetracePath(startNode, targetNode);
        
        requestManager.FinishedProcessingPath(waypoints, true);

        double endTime = (Time.realtimeSinceStartup - startTime);
        print("Compute time: " + endTime);

        print("Path completed!");

    }

    void OnDrawGizmos()
    {
        if (drawThis) { 
        foreach (Node n in drawNodes)
            {

                //if there is no collision = white if collision = red
                Gizmos.color = (n.gCost < n.hCost) ? Color.green : Color.red;

            Gizmos.DrawWireSphere(n.worldPosition, 1);
            }
        }
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
            //currentNode = currentNode.parent;
            if (!useDijkstra)
                currentNode = currentNode.parent;
            else
                currentNode = currentNode.getParentNode();
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
