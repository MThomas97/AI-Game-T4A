using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AgentController : Controller
{
    public float fieldOfView { get; } = 180.0f;
    public float reactionTime { get; } = 0.1f;

    public GameObject lastValidTargetObject = null;

    public Node targetNode = null;
    public GameObject targetObject = null;

    float reactionTimer = 0.0f;

    bool patrolling = false;

    string pathfindingDebugOutput = "Pathfinding";

    public bool AttackAgent()
    {
        if (targetObject != null)
        {
            Controller tc = targetObject.GetComponent<Controller>();

            if (tc != null)
            {
                Attack(tc);
                return true;
            }
        }
        return false;
    }

    public bool PickupCollectable()
    {
        if (targetObject != null)
        {
            BasePickup pickup = targetObject.GetComponent<BasePickup>();

            if (pickup != null)
            {
                Pickup(pickup);
                return true;
            }
        }
        return false;
    }

    //KS - Simulate reactions, once the reaction timer is greater than our agents reaction time, we can reset our target to allow the agent to re-evaluate the current situation, and decide whether a new path or a new target is necessary.
    public bool CheckResetTargetTimer()
    {
        if (targetObject != null && targetObject.GetComponent<Controller>())
        {
            reactionTimer += Time.deltaTime;

            if (reactionTimer > reactionTime)
            {
                SetTarget(null);
                reactionTimer = 0.0f;
            }
        }

        return true;
    }

    public bool HasTarget()
    {
        return targetObject != null || targetNode != null;
    }

    //KS - Attempt to set agents target to the closest active ammo if ammo is needed, returns false if this is not currently possible or they already have ammo.
    public bool SetTargetToClosestActiveAmmo()
    {
        if (HasAmmo())
        {
            return false;
        }

        //KS - Agent already has an ammo target.
        if (targetObject != null)
        {
            AmmoPickup targetAmmo = targetObject.GetComponent<AmmoPickup>();
            if (targetAmmo && targetAmmo.IsPickupActive())
            {
                return true;
            }
        }

        //KS - Find closest ammo target
        GameObject target = null;
        if (World.ammoTiles.Count > 0)
        {
            float closestDistance = -1;
            for (int i = 0; i < World.ammoTiles.Count; i++)
            {
                //KS - If not active skip.
                if (!World.ammoTiles[i].pickupComponent.IsPickupActive()) continue;

                float distance = Vector3.Distance(transform.position, World.ammoTiles[i].mTileObject.transform.position);
                if (distance < closestDistance || closestDistance < 0.0f)
                {
                    closestDistance = distance;
                    target = World.ammoTiles[i].mTileObject;
                }
            }
        }

        SetTarget(target);
        return target != null;
    }

    //KS - Attempt to set agents target to the closest active health if health is needed, returns false if this is not currently possible or they already have full health.
    public bool SetTargetToClosestActiveHealth()
    {
        if (IsFullHealth())
        {
            return false;
        }

        //KS - Agent already has an health target.
        if (targetObject != null)
        {
            HealthPickup targetHealth = targetObject.GetComponent<HealthPickup>();
            if (targetHealth && targetHealth.IsPickupActive())
            {
                return true;
            }
        }

        //KS - Find closest health target
        GameObject target = null;
        if (World.healthTiles.Count > 0)
        {
            float closestDistance = -1;
            for (int i = 0; i < World.healthTiles.Count; i++)
            {
                //KS - If not active skip.
                if (!World.healthTiles[i].pickupComponent.IsPickupActive()) continue;

                float distance = Vector3.Distance(transform.position, World.healthTiles[i].mTileObject.transform.position);
                if (distance < closestDistance || closestDistance < 0.0f)
                {
                    closestDistance = distance;
                    target = World.healthTiles[i].mTileObject;
                }
            }
        }

        SetTarget(target);
        return target != null;
    }

    //KS - Handle setting a new target for the agent.
    void SetTarget(GameObject target, bool isPatrolTarget = false)
    {
        //KS - If the target is different, we will need to recalculate a path. If the new target is null then we should path to the last valid target, it is to simulate a sense of memory.
        if (target != targetObject)
        {
            lastValidTargetObject = target != null && target.GetComponent<Controller>() ? target : lastValidTargetObject;
            pathfindingDebugOutput = "";
            Node pathfindingNode = target == null ? null : PathFinding.CalculatePath(transform.position, target.transform.position, out pathfindingDebugOutput);
            targetNode = pathfindingNode != null ? pathfindingNode.parent : pathfindingNode;
        }
        //KS - If our target has not changed, then we need to check if we have a reached a sub node in the path that was calculated, if so we need to re-target the target node to it's parent which is just effectively traversing the path. 
        else if (targetNode != null && Vector3.Distance(transform.position, targetNode.GetVector3Position()) < 0.1f)
        {
            targetNode = targetNode != null ? targetNode.parent : targetNode;
        }

        targetObject = target;
        targetNode = target == null ? null : targetNode;

        patrolling = isPatrolTarget;
    }

    public bool SetTargetToLastEnemySeen()
    {
        if (lastValidTargetObject)
        {
            SetTarget(lastValidTargetObject);
            return true;
        }

        return false;
    }

    public bool IsTargetInRange()
    {
        //KS - If our target is in range and it is a subnode to a calculated path, then we should trigger a set target to update to the next node in the path before checking is in range.
        if (targetNode != null && targetNode.parent != null)
        {
            if (Vector3.Distance(transform.position, targetNode.GetVector3Position()) < 0.1f)
            {
                SetTarget(targetObject, patrolling);
            }
        }

        //KS - If the target object is on the NoRange layer (8) then we need to be virtually on the tile to be classed as on in range.
        if (targetObject != null && targetObject.layer == 8)
        {
            return (Vector3.Distance(transform.position, targetObject.transform.position) < 0.1f);
        }
        //KS - We need to check if anything is in between us and our target before we can determine if we are in range.
        else if (targetNode != null && targetNode.parent == null)
        {
            RaycastHit2D hit = Physics2D.Linecast(transform.position, targetNode.GetVector3Position(), World.enemyAttackLayerMask);
            if (hit.collider != null) return false;
            return (Vector3.Distance(transform.position, targetNode.GetVector3Position()) < attackRange);
        }

        return false;
    }

    //KS - Attempt to set agents target to the closest enemy in sight, returns false if no targets were found.
    public bool SetTargetToEnemyInSight()
    {
        float closestDistance = -1.0f;
        GameObject target = null;

        for (int enemyTeamNumber = 0; enemyTeamNumber < World.agentTeams.Count; enemyTeamNumber++)
        {
            //KS - Skip if the agent is on the same team.
            if (enemyTeamNumber == teamNumber) continue;

            foreach (Controller enemy in World.agentTeams[enemyTeamNumber])
            {
                if (enemy)
                {
                    Vector3 enemyDir = Vector3.Normalize(enemy.transform.position - transform.position);
                    float angle = Vector3.Angle(transform.right, enemyDir);
                    if (angle > fieldOfView * 0.5f) continue;
                    float distance = Vector3.Distance(transform.position, enemy.transform.position);

                    if (distance < closestDistance || closestDistance < 0)
                    {
                        //KS - If we hit a collider trying to cast to the enemy, we technically can't see them, so ignore them.
                        RaycastHit2D hit = Physics2D.Linecast(transform.position, enemy.transform.position, World.enemyAttackLayerMask);
                        if (hit.collider != null) continue;

                        closestDistance = distance;
                        target = enemy.gameObject;
                    }
                }
            }
        }

        if (target != null) SetTarget(target);
        return target != null;
    }

    //KS - Sets agents target to a random spawn, this is good for a quick random patrol, the spawn points are usually on opposite sides of the map forcing the agent to traverse a lot, this could result in them seeing an enemy along the route.
    bool SetTargetToRandomSpawn()
    {
        int randomSpawnpointIndex = Mathf.RoundToInt(Random.Range(0.0f, World.spawnpointTiles.Count - 1));
        if (Vector3.Distance(transform.position, World.spawnpointTiles[randomSpawnpointIndex].mTileObject.transform.position) < 5.0f)
        {
            randomSpawnpointIndex = (int)Mathf.Repeat(randomSpawnpointIndex + 1, World.spawnpointTiles.Count - 1);
        }

        SetTarget(World.spawnpointTiles[randomSpawnpointIndex].mTileObject, true);
        return true;
    }


    public bool IsFacingTarget()
    {
        return transform.right == GetDirectionToTarget();
    }

    Vector3 GetDirectionToTarget()
    {
        Vector3 targetPosition = targetNode != null ? targetNode.GetVector3Position() : targetObject.transform.position;
        return Vector3.Normalize(targetPosition - transform.position);
    }

    public bool RotateTowards()
    {
        if (targetObject != null)
        {
            Vector3 targetDirection = GetDirectionToTarget();
            float angle = Mathf.Atan2(targetDirection.y, targetDirection.x) * Mathf.Rad2Deg;
            Quaternion quart = Quaternion.AngleAxis(angle, Vector3.forward);

            transform.rotation = Quaternion.RotateTowards(transform.rotation, quart, Time.deltaTime * rotationSpeed);
            return true;
        }

        return false;
    }

    public bool RotateAround()
    {
        transform.Rotate(Vector3.forward, Time.deltaTime * rotationSpeed);
        return true;
    }

    public bool MoveForward()
    {
        transform.localPosition += transform.right * Time.deltaTime * movementSpeed;

        return true;
    }

    public bool PatrolToNextTarget()
    {
        if (patrolling)
        {
            SetTargetToRandomSpawn();
        }

        return false;
    }

    public bool StartPatrol()
    {
        if (!patrolling) SetTargetToRandomSpawn();

        return patrolling;

    }

    //KS - On screen debugging. For drawing the pathfinding path.
    void OnDrawGizmos()
    {
        if (teamNumber > -1)
        {
            Gizmos.color = World.playerColours[teamNumber];

            //KS - Avoid debug overlaps.
            Vector3 offset = new Vector3(0.1f, 0.1f, 0.0f) * teamNumber + new Vector3(-0.2f, -0.2f, -0.0f);

            if (targetNode != null)
            {
                Node currentNode = targetNode;

                Vector3 currentPos = transform.position + offset;
                Gizmos.DrawWireCube(currentPos, new Vector3(1.0f, 1.0f));
                Gizmos.DrawLine(currentPos, new Vector3(currentNode.pos.x, currentNode.pos.y, 0) + offset);

                while (currentNode != null)
                {
                    currentPos = new Vector3(currentNode.pos.x, currentNode.pos.y, 0) + offset;

                    Gizmos.DrawWireCube(currentPos, new Vector3(1.0f, 1.0f));

                    if (currentNode.parent != null)
                    {
                        Vector3 parentPos = new Vector3(currentNode.parent.pos.x, currentNode.parent.pos.y, 0) + offset;
                        Gizmos.DrawLine(currentPos, parentPos);
                    }

                    currentNode = currentNode.parent;
                }
            }
        }
    }

    //KS - On screen debugging. For timings from the pathfinding.
    void OnGUI()
    {
        if (boid.IsLeader)
        {
            const float widthPadding = 10;
            float labelHeight = (Screen.height / (float)World.agentTeams.Count);
            float heightOffset = labelHeight * teamNumber;
            GUI.contentColor = World.playerColours[teamNumber];

            GUI.skin.label.fontSize = Screen.width / 100;
            GUI.skin.label.alignment = TextAnchor.UpperRight;
            GUI.Label(new Rect(Screen.width * 0.5f, heightOffset, (Screen.width * 0.5f) - widthPadding, labelHeight), pathfindingDebugOutput);
        }
    }
}
