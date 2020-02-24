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

    public virtual void Pickup(AgentController instigator)
    {
        pickupTimer = pickupTimerAmount;
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
            pickupTimer = pickupTimer < 0.0f ? 0.0f : pickupTimer;
            spriteRenderer.color = startColour * (1.0f - pickupTimer / pickupTimerAmount);
        }
    }
}
