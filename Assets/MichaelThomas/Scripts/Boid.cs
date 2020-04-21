using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boid : MonoBehaviour
{
    public Vector3 velocity;
    public Vector3 acceleration;
    public Vector3 ahead;
    public Vector3 behind;

    private Vector3 lastPosition = Vector3.zero;

    public Boid Leader
    {
        get
        {
            return World.agentTeams[agentController.teamNumber][0].boid;
        }
    }

    public bool IsLeader
    {
        get
        {
            return Leader == this;
        }
    }

    private Controller agentController;

    private void Start()
    {
        agentController = GetComponent<AgentController>();
        velocity = new Vector3();
    }

    void FixedUpdate()
    {
        if (!IsLeader)
        {
            acceleration = followLeader();
            RaycastCollision();
            acceleration = Vector3.ClampMagnitude(acceleration, agentController.movementSpeed);
            velocity = velocity + acceleration * Time.deltaTime;
            velocity = Vector3.ClampMagnitude(velocity, agentController.movementSpeed);
            transform.position += velocity * Time.deltaTime;

            float angle = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg;
            Quaternion quart = Quaternion.AngleAxis(angle, Vector3.forward);

            transform.rotation = Quaternion.RotateTowards(transform.rotation, quart, Time.deltaTime * BoidConfig.rotationSpeed);
        }
        else
        {
            velocity = transform.position - lastPosition;
        }

        lastPosition = transform.position;
    }

    void RaycastCollision()
    {
        Vector3 leftSensorDir = Quaternion.Euler(90, 180, 0) * new Vector3(1, 1, 0) + velocity;
        Vector3 RightSensorDir = Quaternion.Euler(180, 20, 0) * new Vector3(1, 1, 0) + velocity;
        leftSensorDir.z = 0;
        RightSensorDir.z = 0;
        RaycastHit2D centreHit = Physics2D.Raycast(transform.position, velocity, velocity.magnitude * BoidConfig.maxRayDistance, World.enemyAttackLayerMask);
        RaycastHit2D LeftHit = Physics2D.Raycast(transform.position, leftSensorDir, velocity.magnitude * BoidConfig.maxRayDistance, World.enemyAttackLayerMask);
        RaycastHit2D RightHit = Physics2D.Raycast(transform.position, RightSensorDir, velocity.magnitude * BoidConfig.maxRayDistance, World.enemyAttackLayerMask);
        Vector3 newDir = new Vector3();
        int iterationCount = 6;
        float angleMin = 180 / iterationCount;

        if (centreHit.collider)
        {
            for (int i = 1; i <= iterationCount; i++)
            {
                for (int d = -1; d < 2; d += 2)
                {
                    if(0 > d)
                        newDir = (Quaternion.Euler(0, angleMin * i * d, 0) * velocity);
                    else
                        newDir = (Quaternion.Euler(angleMin * i * d, 0, 0) * velocity);
       
                    if(!Physics2D.Raycast(transform.position, newDir, newDir.magnitude * BoidConfig.maxRayDistance, World.enemyAttackLayerMask))
                    {
                        Debug.DrawRay(transform.position, newDir * BoidConfig.maxRayDistance, Color.green);
                        newDir.z = 0;
                        velocity = newDir.normalized * (BoidConfig.CollisionAvoidancePriority / centreHit.distance);
                        return;
                    }
                    Debug.DrawRay(transform.position, newDir * BoidConfig.maxRayDistance, Color.red);
                }
            }
        }
        else if (LeftHit.collider)
        {
            for (int i = 1; i <= iterationCount; i++)
            {
                for (int d = -1; d < 2; d += 2)
                {
                    if (0 > d)
                        newDir = (Quaternion.Euler(0, angleMin * i * d, 0) * leftSensorDir);
                    else
                        newDir = (Quaternion.Euler(angleMin * i * d, 0, 0) * leftSensorDir);

                    if (!Physics2D.Raycast(transform.position, newDir, newDir.magnitude * BoidConfig.maxRayDistance, World.enemyAttackLayerMask))
                    {
                        Debug.DrawRay(transform.position, newDir * BoidConfig.maxRayDistance, Color.green);
                        newDir.z = 0;
                        velocity = newDir.normalized * (BoidConfig.CollisionAvoidancePriority / LeftHit.distance);
                        return;
                    }
                    Debug.DrawRay(transform.position, newDir * BoidConfig.maxRayDistance, Color.red);
                }
            }
        }
        else if (RightHit.collider)
        {
            for (int i = 1; i <= iterationCount; i++)
            {
                for (int d = -1; d < 2; d += 2)
                {
                    if (0 > d)
                        newDir = (Quaternion.Euler(0, angleMin * i * d, 0) * leftSensorDir);
                    else
                        newDir = (Quaternion.Euler(angleMin * i * d, 0, 0) * leftSensorDir);

                    if (!Physics2D.Raycast(transform.position, newDir, newDir.magnitude * BoidConfig.maxRayDistance, World.enemyAttackLayerMask))
                    {
                        Debug.DrawRay(transform.position, newDir * BoidConfig.maxRayDistance, Color.green);
                        newDir.z = 0;
                        velocity = newDir.normalized * (BoidConfig.CollisionAvoidancePriority / RightHit.distance);
                        return;
                    }
                    Debug.DrawRay(transform.position, newDir * BoidConfig.maxRayDistance, Color.red);
                }
            }
        }
        Debug.DrawRay(transform.position, leftSensorDir * BoidConfig.maxRayDistance, Color.white);
        Debug.DrawRay(transform.position, RightSensorDir * BoidConfig.maxRayDistance, Color.white);
        Debug.DrawRay(transform.position, velocity * BoidConfig.maxRayDistance, Color.white);
    }

    Vector3 followLeader()
    {
        if (Leader)
        {
            Vector3 tv = Leader.velocity;
            Vector3 force = new Vector3();

            //Calculate the ahead point
            Vector3.Normalize(tv);
            tv.Scale(new Vector3(BoidConfig.LEADER_AHEAD_DIST, BoidConfig.LEADER_AHEAD_DIST, 0));
            ahead = Leader.transform.position + tv;
            ahead = Leader.transform.position + tv;

            //Calculate the behind point
            tv.Scale(new Vector3(BoidConfig.LEADER_BEHIND_DIST, BoidConfig.LEADER_BEHIND_DIST, 0));
            behind = Leader.transform.position + tv;

            if (isOnLeaderSight(Leader, ahead))
                force = force + Evade(Leader);

            force = force + Arrival(behind);

            force = force + Separation() * BoidConfig.separationPriority;

            return force;
        }

        return Vector3.zero;
    }

    Vector3 Cohesion()
    {
        Vector3 cohesionVector = new Vector3();
        int countBoids = 0;
        List<Boid> neighbours = GetNeighbours(BoidConfig.cohesionRadius);

        if (neighbours.Count == 0)
            return cohesionVector;

        foreach (Boid boid in neighbours)
        {
            if (isInFOV(boid.transform.position))
            {
                cohesionVector += boid.transform.position;
                countBoids++;
            }
        }

        if (countBoids == 0)
            return cohesionVector;

        cohesionVector /= countBoids;
        cohesionVector = cohesionVector - transform.position;
        cohesionVector = Vector3.Normalize(cohesionVector);
        return cohesionVector;
    }

    Vector3 Alignment()
    {
        Vector3 alignVector = new Vector3();
        List<Boid> boids = GetNeighbours(BoidConfig.alignmentRadius);

        if (boids.Count == 0)
            return alignVector;

        foreach (Boid boid in boids)
        {
            if (isInFOV(boid.transform.position))
                alignVector += boid.velocity;
        }

        return alignVector.normalized;
    }

    Vector3 Separation()
    {
        Vector3 separationVector = new Vector3();
        List<Boid> boids = GetNeighbours(BoidConfig.separationRadius);

        if (boids.Count == 0)
            return separationVector;

        foreach (Boid boid in boids)
        {
            if (isInFOV(boid.transform.position) && !boid.IsLeader)
            {
                Vector3 movingTowards = transform.position - boid.transform.position;
                if (movingTowards.magnitude > 0)
                {
                    separationVector += movingTowards.normalized / movingTowards.magnitude;
                }
            }
        }
        return separationVector.normalized;
    }

    bool isOnLeaderSight(Boid leader, Vector3 leaderAhead)
    {
        return Vector3.Distance(leaderAhead, transform.position) <= BoidConfig.LeaderSightRadius || Vector3.Distance(leader.transform.position, transform.position) <= BoidConfig.LeaderSightRadius;
    }

    Vector3 Evade(Boid boid)
    {
        Vector3 distance = boid.transform.position - transform.position;
        float UpdatesAhead = distance.magnitude / agentController.movementSpeed;
        Vector3 futurePosition = boid.transform.position + boid.velocity * UpdatesAhead;
        return RunAway(futurePosition);
    }

    Vector3 Arrival(Vector3 target)
    {
        Vector3 desiredVelocity = target - transform.position;
        float distance = desiredVelocity.magnitude;

        if (distance < BoidConfig.slowingRadius)
        {
            desiredVelocity = desiredVelocity.normalized * agentController.movementSpeed * (distance / BoidConfig.slowingRadius);
        }
        else
        {
            desiredVelocity = desiredVelocity.normalized * agentController.movementSpeed;
        }

        return desiredVelocity - velocity;
    }

    Vector3 RunAway(Vector3 target)
    {
        Vector3 neededVelocity = (transform.position - target).normalized * agentController.movementSpeed;
        return neededVelocity - velocity;
    }

    bool isInFOV(Vector3 vec)
    {
        return Vector3.Angle(velocity, vec - transform.position) <= BoidConfig.maxFOV;
    }

    public List<Boid> GetNeighbours(float radius)
    {
        List<Controller> team = World.agentTeams[agentController.teamNumber];

        List<Boid> neighboursFound = new List<Boid>();

        foreach (Controller teamController in team)
        {
            if (teamController == agentController)
                continue;

            if (Vector3.Distance(transform.position, teamController.transform.position) <= radius)
            {
                neighboursFound.Add(teamController.boid);
            }
        }

        return neighboursFound;
    }
}
