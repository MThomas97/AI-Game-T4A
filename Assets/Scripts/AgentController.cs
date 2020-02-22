using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(AgentBehaviour))]

public class AgentController : MonoBehaviour
{
    public int ammoCount = 30;
    int health = 100;

    public float rotationSpeed = 80.0f;
    public int teamNumber = -1;
    public float fieldOfView = 90.0f;
    public float attackRange = 20.0f;
    public float reactionTime = 5.0f;
    public float movementSpeed = 1.0f;
    //Debugging
    Text onScreenDebug;
    Node pathDebug = null;

    struct AgentPositionPreviouslySeen
    {
        Vector3 agentPosition;
        float timeSawAt;
    }

    public bool HasAmmo()
    {
        return ammoCount > 0;
    }

    public int GetHealth()
    {
        return health; 
    }

    // Start is called before the first frame update
    void Start()
    {
        if (OnScreenDebug.pathfindingDebugs.Length > teamNumber)
        {
            onScreenDebug = OnScreenDebug.pathfindingDebugs[teamNumber];
            onScreenDebug.color = World.playerColours[teamNumber];
        }
    }

    public void UpdatePathfindingDebug(Node debugPath, string debugString)
    {
        pathDebug = debugPath;

        if (debugString.Length > 0)
        {
            onScreenDebug.text = debugString;
        }
    }

    void OnDrawGizmos()
    {
        if (teamNumber > -1)
        {
            Gizmos.color = World.playerColours[teamNumber];

            //Avoid debug overlaps.
            Vector3 offset = new Vector3(0.1f, 0.1f, 0.0f) * teamNumber + new Vector3(-0.2f, -0.2f, -0.0f);

            if (pathDebug != null)
            {
                Node currentNode = pathDebug;

                Vector3 currentPos = transform.position + offset;
                Gizmos.DrawWireCube(currentPos, new Vector3(1.0f, 1.0f));
                Gizmos.DrawLine(currentPos, new Vector3(currentNode.pos.x, currentNode.pos.y, 0) + offset);

                while (currentNode != null)
                {
                    currentPos = new Vector3(currentNode.pos.x, currentNode.pos.y, 0) + offset;

                    Gizmos.DrawWireCube(currentPos, new Vector3(1.0f, 1.0f));

                    if (currentNode.parent != null)
                    {
                        Vector3 parentPos = new Vector3(currentNode.parent.pos.x, currentNode.parent.pos.y, 0) + offset;
                        Gizmos.DrawLine(currentPos, parentPos);
                    }

                    currentNode = currentNode.parent;
                }
            }
        }
    }
}
