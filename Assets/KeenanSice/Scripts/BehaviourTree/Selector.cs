using System.Collections.Generic;

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

    public override Status Update(ref string outDebug, int branchDepth)
    {
        if (currentChild.Current == null)
        {
            //KS - We could technically return a success and continue as normal, but why waste performance. This will flag the issue.
            return Status.Failure;
        }

        while (true)
        {
            Status status = currentChild.Current.Tick(ref outDebug, branchDepth);

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
