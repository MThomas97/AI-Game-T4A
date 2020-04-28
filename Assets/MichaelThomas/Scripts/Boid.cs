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
        agentController = GetComponent<Controller>();
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
    { //If the leader gets killed as a boid hits a collision skip till a new leader is selected
        if (Leader != null)
        {
            Vector3 leftSensorDir = Quaternion.Euler(90, 180, 0) * new Vector3(1, 1, 0) + velocity;
            Vector3 RightSensorDir = Quaternion.Euler(180, 20, 0) * new Vector3(1, 1, 0) + velocity;
            leftSensorDir.z = 0;
            RightSensorDir.z = 0;
            //Left, centre and right raycasts 
            RaycastHit2D centreHit = Physics2D.Raycast(transform.position, velocity, BoidConfig.maxRayDistance, World.enemyAttackLayerMask);
            RaycastHit2D LeftHit = Physics2D.Raycast(transform.position, leftSensorDir, BoidConfig.maxRayDistance, World.enemyAttackLayerMask);
            RaycastHit2D RightHit = Physics2D.Raycast(transform.position, RightSensorDir, BoidConfig.maxRayDistance, World.enemyAttackLayerMask);
            Vector3 newDir = Vector3.zero;
            Vector3 bestDir = Vector3.zero;
            float ClosestDistance = 1000.0f;
            int iterationCount = 6;
            float angleMin = 120 / iterationCount;

            if (centreHit.collider)
            {
                Vector2 LeaderPosOnCollision = Leader.transform.position;
                
                for (int i = 1; i <= iterationCount; i++)
                {
                    for (int d = -1; d < 2; d += 2)
                    {
                        //makes new direction with degree of rotation
                        if (0 > d)
                            newDir = (Quaternion.Euler(0, angleMin * i * d, 0) * velocity);
                        else
                            newDir = (Quaternion.Euler(angleMin * i * d, 0, 0) * velocity);

                        newDir.z = 0;
                        //Check if the newDir distance to leader is less than the current bestDir
                        if (Vector2.Distance(transform.position + newDir, LeaderPosOnCollision) >= ClosestDistance)
                            continue;
                        //If the new direction isn't hiting a wall make this the best direction
                        if (!Physics2D.Raycast(transform.position, newDir, BoidConfig.maxRayDistance, World.enemyAttackLayerMask))
                        {
                            ClosestDistance = Vector2.Distance(transform.position + newDir, LeaderPosOnCollision);
                            bestDir = newDir;
                        }
                        Debug.DrawRay(transform.position, newDir.normalized * BoidConfig.maxRayDistance, Color.red);
                    }
                }
                //If the boid is touching the target 
                if (centreHit.distance == 0)
                {
                    Debug.DrawRay(transform.position, newDir.normalized * BoidConfig.maxRayDistance, Color.green);
                    velocity = newDir.normalized * (BoidConfig.cohesionPriority / 1);
                    return;
                }
                    
                Debug.DrawRay(transform.position, bestDir.normalized * BoidConfig.maxRayDistance, Color.green);
                velocity = bestDir.normalized * (BoidConfig.CollisionAvoidancePriority / centreHit.distance);
                return;
            }
            else if (LeftHit.collider)
            {
                Vector2 LeaderPosOnCollision = Leader.transform.position;
                for (int i = 1; i <= iterationCount; i++)
                {
                    for (int d = -1; d < 2; d += 2)
                    {
                        //makes new direction with degree of rotation
                        if (0 > d)
                            newDir = (Quaternion.Euler(0, angleMin * i * d, 0) * leftSensorDir);
                        else
                            newDir = (Quaternion.Euler(angleMin * i * d, 0, 0) * leftSensorDir);

                        newDir.z = 0;
                        //Check if the newDir distance to leader is less than the current bestDir
                        if (Vector2.Distance(transform.position + newDir, LeaderPosOnCollision) >= ClosestDistance)
                            continue;

                        //If the new direction isn't hiting a wall make this the best direction
                        if (!Physics2D.Raycast(transform.position, newDir, BoidConfig.maxRayDistance, World.enemyAttackLayerMask))
                        {
                            ClosestDistance = Vector2.Distance(transform.position + newDir, LeaderPosOnCollision);
                            bestDir = newDir;
                        }
                        Debug.DrawRay(transform.position, newDir.normalized * BoidConfig.maxRayDistance, Color.red);
                    }
                }
                //If the boid is touching the target 
                if (LeftHit.distance == 0)
                {
                    Debug.DrawRay(transform.position, newDir.normalized * BoidConfig.maxRayDistance, Color.green);
                    velocity = newDir.normalized * (BoidConfig.cohesionPriority / 1);
                    return;
                }

                Debug.DrawRay(transform.position, bestDir.normalized * BoidConfig.maxRayDistance, Color.green);
                velocity = bestDir.normalized * (BoidConfig.CollisionAvoidancePriority / LeftHit.distance);
                return;
            }
            else if (RightHit.collider)
            {
                Vector2 LeaderPosOnCollision = Leader.transform.position;
                for (int i = 1; i <= iterationCount; i++)
                {
                    for (int d = -1; d < 2; d += 2)
                    {
                        //makes new direction with degree of rotation
                        if (0 > d)
                            newDir = (Quaternion.Euler(0, angleMin * i * d, 0) * leftSensorDir);
                        else
                            newDir = (Quaternion.Euler(angleMin * i * d, 0, 0) * leftSensorDir);

                        newDir.z = 0;
                        //Check if the newDir distance to leader is less than the current bestDir
                        if (Vector2.Distance(transform.position + newDir, LeaderPosOnCollision) >= ClosestDistance)
                            continue;

                        //If the new direction isn't hiting a wall make this the best direction
                        if (!Physics2D.Raycast(transform.position, newDir, BoidConfig.maxRayDistance, World.enemyAttackLayerMask))
                        {
                            ClosestDistance = Vector2.Distance(transform.position + newDir, LeaderPosOnCollision);
                            bestDir = newDir;
                        }
                        Debug.DrawRay(transform.position, newDir.normalized * BoidConfig.maxRayDistance, Color.red);
                    }
                }
                //If the boid is touching the target 
                if (RightHit.distance == 0)
                {
                    Debug.DrawRay(transform.position, newDir.normalized * BoidConfig.maxRayDistance, Color.green);
                    velocity = newDir.normalized * (BoidConfig.cohesionPriority / 1);
                }

                Debug.DrawRay(transform.position, bestDir.normalized * BoidConfig.maxRayDistance, Color.green);
                velocity = bestDir.normalized * (BoidConfig.CollisionAvoidancePriority / RightHit.distance);
                return;
            }
            Debug.DrawRay(transform.position, leftSensorDir.normalized * BoidConfig.maxRayDistance, Color.white);
            Debug.DrawRay(transform.position, RightSensorDir.normalized * BoidConfig.maxRayDistance, Color.white);
            Debug.DrawRay(transform.position, velocity.normalized * BoidConfig.maxRayDistance, Color.white);
        }
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

            //if the boid is in the sight of the leader then evade the leader
            if (isOnLeaderSight(Leader, ahead))
                force = force + Evade(Leader);

            force = force + Arrival(behind);

            force = force + Separation() * BoidConfig.separationPriority;

            return force;
        }

        return Vector3.zero;
    }

    Vector3 Cohesion()
    { //Get all the neighbouring boids and get the average position of all the boids so they stay near that position 
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
    {// Aligns all the boids in the same direction by getting all the neighbouring boids
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
    { //Move away from other boids by taking away the other boid position
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
    { //Evade away from boid if you get too close
        Vector3 distance = boid.transform.position - transform.position;
        float UpdatesAhead = distance.magnitude / agentController.movementSpeed * BoidConfig.maxMovementSpeed;
        Vector3 futurePosition = boid.transform.position + boid.velocity * UpdatesAhead;
        return RunAway(futurePosition);
    }

    Vector3 Arrival(Vector3 target)
    { //Go towards target and slow down once you get in the radius of the target eventually 
        Vector3 desiredVelocity = target - transform.position;
        float distance = desiredVelocity.magnitude;

        if (distance < BoidConfig.slowingRadius)
        {
            desiredVelocity = desiredVelocity.normalized * agentController.movementSpeed * (distance / BoidConfig.slowingRadius);
        }
        else
        {
            desiredVelocity = desiredVelocity.normalized * agentController.movementSpeed * BoidConfig.maxMovementSpeed;
        }

        return desiredVelocity - velocity;
    }

    Vector3 RunAway(Vector3 target)
    { //Runs away from the target by subtracting the target position * maxMovementSpeed
        Vector3 neededVelocity = (transform.position - target).normalized * agentController.movementSpeed * BoidConfig.maxMovementSpeed;
        return neededVelocity - velocity;
    }

    bool isInFOV(Vector3 vec)
    { //Checks the angle between the two velocities and if its within the maxFOV
        return Vector3.Angle(velocity, vec - transform.position) <= BoidConfig.maxFOV;
    }

    public List<Boid> GetNeighbours(float radius)
    { //Gets all the neighbouring boids that are with the radius and adds them to a list
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
