using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    //the target the unit is following, in this case its us, the player
    public Transform target;
    //speed at which it travels
    float speed = 10f;
    //the path the unit is going to take
    Vector3[] path;
    //we use this to increment the vector 3 path array and in turn find more waypoints to follow
    int nextWaypoint;

    void Start()
    {
        //when the game start it request a path from the PathRequestManager for the unit to follow
        PathRequestManager.RequestPath(transform.position, target.position, OnPathFound);
    }

    //when it finds a path it stops any "Follow path" coroutines if they were active and starts them again so as to not accidentally break the game
    //by having multiple same coroutines active at the same time
    public void OnPathFound(Vector3[] newPath, bool pathSuccessful)
    {
        //if the path has been created
        if(pathSuccessful)
        {
            //create a new vector 3 array for the path
            path = newPath;
            //stop the coroutine if it was active by chance
            StopCoroutine("FollowPath");
            //start the coroutine
            StartCoroutine("FollowPath");
        }
    }

    //this makes the unit travel to waypoints and in turn follow the path
    IEnumerator FollowPath()
    {
        //make the current waypoint the starting waypoint
        Vector3 currentWaypoint = path[0];
        //while the unit is following the path
        while(true)
        {
            //if units position is the same as the waypoint send the unit to the next waypoint
            if(transform.position == currentWaypoint)
            {
                //add the next waypoint
                nextWaypoint++;
                //if the waypoint goes past the length of the path or is at the end of the path
                if(nextWaypoint >= path.Length)
                {
                    //stop the unit
                    yield break;
                }
                //make the current waypoint be the next waypoint on the path
                currentWaypoint = path[nextWaypoint];
            }
            //move the unit towards the current waypoint
            transform.position = Vector3.MoveTowards(transform.position, currentWaypoint, speed * Time.deltaTime);
            yield return null;
        }
    }

    //makes the path visible on the screen by using gizmos to draw small cubes that represent the path
    public void OnDrawGizmos()
    {
        //if the path isn't over
        if(path != null)
        {
            //get the full length of the path
            for(int i = nextWaypoint; i < path.Length; i++)
            {
                Gizmos.color = Color.black;
                Gizmos.DrawCube(path[i], Vector3.one);

                if(i == nextWaypoint)
                {
                    Gizmos.DrawLine(transform.position, path[i]);
                }
                else
                {
                    Gizmos.DrawLine(path[i - 1], path[i]);
                }
            }
        }
    }
}
