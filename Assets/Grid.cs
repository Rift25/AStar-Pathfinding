using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    //a public bool to turn gizmos on or off
    public bool displayGridGizmos;
    //get the unwalkable terrain
    public LayerMask unwalkableMask;
    //the size of the grid in the world
    public Vector2 gridWorldSize;
    //the size of the nodes on the grid
    public float nodeRadius;
    Node[,] grid;

    //diameter of the nodes
    float nodeDiameter;
    //size of the grid in the X and the Y direction
    int gridSizeX, gridSizeY;
    void Awake()
    {
        //get the diameter of the node and make it twice the radius of the node
        nodeDiameter = nodeRadius * 2;
        //get the size of the grid in the X and Y direction and convert it from floats to ints
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);
        //create the grid
        CreateGrid();
    }

    //get the max size of the grid
    public int MaxSize
    {
        get
        {
            return gridSizeX * gridSizeY;
        }
    }

    //create the grid
    void CreateGrid()
    {
        grid = new Node[gridSizeX, gridSizeY];
        Vector3 worldBottomLeft = transform.position - Vector3.right * gridWorldSize.x / 2 - Vector3.forward * gridWorldSize.y / 2;
        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.forward * (y * nodeDiameter + nodeRadius);
                bool walkable = !(Physics.CheckSphere(worldPoint, nodeRadius, unwalkableMask));
                grid[x, y] = new Node(walkable, worldPoint, x, y);
            }
        }
    }

    //get a list of nodes that are neighbours to the current node that an object is on
    public List<Node> GetNeighbours(Node node)
    {
        List<Node> neighbours = new List<Node>();

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0)
                    continue;

                int checkX = node.gridX + x;
                int checkY = node.gridY + y;

                if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY)
                {
                    neighbours.Add(grid[checkX, checkY]);
                }
            }
        }
        return neighbours;
    }

    //get the world position of the node in floats and convert it to ints
    public Node NodeFromWorldPoint(Vector3 worldPosition)
    {
        float percentX = (worldPosition.x + gridWorldSize.x / 2) / gridWorldSize.x;
        float percentY = (worldPosition.z + gridWorldSize.y / 2) / gridWorldSize.y;
        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);

        int x = Mathf.RoundToInt((gridSizeX - 1.0f) * percentX);
        int y = Mathf.RoundToInt((gridSizeY - 1.0f) * percentY);
        return grid[x, y];
    }

    public List<Node> path;

    //display the grid on screen
    void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, 1, gridWorldSize.y));

        if (grid != null && displayGridGizmos)
        {
            foreach (Node n in grid)
            {
                Gizmos.color = (n.walkable) ? Color.white : Color.red;
                if (path != null)
                    if (path.Contains(n))
                        Gizmos.color = Color.black;
                Gizmos.DrawCube(n.worldPosition, Vector3.one * (nodeDiameter - .1f));
            } 
        }
    }
}