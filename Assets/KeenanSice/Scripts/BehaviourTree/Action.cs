using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KSAction : Behaviour
{
    protected Func<bool> actionMethod;

    public KSAction(Func<bool> method) => actionMethod = method;

    public override Status Update()
    {
        return actionMethod() ? Status.Success : Status.Failure;
    }
}
