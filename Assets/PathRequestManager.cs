using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PathRequestManager : MonoBehaviour
{
    //create a queue for the path request in case
    //there are more than one being requested at the same time
    Queue<PathRequest> pathRequestQueue = new Queue<PathRequest>();
    PathRequest currentPathRequest;

    static PathRequestManager instance;
    Pathfinding pathfinding;
    //a bool that checks if the path is being processed or not
    bool isProcessingPath;

    void Awake()
    {
        //create the path manager
        instance = this;
        //get the pathfinding component
        pathfinding = GetComponent<Pathfinding>();
    }
    //request the path for the unit to take
    public static void RequestPath(Vector3 pathStart, Vector3 pathEnd, Action<Vector3[], bool> callback)
    {
        //create a new path request
        PathRequest newRequest = new PathRequest(pathStart, pathEnd, callback);
        //add path request into a queue just incase there are more than one being requested at the same time
        instance.pathRequestQueue.Enqueue(newRequest);
        //try to process another queue
        instance.TryProcessNext();
    }

    //remove the last path request that was in the queue and start to find its path
    void TryProcessNext()
    {
        //if the path isn't currently being processed and the queue size is bigger than zero
        if(!isProcessingPath && pathRequestQueue.Count > 0)
        {
            //remove the current path request from the queue
            currentPathRequest = pathRequestQueue.Dequeue();
            //tell the game that a path is currently being processed
            isProcessingPath = true;
            //and start finding its path on the grid
            pathfinding.StartFindPath(currentPathRequest.pathStart, currentPathRequest.pathEnd);
        }
    }
    //the path has finished being created and start creating another path
    public void FinishedProcessPath(Vector3[] path, bool success)
    {
        //tell the PathRequest that a path has been created
        currentPathRequest.callback(path, success);
        //tell the game that a path isn't currently being processed
        isProcessingPath = false;
        //begin processing another path
        TryProcessNext();
    }
    //path request struct
    struct PathRequest
    {
        //start of the path
        public Vector3 pathStart;
        //end of the path
        public Vector3 pathEnd;
        //an action that tells the path request if a path is created or not
        public Action<Vector3[], bool> callback;
        //a constructor for the struct
        public PathRequest(Vector3 _start, Vector3 _end, Action<Vector3[], bool> _callback)
        {
            pathStart = _start;
            pathEnd = _end;
            callback = _callback;
        }
    }
}
