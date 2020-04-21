using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoPickup : BasePickup
{
    const int ammoAmount = 5;

    public override bool Pickup(Controller instigator)
    {
        if (base.Pickup(instigator))
        {
            instigator.SetAmmo(ammoAmount);
            return true;
        }

        return false;
    }
}
