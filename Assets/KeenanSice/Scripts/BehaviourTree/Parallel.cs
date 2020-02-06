using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallel : Composite
{
    public enum Policy
    {
        RequireOne,
        RequireAll,
    }

    protected Policy successPolicy, failurePolicy;

    public Parallel(Policy success, Policy failure)
    {
        successPolicy = success;
        failurePolicy = failure;
    }


    protected override Status Update()
    {
        int successCount = 0, failureCount = 0;

        foreach(Behaviour child in children)
        {
            if (!child.IsTerminated())
            {
                child.Tick();
            }

            if(child.GetStatus() == Status.Success)
            {
                ++successCount;

                if(successPolicy == Policy.RequireOne)
                {
                    return Status.Success;
                }
            }

            if(child.GetStatus() == Status.Failure)
            {
                ++failureCount;
                
                if(failurePolicy == Policy.RequireOne)
                {
                    return Status.Failure;
                }
            }
        }

        if (failurePolicy == Policy.RequireAll && failureCount == children.Count)
        {
            return Status.Failure;
        }

        if(successPolicy == Policy.RequireAll && successCount == children.Count)
        {
            return Status.Success;
        }

        return Status.Running;
    }

    protected override void Terminate(Status newStatus)
    {
        foreach(Behaviour child in children)
        {
            if (child.IsRunning())
            {
                child.Abort();
            }
        }
    }

}

