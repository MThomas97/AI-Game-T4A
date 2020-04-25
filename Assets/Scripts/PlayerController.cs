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

    void HandleInput()
    {
        if (Input.GetButtonDown("Fire_" + teamNumber))
        {
            Debug.Log(Time.fixedTime);

            if (!TryPickup())
            {
                if (HasAmmo())
                {
                    TryAttack();
                }
            }
        }
    }

    void HandleFixedUpdateInput()
    {
        float horizontalAxis = Input.GetAxis("Horizontal_" + teamNumber);
        float verticalAxis = Input.GetAxis("Vertical_" + teamNumber);

        if (Mathf.Abs(horizontalAxis) > axisMovementDeadzone || Mathf.Abs(verticalAxis) > axisMovementDeadzone)
        {
            Move(new Vector2(horizontalAxis, verticalAxis));
        }
    }

    void Move(Vector2 movement)
    {
        Vector2 targetPosition = new Vector2(transform.position.x, transform.position.y) + movement * movementSpeed * Time.deltaTime;
        rb.MovePosition(targetPosition);

        float angle = Mathf.Atan2(movement.y, movement.x) * Mathf.Rad2Deg;
        Quaternion quart = Quaternion.AngleAxis(angle, Vector3.forward);

        transform.rotation = Quaternion.RotateTowards(transform.rotation, quart, Time.deltaTime * rotationSpeed);
    }

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
