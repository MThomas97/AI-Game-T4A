using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]

public class Controller : MonoBehaviour
{
    public int ammoCount { get; protected set; } = 30;
    public int health { get; protected set; } = 100;

    public float movementSpeed { get; } = 6.0f;

    public int attackDamage { get; } = 10;
    public float attackSpeed { get; } = 0.5f;
    protected float attackSpeedTimer = 0.0f;

    public float rotationSpeed { get; } = 600.0f;
    public int teamNumber = -1;

    LineRenderer lr = null;
    const float laserLineLifeLength = 1.0f;
    float laserLineResetTimer = 0.0f;

    protected void Start()
    {
        lr = GetComponent<LineRenderer>();
        lr.widthMultiplier = 0.25f;
        lr.material = new Material(Shader.Find("Unlit/Color"));
        lr.material.color = World.playerColours[teamNumber];
    }

    public bool HasAmmo()
    {
        return ammoCount > 0;
    }

    protected void Update()
    {
        if (attackSpeedTimer > 0.0f)
        {
            attackSpeedTimer -= Time.deltaTime;
        }

        if (laserLineResetTimer > 0.0f)
        {
            laserLineResetTimer -= Time.deltaTime;
            
            if(laserLineResetTimer < 0.0f)
            {
                lr.positionCount = 0;
            }
        }
    }

    public void Attack(Controller attackee)
    {
        if (!(attackSpeedTimer > 0.0f) && HasAmmo())
        {
            attackee.Damage(this, attackDamage);
            ammoCount -= 1;
            attackSpeedTimer = attackSpeed;

            lr.positionCount = 2;
            lr.SetPositions(new Vector3[2] { transform.position, attackee.transform.position });
            laserLineResetTimer = laserLineLifeLength;
        }
    }

    public void Damage(Controller attacker, int amount)
    {
        health -= amount;

        //Temporary
        if (health <= 0) Destroy(transform.gameObject);
    }

    public void Pickup(BasePickup pickup)
    {
        pickup.Pickup(this);
    }

    public void GiveAmmo(int amount)
    {
        ammoCount += amount;
    }
}
