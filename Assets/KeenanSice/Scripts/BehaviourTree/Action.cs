using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KSAction : Behaviour
{
    protected Func<bool> actionMethod;

    public KSAction(Func<bool> method) => actionMethod = method;

    public override Status Update(ref string outDebug, int branchDepth)
    {
        outDebug = outDebug.Remove(outDebug.Length - 1);
        outDebug += "(" + actionMethod.Method.Name + ")\n";

        return actionMethod() ? Status.Success : Status.Failure;
    }
}
