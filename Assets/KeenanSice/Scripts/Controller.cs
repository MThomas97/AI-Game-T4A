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

    public float pickupRange { get; } = 2.0f;

    LineRenderer lr = null;
    const float laserLineLifeLength = 1.0f;
    float laserLineResetTimer = 0.0f;
    const float laserLineMaxMissLength = 20.0f;

    Transform healthbar = null;
    Transform ammobar = null;

    public Boid boid { get; protected set; } = null;

    protected void Start()
    {
        healthbar = transform.GetChild(0);
        ammobar = transform.GetChild(1);

        boid = GetComponent<Boid>();

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

        healthbar.localScale = new Vector3(healthbar.localScale.x, health / (float)healthMax, healthbar.localScale.z);

        if (health <= 0)
        {
            World.RemoveControllerFromTeam(this);
            World.CheckWinState();
            Destroy(transform.gameObject);
        }
    }

    public bool IsFullHealth()
    {
        return health >= healthMax;
    }

    protected void Update()
    {
        UpdateLaserLine();
    }

    void UpdateLaserLine()
    {
        if (attackSpeedTimer > 0.0f)
        {
            attackSpeedTimer -= Time.deltaTime;
        }

        if (laserLineResetTimer > 0.0f)
        {
            laserLineResetTimer -= Time.deltaTime;

            if (laserLineResetTimer < 0.0f)
            {
                lr.positionCount = 0;
            }
        }
    }

    public bool Attack(Controller attackee)
    {
        if (!(attackSpeedTimer > 0.0f) && HasAmmo())
        {
            SetAmmo(ammo - 1);
            attackSpeedTimer = attackSpeed;

            if (attackee)
            {
                RaycastHit2D hit = Physics2D.Linecast(transform.position, attackee.transform.position, World.enemyAttackLayerMask);

                if (hit)
                {
                    DisplayLaserLine(new Vector3(hit.point.x, hit.point.y, 0));
                }
                else
                {
                    DisplayLaserLine(attackee.transform.position);
                    attackee.Damage(this, attackDamage);
                    return true;
                }
            }
            else
            {
                Vector3 endPosition = transform.position + transform.right * laserLineMaxMissLength;
                RaycastHit2D hit = Physics2D.Linecast(transform.position, endPosition, World.enemyAttackLayerMask);
                DisplayLaserLine(hit ? new Vector3(hit.point.x, hit.point.y, 0.0f) : endPosition);
            }
        }

        return false;
    }

    public void DisplayLaserLine(Vector3 endFireLine)
    {
        lr.positionCount = 2;
        lr.SetPositions(new Vector3[2] { transform.position, endFireLine });
        laserLineResetTimer = laserLineLifeLength;
    }

    public void Damage(Controller attacker, int amount)
    {
        SetHealth(health - amount);
    }

    public void Pickup(BasePickup pickup)
    {
        pickup.Pickup(this);
    }

    public void SetAmmo(int newAmmo)
    {
        ammo = Mathf.Min(newAmmo, ammoMax);
        ammobar.localScale = new Vector3(ammobar.localScale.x, ammo / (float)ammoMax, ammobar.localScale.z);
    }

    public void SetOpacity(int amount)
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, amount);
    }
}
