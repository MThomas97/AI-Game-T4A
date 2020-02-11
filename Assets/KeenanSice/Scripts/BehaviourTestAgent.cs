using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BehaviourTestAgent : MonoBehaviour
{
    BehaviourTree bt;

    void SetupBehaviour()
    {
        bt = new BehaviourTree(
            new Sequence(
                new KSAction(SetTargetToClosestAmmoTile),
                new Condition(HasReachedTarget,
                    new KSAction(RotateLeft),
                    new Condition(IsFacingTarget,
                        new KSAction(MoveForward),
                        new KSAction(RotateTowards)
                    )
                )
            )
        );
    }

    World world;
    Vector3 targetPosition;


    void Start()
    {
        SetupBehaviour();

        world = FindObjectOfType<World>();
    }

    // Update is called once per frame
    void Update()
    {
        bt.Tick();
    }

    bool CanMoveForward()
    {
        Vector3 newPosition = transform.position + transform.right * Time.deltaTime;
        return (newPosition.x >= 0 && newPosition.y >= 0 && newPosition.x < 9 && newPosition.y < 9);
    }

    void RotateLeft()
    {
        transform.Rotate(new Vector3(0, 0, Time.deltaTime * 45));
    }

    bool IsEnemyInSight()
    {
        //Loop through enemies
        //Store all that are in set view angle

        //Loop through enemies in view angle
        //Check if nothing is between our line of sight

        //Return true if we can see atleast 1 enemy, we should probs cache too.

        return false;
    }

    bool HasReachedTarget()
    {
        return transform.position == targetPosition;
    }

    void SetTargetToClosestAmmoTile()
    {
        if (world.ammoTiles.Count > 0)
        {
            Vector3 closestTilePosition = world.ammoTiles[0].mTileObject.transform.position;
            float distance = Vector3.Distance(transform.position, closestTilePosition);

            for(int i = 1; i < world.ammoTiles.Count; i++)
            {
                float dist = Vector3.Distance(transform.position, world.ammoTiles[i].mTileObject.transform.position);

                if(dist < distance)
                {
                    closestTilePosition = world.ammoTiles[i].mTileObject.transform.position;
                    distance = dist;
                }
            }

            targetPosition = closestTilePosition;
        }
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
        transform.rotation = Quaternion.Slerp(transform.rotation, quart, Time.deltaTime * 2);
    }

    void MoveForward()
    {
        transform.localPosition += transform.right * Time.deltaTime;
    }
}
