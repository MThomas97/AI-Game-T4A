using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPickup : BasePickup
{
    public override bool Pickup(Controller instigator)
    {
        if (base.Pickup(instigator))
        {
            instigator.SetHealth(instigator.healthMax);
            return true;
        }
        return false;
    }
}
