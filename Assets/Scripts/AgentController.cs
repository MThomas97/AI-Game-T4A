using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(AgentBehaviourTree))]

public class AgentController : MonoBehaviour
{
    Node path = null;
    public int ammoCount = 30;
    int health = 100;

    public float rotationSpeed = 80.0f;
    public int teamNumber = -1;
    public float fieldOfView = 90.0f;
    public float attackRange = 20.0f;

    //Debugging
    Text onScreenDebug;

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

    // Update is called once per frame
    void Update()
    {
        string debugOut = "";
        path = PathFinding.CalculatePath(transform.position, World.ammoTiles[0].mTileObject.transform.position, out debugOut);
        onScreenDebug.text = debugOut;
    }

    void OnDrawGizmos()
    {
        if(teamNumber > -1)
            Gizmos.color = World.playerColours[teamNumber];

        if (path != null)
        {
            Node currentNode = path;

            while (currentNode != null)
            {
                Vector3 currentPos = new Vector3(currentNode.pos.x, currentNode.pos.y, 0);

                Gizmos.DrawWireCube(currentPos, new Vector3(1.0f, 1.0f));

                if (currentNode.parent != null)
                {
                    Gizmos.DrawLine(currentPos, new Vector3(currentNode.parent.pos.x, currentNode.parent.pos.y, 0));
                }

                currentNode = currentNode.parent;
            }
        }
    }
}
