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
        //get the request manager component
        requestManager = GetComponent<PathRequestManager>();
        //get the grid component
        grid = GetComponent<Grid>();
    }

    //start the coroutine to find the path
    public void StartFindPath(Vector3 startPos, Vector3 targetPos)
    {
        //start the coroutine
        StartCoroutine(FindPath(startPos, targetPos));
    }
    //here we find the path to the target that the unit will use
    IEnumerator FindPath(Vector3 startPos, Vector3 targetPos)
    {
        //create a new stopwatch to measure how fast the path is found
        Stopwatch sw = new Stopwatch();
        //start the stopwatch
        sw.Start();
        //create a new array of vector 3 for the waypoints and start at zero
        Vector3[] waypoints = new Vector3[0];
        //a bool that indicates if the path has been created or not
        bool pathSuccess = false;

        //get the starting nodes position on the grid
        Node startNode = grid.NodeFromWorldPoint(startPos);
        //and the target nodes position on the grid
        Node targetNode = grid.NodeFromWorldPoint(targetPos);

        //if the starting and the target nodes can be walked on start creating the path
        if (startNode.walkable && targetNode.walkable)
        {
            //the set of nodes that still have to be evaluated
            List<Node> openSet = new List<Node>(grid.MaxSize);
            //the set of nodes that were already evaluated
            HashSet<Node> closedSet = new HashSet<Node>();
            //add the start node to the openSet
            openSet.Add(startNode);
            //while the open set isn't empty keep looping
            while (openSet.Count > 0)
            {
                //current node is equal to the first element in the open set
                Node currentNode = openSet[0];
                //loop through all of the nodes in the open set
                for (int i = 1; i < openSet.Count; i++)
                {
                    //if the node in the open set has an fCost thats lower or equal to the current nodes fCost
                    if (openSet[i].fCost < currentNode.fCost || openSet[i].fCost == currentNode.fCost)
                    {
                        //if the fCost of the node in the open set is less that the current nodes fCost
                        //or both nodes fCost is the same we also look if the hCost of the open set node is less than the hCost of the current node
                        //to see which node is closest to the target node
                        if (openSet[i].hCost < currentNode.hCost)
                            //the current node is equal to the node in the open set
                            currentNode = openSet[i];
                    }
                }
                //remove the current node from the open list
                openSet.Remove(currentNode);
                //and put it into the closed set
                closedSet.Add(currentNode);
                //if the current node is the same as the target node the path has been found
                if (currentNode == targetNode)
                {
                    //stop the stopwatch
                    sw.Stop();
                    //print out that a path has been found and how long it took
                    print("Path found: " + sw.ElapsedMilliseconds + " ms");
                    //tell the game that a path has been found
                    pathSuccess = true;
                    //end the if loop
                    break;
                }
                //for each neighbour node in the grid thats close to the current node
                foreach (Node neighbour in grid.GetNeighbours(currentNode))
                {
                    //check to see if its not walkable or if the closed set containts that neighbour
                    if (!neighbour.walkable || closedSet.Contains(neighbour))
                    {
                        continue;
                    }
                    //get the new movement cost to the neighbour by calculating the current nodes gCost
                    //and its distance from the neighbour
                    int newMovementCosttoNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour);
                    //if the cost to the neighbour is less than the neighbours gCost or
                    //if the open set doesn't contain the neighbour
                    if (newMovementCosttoNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                    {
                        //make the neighbours gCost the same as its new movement cost to the neighbour
                        neighbour.gCost = newMovementCosttoNeighbour;
                        //set its hCost to be the distance between the neighbour and the target node
                        neighbour.hCost = GetDistance(neighbour, targetNode);
                        //make the neighbour the same as the current node
                        neighbour.parent = currentNode;
                        //if the open set doesn't containt the neighbour
                        if (!openSet.Contains(neighbour))
                        {
                            //add the neighbour to the open set
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
        //get a new list of nodes for the path
        List<Node> path = new List<Node>();
        //make the current node be the end node of the path
        Node currentNode = endNode;
        //while the current node isn't the same as the start node
        while (currentNode != startNode)
        {
            //add the current node to the path
            path.Add(currentNode);
            //make the current node the parent node
            currentNode = currentNode.parent;
        }
        //get the path for the waypoints
        Vector3[] waypoints = GetPath(path);
        //get the reverse array of waypoints
        Array.Reverse(waypoints);
        //return the waypoints
        return waypoints;
    }

    //converts the path into a Vec3 array and gets it
    Vector3[] GetPath(List<Node> path)
    {
        //get a new list of waypoints
        List<Vector3> waypoints = new List<Vector3>();
        //get the position of the waypoints on the path in world position
        for (int i = 1; i < path.Count; i++)
        {
                waypoints.Add(path[i].worldPosition);
        }
        //return the waypoints and put them in an array
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