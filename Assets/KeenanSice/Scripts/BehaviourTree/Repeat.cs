using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Repeat : Decorator
{
    public Repeat(Behaviour newChild, int newCounterLimit) : base(newChild) => counterLimit = newCounterLimit;

    protected int counter = 0;
    protected int counterLimit;

    protected override Status Update()
    {
        while (true)
        {
            child.Tick();

            if (child.GetStatus() == Status.Running)
            {
                break;
            }

            if (child.GetStatus() == Status.Failure)
            {
                return Status.Failure;
            }

            if (++counter == counterLimit)
            {
                return Status.Success;
            }
        }

        return Status.Invalid;
    }
}
