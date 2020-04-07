using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flocking : MonoBehaviour
{
    Vector2 Position = Vector2.zero;

    [SerializeField] List<GameObject> agents;
    
    int neighborCount = 0;

    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += new Vector3(0.05f, 0, 0);
        Seperation();
    }

    Vector3 Seperation()
    {
        Vector3 seperationVector = new Vector3();
        //Get neighbors in future so you dont have to manually put them in the list
        if (agents.Count == 0)
            return seperationVector;

        //for each agent in agents
        //Check FOV 
        //Vector3 movingTowards = this.position - agent.position;
        //if movingTowards.magnitude > 0
        // seperateVector += movingTowards.normalised / movingTowards.magitude;

        return seperationVector.normalized;

        //Vector2 SourcePos = new Vector2(transform.position.x, transform.position.y);
        //for(int i = 0; i < agents.Count; i++)
        //{
        //    Vector2 TargetPos = new Vector2(agents[i].transform.position.x, agents[i].transform.position.y);
        //    float distance = Vector2.Distance(SourcePos, TargetPos);
        //    if(distance < 5)
        //    {
        //        Vector3 newPos = transform.position - agents[i].transform.position;
        //        newPos.Normalize();
        //        transform.position += newPos * Time.deltaTime;
        //        //transform.rotation += newPos;
        //    }
        //}
    }

    void Alignment()
    {

    }

    void Cohesion()
    {

    }
}
