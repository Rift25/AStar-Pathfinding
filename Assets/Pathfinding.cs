using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
using System;

public class Pathfinding : MonoBehaviour
{
    //requests the path
    PathRequestManager requestManager;

    //the grid that we are using to find the path on
    Grid grid;

    //get the components for the request manager and the grid
    void Awake()
    {
        requestManager = GetComponent<PathRequestManager>();
        grid = GetComponent<Grid>();
    }

    //start the coroutine to find the path
    public void StartFindPath(Vector3 startPos, Vector3 targetPos)
    {
        StartCoroutine(FindPath(startPos, targetPos));
    }
    
    IEnumerator FindPath(Vector3 startPos, Vector3 targetPos)
    {
        //start the stopwatch to measure how fast the path is found
        Stopwatch sw = new Stopwatch();
        sw.Start();
        //create new waypoints and start at zero
        Vector3[] waypoints = new Vector3[0];
        //a bool that indicates if the path has been created or not
        bool pathSuccess = false;

        //get the starting and the target node
        Node startNode = grid.NodeFromWorldPoint(startPos);
        Node targetNode = grid.NodeFromWorldPoint(targetPos);

        //if the nodes can be walked on start adding nodes that can be walked on
        if (startNode.walkable && targetNode.walkable)
        {
            //create a new openSet list of nodes with the max size of the grid
            List<Node> openSet = new List<Node>(grid.MaxSize);
            HashSet<Node> closedSet = new HashSet<Node>();
            //add the start node to the openSet
            openSet.Add(startNode);

            while (openSet.Count > 0)
            {
                Node currentNode = openSet[0];

                for (int i = 1; i < openSet.Count; i++)
                {
                    if (openSet[i].fCost < currentNode.fCost || openSet[i].fCost == currentNode.fCost)
                    {
                        if (openSet[i].hCost < currentNode.hCost)
                            currentNode = openSet[i];
                    }
                }

                openSet.Remove(currentNode);
                closedSet.Add(currentNode);
                //if the current node is the same as the target node the path has been created
                if (currentNode == targetNode)
                {
                    sw.Stop();
                    print("Path found: " + sw.ElapsedMilliseconds + " ms");
                    pathSuccess = true;
                    break;
                }

                foreach (Node neighbour in grid.GetNeighbours(currentNode))
                {
                    if (!neighbour.walkable || closedSet.Contains(neighbour))
                    {
                        continue;
                    }
                    int newMovementCosttoNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour);
                    if (newMovementCosttoNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                    {
                        neighbour.gCost = newMovementCosttoNeighbour;
                        neighbour.hCost = GetDistance(neighbour, targetNode);
                        neighbour.parent = currentNode;

                        if (!openSet.Contains(neighbour))
                        {
                            openSet.Add(neighbour);
                        }
                    }
                }
            }
        }
        yield return null;
        //if the path was created retrace it to make sure its correct
        if (pathSuccess)
        {
            waypoints = RetracePath(startNode, targetNode);
        }
        //tell the PathRequestManager that the path is ready
        requestManager.FinishedProcessPath(waypoints, pathSuccess);
    }

    //retrace the path we have to make sure its correct
    Vector3[] RetracePath(Node startNode, Node endNode)
    {
        List<Node> path = new List<Node>();
        Node currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }
        Vector3[] waypoints = GetPath(path);
        Array.Reverse(waypoints);
        return waypoints;
    }

    //converts the path into a Vec3 array and gets it
    Vector3[] GetPath(List<Node> path)
    {
        List<Vector3> waypoints = new List<Vector3>();

        for (int i = 1; i < path.Count; i++)
        {
                waypoints.Add(path[i].worldPosition);
        }
        return waypoints.ToArray();
    }

    //get the distance between two nodes on the path
    int GetDistance(Node nodeA, Node nodeB)
    {
        int dstX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
        int dstY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

        if (dstX > dstY)
            return 14 * dstY + 10 * (dstX - dstY);
        return 14 * dstX + 10 * (dstY - dstX);
    }
}