using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoPickup : BasePickup
{
    const int ammoAmount = 5;

    public override void Pickup(AgentController instigator)
    {
        if (IsPickupActive())
        {
            instigator.GiveAmmo(ammoAmount);
            base.Pickup(instigator);
        }
    }
}
