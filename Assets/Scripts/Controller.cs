using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]

public class Controller : MonoBehaviour
{
    public int ammo { get; protected set; } = 0;
    public int ammoMax { get; } = 5;
    public int healthMax { get; } = 100;
    public int health { get; protected set; } = 0;

    public float movementSpeed { get; } = 6.0f;

    public int attackDamage { get; } = 10;
    public float attackSpeed { get; } = 0.5f;
    protected float attackSpeedTimer = 0.0f;

    public float rotationSpeed { get; } = 600.0f;
    public int teamNumber = -1;

    public float attackRange { get; } = 10.0f;
    public float attackAngle { get; } = 45.0f;

    public float pickupRange { get; } = 1.0f;

    LineRenderer lr = null;
    const float laserLineLifeLength = 1.0f;
    float laserLineResetTimer = 0.0f;

    protected void Start()
    {
        lr = GetComponent<LineRenderer>();
        lr.widthMultiplier = 0.25f;
        lr.material = new Material(Shader.Find("Unlit/Color"));
        lr.material.color = World.playerColours[teamNumber];
        SetHealth(healthMax);
        ammo = ammoMax;
    }

    public bool HasAmmo()
    {
        return ammo > 0;
    }

    public bool HasFullAmmo()
    {
        return ammo >= ammoMax; 
    }

    public void SetHealth(int newHealth)
    {
        health = newHealth;
        if (health <= 0) Destroy(transform.gameObject);
    }

    public bool IsFullHealth()
    {
        return health >= healthMax;
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
            ammo -= 1;
            attackSpeedTimer = attackSpeed;

            RaycastHit2D hit = Physics2D.Linecast(transform.position, attackee.transform.position, World.enemyAttackLayerMask);

            Vector3 hitPoint = hit ? new Vector3(hit.point.x, hit.point.y, 0) : attackee.transform.position;

            lr.positionCount = 2;
            lr.SetPositions(new Vector3[2] { transform.position, hitPoint });
            laserLineResetTimer = laserLineLifeLength;

            if (!hit)
            {
                attackee.Damage(this, attackDamage);
            }
        }
    }

    public void Damage(Controller attacker, int amount)
    {
        SetHealth(health - amount);
    }

    public void Pickup(BasePickup pickup)
    {
        pickup.Pickup(this);
    }

    public void GiveAmmo(int amount)
    {
        ammo = Mathf.Min(ammo + amount, ammoMax);
    }
}
