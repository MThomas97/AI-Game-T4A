using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public Transform target;

    private float speed = 5;
    private Vector2Int[] path;
    private Vector2Int seeker;
    private Vector2Int targetPos;
    private int targetIndex;

    private void Start()
    {
        seeker = new Vector2Int(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y));
        targetPos = new Vector2Int(Mathf.RoundToInt(target.position.x), Mathf.RoundToInt(target.position.y));
        PathRequestManager.RequestPath(seeker, targetPos, OnPathFound);
    }

    public void OnPathFound(Vector2Int[] newPath, bool pathSuccessful)
    {
        if(pathSuccessful)
        {
            path = newPath;
            StopCoroutine("FollowPath");
            StartCoroutine("FollowPath");
        }
    }

    IEnumerator FollowPath()
    {
        Vector2Int currentWaypoint = path[0];

        while(true)
        {
            if(seeker == currentWaypoint)
            {
                targetIndex++;
                if(targetIndex >= path.Length)
                {
                    yield break;
                }
                currentWaypoint = path[targetIndex];
            }
            transform.position = Vector3.MoveTowards(new Vector3(seeker.x, seeker.y, 0), new Vector3(currentWaypoint.x, currentWaypoint.y, 0), speed * Time.deltaTime);
            yield return null;
        }
    }

    public void OnDrawGizmos()
    {
        if(path != null)
        {
            for (int i = targetIndex; i < path.Length; i++)
            {
                Gizmos.color = Color.black;
                Gizmos.DrawCube(new Vector3(path[i].x, path[i].y, 0), Vector3.one);

                if(i == targetIndex)
                {
                    Gizmos.DrawLine(transform.position, new Vector3(path[i].x, path[i].y, 0));
                }
                else
                {
                    Gizmos.DrawLine(new Vector3(path[i - 1].x, path[i - 1].y, 0), new Vector3(path[i].x, path[i].y, 0));
                }
            }
        }
    }
}
