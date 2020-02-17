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
                new Condition(agentController.HasAmmo,
                    //HasAmmo - true
                    new Condition(SetTargetToEnemyInSight,
                        new KSAction(DoNothing),
                        new KSAction(DoNothing)
                    ),
                    //HasAmmo - false
                    new Condition(SetTargetToClosestAmmo,
                        new KSAction(DoNothing),
                        new KSAction(DoNothing)
                    )
                ),
                new Condition(HasTarget,
                    new Condition(IsTargetInRange,
                        //IsTargetInRange - true
                        new KSAction(DoNothing),
                        //IsTargetInRange - false
                        new Condition(HasReachedTarget,
                            //HasReachedTarget - true
                            new KSAction(DoNothing),
                            //HasReachedTarget - false
                            new Condition(IsFacingTarget,
                                //IsFacingTarget - true
                                new KSAction(MoveForward),
                                //IsFacingTarget - false
                                new KSAction(RotateTowards)
                            )
                        )           
                    ),
                    new KSAction(RotateAround)
                )
            )
        );
    }

    public GameObject targetObject;
    AgentController agentController;
    PathFinding pathFinding;

    void Start()
    {
        agentController = GetComponent<AgentController>();
        pathFinding = GetComponent<PathFinding>();

        SetupBehaviour();
    }

    void Update()
    {
        bt.Tick();
    }

    void DoNothing(){ }

    bool HasAmmo()
    {
        return agentController.HasAmmo();
    }

    bool HasTarget()
    {
        return targetObject != null;
    }

    bool SetTargetToClosestAmmo()
    {
        if (World.ammoTiles.Count > 0)
        {
            float closestDistance = Vector3.Distance(transform.position, World.ammoTiles[0].mTileObject.transform.position);
            targetObject = World.ammoTiles[0].mTileObject;

            for(int i = 1; i < World.ammoTiles.Count; i++)
            {
                float distance = Vector3.Distance(transform.position, World.ammoTiles[i].mTileObject.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    targetObject = World.ammoTiles[i].mTileObject;
                }
            }

            return true;
        }

        return false;
    }

    bool IsTargetInRange()
    {
        if (targetObject != null)
        {
            if (targetObject.layer == 8)
            {
                return false;
            }

            RaycastHit2D hit = Physics2D.Linecast(transform.position, targetObject.transform.position);
            if (hit.collider != null) return false;
            return (Vector3.Distance(transform.position, targetObject.transform.position) < agentController.attackRange);
        }

        return false;
    }

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
                RaycastHit2D hit = Physics2D.Linecast(transform.position, enemy.transform.position);
                if (hit.collider != null) continue;

                closestDistance = distance;
                targetObject = enemy.gameObject;

                Debug.DrawRay(transform.position, enemyDir, Color.red);
            }
        }

        //Looks odd I know, but it is faster than >= 0, the chance of distance being 0 is very slim, but for less readable code, I think it is justified.
        return !(closestDistance < 0);
    }

    bool HasReachedTarget()
    {
        return transform.position == targetObject.transform.position;
    }

    bool IsFacingTarget()
    {
        Vector3 targetDirection = Vector3.Normalize(targetObject.transform.position - transform.position);
        return transform.right == targetDirection;
    }

    void RotateTowards()
    {
        Vector3 targetDirection = targetObject.transform.position - transform.position;
        float angle = Mathf.Atan2(targetDirection.y, targetDirection.x) * Mathf.Rad2Deg;
        Quaternion quart = Quaternion.AngleAxis(angle, Vector3.forward);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, quart, Time.deltaTime * agentController.rotationSpeed);
    }

    void RotateAround()
    {
        transform.Rotate(Vector3.forward, Time.deltaTime * agentController.rotationSpeed);
    }

    void MoveForward()
    {
        transform.localPosition += transform.right * Time.deltaTime;
    }
}
