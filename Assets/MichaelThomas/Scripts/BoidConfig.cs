using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoidConfig : MonoBehaviour
{
    public float maxFOV;
    public float maxAcceleration;
    public float maxVelocity;

    //Seperation Variables
    public float separationRadius;
    public float separationPriority;

    //Alignment Variables
    public float alignmentRadius;
    public float alignmentPriority;

    //Cohesion Variables
    public float cohesionRadius;
    public float cohesionPriority;

    //Collision Avoidance Variables
    public float CollisionAvoidancePriority;

    //Raycasting
    public float maxRayDistance;

    //Leader Following
    public float LEADER_BEHIND_DIST;
    public float LEADER_AHEAD_DIST;
    public float LeaderSightRadius;
    public float slowingRadius;
}
