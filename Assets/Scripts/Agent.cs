using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Agent : MonoBehaviour
{
    int ammoCount = 30;
    int health = 100;

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

    public List<Agent> GetAgentsInSight()
    {
        //CHECK FOR AGENTS
        return new List<Agent>();
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
