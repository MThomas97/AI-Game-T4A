using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Selector : Composite
{
    protected List<Behaviour>.Enumerator currentChild;

    protected override void Init()
    {
        currentChild = children.GetEnumerator();
    }

    protected override Status Update()
    {
        while (true)
        {
            Status status = currentChild.Current.Tick();

            if (status != Status.Failure)
            {
                return status;
            }

            //KS - No more children, we successfully moved through all children.
            if (!currentChild.MoveNext())
            {
                return Status.Failure;
            }
        }
    }
}
