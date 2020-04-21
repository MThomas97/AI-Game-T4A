using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasePickup : MonoBehaviour
{
    protected const float pickupTimerAmount = 10.0f;
    protected float pickupTimer = 0.0f;

    protected Color startColour;
    protected SpriteRenderer spriteRenderer;

    public bool IsPickupActive()
    {
        return (!(pickupTimer > 0.0f));
    }

    public virtual bool Pickup(Controller instigator)
    {
        if (IsPickupActive())
        {
            pickupTimer = pickupTimerAmount;

            return true;
        }

        return false;
    }

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        startColour = spriteRenderer.color;
    }

    protected void Update()
    {
        if (!IsPickupActive())
        {
            pickupTimer -= Time.deltaTime;
            if (pickupTimer < 0.0f)
            {
                spriteRenderer.color = startColour;
                pickupTimer = 0.0f;
            }
            else
            {
                spriteRenderer.color = Color.white * (1.0f - pickupTimer / pickupTimerAmount);
            }
        }
    }
}
