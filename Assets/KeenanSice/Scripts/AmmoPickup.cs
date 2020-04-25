using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoPickup : BasePickup
{
    public override bool Pickup(Controller instigator)
    {
        if (base.Pickup(instigator))
        {
            instigator.SetAmmo(instigator.ammoMax);
            return true;
        }

        return false;
    }
}
