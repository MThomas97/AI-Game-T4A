using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentConfig : MonoBehaviour
{
    public float maxFOV = 180;
    public float maxAcceleration;
    public float maxVelocity;

    //Wander Variables
    public float wanderJitter;
    public float wanderRadius;
    public float wanderDistance;
    public float wanderPriority;

    //Seperation Variables
    public float separationRadius;
    public float separationPriority;

    //Alignment Variables
    public float alignmentRadius;
    public float alignmentPriority;

    //Cohesion Variables
    public float cohesionRadius;
    public float cohesionPriority;

    //Avoidance Variables
    public float avoidanceRadius;
    public float avoidancePriority;

    //Raycasting
    public float maxRayDistance = 0.5f;

    //Leader Following
    public float LEADER_BEHIND_DIST = 0.5f;
    public float LEADER_AHEAD_DIST = 0.5f;
    public float LeaderSightRadius = 0.5f;
    public float slowingRadius = 30.0f;
}
