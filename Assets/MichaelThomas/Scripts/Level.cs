using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour
{
    public Transform agentPrefab;
    public Transform enemyPrefab;
    public int numberOfAgents;
    public int numberOfEnemies;
    public List<Agent> agents;
    public List<Enemy> enemies;
    public float bounds;
    public float spawnRadius;

    // Start is called before the first frame update
    void Start()
    {
        agents = new List<Agent>();
        enemies = new List<Enemy>();

        Spawn(agentPrefab, numberOfAgents);
        Spawn(enemyPrefab, numberOfEnemies);

        agents.AddRange(FindObjectsOfType<Agent>());
        enemies.AddRange(FindObjectsOfType<Enemy>());
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

    public List<Enemy> GetEnemies(Agent agent, float radius)
    {
        List<Enemy> returnEnemies = new List<Enemy>();

        foreach(Enemy enemy in enemies)
        {
            if(Vector3.Distance(agent.position, enemy.position) <= radius)
            {
                returnEnemies.Add(enemy);
            }
        }
        return returnEnemies;
    }
}
