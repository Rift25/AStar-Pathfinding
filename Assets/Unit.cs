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
    //we use this to increment the V3 path array and in turn find more waypoints to follow
    int targetIndex;

    void Start()
    {
        //when the game start it request a path from the PathRequestManager for the unit to follow
        PathRequestManager.RequestPath(transform.position, target.position, OnPathFound);
    }

    //when it finds a path it stops any "Follow path" coroutines if they were active and starts them again so as to not accidentally break the game
    //by having multiple same coroutines active at the same time
    public void OnPathFound(Vector3[] newPath, bool pathSuccessful)
    {
        if(pathSuccessful)
        {
            path = newPath;
            StopCoroutine("FollowPath");
            StartCoroutine("FollowPath");
        }
    }

    //this makes the unit travel to waypoints and in turn follow the path
    IEnumerator FollowPath()
    {
        Vector3 currentWaypoint = path[0];

        while(true)
        {
            //if units position is the same as the waypoint send the unit to the next waypoint
            if(transform.position == currentWaypoint)
            {
                targetIndex++;
                //if the waypoint goes past or is at the same position as the unit stop the unit
                if(targetIndex >= path.Length)
                {
                    yield break;
                }
                currentWaypoint = path[targetIndex];
            }
            //move the unit towards the current waypoint
            transform.position = Vector3.MoveTowards(transform.position, currentWaypoint, speed * Time.deltaTime);
            yield return null;
        }
    }

    //makes the path visible on the screen by using gizmos to draw small cubes that represent waypoints
    //and thin lines to represent the path
    public void OnDrawGizmos()
    {
        if(path != null)
        {
            for(int i = targetIndex; i < path.Length; i++)
            {
                Gizmos.color = Color.black;
                Gizmos.DrawCube(path[i], Vector3.one);

                if(i == targetIndex)
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
