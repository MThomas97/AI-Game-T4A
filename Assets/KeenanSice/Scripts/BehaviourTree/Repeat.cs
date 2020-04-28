//KS - Repeat executes the child N number of times, specified by the counter limit.
public class Repeat : Decorator
{
    public Repeat(Behaviour newChild, int newCounterLimit) : base(newChild) => counterLimit = newCounterLimit;

    protected int counter = 0;
    protected int counterLimit;

    public override Status Update(ref string outDebug, int branchDepth)
    {
        counter = 0;

        while (true)
        {
            child.Tick(ref outDebug, branchDepth);

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
