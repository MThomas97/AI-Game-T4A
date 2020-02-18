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
                new KSAction(CheckResetTargetTimer),
                new Condition(agentController.HasAmmo,
                    //HasAmmo - true
                    new KSAction(DoNothing),
                    //HasAmmo - false
                    new Condition(SetTargetToClosestAmmo,
                        new KSAction(DoNothing),
                        new KSAction(DoNothing)
                    )
                ),
                new Condition(HasTarget,
                    //HasTarget - true
                    new Condition(IsTargetInRange,
                        //IsTargetInRange - true
                        new Condition(IsFacingTarget,
                            //IsFacingTarget - true
                            new KSAction(DoNothing),
                            //IsFacingTarget - false
                            new KSAction(RotateTowards)                        
                        ),
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
                    //HasTarget - false
                    new Condition(SetTargetToEnemyInSight,
                        new KSAction(DoNothing),
                        new KSAction(RotateAround)
                    )
                )
            )
        );
    }

    public Node targetNode = null;
    public GameObject targetObject = null;

    AgentController agentController;

    float reactionTimer = 0.0f;

    void CheckResetTargetTimer()
    {
        reactionTimer += Time.deltaTime;

        if (reactionTimer > agentController.reactionTime)
        {
            SetTarget(null);
            reactionTimer = 0.0f;
        }
    }

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

    bool HasAmmo()
    {
        return agentController.HasAmmo();
    }

    bool HasTarget()
    {
        return targetNode != null;
    }

    bool SetTargetToClosestAmmo()
    {
        GameObject target = null;

        if (World.ammoTiles.Count > 0)
        {
            float closestDistance = Vector3.Distance(transform.position, World.ammoTiles[0].mTileObject.transform.position);

            target = World.ammoTiles[0].mTileObject;

            for(int i = 1; i < World.ammoTiles.Count; i++)
            {
                float distance = Vector3.Distance(transform.position, World.ammoTiles[i].mTileObject.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    target = World.ammoTiles[i].mTileObject;
                }
            }
        }

        SetTarget(target);

        return target != null;
    }

    void SetTarget(GameObject target)
    {
        string outDebugString = "";

        if (target != targetObject)
        {
            targetNode = target == null ? null : PathFinding.CalculatePath(transform.position, target.transform.position, out outDebugString);
            targetNode = targetNode != null ? targetNode.parent : targetNode;
        }
        agentController.UpdatePathfindingDebug(targetNode, outDebugString);

        targetObject = target;
    }

    bool IsTargetInRange()
    {
        if (targetNode != null && targetNode.parent == null)
        {
            if (targetObject.layer == 8)
            {
                return false;
            }

            RaycastHit2D hit = Physics2D.Linecast(transform.position, targetNode.GetVector3Position());
            if (hit.collider != null) return false;
            return (Vector3.Distance(transform.position, targetNode.GetVector3Position()) < agentController.attackRange);
        }

        return false;
    }

    bool SetTargetToEnemyInSight()
    {
        float closestDistance = -1.0f;

        GameObject target = null;

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
                target = enemy.gameObject;

                Debug.DrawRay(transform.position, enemyDir, Color.red);
            }
        }

        SetTarget(target);

        return target != null;
    }

    bool HasReachedTarget()
    {
        bool reachedNode = Vector3.Distance(transform.position, targetNode.GetVector3Position()) < 0.05f;

        if(reachedNode)
        {
            targetNode = targetNode.parent;

            return targetNode == null;
        }

        return reachedNode;
    }

    bool IsFacingTarget()
    {
        Vector3 targetDirection = Vector3.Normalize(targetNode.GetVector3Position() - transform.position);
        return transform.right == targetDirection;
    }

    void RotateTowards()
    {
        Vector3 targetDirection = Vector3.Normalize(targetNode.GetVector3Position() - transform.position);
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
        transform.localPosition += transform.right * Time.deltaTime * agentController.movementSpeed;
    }
}
