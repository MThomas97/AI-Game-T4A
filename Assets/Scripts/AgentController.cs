using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PathFinding))]
[RequireComponent(typeof(AgentBehaviourTree))]

public class AgentController : MonoBehaviour
{
    int ammoCount = 30;
    int health = 100;

    public float rotationSpeed = 80.0f;
    public int teamNumber = -1;
    public float fieldOfView = 90.0f;


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
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
