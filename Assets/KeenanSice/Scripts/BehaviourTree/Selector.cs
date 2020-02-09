using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//KS - Selector executes children till one succeeds, if none succeed the selector fails.
public class Selector : Composite
{
    protected List<Behaviour>.Enumerator currentChild;

    public Selector(params Behaviour[] newChildren) : base(newChildren) { }

    public override void Init()
    {
        currentChild = children.GetEnumerator();
        currentChild.MoveNext();
    }

    public override Status Update()
    {
        if (currentChild.Current == null)
        {
            Debug.Log("Selector has no children. Can't do.");
            //KS - We could technically return a success and continue as normal, but why waste performance. This will flag the issue.
            return Status.Failure;
        }

        while (true)
        {
            Status status = currentChild.Current.Tick();

            if (status != Status.Failure)
            {
                return status;
            }

            //KS - No more children, we failed to succeed.
            if (!currentChild.MoveNext())
            {
                return Status.Failure;
            }
        }
    }
}
