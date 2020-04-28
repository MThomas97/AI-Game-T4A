using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BoidConfig
{
    //Boid Variables
    static public float maxFOV = 180;
    static public float maxMovementSpeed = 2.0f;
    static public float rotationSpeed = 90;

    //Seperation Variables
    static public float separationRadius = 2f;
    static public float separationPriority = 90;

    //Alignment Variables
    static public float alignmentRadius = 10;
    static public float alignmentPriority = 90;

    //Cohesion Variables
    static public float cohesionRadius = 10;
    static public float cohesionPriority = 1;

    //Collision Avoidance Variables
    static public float CollisionAvoidancePriority = 3000;

    //Raycasting
    static public float maxRayDistance = 0.75f;

    //Leader Following
    static public float LEADER_BEHIND_DIST = -1.5f;
    static public float LEADER_AHEAD_DIST = 1.0f;
    static public float LeaderSightRadius = 2.0f;
    static public float slowingRadius = 3;
}
