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
    public TerrainType[] walkableRegions;
    public int obstacleProximityPenalty = 10;
    LayerMask walkableMask;
    //dictionary is easier to search than through all walkable Regions
    Dictionary<int, int> walkableRegionsDictionary = new Dictionary<int, int>();
    //nodes in grid
    Node[,] grid;

    float nodeDiameter;
    int gridSizeX, gridSizeY;

    //to visualize penalties
    int penaltyMin = int.MaxValue;
    int penaltyMax = int.MinValue;

    //how many notes, based on the grid radius, can we fit in to the grid
    private void Awake()
    {
        nodeDiameter = nodeRadius * 2;
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);

        //give the Layermask all the Layers
        foreach(TerrainType region in walkableRegions)
        {
            //Layers are stored in 32bit int (32layers), Layer 9 returns value 512, Layer 10 returns 1024 (2^10) etc, adding two mask is simply 1536
            walkableMask.value += region.terrainMask.value;
            //With log get the layer and adds it corresponding penalty
            //What power do we need to raise to to get this value
            walkableRegionsDictionary.Add((int)Mathf.Log(region.terrainMask.value, 2),region.terrainPenalty);
        }
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

                int movementPenalty = 0;
                //shoots rays from the ground and check their layer value for movement penalty
                    Ray ray = new Ray(worldPoint + Vector3.up * 50, Vector3.down);
                    {
                        RaycastHit hit;
                    //get the Layer of the Object which is hit
                    if (Physics.Raycast(ray, out hit, 100, walkableMask))
                    {
                            //assign movement penalty
                            walkableRegionsDictionary.TryGetValue(hit.collider.gameObject.layer, out movementPenalty);
                    }
                    
                    //penalty for moving to close to obstacle

                    }

                if (!walkable)
                {
                    movementPenalty += obstacleProximityPenalty;
                }

                //create new node, populate the grid with node
                grid[x, y] = new Node(walkable, worldPoint, x, y, movementPenalty);
            }
        }

        BlurPenaltyMap(3);

    }

    //Boxblur, Kernel is the number of sorrounding nodes to take into account for blurring
    void BlurPenaltyMap(int blurSize)
    {
        //Blursize of 1 gives a Kernelsize of 3
        int kernelSize = blurSize * 2 + 1;
        //central node and the edge of the nodes, by 3x3 there is only 1 outside of the grid
        int kernelExtents = (kernelSize - 1) / 2;

        //temporary grid to store horicontal and vertical path values
        int[,] penaltiesHorizontalPass = new int[gridSizeX, gridSizeY];
        int[,] penaltiesVerticalPass = new int[gridSizeX, gridSizeY];

        for (int y = 0; y < gridSizeX; y++)
        {
            //for the first node we have to loop through all of the nodes in the kernel to sum the up, its only for the subsequential node to quickly sum them up
            //so for the first node in each row we need this loop
            for (int x = -kernelExtents; x <= kernelExtents; x++)
            {
                //when x is negative we want to clamp it to zero so it just takes the value from the first node instead of going out of bounds (same idea as repeating the values around the grid)
                int sampleX = Mathf.Clamp(x, 0, kernelExtents);
                penaltiesHorizontalPass[0, y] += grid[sampleX, y].movementPenalty;

            }

            //all the remaining columns in the row, start at 1 because we already have calculated 0
            for (int x = 1; x < gridSizeX; x++)
            {
                //calculate the index of the node taht is no longer inside the kernel when the kernel shifts along
                int removeIndex = Mathf.Clamp(x - kernelExtents - 1, 0, gridSizeX);
                //node that just entert the kernel
                int addIndex = Mathf.Clamp(x + kernelExtents, 0, gridSizeX-1);

                //gives the movement penalty to that position
                penaltiesHorizontalPass[x, y] = penaltiesHorizontalPass[x - 1, y] - grid[removeIndex, y].movementPenalty + grid[addIndex, y].movementPenalty;

            }
        }

        //same thing for horicontal path
        for (int x = 0; x < gridSizeY; x++)
        {
            //for the first node we have to loop through all of the nodes in the kernel to sum the up, its only for the subsequential node to quickly sum them up
            //so for the first node in each row we need this loop
            for (int y = -kernelExtents; y <= kernelExtents; y++)
            {
                //when x is negative we want to clamp it to zero so it just takes the value from the first node instead of going out of bounds (same idea as repeating the values around the grid)
                int sampleY = Mathf.Clamp(y, 0, kernelExtents);
                penaltiesVerticalPass[x, 0] += penaltiesHorizontalPass[x, sampleY];

            }

            //assings the penalty to the corner of the grid
            int blurredPenalty = Mathf.RoundToInt((float)penaltiesVerticalPass[x, 0] / (kernelSize * kernelSize));
            grid[x, 0].movementPenalty = blurredPenalty;

            //all the remaining columns in the row, start at 1 because we already have calculated 0
            for (int y = 1; y < gridSizeY; y++)
            {
                //calculate the index of the node taht is no longer inside the kernel when the kernel shifts along
                int removeIndex = Mathf.Clamp(y - kernelExtents - 1, 0, gridSizeY);
                //node that just entert the kernel
                int addIndex = Mathf.Clamp(y + kernelExtents, 0, gridSizeY - 1);

                //gives the movement penalty to that position
                penaltiesVerticalPass[x, y] = penaltiesVerticalPass[x, y-1] - penaltiesHorizontalPass[x, removeIndex] + penaltiesHorizontalPass[x, addIndex];

                //to get the final blurred path for each node, divide for average kernal, rount to int so it will not always round down
                blurredPenalty = Mathf.RoundToInt((float)penaltiesVerticalPass[x, y] / (kernelSize * kernelSize));
                grid[x, y].movementPenalty = blurredPenalty;

                //visualize penalty
                if(blurredPenalty > penaltyMax)
                {
                    penaltyMax = blurredPenalty;
                }
                if(blurredPenalty < penaltyMin)
                {
                    penaltyMin = blurredPenalty;
                }

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

        //Detaches Grid class with anyithing from the path
        //if (onlyDisplayPathGizmos)
        //{
        //    if(path!= null)
        //    {
        //        foreach (Node n in path)
        //        {
        //            Gizmos.color = Color.black;
        //            //-1 for a little outline for each space
        //            Gizmos.DrawCube(n.worldPosition, Vector3.one * (nodeDiameter - .1f));
        //        }

        //    }
        //}
        //else { 

        if (grid!= null && displayGridGizmos){
            foreach(Node n in grid)
            {
                //fade from white to black tependend on the penalty weight, and a value to zero or 1 if the value is lowest or highest
                Gizmos.color = Color.Lerp(Color.white, Color.black, Mathf.InverseLerp(penaltyMin, penaltyMax, n.movementPenalty));
                //if there is no collision = white if collision = red
                Gizmos.color = (n.walkable) ? Gizmos.color : Color.red;
                //if its on the path make the color of the path
                //if (path != null)
                //    if (path.Contains(n))
                //        Gizmos.color = Color.black;
                //-1 for a little outline for each space
                Gizmos.DrawCube(n.worldPosition, Vector3.one * (nodeDiameter));
            }
        
        }
    }

}

//Show up in the inspector
[System.Serializable]
//Pair up a value with a layer
public class TerrainType
{
    public LayerMask terrainMask;
    public int terrainPenalty;
}