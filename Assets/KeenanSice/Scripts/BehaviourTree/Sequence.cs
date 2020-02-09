using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//KS - Sequence executes children till one fails, if none fail the sequence succeeds.
public class Sequence : Composite
{
    protected List<Behaviour>.Enumerator currentChild;

    public Sequence(params Behaviour[] newChildren) : base(newChildren) { }

    public override void Init()
    {
        currentChild = children.GetEnumerator();
        currentChild.MoveNext();
    }

    public override Status Update()
    {
        if (currentChild.Current == null)
        {
            Debug.Log("Sequence has no children. Can't do.");
            //KS - We could technically return a success and continue as normal, but why waste performance. This will flag the issue.
            return Status.Failure;
        }

        while (true)
        {
            Status status = currentChild.Current.Tick();

            if(status != Status.Success)
            {
                return status;
            }

            //KS - No children failed, so the sequence succeeded.
            if (!currentChild.MoveNext())
            {
                return Status.Success;
            }
        }
    }

}
