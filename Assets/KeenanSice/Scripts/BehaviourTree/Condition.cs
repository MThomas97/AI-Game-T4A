using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Condition : Behaviour
{
    protected Func<bool> conditionMethod = new Func<bool>(() => false);

    protected Behaviour successBehaviour, failureBehaviour;

    public Condition(Func<bool> method, Behaviour newSuccessBehaviour, Behaviour newFailureBehaviour)
    {
        conditionMethod = method;
        successBehaviour = newSuccessBehaviour;
        failureBehaviour = newFailureBehaviour;
    }

    public override Status Update()
    {
        if (conditionMethod())
        {
            successBehaviour.Tick();
            return Status.Success;
        }
        else
        {
            failureBehaviour.Tick();
            return Status.Failure;
        }
    }
}
