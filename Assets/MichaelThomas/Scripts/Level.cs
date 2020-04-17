using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour
{
    public Transform agentPrefab;
    public int numberOfAgents;
    public List<Agent> agents;
    public float bounds;
    public float spawnRadius;

    // Start is called before the first frame update
    void Start()
    {
        agents = new List<Agent>();

        Spawn(agentPrefab, numberOfAgents);

        agents.AddRange(FindObjectsOfType<Agent>());
    }

    void Spawn(Transform prefab, int Count)
    {
        for(int i = 0; i < Count; i++)
        {
            Instantiate(prefab, new Vector3(Random.Range(-spawnRadius, spawnRadius), Random.Range(-spawnRadius, spawnRadius), 0), Quaternion.identity);
        }
    }

    public List<Agent> GetNeighbours(Agent agent, float radius)
    {
        List<Agent> neighboursFound = new List<Agent>();
        
        foreach(Agent otherAgent in agents)
        {
            if (otherAgent == agent)
                continue;

            if(Vector3.Distance(agent.position, otherAgent.position) <= radius)
            {
                neighboursFound.Add(otherAgent);
            }
        }

        return neighboursFound;
    }
}
