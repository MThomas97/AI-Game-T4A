using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentBehaviourTree : MonoBehaviour
{
    BehaviourTree bt;

    void SetupBehaviour()
    {
        bt = new BehaviourTree(
            new Sequence(
                new Condition(SetTargetToEnemyInSight,  //Lets see if an enemy is in sight to be our new target.
                    new Condition(HasReachedTarget,         //True -     Are they in range?
                        new KSAction(DoNothing),                //True -   Don't do anything.
                        new Condition(IsFacingTarget,           //False -   Lets see if we are facing our target.
                            new KSAction(MoveForward),              //True -    Move towards our target.
                            new KSAction(RotateTowards)             //False -   Rotate towards our target.
                        )
                    ),
                    new KSAction(DoNothing)                 //False -   Do do anything.
                )
            )
        );
    }

    Vector3 targetPosition;
    AgentController agentController;

    void Start()
    {
        agentController = GetComponent<AgentController>();
        SetupBehaviour();
    }

    void Update()
    {
        bt.Tick();
    }

    void DoNothing(){ }

    bool SetTargetToEnemyInSight()
    {
        float closestDistance = -1.0f;

        foreach (AgentController enemy in World.agents)
        {
            if (enemy.teamNumber == agentController.teamNumber) continue;

            Vector3 enemyDir = Vector3.Normalize(enemy.transform.position - transform.position);

            float angle = Vector3.Angle(transform.right, enemyDir);

            if(angle > agentController.fieldOfView * 0.5f) continue;

            float distance = Vector3.Distance(transform.position, enemy.transform.position);            

            if (distance < closestDistance || closestDistance < 0)
            {
                //If we hit a collider trying to cast to the enemy, we technically can't see them, so ignore them. Most expensive check so left till last.
                RaycastHit2D hit = Physics2D.Raycast(transform.position, enemyDir, distance);
                if (hit.collider != null) continue;

                closestDistance = distance;
                targetPosition = enemy.transform.position;

                Debug.DrawRay(transform.position, enemyDir, Color.red);
            }
        }

        //Looks odd I know, but it is faster than >= 0, the chance of distance being 0 is very slim, but for less readable code, I think it is justified.
        return !(closestDistance < 0);
    }

    bool HasReachedTarget()
    {
        return transform.position == targetPosition;
    }

    bool IsFacingTarget()
    {
        Vector3 targetDirection = Vector3.Normalize(targetPosition - transform.position);
        return transform.right == targetDirection;
    }

    void RotateTowards()
    {
        Vector3 targetDirection = targetPosition - transform.position;
        float angle = Mathf.Atan2(targetDirection.y, targetDirection.x) * Mathf.Rad2Deg;
        Quaternion quart = Quaternion.AngleAxis(angle, Vector3.forward);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, quart, Time.deltaTime * agentController.rotationSpeed);
    }

    void MoveForward()
    {
        transform.localPosition += transform.right * Time.deltaTime;
    }
}
