using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour
{
    public List<Boid> boids;

    // Start is called before the first frame update
    void Start()
    {
        boids = new List<Boid>();

        boids.AddRange(FindObjectsOfType<Boid>());
    }

    public List<Boid> GetNeighbours(Boid boid, float radius)
    {
        List<Boid> neighboursFound = new List<Boid>();
        
        foreach(Boid otherBoid in boids)
        {
            if (otherBoid == boid)
                continue;

            if(Vector3.Distance(boid.position, otherBoid.position) <= radius)
            {
                neighboursFound.Add(otherBoid);
            }
        }

        return neighboursFound;
    }
}
