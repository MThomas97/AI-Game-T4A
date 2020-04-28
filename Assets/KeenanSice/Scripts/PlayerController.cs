using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]

public class PlayerController : Controller
{
    Rigidbody2D rb = null;
    float axisMovementDeadzone = 0.01f;

    protected new void Start()
    {
        base.Start();

        //KS - Setup Rigidbody
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0;
        rb.freezeRotation = true;
    }

    protected new void Update()
    {
        base.Update();
        HandleInput();
    }

    private void FixedUpdate()
    {
        HandleFixedUpdateInput();
    }

    //KS - Handle inputs that do not require physics updates.
    void HandleInput()
    {
        if (Input.GetButtonDown("Fire_" + teamNumber))
        {
            //KS - If we cannot pick something up attempt to shoot.
            if (!TryPickup())
            {
                //KS - If we have ammo, try attacking.
                if (HasAmmo())
                {
                    TryAttack();
                }
            }
        }
    }

    //KS - Handle inputs that require physics updates.
    void HandleFixedUpdateInput()
    {
        float horizontalAxis = Input.GetAxis("Horizontal_" + teamNumber);
        float verticalAxis = Input.GetAxis("Vertical_" + teamNumber);

        //KS - If either axis is larger than the deadzone threshold, we will move the agent.
        if (Mathf.Abs(horizontalAxis) > axisMovementDeadzone || Mathf.Abs(verticalAxis) > axisMovementDeadzone)
        {
            Move(new Vector2(horizontalAxis, verticalAxis));
        }
    }

    //KS - Handle agent movement, and rotate towards the directly were heading.
    void Move(Vector2 movement)
    {
        Vector2 targetPosition = new Vector2(transform.position.x, transform.position.y) + movement * movementSpeed * Time.deltaTime;
        rb.MovePosition(targetPosition);

        float angle = Mathf.Atan2(movement.y, movement.x) * Mathf.Rad2Deg;
        Quaternion quart = Quaternion.AngleAxis(angle, Vector3.forward);

        transform.rotation = Quaternion.RotateTowards(transform.rotation, quart, Time.deltaTime * rotationSpeed);
    }

    //KS - Try and find a target instead our attack cone, this is a simple form of aim assist. If a target is found we will attack them.
    bool TryAttack()
    {
        Controller attackee = null;

        for (int enemyTeamNumber = 0; enemyTeamNumber < World.agentTeams.Count; enemyTeamNumber++)
        {
            //Skip if on the same team.
            if (enemyTeamNumber == teamNumber) continue;

            foreach (Controller enemy in World.agentTeams[enemyTeamNumber])
            {
                if (enemy && Vector3.Distance(enemy.transform.position, transform.position) < attackRange)
                {
                    Vector3 enemyDir = Vector3.Normalize(enemy.transform.position - transform.position);
                    float angle = Vector3.Angle(transform.right, enemyDir);
                    if (angle > attackAngle * 0.5f) continue;

                    attackee = enemy;
                    break;
                }
            }
        }

        return Attack(attackee);
    }

    bool TryPickup()
    {
        return (TryPickupAmmo() || TryPickupHealth());
    }

    bool TryPickupAmmo()
    {
        foreach (AmmoTile ammoTile in World.ammoTiles)
        {
            if (TryPickupBase(ammoTile))
            {
                return true;
            }
        }

        return false;
    }

    bool TryPickupHealth()
    {
        foreach (HealthTile healthTile in World.healthTiles)
        {
            if (TryPickupBase(healthTile))
            {
                return true;
            }
        }

        return false;
    }

    bool TryPickupBase(PickupTile pickupTile)
    {
        if (Vector3.Distance(pickupTile.mTileObject.transform.position, transform.position) < pickupRange)
        {
            return pickupTile.Pickup(this);
        }

        return false;
    }
}
