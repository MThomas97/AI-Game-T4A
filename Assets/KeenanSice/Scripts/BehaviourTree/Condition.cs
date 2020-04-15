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

    public override Status Update(ref string outDebug, int branchDepth)
    {
        outDebug = outDebug.Remove(outDebug.Length - 1);
        outDebug += "(" + conditionMethod.Method.Name + ")\n";

        if (conditionMethod())
        {
            successBehaviour.Tick(ref outDebug, branchDepth);
        }
        else
        {
            failureBehaviour.Tick(ref outDebug, branchDepth);
        }

        return Status.Success;
    }
}
