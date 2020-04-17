using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Agent : MonoBehaviour
{
    public Vector3 position;
    public Quaternion rotation;
    public Vector3 velocity;
    public Vector3 acceleration;
    public Vector3 ahead;
    public Vector3 behind;
    public GameObject leaderGameObject;

    public Level level;
    public AgentConfig config;

    private Vector3 wanderTarget;
    public bool isLeader = false;
    private Agent leader;

    private void Start()
    {

        level = FindObjectOfType<Level>();
        config = FindObjectOfType<AgentConfig>();

        if (leaderGameObject == null && this.name != "Leader")
        {
            leaderGameObject = GameObject.Find("Leader");
            leader = leaderGameObject.GetComponent<Agent>();
            leader.isLeader = true;
        }
        
        position = transform.position;
        rotation = transform.rotation;
        velocity = new Vector3(Random.Range(-3, 3), Random.Range(-3, 3), 0);
    }

    void FixedUpdate()
    {
        if (isLeader)
        {
            //RaycastCollison();
            //acceleration = CombineWander();
            acceleration = Vector3.ClampMagnitude(acceleration, config.maxAcceleration);
            velocity = new Vector3(0, 3, 0);
            velocity = velocity + acceleration * Time.deltaTime;
            velocity = Vector3.ClampMagnitude(velocity, config.maxVelocity);
            position = position + velocity * Time.deltaTime;
            ////WrapAround(ref position, -level.bounds, level.bounds);
            transform.position = position;
            transform.rotation = rotation;
        }
        else
        {
            acceleration.x = RaycastCollison().x * config.CollisionAvoidancePriority;
            acceleration.y = RaycastCollison().y * config.CollisionAvoidancePriority;
            acceleration += followLeader();
            acceleration = Vector3.ClampMagnitude(acceleration, config.maxAcceleration);
            velocity = velocity + acceleration * Time.deltaTime;
            velocity = Vector3.ClampMagnitude(velocity, config.maxVelocity);
            position = position + velocity * Time.deltaTime;
            //WrapAround(ref position, -level.bounds, level.bounds);
            transform.position = position;
            transform.rotation = rotation;

        }

    }

    Vector3 RaycastCollison()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, velocity, velocity.magnitude * config.maxRayDistance);
        Vector3 newDir = new Vector3();

        int iterationCount = 20;
        float angleMin = 180 / iterationCount;

        if (hit.collider != null)
        {
            Debug.DrawRay(transform.position, velocity * config.maxRayDistance, Color.red);
            for (int i = 0; i < iterationCount; i++)
            {
                for (int d = -1; d < 2; d += 2)
                {
                    newDir = (Quaternion.Euler(angleMin * i * d, 0, 0) * velocity);
                    if (newDir.x >= hit.collider.transform.position.x && newDir.x <= hit.collider.transform.position.x + hit.collider.transform.localScale.x && newDir.y >= hit.collider.transform.position.y && newDir.y <= hit.collider.transform.position.y + hit.collider.transform.localScale.y)
                    {//Figure a way to exit early if a good direction is found
                        Debug.Log("test");
                        return newDir.normalized;

                    } 
                }
            }
        }
        
        Debug.DrawRay(transform.position, velocity * config.maxRayDistance, Color.green);
        return newDir.normalized;
    }

    Vector3 followLeader()
    {
        Vector3 tv = leader.velocity;
        Vector3 force = new Vector3();

        //Calculate the ahead point
        Vector3.Normalize(tv);
        tv.Scale(new Vector3(config.LEADER_AHEAD_DIST, config.LEADER_AHEAD_DIST, 0));
        ahead = leader.position + tv;

        //Calculate the behind point
        tv.Scale(new Vector3(config.LEADER_BEHIND_DIST, config.LEADER_BEHIND_DIST, 0));
        behind = leader.position + tv;

        if (isOnLeaderSight(leader, ahead))
            force = force + Evade(leader);

        force = force + Arrival(behind);

        force = force + Separation() * config.separationPriority;

        return force;
    }

    Vector3 Cohesion()
    {
        Vector3 cohesionVector = new Vector3();
        int countAgents = 0;
        List<Agent> neighbours = level.GetNeighbours(this, config.cohesionRadius);

        if (neighbours.Count == 0)
            return cohesionVector;

        foreach (Agent agent in neighbours)
        {
            if (isInFOV(agent.position))
            {
                cohesionVector += agent.position;
                countAgents++;
            }
        }

        if (countAgents == 0)
            return cohesionVector;

        cohesionVector /= countAgents;
        cohesionVector = cohesionVector - this.position;
        cohesionVector = Vector3.Normalize(cohesionVector);
        return cohesionVector;
    }

    Vector3 Alignment()
    {
        Vector3 alignVector = new Vector3();
        List<Agent> agents = level.GetNeighbours(this, config.alignmentRadius);

        if (agents.Count == 0)
            return alignVector;

        foreach (Agent agent in agents)
        {
            if (isInFOV(agent.position))
                alignVector += agent.velocity;
        }

        return alignVector.normalized;
    }

    Vector3 Separation()
    {
        Vector3 separationVector = new Vector3();
        List<Agent> agents = level.GetNeighbours(this, config.separationRadius);

        if (agents.Count == 0)
            return separationVector;

        foreach (Agent agent in agents)
        {
            if (isInFOV(agent.position) && !agent.isLeader)
            {
                Vector3 movingTowards = this.position - agent.position;
                if (movingTowards.magnitude > 0)
                {
                    separationVector += movingTowards.normalized / movingTowards.magnitude;
                }
            }
        }
        return separationVector.normalized;
    }

    bool isOnLeaderSight(Agent leader, Vector3 leaderAhead)
    {
        return Vector3.Distance(leaderAhead, transform.position) <= config.LeaderSightRadius || Vector3.Distance(leader.position, transform.position) <= config.LeaderSightRadius;
    }

    Vector3 Evade(Agent agent)
    {
        Vector3 distance = agent.position - position;
        float UpdatesAhead = distance.magnitude / config.maxVelocity;
        Vector3 futurePosition = agent.position + agent.velocity * UpdatesAhead;
        return RunAway(futurePosition);
    }

    Vector3 Arrival(Vector3 target)
    {
        Vector3 desiredVelocity = target - position;
        float distance = desiredVelocity.magnitude;

        if (distance < config.slowingRadius)
        {
            desiredVelocity = desiredVelocity.normalized * config.maxVelocity * (distance / config.slowingRadius);
        }
        else
        {
            desiredVelocity = desiredVelocity.normalized * config.maxVelocity;
        }

        return desiredVelocity - velocity;
    }

    Vector3 RunAway(Vector3 target)
    {
        Vector3 neededVelocity = (position - target).normalized * config.maxVelocity;
        return neededVelocity - velocity;
    }

    bool isInFOV(Vector3 vec)
    {
        return Vector3.Angle(this.velocity, vec - this.position) <= config.maxFOV;
    }
}
