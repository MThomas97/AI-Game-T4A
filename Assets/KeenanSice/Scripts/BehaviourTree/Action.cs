using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KSAction : Behaviour
{
    protected Action actionMethod;

    protected Action successBehaviour, failureBehaviour;

    public KSAction(Action method) => actionMethod = method;

    public override Status Update()
    {
        actionMethod();
        return Status.Success;
    }
}
