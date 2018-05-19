using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//put on A* gameobject
public class Grid : MonoBehaviour{

    public bool displayGridGizmos;
    public LayerMask unwalkableMask;
    public Vector2 gridWorldSize;
    //how much space each individual node covers
    public float nodeRadius;
    //nodes in grid
    Node[,] grid;

    float nodeDiameter;
    int gridSizeX, gridSizeY;

    //how many notes, based on the grid radius, can we fit in to the grid
    private void Awake()
    {
        nodeDiameter = nodeRadius * 2;
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);
        CreateGrid();
    }

    //for initializing we need to know maxHeapSize
    public int MaxSize
    {
        get
        {
            return gridSizeX * gridSizeY;
        }
    }

    void CreateGrid()
    {
        grid = new Node[gridSizeX, gridSizeY];
        //give the left edge of the world & give the bottem left corner
        Vector3 worldBottomLeft = transform.position - Vector3.right * gridWorldSize.x / 2 - Vector3.forward * gridWorldSize.y / 2;

        //loop to all the position the nodes are in to do collision check to see if they are walkable
        for (int x = 0; x< gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                //get the world position for each collision check
                //as x increases we go along increments of node diameter along the world until we reach the end
                Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.forward * (y * nodeDiameter + nodeRadius);
                //Collision check for each point, true if we dont collide with anything in the unwalkable path, Checksphere returns true if there is a collision
                bool walkable = !(Physics.CheckSphere(worldPoint, nodeRadius, unwalkableMask));
                //create new node, populate the grid with node
                grid[x, y] = new Node(walkable, worldPoint, x, y);
            }
        }

    }

    //we cant use array because we dont know how many nodes in the neighbour are so list
    public List<Node> GetNeighbours(Node node)
    {
        //Where exactly is this node in the grid node
        List<Node> neighbours = new List<Node>();

        //search in 3x3 block
        for (int x = -1; x <= 1; x++) {
            for (int y = -1; y <= 1; y++)
            {
                //when both equal 0 than we are in the center so we skip that because thats the given node
                if (x == 0 && y == 0)
                    continue;

                int checkX = node.gridX + x;
                int checkY = node.gridY + y;


                //check if it is inside the grid
                if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY)
                {
                    neighbours.Add(grid[checkX, checkY]);
                }
            }
        }

        return neighbours;

    }


    //Gives the position of the player back
    public Node NodeFromWorldPoint(Vector3 worldPosition)
    {
        //gives the world coordinates back in percentage. Left = 0, Right = 1
        float percentX = (worldPosition.x + gridWorldSize.x / 2) / gridWorldSize.x;
        // z world position because Vector 3 
        float percentY = (worldPosition.z + gridWorldSize.y / 2) / gridWorldSize.y;

        //if its outside of the grid it wont get invalid
        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);

        //get the x and y indices of the grid array, -1 because array
        int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
        int y = Mathf.RoundToInt((gridSizeY - 1) * percentY);
        return grid[x, y];
    }

    //public List<Node> path;
    void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, 1, gridWorldSize.y));

            if (grid!= null && displayGridGizmos){
            foreach(Node n in grid)
            {
                //if there is no collision = white if collision = red
                Gizmos.color = (n.walkable) ? Color.white : Color.red;
                //if its on the path make the color of the path
                //if (path != null)
                //    if (path.Contains(n))
                //        Gizmos.color = Color.black;
                //-1 for a little outline for each space
                Gizmos.DrawCube(n.worldPosition, Vector3.one * (nodeDiameter - .1f));
            }
        
        }
    }

}
