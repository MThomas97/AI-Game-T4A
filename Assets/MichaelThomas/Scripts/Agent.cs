using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Agent : MonoBehaviour
{
    public Vector3 position;
    public Quaternion rotation;
    public Vector3 velocity;
    public Vector3 acceleration;

    public Level level;
    public AgentConfig config;

    private Vector3 wanderTarget;

    private void Start()
    {
        level = FindObjectOfType<Level>();
        config = FindObjectOfType<AgentConfig>();

        position = transform.position;
        rotation = transform.rotation;
        velocity = new Vector3(Random.Range(-3, 3), Random.Range(-3, 3), 0);
    }

    void FixedUpdate()
    {
        RaycastCollison();
        acceleration = Combine();
        acceleration = Vector3.ClampMagnitude(acceleration, config.maxAcceleration);
        velocity = velocity + acceleration * Time.deltaTime;
        velocity = Vector3.ClampMagnitude(velocity, config.maxVelocity);
        position = position + velocity * Time.deltaTime;
        //Add in rotation to movement
        //float angle = position.normalized.z * Mathf.Rad2Deg;
        //rotation.z = rotation.z + angle * Time.deltaTime + 100;
        WrapAround(ref position, -level.bounds, level.bounds);
        transform.position = position;
        transform.rotation = rotation;
    }

    void RaycastCollison()
    {
        Ray ray = new Ray(transform.position, velocity);
        //RaycastHit hit;

        Debug.DrawLine(transform.position, transform.position + velocity * config.maxRayDistance, Color.red);
        

        //float furthestUnobstructedDst = 0;

        //if (Physics.SphereCast(transform.position,5, velocity, out hit, 20))
        //{
        //    if(hit.distance > furthestUnobstructedDst)
        //    {

        //        furthestUnobstructedDst = hit.distance;
        //    }
        //    //int numPoints = 10;
        //    //for(int i = 0; i < numPoints; i++)
        //    //{
        //    //    float dst = i / (numPoints - 1f);
        //    //    float angle = 2.0f * Mathf.PI * 0.10f * i;

        //    //    float x = dst * 
        //    //}
        //    // Debug.Log("You hit a ray");
        //}
    }

    protected Vector3 Wander()
    {
        float jitter = config.wanderJitter * Time.deltaTime;
        wanderTarget += new Vector3(RandomBinomial() * jitter, RandomBinomial() * jitter, 0);
        wanderTarget = wanderTarget.normalized;
        wanderTarget *= config.wanderRadius;
        Vector3 targetInLocalSpace = wanderTarget + new Vector3(config.wanderDistance, config.wanderDistance, 0);
        Vector3 targetInWorldSpace = transform.TransformPoint(targetInLocalSpace);
        targetInWorldSpace -= this.position;
        return targetInWorldSpace.normalized;
    }

    Vector3 Cohesion()
    {
        Vector3 cohesionVector = new Vector3();
        int countAgents = 0;
        List<Agent> neighbours = level.GetNeighbours(this, config.cohesionRadius);

        if (neighbours.Count == 0)
            return cohesionVector;

        foreach(Agent agent in neighbours)
        {
            if(isInFOV(agent.position))
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

        foreach(Agent agent in agents)
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

        foreach(Agent agent in agents)
        {
            if(isInFOV(agent.position))
            {
                Vector3 movingTowards = this.position - agent.position;
                if(movingTowards.magnitude > 0)
                {
                    separationVector += movingTowards.normalized / movingTowards.magnitude;
                }
            }
        }
        return separationVector.normalized;
    }

    Vector3 Avoidance()
    {
        Vector3 avoidVector = new Vector3();
        List<Enemy> enemyList = level.GetEnemies(this, config.avoidanceRadius);

        if (enemyList.Count == 0)
            return avoidVector;

        foreach(Enemy enemy in enemyList)
        {
            avoidVector += RunAway(enemy.position);
        }

        return avoidVector.normalized;
    }

    Vector3 RunAway(Vector3 target)
    {
        Vector3 neededVelocity = (position = target).normalized * config.maxVelocity;
        return neededVelocity - velocity;
    }

    virtual protected Vector3 Combine()
    {
        Vector3 finalVec = config.cohesionPriority * Cohesion() + config.wanderPriority * Wander() + config.alignmentPriority * Alignment() + config.separationPriority * Separation() + config.avoidancePriority * Avoidance();
        return finalVec;
    }

    void WrapAround(ref Vector3 vector, float min, float max)
    {
        vector.x = WrapAroundFloat(vector.x, min, max);
        vector.y = WrapAroundFloat(vector.y, min, max);
        vector.z = WrapAroundFloat(vector.z, min, max);
    }

    float WrapAroundFloat(float value, float min, float max)
    {
        if (value > max)
            value = min;
        else if (value < min)
            value = max;
        return value;
    }

    float RandomBinomial()
    {
        return Random.Range(0f, 1f) - Random.Range(0f, 1f);
    }

    bool isInFOV(Vector3 vec)
    {
        return Vector3.Angle(this.velocity, vec - this.position) <= config.maxFOV;
    }
}
